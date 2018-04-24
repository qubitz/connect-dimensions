/*
 * 
 * Author: Andrew Frost
 * 
 * Copyright (c) 2018 All Rights Reserved
 * 
 * 4/24/2018
 * 
 */

using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Threading;

//the game's AI. 
public class GameAI : MonoBehaviour
{
    public Token myToken = Token.red;

    [Range(1, 5)]
    public int depth = 3;

    private GameController gameController;

    private bool planning = false;
    private bool planFound = false;

    private List<Thread> threads;

    private void Awake()
    {
        gameController = GetComponent<GameController>();

        planning = false;
        planFound = false;

        threads = new List<Thread>();
    }

    private void OnDestroy()
    {
        int index;
        
        //abort all threaded jobs
        MoveJob.abort = true;        
        
        for(index = 0; index < threads.Count; index++)
        {
            threads[index].Join();
        }
        
        StopAllCoroutines();
    }

    public void PlaceToken()
    {
        if (!planning && !planFound)
        {
            //plan now
            StartCoroutine(PlayTurnCoroutine());
        }
        else
        {
            //get the move from the predicted outcome
        }
        
    }

    private IEnumerator PlayTurnCoroutine()
    {
        Thread thread;

#if UNITY_EDITOR
        float time = Time.timeSinceLevelLoad;
#endif

        MoveJob moveJob = new MoveJob()
        {
            move = Vector3Int.zero,
            board = new BoardData(gameController.Board),
            myToken = myToken,
            depth = depth
        };

        thread = GetMoveAsync(moveJob);

        threads.Add(thread);

        thread.Start();

#if UNITY_EDITOR
        Debug.Log("Start Job!");
#endif
        //ensure that the thread has started
        yield return new WaitUntil(() => thread.IsAlive);

#if UNITY_EDITOR
        Debug.Log("Working!");
#endif
        //wait for the thread to finish
        yield return new WaitWhile(() => thread.IsAlive);

#if UNITY_EDITOR
        Debug.Log("Job complete with move: " + moveJob.move + ".");
#endif
        
        //place the token based on selected move
        gameController.PlaceToken(moveJob.move, myToken);

        //remove the thread from the list of threads
        threads.Remove(thread);

#if UNITY_EDITOR
        Debug.Log("Completed in " + (Time.timeSinceLevelLoad - time) + " seconds.");
#endif
    }

    private static Thread GetMoveAsync(MoveJob moveJob)
    {
        return new Thread(() => moveJob.GetMove());
    }

    //returns the selected move on success and (-1, -1, -1) on failure
    private static Vector3Int FindMove(BoardData board, Token myToken, int depth, ref bool abort)
    {
        Vector3Int move = new Vector3Int(-1, -1, -1);

        //run AlphaBeta to find move
        AlphaBeta(board, myToken, depth, int.MinValue, int.MaxValue, true, ref move, ref abort);

        return move;
    }

    private static int AlphaBeta(BoardData leaf, Token current, int depth, int alpha, int beta, bool maximize, ref Vector3Int movePosition, ref bool abort)
    {
        MoveData[] childLeaves;
        Vector3Int placeHolder = Vector3Int.zero;
        Token token = Token.empty;
        int bestValue = 0, value, index;

        //if the thread executing this job has stopped then return
        if (abort)
        {
            return -1;
        }

        //null test BoardData
        if (leaf != null)
        {
            //check for depth being reached or if game is over
            if (depth <= 0 || GameStatus.IsGameOver(leaf, ref token))
            {
                return EvaluateBoard(leaf, current);
            }

            //generate child leaves
            childLeaves = GetAllChildrenOf(leaf, current, ref abort);

            //search each child leaf for most valuable outcome recursively
            if (maximize)
            {
                bestValue = int.MinValue;

                for (index = 0; index < childLeaves.Length; index++)
                {
                    value = AlphaBeta(childLeaves[index].board, 
                                      GameStatus.GetOppositePlayerOf(current), 
                                      depth - 1, alpha, beta, false, 
                                      ref placeHolder, ref abort);

                    if (value > bestValue)
                    {
                        bestValue = value;

                        //select move
                        movePosition = childLeaves[index].move;
                    }

                    alpha = Mathf.Max(bestValue, alpha);

                    if (beta <= alpha)
                    {
                        break;
                    }
                }
            }
            else
            {
                bestValue = int.MaxValue;

                for (index = 0; index < childLeaves.Length; index++)
                {
                    value = AlphaBeta(childLeaves[index].board,
                                      GameStatus.GetOppositePlayerOf(current),
                                      depth - 1, alpha, beta, true, 
                                      ref placeHolder, ref abort);

                    if (value < bestValue)
                    {
                        bestValue = value;
                    }

                    beta = Mathf.Min(bestValue, beta);

                    if (beta <= alpha)
                    {
                        break;
                    }
                }
            }
        }

        return bestValue;
    }

    private static MoveData[] GetAllChildrenOf(BoardData leaf, Token current, ref bool abort)
    {
        Vector3Int position = Vector3Int.zero;
        List<MoveData> moves = new List<MoveData>();
        MoveData move;

        //if the thread executing this job has stopped then return
        if (abort)
        {
            return null;
        }

        //iterate through the board bottom to top and build a list of all potential moves
        for (position.x = 0; position.x < leaf.Size.x; position.x++)
        {
            for (position.z = 0; position.z < leaf.Size.z; position.z++)
            {
                for (position.y = 0; position.y < leaf.Size.y; position.y++)
                {                    
                    if (leaf.IsSpaceAvailable(position))
                    {
                        //create a new move
                        move = new MoveData()
                        {
                            move = position,
                            board = new BoardData(leaf)
                        };

                        //add the new token to the board
                        move.board.TrySetValue(position, current);

                        //add the move to moves
                        moves.Add(move);

                        //don't check above this position
                        break;
                    }
                }
            }            
        }

        return moves.ToArray();
    }

    private static int EvaluateBoard(BoardData board, Token myToken)
    {
        int value = 0;
        string boardState;

        if (board == null)
        {
            return -1;
        }
        
        boardState = GameStatus.GetBoardState(board);

        //evaluate board state

        //our value
        value += 2 * GetTokenValue(boardState, myToken);

        //opponent value
        value -= GetTokenValue(boardState, (myToken == Token.red ? Token.yellow : Token.red));
        
        return value;
    }

    private static int GetTokenValue(string boardState, Token token)
    {
        int index, value = 0, count;
        string testStr;

        Token other;

        testStr = "";

        //note: I'm aware that instances of "double counting" of combinations
        //occurs in this heurisitic

        //get weight of number of consecutive instances
        for (index = 0; index < 4; index++)
        {
            testStr += token.ToString() + " ";

            //count number of token combo
            count = Regex.Matches(boardState, testStr).Count;

            value += (index + 1) * count;
        }

        //get weight of number of blocks
        other = (token == Token.red ? Token.yellow : Token.red);

        testStr = token.ToString() + " " + other.ToString() + " ";
        count = Regex.Matches(boardState, testStr).Count;

        value += 3 * count; //weight blocks heavier than most consecutive piece setups

        testStr = other.ToString() + " " + token.ToString() + " ";
        count = Regex.Matches(boardState, testStr).Count;

        value += 3 * count; //weight blocks heavier than most consecutive piece setups

        return value;
    }

    private class MoveData
    {
        public Vector3Int move;
        public BoardData board;
    }

    private class MoveJob
    {
        public Vector3Int move;
        public BoardData board;
        public Token myToken;
        public int depth;

        public static bool abort = false;

        public void GetMove()
        {
            move = FindMove(board, myToken, depth, ref abort);
        }
    }

}

/*
 * 
 * Author: Andrew Frost
 * 
 * Copyright (c) 2018 All Rights Reserved
 * 
 * 5/14/2018
 * 
 */

using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Threading;
using UnityEngine.Events;

//the game's AI. 
public class GameAI : MonoBehaviour
{
    [Range(1, 5)]
    public int depth = 3;
    [SerializeField]
    private GameObject TokenPrefabToPlace;

    public UnityEvent onStartPlanning = null; //called when a new plan has to be determined on demand
    public UnityEvent onStopPlanning = null; //called when a new plan determined on demand has completed
    public UnityEvent onPlacePiece = null; //called when a piece is placed

    private GameController gameController;

    private List<Thread> threads;

    private Dictionary<string, Vector3Int> predictedMoves;

    private void Awake()
    {
        ResetAI();
    }

    public void ResetAI()
    {
        StopAllCoroutines();
        StopAllThreads();

        gameController = GetComponent<GameController>();

        MoveJob.abortAll = false;

        predictedMoves = new Dictionary<string, Vector3Int>();

        threads = new List<Thread>();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        StopAllThreads();
    }

    private void StopAllThreads()
    {
        int index;

        //abort all threaded jobs
        MoveJob.abortAll = true;

        if (threads != null)
        {
            for (index = 0; index < threads.Count; index++)
            {
                threads[index].Join();
            }

            //clean up
            threads.Clear();
        }
        
        MoveJob.abortAll = false;
    }

    public void StartPlaceToken()
    {
        Vector3Int move;

        lock (predictedMoves)
        {
            //if we have predicted this move then
            if (predictedMoves.ContainsKey(gameController.Board.GetXYZBoard()))
            {
                //get the move from the predicted outcome
                move = predictedMoves[gameController.Board.GetXYZBoard()];

                //place token at position
                PlaceToken(move);

#if UNITY_EDITOR
                Debug.Log("Placed token at " + move + " using predicted move.");
#endif
            }
            else //if this move wasn't predicted then
            {
                //stop any coroutines from starting more predictions
                StopAllCoroutines();

                //stop any predictions that are running
                StopAllThreads();

                //plan the next move now
                StartCoroutine(PlayTurnCoroutine(new BoardData(gameController.Board)));
            }
        }
        
    }

    public void PredictMoves()
    {
        StartCoroutine(PredictMovesCoroutine());
    }

    private IEnumerator PlayTurnCoroutine(BoardData board)
    {

#if UNITY_EDITOR
        float time = Time.timeSinceLevelLoad;
#endif
        MoveJob moveJob = new MoveJob()
        {
            move = Vector3Int.zero,
            board = board,
            myToken = TokenType.AI,
            depth = depth,
            jobID = 10101
        };

        JobMonitor jobMonitor = new JobMonitor();

        if (onStartPlanning != null)
        {
            onStartPlanning.Invoke();
        }

        //start the job
        StartCoroutine(GetMove(moveJob, jobMonitor));

#if UNITY_EDITOR
        Debug.Log("Working");
#endif

        //wait for it to complete
        yield return new WaitUntil(() => jobMonitor.complete);

        if (onStopPlanning != null)
        {
            onStopPlanning.Invoke();
        }

        //place the token based on selected move
        PlaceToken(moveJob.move);      

#if UNITY_EDITOR
        Debug.Log("Completed in " + (Time.timeSinceLevelLoad - time) + " seconds.");
        Debug.Log("Placed token at " + moveJob.move + ".");
#endif


    }

    private void PlaceToken(Vector3Int move)
    {
        //place the token based on selected move
        if (onPlacePiece != null)
        {
            onPlacePiece.Invoke();
        }

        gameController.PlaceToken(move, TokenType.AI);

        var token = Instantiate(TokenPrefabToPlace);
        TokenZoneController.instance.PlaceToken(token, move);
    }

    private IEnumerator GetMove(MoveJob moveJob, JobMonitor jobMonitor, OnMoveJobComplete onMoveJobComplete = null)
    {
        Thread thread;        

        thread = GetMoveAsync(moveJob);

        threads.Add(thread);

        thread.Start();

        //ensure that the thread has started
        yield return new WaitUntil(() => thread.IsAlive);

        jobMonitor.started = true;

        //wait for the thread to finish
        yield return new WaitWhile(() => thread.IsAlive);

        //mark the job as complete
        if (onMoveJobComplete != null)
        {
            onMoveJobComplete.Invoke(moveJob);
        }

        jobMonitor.complete = true;

        //remove the thread from the list of threads
        threads.Remove(thread);
    }

    private IEnumerator PredictMovesCoroutine()
    {
        BoardData board;
        int coreCount, childIndex;

        List<MoveJob> moveJobs;
        JobMonitor jobMonitor;
        
        MoveData[] children;

        predictedMoves.Clear();

        board = new BoardData(gameController.Board);

        coreCount = Mathf.Max(System.Environment.ProcessorCount - 1, 1);

        //get all possible moves that the other player could make
        children = GetAllChildrenOf(board, TokenType.Player, ref MoveJob.abortAll);

        //run MoveJobs for all possible moves asynchronously
        childIndex = 0;
        moveJobs = new List<MoveJob>();

        while (childIndex < children.Length)
        {
            yield return new WaitUntil(() => (MoveJob.abortAll || threads.Count < coreCount));

            if (MoveJob.abortAll)
            {
                //terminate this operation if all jobs are being aborted
                break;
            }

            jobMonitor = new JobMonitor();

            //create the job
            moveJobs.Add(new MoveJob()
            {
                move = Vector3Int.zero,
                board = children[childIndex].board,
                myToken = TokenType.AI,
                depth = depth,
                jobID = childIndex
            });

            //run the job
            StartCoroutine(GetMove(moveJobs[moveJobs.Count - 1], jobMonitor, AddJobToPredictions));

            //wait for the job to start
            yield return new WaitUntil(() => jobMonitor.started);

            //move to the next child
            childIndex++;
        }

        //wait for all jobs to finish
        yield return new WaitUntil(() => MoveJob.abortAll || threads.Count == 0);
    }

    private void AddJobToPredictions(MoveJob moveJob)
    {
        lock (predictedMoves)
        {
            predictedMoves[moveJob.board.GetXYZBoard()] = moveJob.move;
        }
    }

    //gets a move using a separate thread
    private static Thread GetMoveAsync(MoveJob moveJob)
    {
        return new Thread(() => moveJob.GetMove());
    }

    //returns the selected move on success and (-1, -1, -1) on failure
    private static Vector3Int FindMove(BoardData board, TokenType myToken, int depth, ref bool abort)
    {
        Vector3Int move = new Vector3Int(-1, -1, -1);

        //run AlphaBeta to find move
        AlphaBeta(board, myToken, depth, int.MinValue, int.MaxValue, true, ref move, ref abort);

        return move;
    }

    private static int AlphaBeta(BoardData leaf, TokenType current, int depth, int alpha, int beta, bool maximize, ref Vector3Int movePosition, ref bool abort)
    {
        MoveData[] childLeaves;
        Vector3Int placeHolder = Vector3Int.zero;
        TokenType token = TokenType.Empty;
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
                return EvaluateBoard(leaf);
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

    private static MoveData[] GetAllChildrenOf(BoardData leaf, TokenType current, ref bool abort)
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

    private static int EvaluateBoard(BoardData board)
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
        value += 2 * GetTokenValue(boardState, TokenType.AI);

        //opponent value
        value -= GetTokenValue(boardState, TokenType.Player);
        
        return value;
    }

    private static int GetTokenValue(string boardState, TokenType token)
    {
        int index, value = 0, count;
        string testStr;

        TokenType other;

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
        other = (token == TokenType.Player ? TokenType.AI : TokenType.Player);

        testStr = token.ToString() + " " + other.ToString() + " ";
        count = Regex.Matches(boardState, testStr).Count;

        value += 3 * count; //weight blocks heavier than most consecutive piece setups

        testStr = other.ToString() + " " + token.ToString() + " ";
        count = Regex.Matches(boardState, testStr).Count;

        value += 3 * count; //weight blocks heavier than most consecutive piece setups

        return value;
    }

    private delegate void OnMoveJobComplete(MoveJob moveJob);

    private class MoveData
    {
        public Vector3Int move;
        public BoardData board;
    }

    private class JobMonitor
    {
        public bool started;
        public bool complete;

        public JobMonitor()
        {
            started = false;
            complete = false;
        }
    }


    private class MoveJob
    {
        public Vector3Int move;
        public BoardData board;
        public TokenType myToken;
        public int depth;
        public int jobID;

        public static bool abortAll = false;

        public void GetMove()
        {
            move = FindMove(board, myToken, depth, ref abortAll);
        }
    }

}

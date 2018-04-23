/*
 * 
 * Author: Andrew Frost
 * 
 * Copy Right (c) 2018 All Rights Reserved
 * 
 * 4/21/2018
 * 
 */

using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

//the game's AI. 
public class GameAI : MonoBehaviour
{
    public Token myToken = Token.red;

    private GameController gameController;

    private void Awake()
    {
        gameController = GetComponent<GameController>();
    }

    // Use this for initialization
    private void Start ()
    {
		
	}
	
	// Update is called once per frame
	private void Update ()
    {
		
	}

    public static void FindMove(BoardData board, Token myToken, int depth)
    {
        //run AlphaBeta to find move
    }

    private static int AlphaBeta(BoardData leaf, Token current, int depth, int alpha, int beta, bool maximize, ref Vector3Int movePosition)
    {
        MoveData[] childLeaves;
        Vector3Int placeHolder = Vector3Int.zero;
        Token token = Token.empty;
        int bestValue = 0, value, index;

        //null test BoardData
        if (leaf != null)
        {
            //check for depth being reached or if game is over
            if (depth <= 0 || GameStatus.IsGameOver(leaf, ref token))
            {
                return EvaluateBoard(leaf, current);
            }

            //generate child leaves
            childLeaves = GetAllChildrenOf(leaf, current);

            //search each child leaf for most valuable outcome recursively
            if (maximize)
            {
                bestValue = int.MinValue;

                for (index = 0; index < childLeaves.Length; index++)
                {
                    value = AlphaBeta(childLeaves[index].board, 
                                      GameStatus.GetOppositePlayerOf(current), 
                                      depth - 1, alpha, beta, false, 
                                      ref placeHolder);

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
                                      ref placeHolder);

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

    private static MoveData[] GetAllChildrenOf(BoardData leaf, Token current)
    {
        return new List<MoveData>().ToArray(); //to do implement
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

    private struct MoveData
    {
        public Vector3Int move;
        public BoardData board;
    }

}

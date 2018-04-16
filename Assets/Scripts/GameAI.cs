/*
 * 
 * Author: Andrew Frost
 * 
 * Copy Right (c) 2018 All Rights Reserved
 * 
 * 4/16/2018
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

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

        //to do implement

        return value;
    }

    private static int GetConsecutiveTokenValue(string boardState, Token token)
    {
        int index, value = 0, count;
        string testStr;

        testStr = "";

        for (index = 0; index < 4; index++)
        {
            testStr += token.ToString() + " ";

            //count number of token combo
            count = Regex.Matches(boardState, testStr).Count;

            value += (index + 1) * count;
        }

        return value;
    }
    
}

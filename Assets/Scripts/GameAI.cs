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
    
}

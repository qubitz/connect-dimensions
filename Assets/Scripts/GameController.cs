/*
 * 
 * Author: Andrew Frost
 * 
 * Copy Right (c) 2018 All Rights Reserved
 * 
 * 4/9/2018
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//runs the game, determines winner, etc
public class GameController : MonoBehaviour
{
    public Vector3Int boardSize = new Vector3Int(4, 4, 4);

    [SerializeField]
    private PlayerEvent winEvent = null;

    [SerializeField]
    private UnityEvent yellowPlayerTurn = null;

    [SerializeField]
    private UnityEvent redPlayerTurn = null;    

    private bool gameOver = false;

    [SerializeField]
    private Token currentPlayer = Token.yellow;

    private Token winner = Token.empty;

    private BoardData board;

	// Use this for initialization
	void Start ()
    {
        gameOver = false;
        currentPlayer = Token.yellow;
        board = new BoardData(boardSize);
        boardSize = board.Size;
	}

    public void PlaceToken(Vector3Int coordinate, Token token)
    {
        if (!gameOver 
            && token == currentPlayer 
            && board.TrySetValue(coordinate, token))
        {
            if (IsGameOver())
            {
                //set whichever player won as winner
                SetWinner();
            }
            else
            {
                //switch player
                ChangeTurn();
            }            
        }        
    }

    //check if the game has been won. Sets winner to the winning player if a winner exists
    public bool IsGameOver()
    {
        string test;
        Vector3Int position = Vector3Int.zero;

        test = "";

        // build test data 
        test = board.GetXYZBoard()
            + board.GetXZYBoard()
            + board.GetYXZBoard()
            + board.GetYZXBoard()
            + board.GetZXYBoard()
            + board.GetZYXBoard()
            + board.GetDxDyDzBoard()
            + board.GetDxDyInverseDzBoard()
            + board.GetDxInverseDyDzBoard()
            + board.GetDxInverseDyInverseDzBoard()
            + board.GetXDyDzBoard()
            + board.GetXDyDzInverseBoard()
            + board.GetYDxDzBoard()
            + board.GetYDxDzInverseBoard()
            + board.GetZDxDyBoard()
            + board.GetZDxDyInverseBoard();

        //check for yellow win
        if (IsWinner(Token.yellow, test))
        {
            winner = Token.yellow;

            return true;
        }

        //check for red win
        if (IsWinner(Token.red, test))
        {
            winner = Token.red;

            return true;
        }

        //check for draw
        if (!test.Contains(Token.empty.ToString()))
        {
            winner = Token.empty;

            return true;
        }

        return false;
    }

    private bool IsWinner(Token token, string test)
    {
        if (test.Contains(GetKey(token)))
        {
            return true;
        }

        return false;
    }

    private string GetKey(Token token)
    {
        int index;
        string key = "";

        for (index = 0; index < 4; index++)
        {
            key += token.ToString() + " ";
        }

        return key;
    }

    private void SetWinner()
    {
        gameOver = true;

        if (winEvent != null)
        {
            winEvent.Invoke(winner);
        }
        else
        {
            Debug.LogError("Cannot invoke win event: event is null!");
        }
    }

    private void ChangeTurn()
    {
        currentPlayer = (currentPlayer == Token.yellow ? Token.red : Token.yellow);

        InvokeTurnChange((currentPlayer == Token.yellow ? yellowPlayerTurn : redPlayerTurn), currentPlayer);
    }

    private static void InvokeTurnChange(UnityEvent callback, Token playerID)
    {
        if (callback != null)
        {
            callback.Invoke();
        }
        else
        {
            Debug.LogError("Cannot pass game to " + playerID.ToString() + " player: event is null!");
        }
    }
}

[System.Serializable]
public class PlayerEvent : UnityEvent<Token> { }

[System.Serializable]
public class TokenEvent : UnityEvent<Vector3Int, Token> { }

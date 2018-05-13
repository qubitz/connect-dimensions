/*
 * 
 * Author: Andrew Frost
 * 
 * Copyright (c) 2018 All Rights Reserved
 * 
 * 4/20/2018
 * 
 */
 
using UnityEngine;
using UnityEngine.Events;

//runs the game, determines winner, etc
public class GameController : MonoBehaviour
{
    public Vector3Int boardSize = new Vector3Int(4, 4, 4);

    [SerializeField]
    private PlayerEvent winEvent = null;

    [SerializeField]
    private UnityEvent playerTurn = null;

    [SerializeField]
    private UnityEvent aiTurn = null;    

    private bool gameOver = false;

    [SerializeField]
    private TokenType currentPlayer = TokenType.AI;

    private TokenType winner = TokenType.Empty;

    public BoardData Board
    {
        get
        {
            return board;
        }
    }

    private BoardData board;
    
	void Awake ()
    {
        gameOver = false;
        board = new BoardData(boardSize);
        boardSize = board.Size;
	}

    public void PlaceToken(Vector3Int coordinate, TokenType token)
    {
        Debug.Log("Piece placed " + token + " at " + coordinate +" and current player is " + currentPlayer);


        if (!gameOver 
            && token == currentPlayer 
            && board.TrySetValue(coordinate, token))
        {
            if (GameStatus.IsGameOver(board, ref winner))
            {
                //set whichever player won as winner
                SetWinner();
            }
            else
            {
                //switch player
                Debug.Log("Changing turn.");
                ChangeTurn();
            }            
        }        
    }    

    public string Print()
    {
        return board.ToString();
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
        currentPlayer = GameStatus.GetOppositePlayerOf(currentPlayer);

        InvokeTurnChange((currentPlayer == TokenType.Player ? playerTurn : aiTurn), currentPlayer);
    }

    private static void InvokeTurnChange(UnityEvent callback, TokenType playerID)
    {
        if (callback != null)
        {
            Debug.Log("Invoking " + playerID + "'s callback.");

            callback.Invoke();
        }
        else
        {
            Debug.LogError("Cannot pass game to " + playerID.ToString() + " player: event is null!");
        }
    }
}

[System.Serializable]
public class PlayerEvent : UnityEvent<TokenType> { }

[System.Serializable]
public class TokenEvent : UnityEvent<Vector3Int, TokenType> { }

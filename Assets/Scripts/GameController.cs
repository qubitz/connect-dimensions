/*
 * 
 * Author: Andrew Frost
 * 
 * Copy Right (c) 2018 All Rights Reserved
 * 
 * 4/15/2018
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
    private UnityEvent yellowPlayerTurn = null;

    [SerializeField]
    private UnityEvent redPlayerTurn = null;    

    private bool gameOver = false;

    [SerializeField]
    private Token currentPlayer = Token.yellow;

    private Token winner = Token.empty;

    private BoardData board;
    
	void Awake ()
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
        string boardState;
        Vector3Int position = Vector3Int.zero;

        boardState = "";

        // build test data 
        boardState = GameStatus.GetBoardState(board);

        //check for yellow win
        if (GameStatus.IsWinner(Token.yellow, boardState))
        {
            winner = Token.yellow;

            return true;
        }

        //check for red win
        if (GameStatus.IsWinner(Token.red, boardState))
        {
            winner = Token.red;

            return true;
        }

        //check for draw
        if (!boardState.Contains(Token.empty.ToString()))
        {
            winner = Token.empty;

            return true;
        }

        return false;
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

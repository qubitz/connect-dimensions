/*
 * 
 * Author: Andrew Frost
 * 
 * Copy Right (c) 2018 All Rights Reserved
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
    private UnityEvent yellowPlayerTurn = null;

    [SerializeField]
    private UnityEvent redPlayerTurn = null;    

    private bool gameOver = false;

    [SerializeField]
    private Token currentPlayer = Token.yellow;

    private Token winner = Token.empty;

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
            if (GameStatus.IsGameOver(board, ref winner))
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

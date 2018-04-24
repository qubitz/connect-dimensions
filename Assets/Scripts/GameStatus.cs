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

//helper class for helping to determine if a player has won and reading the game's board
public static class GameStatus
{
    //returns the board state
    public static string GetBoardState(BoardData board)
    {
        return board.GetXYZBoard()
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
    }

    public static Token GetOppositePlayerOf(Token token)
    {
        if (token == Token.empty)
        {
            return Token.empty;
        }

        return (token == Token.yellow ? Token.red : Token.yellow);
    }

    //check if the game has been won. Sets winner to the winning player if a winner exists
    public static bool IsGameOver(BoardData board, ref Token winner)
    {
        string boardState;
        Vector3Int position = Vector3Int.zero;

        boardState = "";

        // build test data 
        boardState = GameStatus.GetBoardState(board);

        //check for yellow win
        if (IsWinner(Token.yellow, boardState))
        {
            winner = Token.yellow;

            return true;
        }

        //check for red win
        if (IsWinner(Token.red, boardState))
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

    //returns a bool based on if token has won the game given a board state 
    public static bool IsWinner(Token token, string boardState)
    {
        if (boardState.Contains(GetWinKey(token)))
        {
            return true;
        }

        return false;
    }

    //returns a winning string for a given token
    public static string GetWinKey(Token token)
    {
        int index;
        string key = "";

        for (index = 0; index < 4; index++)
        {
            key += token.ToString() + " ";
        }

        return key;
    }
}

/*
 * 
 * Author: Andrew Frost
 * 
 * Copyright (c) 2018 All Rights Reserved
 * 
 * 4/24/2018
 * 
 */
using UnityEngine;
using System.Text;

//helper class for helping to determine if a player has won and reading the game's board
public static class GameStatus
{
    //returns the board state
    public static string GetBoardState(BoardData board)
    {
        StringBuilder stringBuilder = new StringBuilder(board.GetXYZBoard());

        stringBuilder.Append(board.GetXZYBoard());
        stringBuilder.Append(board.GetYXZBoard());
        stringBuilder.Append(board.GetYZXBoard());
        stringBuilder.Append(board.GetZXYBoard());
        stringBuilder.Append(board.GetZYXBoard());
        stringBuilder.Append(board.GetDxDyDzBoard());
        stringBuilder.Append(board.GetDxDyInverseDzBoard());
        stringBuilder.Append(board.GetDxInverseDyDzBoard());
        stringBuilder.Append(board.GetDxInverseDyInverseDzBoard());
        stringBuilder.Append(board.GetXDyDzBoard());
        stringBuilder.Append(board.GetXDyDzInverseBoard());
        stringBuilder.Append(board.GetYDxDzBoard());
        stringBuilder.Append(board.GetYDxDzInverseBoard());
        stringBuilder.Append(board.GetZDxDyBoard());
        stringBuilder.Append(board.GetZDxDyInverseBoard());


        return stringBuilder.ToString();
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
        boardState = GetBoardState(board);

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
        StringBuilder key = new StringBuilder("");

        for (index = 0; index < 4; index++)
        {
            key.Append(token.ToString() + " ");
        }

        return key.ToString();
    }
}

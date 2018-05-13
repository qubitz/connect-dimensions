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

    public static TokenType GetOppositePlayerOf(TokenType token)
    {
        if (token == TokenType.Empty)
        {
            return TokenType.Empty;
        }

        return (token == TokenType.AI ? TokenType.Player : TokenType.AI);
    }

    //check if the game has been won. Sets winner to the winning player if a winner exists
    public static bool IsGameOver(BoardData board, ref TokenType winner)
    {
        string boardState;
        Vector3Int position = Vector3Int.zero;

        boardState = "";

        // build test data 
        boardState = GetBoardState(board);

        //check for yellow win
        if (IsWinner(TokenType.AI, boardState))
        {
            winner = TokenType.AI;

            return true;
        }

        //check for red win
        if (IsWinner(TokenType.Player, boardState))
        {
            winner = TokenType.Player;

            return true;
        }

        //check for draw
        if (!boardState.Contains(TokenType.Empty.ToString()))
        {
            winner = TokenType.Empty;

            return true;
        }

        return false;
    }

    //returns a bool based on if token has won the game given a board state 
    public static bool IsWinner(TokenType token, string boardState)
    {
        if (boardState.Contains(GetWinKey(token)))
        {
            return true;
        }

        return false;
    }

    //returns a winning string for a given token
    public static string GetWinKey(TokenType token)
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

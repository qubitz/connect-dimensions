/*
 * 
 * Author: Andrew Frost
 * 
 * Copy Right (c) 2018 All Rights Reserved
 * 
 * 4/15/2018
 * 
 */

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

﻿/*
 * 
 * Author: Andrew Frost
 * 
 * Copy Right (c) 2018 All Rights Reserved
 * 
 * 4/6/2018
 * 
 */

using UnityEngine;

//this class stores the gameboard's data
public class BoardData
{
    //the game's board
    private Token[,,] board = null;

    private Vector3Int size = Vector3Int.zero;

    public Vector3Int Size
    {
        get
        {
            return size;
        }
    }

    //allocate board and initialize each value as empty
    public BoardData(Vector3Int size)
    {
        int x, y, z;        

        //ensure size is of each dimension is at least 4
        Mathf.Max(4, size.x);
        Mathf.Max(4, size.y);
        Mathf.Max(4, size.z);

        this.size = size;

        board = new Token[size.x, size.y, size.z];

        for (x = 0; x < board.GetLength(0); x++)
        {
            for (y = 0; y < board.GetLength(1); y++)
            {
                for (z = 0; z < board.GetLength(2); z++)
                {
                    board[x, y, z] = Token.empty;
                }
            }
        }        
    }

    //allocate board and initialize each value as 0
    public BoardData(BoardData other)
    {
        int x, y, z;

        size = other.size;

        board = new Token[size.x, size.y, size.y];

        for (x = 0; x < board.GetLength(0); x++)
        {
            for (y = 0; y < board.GetLength(1); y++)
            {
                for (z = 0; z < board.GetLength(2); z++)
                {
                    board[x, y, z] = other.board[x, y, z];
                }
            }
        }
    }

    //check is coordinate is in bounds of the board
	public bool IsInBounds(Vector3Int coordinate)
    {
        return (board != null 
                && coordinate.x >= 0 
                && coordinate.x < board.GetLength(0) 
                && coordinate.y >= 0 
                && coordinate.y < board.GetLength(1) 
                && coordinate.z >= 0 
                && coordinate.z < board.GetLength(2));
    }

    //checks if a token can be placed at coordinate
    public bool IsSpaceAvailable(Vector3Int coordinate)
    {
        //bounds and availability checking
        if (!IsInBounds(coordinate) 
            || board[coordinate.x, coordinate.y, coordinate.z] != Token.empty)
        {
            return false;
        }

        //can the token be stacked here?
        if (coordinate.z == 0)
        {
            return true;
        }

        return board[coordinate.x, coordinate.y, coordinate.z - 1] != Token.empty;
    }

    //retrieves the token value at coordinate if coordinate is in bounds
    public bool TryGetValue(Vector3Int coordinate, out Token token)
    {
        token = Token.empty;

        if (!IsInBounds(coordinate))
        {
            return false;
        }

        token = board[coordinate.x, coordinate.y, coordinate.z];

        return true;
    }

    //sets the token value at coordinate if the space specified by coordinate is available
    public bool TrySetValue(Vector3Int coordinate, Token token)
    {
        if (!IsSpaceAvailable(coordinate))
        {
            return false;
        }

        board[coordinate.x, coordinate.y, coordinate.z] = token;

        return true;
    }

}

// the defining pieces of the board
public enum Token
{    
    empty,
    red,
    yellow
}

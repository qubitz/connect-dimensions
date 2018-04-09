/*
 * 
 * Author: Andrew Frost
 * 
 * Copy Right (c) 2018 All Rights Reserved
 * 
 * 4/9/2018
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

    public string GetXYZBoard()
    {
        Vector3Int position = Vector3Int.zero;

        string boardStr = "";

        for (position.x = 0; position.x < Size.x; position.x++)
        {
            for (position.y = 0; position.y < Size.y; position.y++)
            {
                for (position.z = 0; position.z < Size.z; position.z++)
                {
                    boardStr += board[position.x, position.y, position.z].ToString() + " ";
                }

                boardStr += '/';
            }
        }

        return boardStr;
    }

    public string GetYXZBoard()
    {
        Vector3Int position = Vector3Int.zero;

        string boardStr = "";

        for (position.y = 0; position.y < Size.y; position.y++) 
        {
            for (position.x = 0; position.x < Size.x; position.x++)
            {
                for (position.z = 0; position.z < Size.z; position.z++)
                {
                    boardStr += board[position.x, position.y, position.z].ToString() + " ";
                }

                boardStr += '/';
            }
        }

        return boardStr;
    }

    public string GetYZXBoard()
    {
        Vector3Int position = Vector3Int.zero;

        string boardStr = "";

        for (position.y = 0; position.y < Size.y; position.y++)
        {
            for (position.z = 0; position.z < Size.z; position.z++)
            {
                for (position.x = 0; position.x < Size.x; position.x++) 
                {
                    boardStr += board[position.x, position.y, position.z].ToString() + " ";
                }

                boardStr += '/';
            }
        }

        return boardStr;
    }

    public string GetZXYBoard()
    {
        Vector3Int position = Vector3Int.zero;

        string boardStr = "";

        for (position.z = 0; position.z < Size.z; position.z++)
        {
            for (position.x = 0; position.x < Size.x; position.x++)
            {
                for (position.y = 0; position.y < Size.y; position.y++)
                {
                    boardStr += board[position.x, position.y, position.z].ToString() + " ";
                }

                boardStr += '/';
            }
        }

        return boardStr;
    }

    public string GetZYXBoard()
    {
        Vector3Int position = Vector3Int.zero;

        string boardStr = "";

        for (position.z = 0; position.z < Size.z; position.z++)
        {
            for (position.y = 0; position.y < Size.y; position.y++)
            {
                for (position.x = 0; position.x < Size.x; position.x++)
                {
                    boardStr += board[position.x, position.y, position.z].ToString() + " ";
                }

                boardStr += '/';
            }
        }

        return boardStr;
    }

    public string GetXZYBoard()
    {
        Vector3Int position = Vector3Int.zero;

        string boardStr = "";

        for (position.x = 0; position.x < Size.x; position.x++)
        {
            for (position.z = 0; position.z < Size.z; position.z++)
            {
                for (position.y = 0; position.y < Size.y; position.y++)
                {
                    boardStr += board[position.x, position.y, position.z].ToString() + " ";
                }

                boardStr += '/';
            }
        }

        return boardStr;
    }

    public string GetXDyDzBoard()
    {
        Vector3Int position = Vector3Int.zero;

        int slice, w1, w2, index;

        string boardStr = "";

        for (position.x = 0; position.x < Size.x; position.x++)
        {           
            for (slice = 0; slice < Size.y + Size.z - 1; slice++)
            {
                w1 = slice < Size.z ? 0 : slice - Size.z + 1;
                w2 = slice < Size.y ? 0 : slice - Size.y + 1;

                for (index = slice - w2; index >= w1; index--)
                {
                    position.y = index;
                    position.z = slice - index;

                    boardStr += board[position.x, position.y, position.z].ToString() + " ";
                }

                boardStr += '/';
            }   
        }

        return boardStr;
    }

    public string GetXDyDzInverseBoard()
    {
        Vector3Int position = Vector3Int.zero;

        int slice, w1, w2, index;

        string boardStr = "";

        for (position.x = 0; position.x < Size.x; position.x++)
        {
            for (slice = 0; slice < Size.y + Size.z - 1; slice++)
            {
                w1 = slice < Size.z ? 0 : slice - Size.z + 1;
                w2 = slice < Size.y ? 0 : slice - Size.y + 1;

                for (index = (Size.y - 1) - slice + w2; 
                                            index <= (Size.y - 1) - w1; index++)
                {
                    position.y = index;
                    position.z = index + (slice - Size.y + 1);

                    boardStr += board[position.x, position.y, position.z].ToString() + " ";
                }

                boardStr += '/';
            }
        }

        return boardStr;
    }

    public string GetYDxDzBoard()
    {
        Vector3Int position = Vector3Int.zero;

        int slice, w1, w2, index;

        string boardStr = "";

        for (position.y = 0; position.y < Size.y; position.y++)
        {
            for (slice = 0; slice < Size.x + Size.z - 1; slice++)
            {
                w1 = slice < Size.z ? 0 : slice - Size.z + 1;
                w2 = slice < Size.x ? 0 : slice - Size.x + 1;

                for (index = slice - w2; index >= w1; index--)
                {
                    position.x = index;
                    position.z = slice - index;

                    boardStr += board[position.x, position.y, position.z].ToString() + " ";
                }

                boardStr += '/';
            }
        }

        return boardStr;
    }

    public string GetYDxDzInverseBoard()
    {
        Vector3Int position = Vector3Int.zero;

        int slice, w1, w2, index;

        string boardStr = "";

        for (position.y = 0; position.y < Size.y; position.y++)
        {
            for (slice = 0; slice < Size.x + Size.z - 1; slice++)
            {
                w1 = slice < Size.z ? 0 : slice - Size.z + 1;
                w2 = slice < Size.x ? 0 : slice - Size.x + 1;

                for (index = (Size.x - 1) - slice + w2;
                                            index <= (Size.x - 1) - w1; index++)
                {
                    position.x = index;
                    position.z = index + (slice - Size.x + 1);

                    boardStr += board[position.x, position.y, position.z].ToString() + " ";
                }

                boardStr += '/';
            }
        }

        return boardStr;
    }

    public string GetZDxDyBoard()
    {
        Vector3Int position = Vector3Int.zero;

        int slice, w1, w2, index;

        string boardStr = "";

        for (position.z = 0; position.z < Size.z; position.z++)
        {
            for (slice = 0; slice < Size.x + Size.y - 1; slice++)
            {
                w1 = slice < Size.y ? 0 : slice - Size.y + 1;
                w2 = slice < Size.x ? 0 : slice - Size.x + 1;

                for (index = slice - w2; index >= w1; index--)
                {
                    position.x = index;
                    position.y = slice - index;

                    boardStr += board[position.x, position.y, position.z].ToString() + " ";
                }

                boardStr += '/';
            }
        }

        return boardStr;
    }

    public string GetZDxDyInverseBoard()
    {
        Vector3Int position = Vector3Int.zero;

        int slice, w1, w2, index;

        string boardStr = "";

        for (position.z = 0; position.z < Size.z; position.z++)
        {
            for (slice = 0; slice < Size.x + Size.y - 1; slice++)
            {
                w1 = slice < Size.y ? 0 : slice - Size.y + 1;
                w2 = slice < Size.x ? 0 : slice - Size.x + 1;

                for (index = (Size.x - 1) - slice + w2;
                                            index <= (Size.x - 1) - w1; index++)
                {
                    position.x = index;
                    position.y = index + (slice - Size.x + 1);

                    boardStr += board[position.x, position.y, position.z].ToString() + " ";
                }

                boardStr += '/';
            }
        }

        return boardStr;
    }

    public string GetDxDyInverseDzBoard()
    {
        Vector3Int position = Vector3Int.zero, coordinate = Vector3Int.zero;

        string boardStr = "";

        for (coordinate.x = 0; coordinate.x < Size.x; coordinate.x++)
        {
            for (coordinate.y = 0; coordinate.y < Size.y; coordinate.y++)
            {
                for (coordinate.z = 0; coordinate.z < Size.z; coordinate.z++)
                {
                    position = coordinate;

                    while (IsInBounds(position))
                    {
                        boardStr += board[position.x, position.y, position.z].ToString() + " ";

                        position += new Vector3Int(1, -1, 1);
                    }

                    boardStr += '/';
                }
            }
        }

        return boardStr;
    }

    public string GetDxInverseDyDzBoard()
    {
        Vector3Int position = Vector3Int.zero, coordinate = Vector3Int.zero;

        string boardStr = "";

        for (coordinate.x = 0; coordinate.x < Size.x; coordinate.x++)
        {
            for (coordinate.y = 0; coordinate.y < Size.y; coordinate.y++)
            {
                for (coordinate.z = 0; coordinate.z < Size.z; coordinate.z++)
                {
                    position = coordinate;

                    while (IsInBounds(position))
                    {
                        boardStr += board[position.x, position.y, position.z].ToString() + " ";

                        position += new Vector3Int(-1, 1, 1);
                    }

                    boardStr += '/';
                }
            }
        }

        return boardStr;
    }

    public string GetDxDyDzBoard()
    {
        Vector3Int position = Vector3Int.zero, coordinate = Vector3Int.zero;

        string boardStr = "";

        for (coordinate.x = 0; coordinate.x < Size.x; coordinate.x++)
        {
            for (coordinate.y = 0; coordinate.y < Size.y; coordinate.y++)
            {
                for (coordinate.z = 0; coordinate.z < Size.z; coordinate.z++)
                {
                    position = coordinate;

                    while (IsInBounds(position))
                    {
                        boardStr += board[position.x, position.y, position.z].ToString() + " ";

                        position += new Vector3Int(1, 1, 1);
                    }

                    boardStr += '/';
                }
            }
        }

        return boardStr;
    }

    public string GetDxInverseDyInverseDzBoard()
    {
        Vector3Int position = Vector3Int.zero, coordinate = Vector3Int.zero;

        string boardStr = "";

        for (coordinate.x = 0; coordinate.x < Size.x; coordinate.x++)
        {
            for (coordinate.y = 0; coordinate.y < Size.y; coordinate.y++)
            {
                for (coordinate.z = 0; coordinate.z < Size.z; coordinate.z++)
                {
                    position = coordinate;

                    while (IsInBounds(position))
                    {
                        boardStr += board[position.x, position.y, position.z].ToString() + " ";

                        position += new Vector3Int(-1, -1, 1);
                    }

                    boardStr += '/';
                }
            }
        }

        return boardStr;
    }
}

// the pieces of the board
[System.Serializable]
public enum Token
{    
    empty,
    red,
    yellow
}

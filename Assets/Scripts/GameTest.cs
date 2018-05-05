/*
 * 
 * Author: Andrew Frost
 * 
 * Copyright (c) 2018 All Rights Reserved
 * 
 * 4/15/2018
 * 
 */

using UnityEngine;

public class GameTest : MonoBehaviour
{

    public Vector3Int position = Vector3Int.zero;

    private GameController gameController;

    private Token token;

	// Use this for initialization
	void Start ()
    {
        gameController = GetComponent<GameController>();
        token = Token.Yellow;

        Debug.Log(gameController.Print());
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.Space))
        {
            gameController.PlaceToken(position, token);

            Debug.Log(gameController.Print());
        }
	}

    public void SetYellow()
    {
        token = Token.Yellow;
    }
    
    public void SetRed()
    {
        token = Token.Red;
    }

    public void PrintWinner(Token token)
    {
        Debug.Log(token.ToString() + " won!");
    }
}

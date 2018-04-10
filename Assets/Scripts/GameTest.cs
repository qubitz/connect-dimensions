using System.Collections;
using System.Collections.Generic;
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
        token = Token.yellow;

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
        token = Token.yellow;
    }
    
    public void SetRed()
    {
        token = Token.red;
    }

    public void PrintWinner(Token token)
    {
        Debug.Log(token.ToString() + " won!");
    }
}

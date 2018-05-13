/*
 * 
 * Author: Andrew Frost
 * 
 * Copyright (c) 2018 All Rights Reserved
 * 
 * 5/13/2018
 * 
 */
using UnityEngine;
using UnityEngine.UI;

public class WinLoseSetter : MonoBehaviour
{

    public TextMesh textMesh = null;

    private void Start()
    {
        if (!textMesh)
        {
            textMesh = GetComponent<TextMesh>();
        }

        ResetText();
    }

    public void SetWinner(TokenType type)
    {
        switch (type)
        {
            case TokenType.AI:
                {
                    textMesh.text = "Loss!";
                    break;
                }
            case TokenType.Player:
                {
                    textMesh.text = "Win!";
                    break;
                }
            case TokenType.Empty:
                {
                    textMesh.text = "Draw!";
                    break;
                }
            default:
                {
                    textMesh.text = "Error occured :(";
                    break;
                }
        }
    }

    public void ResetText()
    {
        textMesh.text = "";
    }
}

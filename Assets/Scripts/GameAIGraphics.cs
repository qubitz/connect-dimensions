/*
 * 
 * Author: Andrew Frost
 * 
 * Copyright (c) 2018 All Rights Reserved
 * 
 * 5/4/2018
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//moves an object to show if the game AI is thinking or if it has completed its move
public class GameAIGraphics : MonoBehaviour
{
    public float thinkingMovementRadius = 3f;
    public float thinkingMovementSpeed = 6f;

    public Color finishedColor = Color.green;
    public float colorChangeWaitTime = .75f;

    private bool thinking = false;
    private Vector3 originalPosition;

    private Vector3 currentPosition = Vector3.zero;

    private float angle;

    private void Start()
    {
        originalPosition = transform.position;
        thinking = false;

        angle = 0f;
    }

    public void ResetAIGraphics()
    {
        thinking = false;
    }

    private void Update()
    {
        angle += thinkingMovementSpeed * Time.deltaTime;

        if (thinking)
        {
            currentPosition.x = thinkingMovementRadius * Mathf.Cos(angle);
            currentPosition.y = thinkingMovementRadius * Mathf.Sin(angle);
            currentPosition.z = 0;

            currentPosition += originalPosition;

            currentPosition.x = Mathf.Lerp(transform.position.x, currentPosition.x, thinkingMovementSpeed * Time.deltaTime);
            currentPosition.y = Mathf.Lerp(transform.position.y, currentPosition.y, thinkingMovementSpeed * Time.deltaTime);
            currentPosition.z = Mathf.Lerp(transform.position.z, currentPosition.z, thinkingMovementSpeed * Time.deltaTime);
        }
        else
        {
            currentPosition.x = Mathf.Lerp(transform.position.x, originalPosition.x, thinkingMovementSpeed * Time.deltaTime);
            currentPosition.y = Mathf.Lerp(transform.position.y, originalPosition.y, thinkingMovementSpeed * Time.deltaTime);
            currentPosition.z = Mathf.Lerp(transform.position.z, originalPosition.z, thinkingMovementSpeed * Time.deltaTime);
        }

        transform.position = currentPosition;
    }

    //starts the thinking movement of the ai
    public void StartThinking()
    {
        thinking = true;
    }

    //stops the thinking movement of the ai
    public void StopThinking()
    {
        thinking = false;
    }

    //starts the place piece color change effect of the ai
    public void PlacePiece()
    {
        StartCoroutine(PlacePieceMovement());
    }

    //executes the thinking movement
    IEnumerator ThinkingMovement()
    {
        yield return null;
    }

    //executes the place piece color change
    IEnumerator PlacePieceMovement()
    {
        Color originalColor;
        Renderer renderer = GetComponent<Renderer>();
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        originalColor = renderer.material.color;

        propertyBlock.SetColor("_Color", finishedColor);
        renderer.SetPropertyBlock(propertyBlock);

        yield return new WaitForSeconds(colorChangeWaitTime);

        propertyBlock.SetColor("_Color", originalColor);
        renderer.SetPropertyBlock(propertyBlock);
    }
}

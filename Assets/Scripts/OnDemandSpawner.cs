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

public class OnDemandSpawner : MonoBehaviour
{
    public GameObject prefab = null;

    private GameObject lastSpawned = null;

    private void Start()
    {
        ResetSpawner();
    }

    public void ResetSpawner()
    {
        if (lastSpawned != null)
        {
            DestroyImmediate(lastSpawned);
        }

        Spawn();
    }

    public void Spawn()
    {
        if (prefab)
        {
            lastSpawned = Instantiate(prefab, transform.position, Quaternion.identity);
        }
    }
}

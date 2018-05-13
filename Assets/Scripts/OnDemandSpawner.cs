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

    public void Spawn()
    {
        if (prefab)
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
        }
    }
}

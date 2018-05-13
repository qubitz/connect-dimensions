using System;
using UnityEngine;
using VRTK;

public class TokenZoneController : MonoBehaviour
{
    public static TokenZoneController instance = null;

    [Header("Placement Settings")]
    [SerializeField]
    private Vector3 spawnPoint = Vector3.zero;
    [SerializeField]
    private float baseWidth = 0.3f;

    [Space]
    [SerializeField]
    [Tooltip("ObjectAutoGrab of which to attach the game board. If left empty the game board will be placed at (0,0,0) and disable the first ObjectAutoGrab it finds.")]
    private VRTK_ObjectAutoGrab autoGrabToAttach = null;
    public GameObject tokenBoardPrefab;
    public GameObject tokenZonePrefab;

    private GameObject zoneBoard;
    private GameObject[][][] zoneData;

    public GameObject TokenAt(int x, int y, int z)
    {
        GameObject token = null;

        try
        {
            token = zoneData[z][x][y];
        }
        catch (IndexOutOfRangeException)
        {
            token = null;
        }

        return token;
    }

    public GameObject TokenAt(Vector3Int index)
    {
        return TokenAt(index.x, index.y, index.z);
    }

    public void ConstructNewBoard(Vector3Int size, bool shouldReplace = true)
    {
        if (shouldReplace) Destroy(zoneBoard);

        zoneBoard = Instantiate(tokenBoardPrefab, spawnPoint, new Quaternion());
        var planes = new GameObject("Planes");
        planes.SetParentRelative(zoneBoard);
        zoneData = GridSpawner.ConstructPlanes(size, planes, tokenZonePrefab);

        FitCollider(size);
        FitBase(size);
        InitZoneData();
    }

    public void OnTokenPlaced(Vector3Int index)
    {
        Debug.Log("Token placed at: " + index);
        TokenAt(index + Vector3Int.up)?.SetActive(true);
        FindObjectOfType<GameController>().PlaceToken(index, Token.Red);
    }

    protected void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected void Start()
    {
        ConstructNewBoard(new Vector3Int(4, 4, 4));  // hardcoded

        if (autoGrabToAttach)
        {
            autoGrabToAttach.objectToGrab = zoneBoard.GetComponent<VRTK_InteractableObject>();
        }
        else
        {
            FindObjectOfType<VRTK_ObjectAutoGrab>().enabled = false;
        }
    }

    protected void InitZoneData()
    {
        for (int i = 0; i < zoneData.Length; i++)
        {
            for (int j = 0; j < zoneData[i].Length; j++)
            {
                for (int k = 0; k < zoneData[i][j].Length; k++)
                {
                    TokenSnapDropZone tokenZone;
                    if (tokenZone = zoneData[i][j][k].GetComponent<TokenSnapDropZone>())
                    {
                        tokenZone.index = new Vector3Int(j, k, i);
                        // Enable zone if zone is at bottom layer
                        tokenZone.gameObject.SetActive(k == 0);
                    }
                }
            }
        }
    }

    private void FitCollider(Vector3Int size)
    {
        var collider = zoneBoard.GetComponent<BoxCollider>();
        if (collider != null) collider.size = size;
    }

    private void FitBase(Vector3Int size)
    {
        var @base = zoneBoard.transform.Find("Base");
        float basePos = (size.y / 2.0f) + (baseWidth / 2.0f);
        @base.localPosition += Vector3.down * basePos;
        @base.localScale = new Vector3(size.x, baseWidth, size.z);
    }
}

using System.Collections;
using UnityEngine;
using VRTK;

public class TokenZoneController : MonoBehaviour
{
    public static TokenZoneController instance = null;

    [SerializeField]
    [Tooltip("ObjectAutoGrab of which to attach the game board. If left empty the game board will be placed at (0,0,0).")]
    private VRTK_ObjectAutoGrab autoGrabToAttach = null;
    public GameObject tokenBoardPrefab;
    public GameObject tokenZonePrefab;

    private GameObject zoneBoard;
    private GameObject[][][] zoneData;

    public GameObject TokenAt(int x, int y, int z)
    {
        return zoneData[z][x][y];
    }

    public GameObject TokenAt(Vector3Int index)
    {
        return zoneData[index.z][index.x][index.y];
    }

    public void ConstructNewBoard(Vector3Int size, bool shouldReplace = true)
    {
        if (shouldReplace) Destroy(zoneBoard);
        zoneBoard = Instantiate(tokenBoardPrefab);
        var planes = new GameObject("Planes");
        planes.SetParentRelative(zoneBoard);
        zoneData = GridSpawner.ConstructPlanes(size, planes, tokenZonePrefab);

        var collider = tokenBoardPrefab.GetComponent<BoxCollider>();
        if (collider != null) collider.size = size;
        InitZoneData();
    }

    public void OnTokenPlaced(Vector3Int index)
    {
        Debug.Log("Token placed at:" + index);
        // Call andys thing
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
        ConstructNewBoard(new Vector3Int(3, 4, 5));

        if (autoGrabToAttach)
        {
            autoGrabToAttach.objectToGrab = zoneBoard.GetComponent<VRTK_InteractableObject>();
        }
        else
        {
            autoGrabToAttach.enabled = false;
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
                        tokenZone.index = new Vector3Int(i, j, k);
                        // Enable zone only if zone is at bottom layer; otherwise false
                        tokenZone.enabled = (k == 0);
                    }
                }
            }
        }
    }
}

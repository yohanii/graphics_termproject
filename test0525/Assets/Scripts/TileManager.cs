using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] tilePrefabs;

    private Transform playerTransform;
    private PlayerMotor pm;
    private float spawnForward = -9.0f;
    private float spawnForwardOffset = 0.0f;
    private float spawnRight = 0.0f;
    private float tileLengthForward = 9.0f;
    private float tileLength = 9.0f;
    private int tile_idx = 0;

    private int amnTilesOnScreen = 9;
   

    private List<GameObject> activeTiles;
    private float safeZone = 21.0f;
    private int spawn_gap = 5;
    private int lastPrefabIndex = 0;

    private Vector3 dir;
    private Vector3 lastPos;
    // Start is called before the first frame update
    void Start()
    {
        pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        foreach( GameObject tp in tilePrefabs)
        {
            Transform tile = tp.transform.GetChild(0);
            var allChildren = tile.GetComponentsInChildren<Transform>();
            foreach(Transform child in allChildren)
            {
                if (child.gameObject.GetComponent<MeshCollider>() == null)
                {
                    child.gameObject.AddComponent<MeshCollider>();
                }
            }

        }
        lastPos = new Vector3(0.0f, 0.0f, -tileLength);
        dir = Vector3.forward;
        activeTiles = new List<GameObject>();
        /*SpawnTile(0);
        SpawnTile(0);
        SpawnTile(0);
        SpawnTile(0);
        SpawnTile(4);
        SpawnTile(5);
        SpawnTile(0);
        */
        for (int i = 0; i < amnTilesOnScreen; i++) {
            if (i < 4)
                SpawnTile(0);
            else if (i == 4)
                SpawnTile(2);
            else
                SpawnTile();
        }

    }

    // Update is called once per frame
    void Update()
    {
        //if(playerTransform.position.z-safeZone > (spawnZ - amnTilesOnScreen * tileLength))
        if(tile_idx - pm.tile_on <= spawn_gap)
        {
            SpawnTile();
            DeleteTile();
        }
        
    }

    private void SpawnTile(int prefabIndex = -1)
    {
        float angle;
        GameObject go;
        if (prefabIndex == -1)
            prefabIndex = RandomPrefabIndex();

        go = Instantiate(tilePrefabs[prefabIndex]) as GameObject;
        go.tag = "Tile";
        int[] param = new int[2] { tile_idx, prefabIndex };
        go.SendMessage("SetParams", param);
        go.transform.SetParent(transform);

        if (lastPrefabIndex == 4)
        {
            //transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));
            dir = Quaternion.AngleAxis(-90.0f, Vector3.up) * dir;
            //dir = Quaternion.Euler(0.0f, -90.0f, 0.0f) * dir;
        }
        else if (lastPrefabIndex == 5)
        {
            dir = Quaternion.AngleAxis(90.0f, Vector3.up) * dir;
        }
       
        
        angle = Vector3.Angle(Vector3.forward, dir);
        Vector3 cross = Vector3.Cross(Vector3.forward, dir);
        if (cross.y < 0) angle = -angle;
        // angle function not gives negative
        //Debug.Log("tile idx : " + tile_idx + " dir : " + dir + " rotation : " + angle);

        go.transform.Rotate(Vector3.up, angle);

        go.transform.position = lastPos + dir * (tileLength);

        
        //spawnForward += tileLength;
        lastPos = go.transform.position;
        lastPrefabIndex = prefabIndex;
        tile_idx++;

        activeTiles.Add(go);
    }
    private void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }

    private int RandomPrefabIndex()
    {
        if (tilePrefabs.Length <= 1)
            return 0;
        int randomIndex = lastPrefabIndex;
        while(randomIndex == lastPrefabIndex)
        {
            randomIndex = Random.Range(0,tilePrefabs.Length);
        }
        
        return randomIndex;
    }
}

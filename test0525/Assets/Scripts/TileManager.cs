using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] tilePrefabs;

    private Transform playerTransform;
    private float spawnZ = -9.0f;
    private float tileLength = 9.0f;
    private int amnTilesOnScreen = 7;

    private List<GameObject> activeTiles;
    private float safeZone = 15.0f;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        foreach( GameObject tp in tilePrefabs)
        {
            Transform tile = tp.transform.GetChild(0);
            var allChildren = tile.GetComponentsInChildren<Transform>();
            foreach(Transform child in allChildren)
            {
                child.gameObject.AddComponent<MeshCollider>();
            }

        }
        activeTiles = new List<GameObject>();
        for (int i = 0; i < amnTilesOnScreen; i++) { 
            SpawnTile();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerTransform.position.z-safeZone > (spawnZ - amnTilesOnScreen * tileLength))
        {
            SpawnTile();
            DeleteTile();
        }
    }

    private void SpawnTile(int prefabIndex = -1)
    {
        GameObject go;
        go = Instantiate(tilePrefabs[0]) as GameObject;
        go.transform.SetParent(transform);
        go.transform.position = Vector3.forward * spawnZ;
        spawnZ += tileLength;
        activeTiles.Add(go);
    }
    private void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }
}

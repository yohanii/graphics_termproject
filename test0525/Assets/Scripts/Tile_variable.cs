using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_variable : MonoBehaviour
{
    public int tile_idx;
    public int tile_type;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetParams(int[] objects)
    {
        int idx = objects[0];
        int type = objects[1];
        this.tile_idx = idx;
        this.tile_type = type;
    }
}

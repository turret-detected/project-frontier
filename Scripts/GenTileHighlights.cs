using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenTileHighlights : MonoBehaviour
{

    public int TerrainSizeX;
    public int TerrainSizeZ;
    public GameObject emptyTilePrefab;



    // Start is called before the first frame update
    void Start()
    {
        //Load Grid Highlights
        for(int x = 0; x < TerrainSizeX; x++) {
            for(int z = 0; z < TerrainSizeZ; z++) {
                Vector3 loc = new Vector3(x, 0, z);
                Instantiate(emptyTilePrefab, loc, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

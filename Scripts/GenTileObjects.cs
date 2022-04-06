using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenTileObjects : MonoBehaviour
{

    public int TerrainSizeX;
    public int TerrainSizeZ;
    public GameObject emptyTilePrefab;
    public int rockChance;
    public GameObject tinyRocks;
    public int grassChance;
    public GameObject smallGrass;
    private int roll;
    private Vector3 loc;


    // Start is called before the first frame update
    void Start()
    {
        GenObjects(emptyTilePrefab, 100);
        GenObjects(tinyRocks, rockChance);
        GenObjects(smallGrass, grassChance);
    }


    public void GenObjects(GameObject obj, int chance) {
        for(int x = 0; x < TerrainSizeX; x++) {
            for(int z = 0; z < TerrainSizeZ; z++) {
                roll = Random.Range(1, 99); // 1 to 100
                if (roll <= chance) {
                    loc = new Vector3(x, 0, z);
                    Instantiate(obj, loc, Quaternion.identity);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

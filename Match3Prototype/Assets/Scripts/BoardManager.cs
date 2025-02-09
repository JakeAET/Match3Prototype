using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] float tileSpawnDelay;
    private Tile[,] allTiles;

    // Start is called before the first frame update
    void Start()
    {
        allTiles = new Tile[width, height];
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Setup()
    {
        int count = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPos = new Vector2(i, j);
                GameObject thisTile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;
                thisTile.transform.parent = transform;
                thisTile.name = "Tile ( " + i + "," + j + " )";
                Tile tileRef = thisTile.GetComponent<Tile>();
                tileRef.spawnDelay = count * tileSpawnDelay;
                tileRef.Initialize();
                count++;
            }
        }
    }
}

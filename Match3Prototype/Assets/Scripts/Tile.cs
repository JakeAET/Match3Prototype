using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Tile : MonoBehaviour
{
    //0 = red
    //1 = blue
    //2 = green
    //3 = purple
    //4 = yellow

    //public int colorIndex = -1;
    public int column;
    public int row;
    public GameObject assignedElement;
    private BoardManager boardManager;

    // Start is called before the first frame update
    void Start()
    {
        boardManager = FindObjectOfType<BoardManager>();
    }

    void Update()
    {
        assignedElement = boardManager.allElements[column,row];
    }
}

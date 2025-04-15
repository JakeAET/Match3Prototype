using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class BoardManager : MonoBehaviour
{
    [SerializeField] bool spawnWithoutMatches = false;

    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offsetHeight;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject[] tileElements;
    [SerializeField] float tileSpawnDelay;
    [SerializeField] float elementSpawnHeight;
    //private Tile[,] allTiles;
    public GameObject[,] allElements;
    private FindMatches findMatches;
    private GameManager gameManager;
    private List<Element> roundMatches = new List<Element>();

    public int redSpawnRate = 1;
    public int blueSpawnRate = 1;
    public int greenSpawnRate = 1;
    public int purpleSpawnRate = 1;
    public int yellowSpawnRate = 1;

    private bool collapsingActive = false;

    public float xSpawnOffsetMult;
    public float ySpawnOffsetMult;

    int[,] prevBoardTilesGrid;
    int[,] boardTilesGrid;

    // Start is called before the first frame update
    void Start()
    {
        prevBoardTilesGrid = new int[width, height];
        boardTilesGrid = new int[width, height];
        gameManager = FindObjectOfType<GameManager>();
        findMatches = FindObjectOfType<FindMatches>();
        //allTiles = new Tile[width, height];
        allElements = new GameObject[width, height];
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            undoLastMove();
        }
    }

    public void Setup()
    {
        //int count = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPos = new Vector2(i * xSpawnOffsetMult, j * ySpawnOffsetMult);
                GameObject thisTile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;
                //Tile tileRef = thisTile.GetComponent<Tile>();
                thisTile.transform.parent = transform;
                thisTile.name = "Tile ( " + i + "," + j + " )";
                //tileRef.coord = new Vector2(i, j);

                int elementToUse = Random.Range(0, tileElements.Length);

                if (spawnWithoutMatches)
                {
                    int maxIterations = 0;

                    //check for matches
                    while (MatchesAt(i, j, tileElements[elementToUse]) && maxIterations < 100)
                    {
                        elementToUse = Random.Range(0, tileElements.Length);
                        maxIterations++;
                    }
                    maxIterations = 0; 
                }

                //tileRef.colorIndex = elementToUse;
                boardTilesGrid[i, j] = elementToUse;

                tempPos = new Vector2(i * xSpawnOffsetMult, (j * ySpawnOffsetMult) + offsetHeight);
                GameObject element = Instantiate(tileElements[elementToUse], tempPos, Quaternion.identity);
                element.GetComponent<Element>().row = j;
                element.GetComponent<Element>().column = i;
                element.transform.parent = transform;
                element.name = element.GetComponent<Element>().colorName + " Element";
                allElements[i, j] = element;
                //count++;

            }
        }
    }

    public void clearBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Destroy(allElements[i, j]);
                allElements[i, j] = null;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allElements[column - 1, row].tag == piece.tag && allElements[column - 2, row].tag == piece.tag)
            {
                return true;
            }

            if (allElements[column, row - 1].tag == piece.tag && allElements[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }
        else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if (allElements[column, row - 1].tag == piece.tag && allElements[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allElements[column - 1, row].tag == piece.tag && allElements[column - 2,row].tag == piece.tag)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allElements[column, row].GetComponent<Element>().isMatched)
        {
            roundMatches.Add(allElements[column, row].GetComponent<Element>());
            float scoreIncrease = gameManager.baseElementValue;

            if (findMatches.currentMatches.Count >= 4)
            {
                scoreIncrease += 5 * (findMatches.currentMatches.Count - 3);
                Debug.Log("biggg match made of size: " + findMatches.currentMatches.Count);
            }

            //Debug.Log("match length: " + findMatches.currentMatches.Count);
            findMatches.currentMatches.Remove(allElements[column, row]);
            Destroy(allElements[column, row]);
            gameManager.IncreaseScore(scoreIncrease * gameManager.streakValue);
            allElements[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++){
            for (int j = 0; j < height; j++)
            {
                if (allElements[i,j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRow());
    }

    private IEnumerator DecreaseRow()
    {
        //collapsingActive = true;
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allElements[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allElements[i,j].GetComponent<Element>().row -= nullCount;
                    allElements[i,j] = null;
                }
            }
            nullCount = 0;
        }

        yield return new WaitForSeconds(0.2f);
        StartCoroutine(FillBoard());
        collapsingActive = false;
    }

    public void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allElements[i,j] == null)
                {
                    Vector2 tempPos = new Vector2(i, j + offsetHeight);


                    int elementToUse = weightedElementToUse();


                    GameObject element = Instantiate(tileElements[elementToUse], tempPos, Quaternion.identity);
                    element.name = element.GetComponent<Element>().colorName + " Element";
                    allElements[i,j] = element;
                    element.GetComponent<Element>().row = j;
                    element.GetComponent<Element>().column = i;
                    element.transform.parent = transform;
                }
            }
        }
    }

    public void ResetBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allElements[i, j] == null)
                {
                    Vector2 tempPos = new Vector2(i, j + offsetHeight);

                    int elementToUse = weightedElementToUse();

                    if (spawnWithoutMatches)
                    {
                        int maxIterations = 0;

                        //check for matches
                        while (MatchesAt(i, j, tileElements[elementToUse]) && maxIterations < 100)
                        {
                            elementToUse = weightedElementToUse();
                            maxIterations++;
                        }
                        maxIterations = 0;
                    }

                    boardTilesGrid[i, j] = elementToUse;
                    prevBoardTilesGrid[i,j] = elementToUse;

                    GameObject element = Instantiate(tileElements[elementToUse], tempPos, Quaternion.identity);
                    element.name = element.GetComponent<Element>().colorName + " Element";
                    allElements[i, j] = element;
                    element.GetComponent<Element>().row = j;
                    element.GetComponent<Element>().column = i;
                    element.transform.parent = transform;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allElements[i,j] != null)
                {
                    if (allElements[i,j].GetComponent<Element>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoard()
    {
        RefillBoard();
        findMatches.FindAllMatchesStart();

        yield return new WaitForSeconds(0.5f);

        while (MatchesOnBoard())
        {
            gameManager.increaseStreak();
            yield return new WaitForSeconds(0.8f);
            collapsingActive = true;
            DestroyMatches();
        }
        StartCoroutine(checkTurnEnd());
        yield return new WaitForSeconds(0.5f);
        gameManager.streakValue = 1;
    }

    private IEnumerator checkTurnEnd()
    {
        //yield return new WaitForSeconds(0.6f);
        yield return null;
        if (!MatchesOnBoard() && !collapsingActive)
        {
            int red = 0;
            int blue = 0;
            int orange = 0;
            int yellow = 0;
            int green = 0;
            foreach (Element element in roundMatches)
            {
                if(element.colorName == "Red")
                {
                    red++;
                }
                if (element.colorName == "Blue")
                {
                    blue++;
                }
                if (element.colorName == "Orange")
                {
                    orange++;
                }
                if (element.colorName == "Yellow")
                {
                    yellow++;
                }
                if (element.colorName == "Green")
                {
                    green++;
                }
            }
            Debug.Log("turn " + (gameManager.maxTurns - gameManager.currentTurn) + " - red: " + red + " , blue: " + blue + " , orange: " + orange + " , yellow: " + yellow + " , green: " + green);
            roundMatches.Clear();
            Debug.Log("all matches collapsed");
            gameManager.turnEnded();
        }
    }

    private int weightedElementToUse()
    {
        //0 = red
        //1 = blue
        //2 = green
        //3 = purple
        //4 = yellow

        List<int> elementInts = new List<int>();

        for (int i = 0; i < redSpawnRate; i++)
        {
            elementInts.Add(0);
        }

        for (int i = 0; i < blueSpawnRate; i++)
        {
            elementInts.Add(1);
        }

        for (int i = 0; i < greenSpawnRate; i++)
        {
            elementInts.Add(2);
        }

        for (int i = 0; i < purpleSpawnRate; i++)
        {
            elementInts.Add(3);
        }

        for (int i = 0; i < yellowSpawnRate; i++)
        {
            elementInts.Add(4);
        }

        return elementInts[Random.Range(0, elementInts.Count)];
    }

    public void reassignTileIDs()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int temp = boardTilesGrid[i, j];
                prevBoardTilesGrid[i, j] = temp;
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allElements[i, j] != null)
                {
                    Element elementRef = allElements[i, j].GetComponent<Element>();
                    boardTilesGrid[elementRef.column, elementRef.row] = elementRef.colorIndex;
                }
            }
        }
    }

    public void undoLastMove()
    {
        Debug.Log("UNDO USED");
        clearBoard();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allElements[i, j] == null)
                {
                    Vector2 tempPos = new Vector2(i, j + offsetHeight);

                    int elementToUse = prevBoardTilesGrid[i, j];
                    boardTilesGrid[i, j] = elementToUse;

                    GameObject element = Instantiate(tileElements[elementToUse], tempPos, Quaternion.identity);
                    element.name = element.GetComponent<Element>().colorName + " Element";
                    allElements[i, j] = element;
                    element.GetComponent<Element>().row = j;
                    element.GetComponent<Element>().column = i;
                    element.transform.parent = transform;
                }
            }
        }
    }
}

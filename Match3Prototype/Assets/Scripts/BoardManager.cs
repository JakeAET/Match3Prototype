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
    private Tile[,] allTiles;
    public GameObject[,] allElements;
    private FindMatches findMatches;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        findMatches = FindObjectOfType<FindMatches>();
        allTiles = new Tile[width, height];
        allElements = new GameObject[width, height];
        Setup();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup()
    {
        //int count = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPos = new Vector2(i, j);
                GameObject thisTile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;
                thisTile.transform.parent = transform;
                thisTile.name = "Tile ( " + i + "," + j + " )";

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

                tempPos = new Vector2(i,j + offsetHeight);
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
            float scoreIncrease = gameManager.baseElementValue;

            if (findMatches.currentMatches.Count >= 4)
            {
                scoreIncrease += 5 * (findMatches.currentMatches.Count - 3);
            }

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

        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillBoard());
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


                    int elementToUse = Random.Range(0, tileElements.Length);


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

        yield return new WaitForSeconds(0.5f);

        while (MatchesOnBoard())
        {
            gameManager.increaseStreak();
            yield return new WaitForSeconds(0.5f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(0.5f);
        currentState = GameState.move;
        gameManager.turnEnded();
        gameManager.streakValue = 1;
    }
}

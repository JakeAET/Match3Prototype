using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum GameState
{
    // Initial state
    Init,

    // Board is filled, wait for player action
    Waiting,

    // Initialize board for new round
    SettingBoard,

    // Swap two tiles
    MovingTiles,

    // Check board for matches
    CheckMatches,

    // Clear all matched tiles
    DestroyMatches,

    // Refill empty slots after cleared matches
    RefillBoard,

    // Add elemental effects to existing tiles
    CreateElementalTiles,

    // Round won or lost
    RoundEnded
}

public class BoardManager : MonoBehaviour
{
    [SerializeField] bool spawnWithoutMatches = false;

    // Game States
    public GameState currentState = GameState.Init;
    private bool boardInitialized = false;
    private bool tilesInitialized = false;
    private bool turnStarted = false;
    private bool turnOver = true;
    private bool enchantedTilesCreated = false;
    private bool frozenTilesCreated = false;
    private bool findingMatches = false;
    private bool matchesToDestroy = false;
    private bool destroyingMatches = false;
    private bool refillingBoard = false;

    private bool checkMatchesInitialized = false;
    private bool destroyMatchesInitialized = false;

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
    [SerializeField] GameObject textPopup;
    [SerializeField] GameObject worldspaceCanvas;

    public int redSpawnRate = 1;
    public int blueSpawnRate = 1;
    public int greenSpawnRate = 1;
    public int purpleSpawnRate = 1;
    public int yellowSpawnRate = 1;
    public int maxEnchantedTiles;
    public int currentEnchantedTiles;
    public int maxFrozenTiles;
    public int currentFrozenTiles;

    //private bool collapsingActive = false;

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
        currentState = GameState.SettingBoard;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            undoLastMove();
        }

        #region State Actions
        switch (currentState)
        {
            case GameState.Waiting:

                break;

            case GameState.SettingBoard:
                if (!boardInitialized)
                {
                    Setup();
                }
                else
                {
                    ResetBoard();
                }

                break;

            case GameState.MovingTiles:
                turnStarted = true;
                turnOver= false;
                break;

            case GameState.CheckMatches:
                if (!checkMatchesInitialized)
                {
                    findMatches.FindAllMatchesStart();
                    checkMatchesInitialized = true;
                }
                break;

            case GameState.DestroyMatches:
                if (!destroyMatchesInitialized)
                {
                    DestroyMatches();
                    matchesToDestroy = false;
                    destroyMatchesInitialized = true;
                }
                break;

            case GameState.RefillBoard:
                StartCoroutine(RefillBoard());
                break;

            case GameState.CreateElementalTiles:
                if (maxEnchantedTiles > currentEnchantedTiles)
                {
                    StartCoroutine(checkToSpawnEnchanted(0f));
                }
                else
                {
                    enchantedTilesCreated = true;
                }

                if (maxFrozenTiles > currentFrozenTiles)
                {
                    StartCoroutine(checkToSpawnFrozen(0f));
                }
                else
                {
                    frozenTilesCreated = true;
                }

                break;
        }
        #endregion
        determineState();
    }

    public void Setup()
    {
        // Initialize board
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPos = new Vector2(i * xSpawnOffsetMult, j * ySpawnOffsetMult);
                GameObject thisTile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;
                thisTile.transform.parent = transform;
                thisTile.name = "Tile ( " + i + "," + j + " )";
            }
        }
        boardInitialized = true;
    }

    public void clearBoard()
    {
        StopAllCoroutines();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Destroy(allElements[i, j]);
                allElements[i, j] = null;
            }
        }
        currentEnchantedTiles = 0;
        currentFrozenTiles = 0;
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
        Element targetElement = allElements[column, row].GetComponent<Element>();
        if (targetElement.isMatched)
        {
            float scoreIncrease = gameManager.baseElementValue;
            float scoreMulti = 1;
            Color targetColor = Color.white;

            roundMatches.Add(allElements[column, row].GetComponent<Element>());

            // Large Match Bonus
            if (findMatches.currentMatches.Count >= 4)
            {
                scoreIncrease += gameManager.largeMatchBonus;
                //Debug.Log("biggg match made of size: " + findMatches.currentMatches.Count);
            }

            //Debug.Log("match length: " + findMatches.currentMatches.Count);

            // Enchanted Tiles
            if (targetElement.isEnchanted)
            {
                scoreMulti += gameManager.enchantedTileMulti;
                currentEnchantedTiles--;
                targetColor = Color.green;
            }

            if (targetElement.isFrozen)
            {
                currentFrozenTiles--;
            }

            if(scoreIncrease * scoreMulti >= 100)
            {
                targetColor = Color.red;
            }

            float targetX = column * xSpawnOffsetMult;
            float targetY = row * ySpawnOffsetMult;
            Vector2 targetPos = new Vector2(targetX, targetY);
            //Debug.Log("Tile Pos: " + targetPos + "Coord: " + column + ", " + row);
            findMatches.currentMatches.Remove(allElements[column, row]);
            GameObject newPopup = Instantiate(textPopup, targetPos, Quaternion.identity);
            newPopup.transform.parent = transform;
            newPopup.GetComponent<ScorePopup>().initialize(scoreIncrease * scoreMulti, targetColor);
            //newPopup.transform.parent = worldspaceCanvas.transform;
            Destroy(allElements[column, row]);

            // Increase Score
            gameManager.IncreaseScore(scoreIncrease * scoreMulti);
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

        yield return new WaitForSeconds(0f);

        destroyingMatches = false;
        //StartCoroutine(FillBoard());
        //collapsingActive = false;
    }

    public IEnumerator RefillBoard()
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
        yield return new WaitForSeconds(0.3f);
        refillingBoard = false;
    }

    public void ResetBoard()
    {
        // Fill board with elements
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allElements[i, j] == null)
                {
                    Vector2 tempPos = new Vector2(i * xSpawnOffsetMult, j * ySpawnOffsetMult);

                    int elementToUse = weightedElementToUse();

                    if (spawnWithoutMatches)
                    {
                        int maxIterations = 0;

                        // Check for potential matches before choosing element to use
                        while (MatchesAt(i, j, tileElements[elementToUse]) && maxIterations < 100)
                        {
                            elementToUse = weightedElementToUse();
                            maxIterations++;
                        }
                        maxIterations = 0;
                    }

                    boardTilesGrid[i, j] = elementToUse;
                    prevBoardTilesGrid[i,j] = elementToUse;

                    tempPos = new Vector2(i * xSpawnOffsetMult, (j * ySpawnOffsetMult) + offsetHeight);
                    GameObject element = Instantiate(tileElements[elementToUse], tempPos, Quaternion.identity);
                    element.name = element.GetComponent<Element>().colorName + " Element";
                    allElements[i, j] = element;
                    element.GetComponent<Element>().row = j;
                    element.GetComponent<Element>().column = i;
                    element.transform.parent = transform;
                }
            }
        }
        tilesInitialized = true;
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
        //RefillBoard();
        //findMatches.FindAllMatchesStart();

        yield return new WaitForSeconds(0.5f);

        //while (MatchesOnBoard())
        //{
        //    gameManager.increaseStreak();
        //    yield return new WaitForSeconds(0.8f);
        //    collapsingActive = true;
        //    DestroyMatches();
        //}
        //StartCoroutine(checkTurnEnd());
        //yield return new WaitForSeconds(0.5f);
        //gameManager.streakValue = 1;
    }

    //private IEnumerator checkTurnEnd()
    //{
    //    //yield return new WaitForSeconds(0.6f);
    //    yield return null;
    //    if (!MatchesOnBoard() && !collapsingActive)
    //    {
    //        int red = 0;
    //        int blue = 0;
    //        int orange = 0;
    //        int yellow = 0;
    //        int green = 0;
    //        foreach (Element element in roundMatches)
    //        {
    //            if(element.colorName == "Red")
    //            {
    //                red++;
    //            }
    //            if (element.colorName == "Blue")
    //            {
    //                blue++;
    //            }
    //            if (element.colorName == "Orange")
    //            {
    //                orange++;
    //            }
    //            if (element.colorName == "Yellow")
    //            {
    //                yellow++;
    //            }
    //            if (element.colorName == "Green")
    //            {
    //                green++;
    //            }
    //        }
    //        Debug.Log("turn " + (gameManager.maxTurns - gameManager.currentTurn) + " - red: " + red + " , blue: " + blue + " , orange: " + orange + " , yellow: " + yellow + " , green: " + green);
    //        roundMatches.Clear();
    //        Debug.Log("all matches collapsed");
    //        StartCoroutine(checkToSpawnFrozen(0f));
    //        StartCoroutine(checkToSpawnEnchanted(0f));
    //        gameManager.turnEnded();
    //    }
    //}

    private void turnEnded()
    {
        turnStarted = false;
        //int red = 0;
        //int blue = 0;
        //int orange = 0;
        //int yellow = 0;
        //int green = 0;
        //foreach (Element element in roundMatches)
        //{
        //    if (element.colorName == "Red")
        //    {
        //        red++;
        //    }
        //    if (element.colorName == "Blue")
        //    {
        //        blue++;
        //    }
        //    if (element.colorName == "Orange")
        //    {
        //        orange++;
        //    }
        //    if (element.colorName == "Yellow")
        //    {
        //        yellow++;
        //    }
        //    if (element.colorName == "Green")
        //    {
        //        green++;
        //    }
        //}
        //Debug.Log("turn " + (gameManager.maxTurns - gameManager.currentTurn) + " - red: " + red + " , blue: " + blue + " , orange: " + orange + " , yellow: " + yellow + " , green: " + green);
        //Debug.Log("all matches collapsed");
        roundMatches.Clear();
        gameManager.turnEnded();
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

    private IEnumerator checkToSpawnEnchanted(float delay)
    {
        yield return new WaitForSeconds(delay);

        int numToSpawn = maxEnchantedTiles - currentEnchantedTiles;
        for (int i = 0; i < numToSpawn; i++)
        {
            // pick an unenchanted element
            Element targetElement = allElements[Random.Range(0, width - 1), Random.Range(0, height - 1)].GetComponent<Element>();
            while (targetElement.isEnchanted)
            {
                targetElement = allElements[Random.Range(0, width - 1), Random.Range(0, height - 1)].GetComponent<Element>();
            }
            targetElement.enchantElement();
            currentEnchantedTiles++;
        }
        enchantedTilesCreated = true;
    }

    private IEnumerator checkToSpawnFrozen(float delay)
    {
        yield return new WaitForSeconds(delay);

        int numToSpawn = maxFrozenTiles - currentFrozenTiles;
        for (int i = 0; i < numToSpawn; i++)
        {
            // pick an unenchanted element
            Element targetElement = allElements[Random.Range(0, width - 1), Random.Range(0, height - 1)].GetComponent<Element>();
            while (targetElement.isFrozen)
            {
                targetElement = allElements[Random.Range(0, width - 1), Random.Range(0, height - 1)].GetComponent<Element>();
            }
            targetElement.freezeElement();
            currentFrozenTiles++;
        }
        frozenTilesCreated = true;
    }

    public void increaseMaxFrozenTiles(int num)
    {
        maxFrozenTiles += num;
    }

    public void increaseMaxEnchantedTiles(int num)
    {
        maxEnchantedTiles += num;
    }

    public void startFindingMatches()
    {
        if(currentState == GameState.MovingTiles || currentState == GameState.RefillBoard)
        {
            checkMatchesInitialized = false;
            findingMatches = true;
        }
    }

    public void allMatchesFound(bool matchesFound)
    {
        matchesToDestroy = matchesFound;
        findingMatches = false;
    }

    #region State Switch Conditions
    private void determineState()
    {
        switch (currentState)
        {
            case GameState.Waiting:

                break;

            case GameState.SettingBoard:
                if (boardInitialized && tilesInitialized)
                {
                    currentState = GameState.CreateElementalTiles;
                }
                break;

            case GameState.MovingTiles:
                if (findingMatches)
                {
                    Debug.Log("Moving Tiles -> Check Matches");
                    currentState = GameState.CheckMatches;
                }
                break;

            case GameState.CheckMatches:
                if (!findingMatches && matchesToDestroy)
                {
                    Debug.Log("Check Matches -> Destroy Matches");
                    destroyingMatches = true;
                    currentState = GameState.DestroyMatches;
                }
                if (!findingMatches && !matchesToDestroy)
                {
                    turnOver = true;
                    Debug.Log("Check Matches -> Create Elemental Tiles");
                    currentState = GameState.CreateElementalTiles;
                }
                break;

            case GameState.DestroyMatches:
                if (!destroyingMatches)
                {
                    destroyMatchesInitialized = false;
                    refillingBoard = true;
                    Debug.Log("Destroy Matches -> Refill Board");
                    currentState = GameState.RefillBoard;
                }
                break;

            case GameState.RefillBoard:
                if (!refillingBoard)
                {
                    Debug.Log("Refill Board -> Check Matches");
                    findingMatches = true;
                    checkMatchesInitialized = false;
                    currentState = GameState.CheckMatches;
                }
                break;

            case GameState.CreateElementalTiles:
                if (enchantedTilesCreated && frozenTilesCreated)
                {
                    Debug.Log("Create Elemental Tiles -> Waiting");
                    if (turnOver && turnStarted)
                    {
                        turnEnded();
                    }
                    currentState = GameState.Waiting;
                    enchantedTilesCreated = false;
                    frozenTilesCreated = false;
                }
                break;
            case GameState.RoundEnded:

                break;
        }
    }
    #endregion
}

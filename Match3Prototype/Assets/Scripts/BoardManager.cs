using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;
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
    private static BoardManager instance;

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
    private bool boardRefilled = false;
    private bool boardReset = false;
    private bool roundEnded = false;
    private bool iceBreakPlayed = false;

    private bool checkMatchesInitialized = false;
    private bool destroyMatchesInitialized = false;

    public int width;
    public int height;
    public int offsetHeight;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject[] tileElements;
    [SerializeField] float tileSpawnDelay;
    [SerializeField] float elementSpawnHeight;
    public Tile[,] allTiles;
    public GameObject[,] allElements;
    private FindMatches findMatches;
    private GameManager gameManager;
    private List<Element> roundMatches = new List<Element>();
    [SerializeField] GameObject textPopup;
    [SerializeField] Color[] popUpColors;
    [SerializeField] GameObject frozenBurstPrefab;
    [SerializeField] GameObject bombExplosionPrefab;
    [SerializeField] GameObject rocketPrefab;
    [SerializeField] GameObject bombPrefab;

    private int matchStreak = 1;
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

    ElementInfo[,] prevBoardTilesGrid;
    ElementInfo[,] boardTilesGrid;
    bool[,] maskedTilesGrid;

    [SerializeField] GameObject boardContainer;


    //0 = red
    //1 = blue
    //2 = green
    //3 = purple
    //4 = yellow

    public Dictionary<TargetColor, int> targetColorDict = new Dictionary<TargetColor, int>
    {
        {TargetColor.Red, 0},
        {TargetColor.Blue, 1},
        {TargetColor.Green, 2},
        {TargetColor.Purple, 3},
        {TargetColor.Yellow, 4},
    };

    public Color[] gemColors;

    // Boss Round
    public TargetColor banishedType;
    public float basePointDebuff = 1;

    // Events
    public delegate void TileDestroyed();
    public static event TileDestroyed OnRocketDestroyed;
    public static event TileDestroyed OnBombDestroyed;

    public delegate float ScoutTrigger(string colorName);
    public static event ScoutTrigger OnScoutTrigger;

    public delegate float RogueCritTrigger();
    public static event RogueCritTrigger OnRogueCritTrigger;

    public delegate float ElementalTrigger(bool isElemental);
    public static event ElementalTrigger OnFrozenTrigger;
    public static event ElementalTrigger OnEnchantedTrigger;

    public delegate float TurnBonus();
    public static event TurnBonus OnFirstTurn;
    public static event TurnBonus OnLastTurn;

    public delegate float UndoBonus();
    public static event UndoBonus OnUndoBonusTrigger;

    public delegate void abilityProcEffect();
    public static event abilityProcEffect OnFrozenTileCreated;
    public static event abilityProcEffect OnEnchantedTileCreated;
    public static event abilityProcEffect OnUndoUsed;
    public static event abilityProcEffect OnFullBoardSet;

    // Tilemap
    [SerializeField] Tilemap tileBase;
    [SerializeField] Tilemap currentTileMask;
    [SerializeField] GameObject[] tileMaskPrefabs;
    public List<Tilemap> availableTileMasks;
    [SerializeField] Tilemap tilemapBase;
    [SerializeField] Tilemap tilemapBasePrefab;

    public float ppDelayInc;

    [SerializeField] GameObject popupPrefab;
    //[SerializeField] Color[] comboColors;
    [SerializeField] float comboFontSize;
    public Vector3 boardCenter;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Board Manager in the scene.");
        }
        instance = this;
    }

    public static BoardManager GetInstance()
    {
        return instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        prevBoardTilesGrid = new ElementInfo[width, height];
        boardTilesGrid = new ElementInfo[width, height];
        maskedTilesGrid = new bool[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                prevBoardTilesGrid[i,j] = new ElementInfo();
                boardTilesGrid[i, j] = new ElementInfo();
            }
        }

        gameManager = FindObjectOfType<GameManager>();
        findMatches = FindObjectOfType<FindMatches>();
        allTiles = new Tile[width, height];
        allElements = new GameObject[width, height];
        currentState = GameState.SettingBoard;

        foreach (GameObject tm in tileMaskPrefabs)
        {
            availableTileMasks.Add(tm.GetComponent<Tilemap>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(Time.timeScale != 1f)
            {
                Time.timeScale = 1f;
            }
            else
            {
                Time.timeScale = 0.1f;
            }
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
                else if(!boardReset)
                {
                    StartCoroutine(ResetBoard());
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
                if (!boardRefilled)
                {
                    StartCoroutine(RefillBoard());
                }
                break;

            case GameState.CreateElementalTiles:

                if (maxFrozenTiles > currentFrozenTiles)
                {
                    StartCoroutine(checkToSpawnFrozen(0f));
                }
                else
                {
                    frozenTilesCreated = true;
                }

                if (maxEnchantedTiles > currentEnchantedTiles)
                {
                    StartCoroutine(checkToSpawnEnchanted(0f));
                }
                else
                {
                    enchantedTilesCreated = true;
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
                Tile tile = thisTile.GetComponent<Tile>();
                thisTile.transform.parent = boardContainer.transform;
                tile.column = i;
                tile.row = j;
                thisTile.name = "Tile ( " + i + "," + j + " )";
                allTiles[i,j] = tile;
            }
        }

        //temp
        assignTileMask(currentTileMask);
        //assignRandomTileMask();

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
            if (!allTiles[column - 1, row].isMasked && !allTiles[column - 2, row].isMasked)
            {
                if (allElements[column - 1, row].tag == piece.tag && allElements[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }

            if (!allTiles[column, row - 1].isMasked && !allTiles[column, row - 2].isMasked)
            {
                if (allElements[column, row - 1].tag == piece.tag && allElements[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        else if(column <= 1 || row <= 1)
        {

            if(row > 1)
            {
                if (!allTiles[column, row - 1].isMasked && !allTiles[column, row - 2].isMasked)
                {
                    if (allElements[column, row - 1].tag == piece.tag && allElements[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if (!allTiles[column - 1, row].isMasked && !allTiles[column - 2, row].isMasked)
                {
                    if (allElements[column - 1, row].tag == piece.tag && allElements[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
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
            if (!targetElement.isScored)
            {
                float finalScore = 0;
                float scoreIncrease = gameManager.baseElementValue;
                Color targetColor = popUpColors[0];

                int horizMatchLength = targetElement.horizMatchLength;
                int vertMatchLength = targetElement.vertMatchLength;

                float spawnX = column;
                float spawnY = row;

                if (horizMatchLength > 0 || vertMatchLength > 0 || targetElement.specialMatchedElements.Count > 0)
                {
                    if (targetElement.isBombMatch)
                    {
                        foreach (Element e in targetElement.specialMatchedElements)
                        {
                            e.ppDelay = targetElement.specialMatchedElements.IndexOf(e) * ppDelayInc;

                            if (e.tileType == TileType.Bomb)
                            {
                                spawnX = e.column;
                                spawnY = e.row;

                            }

                            if (!e.isScored)
                            {
                                finalScore += scoreOfTile(e.column, e.row);
                                e.isScored = true;
                            }
                        }
                    }
                    else if (targetElement.isRocketMatch)
                    {
                        foreach (Element e in targetElement.specialMatchedElements)
                        {
                            if (e.tileType == TileType.Rocket)
                            {
                                spawnX = e.column;
                                spawnY = e.row;

                            }

                            if (!e.isScored && allElements[e.column, e.row] != null)
                            {
                                finalScore += scoreOfTile(e.column, e.row);
                                e.isScored = true;
                            }
                        }
                    }
                    else if (vertMatchLength > horizMatchLength) // vert match
                    {
                        bool crossMatchFound = false;
                        float minRow = targetElement.vertMatchedElements[0].row;
                        float maxRow = targetElement.vertMatchedElements[0].row;

                        foreach (Element e in targetElement.vertMatchedElements)
                        {
                            e.ppDelay = targetElement.vertMatchedElements.IndexOf(e) * ppDelayInc;

                            if (allElements[e.column, e.row] != null)
                            {
                                if (!e.isScored)
                                {
                                    finalScore += scoreOfTile(e.column, e.row);
                                    e.isScored = true;
                                }

                                if (e.row < minRow)
                                {
                                    minRow = e.row;
                                }

                                if (e.row > maxRow)
                                {
                                    maxRow = e.row;
                                }

                                if (e.vertMatchLength == e.horizMatchLength)
                                {
                                    foreach (Element e2 in targetElement.horizMatchedElements)
                                    {
                                        if (!e2.isScored && allElements[e2.column, e2.row] != null)
                                        {
                                            finalScore += scoreOfTile(e2.column, e2.row);
                                            e2.isScored = true;
                                        }
                                    }
                                    spawnY = e.column;
                                    crossMatchFound = true;
                                }
                            }
                        }

                        if (!crossMatchFound)
                        {
                            spawnY = ((maxRow - minRow) / 2) + minRow;
                        }
                    }
                    else if (vertMatchLength < horizMatchLength) // horiz match
                    {
                        bool crossMatchFound= false;
                        float minColumn = targetElement.horizMatchedElements[0].column;
                        float maxColumn = targetElement.horizMatchedElements[0].column;

                        foreach (Element e in targetElement.horizMatchedElements)
                        {
                            e.ppDelay = targetElement.horizMatchedElements.IndexOf(e) * ppDelayInc;

                            if (allElements[e.column, e.row] != null)
                            {
                                if (!e.isScored)
                                {
                                    finalScore += scoreOfTile(e.column, e.row);
                                    e.isScored = true;
                                }

                                if (e.column < minColumn)
                                {
                                    minColumn = e.column;
                                }

                                if (e.column > maxColumn)
                                {
                                    maxColumn = e.column;
                                }

                                if (e.vertMatchLength == e.horizMatchLength)
                                {
                                    foreach (Element e2 in targetElement.vertMatchedElements)
                                    {
                                        if (!e2.isScored && allElements[e2.column, e2.row] != null)
                                        {
                                            finalScore += scoreOfTile(e2.column, e2.row);
                                            e2.isScored = true;
                                        }
                                    }
                                    spawnX = e.column;
                                    crossMatchFound = true;
                                }
                            }
                        }

                        if (!crossMatchFound)
                        {
                            spawnX = ((maxColumn - minColumn) / 2) + minColumn;
                        }
                    }
                    else if (vertMatchLength == horizMatchLength)
                    {
                        foreach (Element e in targetElement.horizMatchedElements)
                        {
                            if (!e.isScored && allElements[e.column, e.row] != null)
                            {
                                e.ppDelay = targetElement.horizMatchedElements.IndexOf(e) * ppDelayInc;

                                finalScore += scoreOfTile(e.column, e.row);
                                e.isScored = true;
                            }
                        }

                        foreach (Element e in targetElement.vertMatchedElements)
                        {
                            if (!e.isScored && allElements[e.column, e.row] != null)
                            {
                                e.ppDelay = targetElement.vertMatchedElements.IndexOf(e) * ppDelayInc;

                                finalScore += scoreOfTile(e.column, e.row);
                                e.isScored = true;
                            }
                        }
                    }
                    gameManager.matchesMade++;
                }
                else
                {
                    finalScore = scoreOfTile(column, row);
                    targetElement.isScored = true;
                }

                if(finalScore >= 100)
                {
                    targetColor = popUpColors[1];
                }

                if(finalScore >= 300)
                {
                    targetColor = popUpColors[2];
                }

                if (finalScore >= 500)
                {
                    targetColor = popUpColors[3];
                }

                if (finalScore >= 1000)
                {
                    targetColor = popUpColors[4];
                }

                Vector2 targetPos = new Vector2(spawnX * xSpawnOffsetMult, (spawnY * ySpawnOffsetMult * 1.1f));
                StartCoroutine(spawnPopUpScore(targetPos, finalScore, targetColor));

                gameManager.IncreaseScore(finalScore);
            }
            

            if (targetElement.isEnchanted)
            {
                currentEnchantedTiles--;
            }

            if (targetElement.isFrozen)
            {
                currentFrozenTiles--;
                Vector2 targetPos = new Vector2(column * xSpawnOffsetMult, row * ySpawnOffsetMult);
                Instantiate(frozenBurstPrefab, targetPos, Quaternion.identity);
                if (!iceBreakPlayed)
                {
                    FindObjectOfType<AudioManager>().Play("ice break");
                    iceBreakPlayed = true;
                }
            }
            else
            {
                Vector2 targetPos = new Vector2(column * xSpawnOffsetMult, row * ySpawnOffsetMult);
                Instantiate(targetElement.burstEffectPrefab, targetPos, Quaternion.identity);
                //
            }

            if(targetElement.tileType == TileType.Bomb)
            {
                if(OnBombDestroyed != null)
                {
                    OnBombDestroyed();
                }

                //Vector2 targetPosHoriz = new Vector2(width/2 * xSpawnOffsetMult,row * ySpawnOffsetMult);
                //GameObject horizExplosion = Instantiate(bombExplosionPrefab, targetPosHoriz, Quaternion.identity);
                //horizExplosion.transform.eulerAngles = new Vector3(0, 0, 90);
                //Vector2 targetPosVert = new Vector2(column * xSpawnOffsetMult, height / 2 * ySpawnOffsetMult);
                //Instantiate(bombExplosionPrefab, targetPosVert, Quaternion.identity);
            }
            else if (targetElement.tileType == TileType.Rocket)
            {
                if (OnRocketDestroyed != null)
                {
                    OnRocketDestroyed();
                }

                //if (targetElement.isHorizRocket)
                //{
                //    Vector2 targetPosHoriz = new Vector2(width / 2 * xSpawnOffsetMult, row * ySpawnOffsetMult);
                //    GameObject horizTrail = Instantiate(highlightRailPrefab, targetPosHoriz, Quaternion.identity);
                //    Color col = gemColors[targetColorDict[targetElement.color]];
                //    col.a = 0.5f;
                //    horizTrail.GetComponent<SpriteRenderer>().color = col;
                //    horizTrail.transform.eulerAngles = new Vector3(0, 0, 90);
                //}

                //if (targetElement.isVertRocket)
                //{
                //    Vector2 targetPosVert = new Vector2(column * xSpawnOffsetMult, height / 2 * ySpawnOffsetMult);
                //    GameObject vertTrail = Instantiate(highlightRailPrefab, targetPosVert, Quaternion.identity);
                //    Color col = gemColors[targetColorDict[targetElement.color]];
                //    col.a = 0.5f;
                //    vertTrail.GetComponent<SpriteRenderer>().color = col;
                //}
            }

            SpawnOnDestroy spawn = targetElement.spawnType;
            TargetColor colorRef = targetElement.color;
            int colorIndexRef = targetElement.colorIndex;
            string tagNameRef = targetElement.colorName;
            GameObject tileToDestroy = allElements[column, row];

            findMatches.currentMatches.Remove(allElements[column, row]);
            allElements[column, row] = null;

            if (targetElement.tileType == TileType.Rocket)
            {
                StartCoroutine(DestroyTile(tileToDestroy, 0.5f));
            }
            else if(targetElement.tileType == TileType.Bomb)
            {
                StartCoroutine(DestroyTile(tileToDestroy, 0.1f));
            }
            else
            {
                StartCoroutine(DestroyTile(tileToDestroy, 0f));
            }

            if(spawn == SpawnOnDestroy.Bomb)
            {
                Debug.Log("spawn bomb at " + column + " , " + row);
                spawnBomb(column, row, colorRef, colorIndexRef, tagNameRef);
            }
            else if (spawn == SpawnOnDestroy.VertRocket)
            {
                Debug.Log("spawn vert rocket at " + column + " , " + row);
                spawnRocket(true, column, row, colorRef, colorIndexRef, tagNameRef);

            }
            else if (spawn == SpawnOnDestroy.HorizRocket)
            {
                Debug.Log("spawn horiz rocket at " + column + " , " + row);
                spawnRocket(false, column, row, colorRef, colorIndexRef, tagNameRef);
            }
        }
    }

    public void spawnBomb(int column, int row, TargetColor colorRef, int colorIndexRef, string tagNameRef)
    {
        Debug.Log("spawn bomb at " + column + " , " + row);
        Vector2 targetPos = new Vector2(column * xSpawnOffsetMult, row * ySpawnOffsetMult);
        GameObject newBomb = Instantiate(bombPrefab, targetPos, Quaternion.identity);
        Element targetSpawned = newBomb.GetComponent<Element>();
        newBomb.GetComponent<Element>().initializeBomb(colorRef, colorIndexRef, tagNameRef);
        newBomb.name = targetSpawned.colorName + " Bomb";
        allElements[column, row] = newBomb;
        targetSpawned.row = row;
        targetSpawned.column = column;
        newBomb.transform.parent = boardContainer.transform;
        if (targetSpawned.color == banishedType)
        {
            targetSpawned.banish();
        }

        AudioManager.instance.Play("spawn special");
    }

    public void spawnRocket(bool vertical, int column, int row, TargetColor colorRef, int colorIndexRef, string tagNameRef)
    {
        if (vertical)
        {
            Vector2 targetPos = new Vector2(column * xSpawnOffsetMult, row * ySpawnOffsetMult);
            GameObject newRocket = Instantiate(rocketPrefab, targetPos, Quaternion.identity);
            Element targetSpawned = newRocket.GetComponent<Element>();
            newRocket.GetComponent<Element>().initializeRocket(colorRef, colorIndexRef, tagNameRef, true, false);
            newRocket.name = targetSpawned.colorName + " Vert Rocket";
            allElements[column, row] = newRocket;
            targetSpawned.row = row;
            targetSpawned.column = column;
            newRocket.transform.parent = boardContainer.transform;
            if (targetSpawned.color == banishedType)
            {
                targetSpawned.banish();
            }
        }
        else // horizontal
        {
            Debug.Log("spawn horiz rocket at " + column + " , " + row);
            Vector2 targetPos = new Vector2(column * xSpawnOffsetMult, row * ySpawnOffsetMult);
            GameObject newRocket = Instantiate(rocketPrefab, targetPos, Quaternion.identity);
            Element targetSpawned = newRocket.GetComponent<Element>();
            newRocket.GetComponent<Element>().initializeRocket(colorRef, colorIndexRef, tagNameRef, false, true);
            newRocket.name = targetSpawned.colorName + " Horiz Rocket";
            allElements[column, row] = newRocket;
            targetSpawned.row = row;
            targetSpawned.column = column;
            newRocket.transform.parent = boardContainer.transform;
            if (targetSpawned.color == banishedType)
            {
                targetSpawned.banish();
            }
        }

        AudioManager.instance.Play("spawn special");
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
        iceBreakPlayed = false;

        FindObjectOfType<AudioManager>().PlayCustomPitch("tile break", 0.9f * (1f + (0.06f * matchStreak)));

        StartCoroutine(DecreaseRow());
    }

    private IEnumerator DestroyTile(GameObject target, float waitTime)
    {
        Element targetElement = target.GetComponent<Element>();

        gameManager.colorTilesCleared[targetElement.colorName]++;

        gameManager.tilesCleared++;

        yield return new WaitForSeconds(waitTime);
        targetElement.destroyThis();
    }

    private IEnumerator DecreaseRow()
    {
        yield return new WaitForSeconds(0.2f);
        //collapsingActive = true;
        //int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            List<Vector2Int> nullTiles = new List<Vector2Int>();
            for (int j = 0; j < height; j++)
            {
                if(!allTiles[i, j].isMasked)
                {

                    if(allElements[i, j] == null)
                    {
                        nullTiles.Add(new Vector2Int(i, j));
                    }
                    else
                    {
                        if(nullTiles.Count > 0)
                        {
                            Element target = allElements[i, j].GetComponent<Element>();
                            target.row = nullTiles[0].y;
                            nullTiles.RemoveAt(0);
                            allElements[i, j] = null;
                            nullTiles.Add(new Vector2Int(i, j));
                        }
                    }
                }

                // while tile below is null
                // check if tile below is a mask
                // if yes, check if tile after mask is null, move to that if yes, otherwise break
                // if no, move one down

                //bool validSpaceFound = false;
                //bool noMoveNeeded = false;
                //int downShift = 0;
                //int rowChecking = 1;
                //int maskCount = 0;

                //while (!validSpaceFound && !noMoveNeeded && j - rowChecking > 0) // break if space is found or bottom limit reached
                //{
                //    Element target = allElements[i,j - rowChecking].GetComponent<Element>();

                //    if(target == null)
                //    {
                //        if(allTiles[i, j].isMasked)
                //        {
                //            maskCount++;
                //            downShift++;
                //            rowChecking++;
                //        }
                //        else
                //        {

                //        }
                //    }
                //    else
                //    {
                //        if(j + 1 < height)
                //        {
                //            if (allTiles[i, j].isMasked)
                //            {

                //            }
                //        }
                //        noMoveNeeded = true;
                //    }
                //}

                //if (validSpaceFound)
                //{
                //    allElements[i, j].GetComponent<Element>().row -= downShift;
                //}

                //if (!allTiles[i, j].isMasked)
                //{
                //    if (allElements[i, j] == null)
                //    {
                //        nullCount++;
                //    }
                //    else if (nullCount > 0)
                //    {
                //        allElements[i, j].GetComponent<Element>().row -= nullCount;
                //        allElements[i, j] = null;
                //    }
                //}
            }
            //nullCount = 0;
        }

        //yield return new WaitForSeconds(0f);

        destroyingMatches = false;
        //StartCoroutine(FillBoard());
        //collapsingActive = false;
    }

    public IEnumerator RefillBoard()
    {
        boardRefilled = true;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allElements[i,j] == null && !allTiles[i,j].isMasked)
                {
                    //yield return new WaitForSeconds(0.07f);
                    Vector2 tempPos = new Vector2(i, j + offsetHeight);


                    int elementToUse = weightedElementToUse();


                    GameObject element = Instantiate(tileElements[elementToUse], tempPos, Quaternion.identity);
                    Element targetElement = element.GetComponent<Element>();
                    element.name = targetElement.colorName + " Element";
                    allElements[i,j] = element;
                    targetElement.row = j;
                    targetElement.column = i;
                    element.transform.parent = boardContainer.transform;
                    if (targetElement.color == banishedType)
                    {
                        targetElement.banish();
                    }
                }
            }
        }
        //yield return null;
        yield return new WaitForSeconds(0.2f);
        refillingBoard = false;

        //if (OnFullBoardSet != null)
        //{
        //    OnFullBoardSet();
        //    //Debug.Log("OnFullBoardSet called");
        //}
    }

    public IEnumerator ResetBoard()
    {
        boardReset = true;
        // Fill board with elements
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allElements[i, j] == null && !allTiles[i,j].isMasked)
                {
                    yield return new WaitForSecondsRealtime(0.001f);

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

                    tempPos = new Vector2(i * xSpawnOffsetMult, (j * ySpawnOffsetMult) + offsetHeight);
                    GameObject element = Instantiate(tileElements[elementToUse], tempPos, Quaternion.identity);
                    element.name = element.GetComponent<Element>().colorName + " Element";
                    allElements[i, j] = element;
                    Element targetElement = element.GetComponent<Element>();
                    targetElement.row = j;
                    targetElement.column = i;
                    element.transform.parent = boardContainer.transform;

                    if(targetElement.color == banishedType)
                    {
                        targetElement.banish();
                    }

                    boardTilesGrid[i, j].colorIndex = targetElement.colorIndex;
                    boardTilesGrid[i, j].colorName = targetElement.colorName;
                    boardTilesGrid[i, j].targetColor = targetElement.color;
                    boardTilesGrid[i, j].tileType = targetElement.tileType;
                    boardTilesGrid[i, j].isEnchanted = targetElement.isEnchanted;
                    boardTilesGrid[i, j].isFrozen = targetElement.isFrozen;

                    prevBoardTilesGrid[i, j].colorIndex = targetElement.colorIndex;
                    prevBoardTilesGrid[i, j].colorName = targetElement.colorName;
                    prevBoardTilesGrid[i, j].targetColor = targetElement.color;
                    prevBoardTilesGrid[i, j].tileType = targetElement.tileType;
                    prevBoardTilesGrid[i, j].isEnchanted = targetElement.isEnchanted;
                    prevBoardTilesGrid[i, j].isFrozen = targetElement.isFrozen;
                }
            }
        }
        FindObjectOfType<AudioManager>().Play("tile setting");
        yield return new WaitForSeconds(0.5f);
        tilesInitialized = true;

        if (OnFullBoardSet != null)
        {
            OnFullBoardSet();
            //Debug.Log("OnFullBoardSet called");
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

    private void turnEnded()
    {
        turnStarted = false;
        matchStreak = 1;
        findMatches.rogueCanTrigger = true;
        roundMatches.Clear();
        gameManager.turnEnded();
    }

    private int weightedElementToUse()
    {
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

        return elementInts[UnityEngine.Random.Range(0, elementInts.Count)];
    }

    public void reassignTileIDs()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                string colorNameTEMP = boardTilesGrid[i, j].colorName;
                int colorIndexTEMP = boardTilesGrid[i, j].colorIndex;
                TargetColor targetColorTEMP = boardTilesGrid[i, j].targetColor;
                TileType tileTypeTEMP = boardTilesGrid[i, j].tileType;
                RocketFacing rocketFacingTEMP = boardTilesGrid[i, j].rocketFacing;
                bool isFrozenTEMP = boardTilesGrid[i, j].isFrozen;
                bool isEnchantedTEMP = boardTilesGrid[i, j].isEnchanted;

                prevBoardTilesGrid[i, j].colorName = colorNameTEMP;
                prevBoardTilesGrid[i, j].colorIndex = colorIndexTEMP;
                prevBoardTilesGrid[i, j].targetColor = targetColorTEMP;
                prevBoardTilesGrid[i, j].tileType = tileTypeTEMP;
                prevBoardTilesGrid[i, j].rocketFacing = rocketFacingTEMP;
                prevBoardTilesGrid[i, j].isFrozen = isFrozenTEMP;
                prevBoardTilesGrid[i, j].isEnchanted = isEnchantedTEMP;
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allElements[i, j] != null)
                {
                    Element elementRef = allElements[i, j].GetComponent<Element>();
                    //boardTilesGrid[elementRef.column, elementRef.row] = elementRef.colorIndex;

                    boardTilesGrid[elementRef.column, elementRef.row].colorName = elementRef.colorName;
                    boardTilesGrid[elementRef.column, elementRef.row].colorIndex = elementRef.colorIndex;
                    boardTilesGrid[elementRef.column, elementRef.row].targetColor = elementRef.color;
                    boardTilesGrid[elementRef.column, elementRef.row].tileType = elementRef.tileType;
                    boardTilesGrid[elementRef.column, elementRef.row].rocketFacing = elementRef.rocketFacing;
                    boardTilesGrid[elementRef.column, elementRef.row].isFrozen = elementRef.isFrozen;
                    boardTilesGrid[elementRef.column, elementRef.row].isEnchanted = elementRef.isEnchanted;
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

                    // determine the color from old grid
                    string colorNameTEMP = prevBoardTilesGrid[i, j].colorName;
                    int colorIndexTEMP = prevBoardTilesGrid[i, j].colorIndex;
                    TargetColor targetColorTEMP = prevBoardTilesGrid[i, j].targetColor;
                    TileType tileTypeTEMP = prevBoardTilesGrid[i, j].tileType;
                    RocketFacing rocketFacingTEMP = prevBoardTilesGrid[i, j].rocketFacing;
                    bool isFrozenTEMP = prevBoardTilesGrid[i, j].isFrozen;
                    bool isEnchantedTEMP = prevBoardTilesGrid[i, j].isEnchanted;

                    int elementToUse = targetColorDict[prevBoardTilesGrid[i, j].targetColor];

                    if (prevBoardTilesGrid[i, j].tileType == TileType.Gem)
                    {
                        GameObject element = Instantiate(tileElements[elementToUse], tempPos, Quaternion.identity);
                        element.name = element.GetComponent<Element>().colorName + " Element";
                        allElements[i, j] = element;
                        element.GetComponent<Element>().row = j;
                        element.GetComponent<Element>().column = i;
                        element.transform.parent = boardContainer.transform;
                        if (element.GetComponent<Element>().color == banishedType)
                        {
                            element.GetComponent<Element>().banish();
                        }

                        if (prevBoardTilesGrid[i, j].isEnchanted)
                        {
                            element.GetComponent<Element>().enchantElement();
                            currentEnchantedTiles++;
                        }

                        if (prevBoardTilesGrid[i, j].isFrozen)
                        {
                            element.GetComponent<Element>().freezeElement();
                            currentFrozenTiles++;

                        }
                    }

                    if (prevBoardTilesGrid[i, j].tileType == TileType.Rocket)
                    {

                        //horiz
                        if(prevBoardTilesGrid[i, j].rocketFacing == RocketFacing.Left || prevBoardTilesGrid[i, j].rocketFacing == RocketFacing.Right)
                        {
                            spawnRocket(false, i, j, prevBoardTilesGrid[i, j].targetColor, prevBoardTilesGrid[i, j].colorIndex, prevBoardTilesGrid[i, j].colorName);
                        }

                        // vert
                        if (prevBoardTilesGrid[i, j].rocketFacing == RocketFacing.Up || prevBoardTilesGrid[i, j].rocketFacing == RocketFacing.Down)
                        {
                            spawnRocket(true, i, j, prevBoardTilesGrid[i, j].targetColor, prevBoardTilesGrid[i, j].colorIndex, prevBoardTilesGrid[i, j].colorName);
                        }
                    }

                    if (prevBoardTilesGrid[i, j].tileType == TileType.Bomb)
                    {
                        spawnBomb(i, j, prevBoardTilesGrid[i, j].targetColor, prevBoardTilesGrid[i, j].colorIndex, prevBoardTilesGrid[i, j].colorName);
                    }

                    // reassign current grid
                    boardTilesGrid[i, j].colorName = colorNameTEMP;
                    boardTilesGrid[i, j].colorIndex = colorIndexTEMP;
                    boardTilesGrid[i, j].targetColor = targetColorTEMP;
                    boardTilesGrid[i, j].tileType = tileTypeTEMP;
                    boardTilesGrid[i, j].rocketFacing = rocketFacingTEMP;
                    boardTilesGrid[i, j].isFrozen = isFrozenTEMP;
                    boardTilesGrid[i, j].isEnchanted = isEnchantedTEMP;
                }
            }
        }

        if (OnUndoUsed != null)
        {
            OnUndoUsed();
        }

        if (OnFullBoardSet != null)
        {
            OnFullBoardSet();
        }
    }

    private IEnumerator checkToSpawnEnchanted(float delay)
    {
        yield return new WaitForSeconds(delay);

        int numToSpawn = maxEnchantedTiles - currentEnchantedTiles;
        for (int i = 0; i < numToSpawn; i++)
        {
            // pick an unenchanted gem element
            Element targetElement = targetElement = randomUnmaskedElement();
            while (targetElement.isEnchanted || targetElement.tileType == TileType.Bomb || targetElement.tileType == TileType.Rocket || allTiles[targetElement.column, targetElement.row].isMasked)
            {
                targetElement = targetElement = randomUnmaskedElement();
            }
            targetElement.enchantElement();
            currentEnchantedTiles++;
        }
        enchantedTilesCreated = true;

        if(numToSpawn > 0)
        {
            if (OnEnchantedTileCreated != null)
            {
                OnEnchantedTileCreated();
            }
        }
    }

    private IEnumerator checkToSpawnFrozen(float delay)
    {
        yield return new WaitForSeconds(delay);

        int numToSpawn = maxFrozenTiles - currentFrozenTiles;
        for (int i = 0; i < numToSpawn; i++)
        {
            Element targetElement = randomUnmaskedElement();

            while (targetElement.isFrozen || allTiles[targetElement.column, targetElement.row].isMasked)
            {
                targetElement = targetElement = randomUnmaskedElement();
            }
            targetElement.freezeElement();
            currentFrozenTiles++;
        }
        frozenTilesCreated = true;

        if (numToSpawn > 0)
        {
            if (OnFrozenTileCreated != null)
            {
                OnFrozenTileCreated();
            }
        }
    }

    public Element randomUnmaskedElement()
    {
        int randCol = UnityEngine.Random.Range(0, width - 1);
        int randRow = UnityEngine.Random.Range(0, height - 1);

        Tile targetTile = allTiles[randCol, randRow];

        while (targetTile.isMasked)
        {
            randCol = UnityEngine.Random.Range(0, width - 1);
            randRow = UnityEngine.Random.Range(0, height - 1);

            targetTile = allTiles[randCol, randRow];
        }

        return allElements[targetTile.column, targetTile.row].GetComponent<Element>();
    }

    public void startFindingMatches()
    {
        if(currentState == GameState.MovingTiles || currentState == GameState.RefillBoard)
        {
            checkMatchesInitialized = false;
            findingMatches = true;
        }
    }

    public void roundOver(bool isOver)
    {
        roundEnded = isOver;
    }

    private float scoreOfTile(int column, int row)
    {
        Element targetElement = allElements[column, row].GetComponent<Element>();

        if (targetElement.isBanished)
        {
            targetElement.pointValue = 0;
        }
        else
        {
            targetElement.pointValue = gameManager.colorElementIncrease[targetElement.color] + gameManager.baseElementValue + gameManager.bonusBaseElementValue;

            // check for scout
            //foreach (Patron patron in FindObjectOfType<PatronManager>().activePatrons)
            //{
            //    if (patron.title == "Scout")
            //    {
            //        targetElement.pointValue += patron.GetComponent<PtrnScout>().increaseAmount(targetElement.colorName);
            //    }
            //}

            if (OnFrozenTrigger != null)
            {
                targetElement.pointValue += OnFrozenTrigger(targetElement.isFrozen);
            }

            if (OnEnchantedTrigger != null)
            {
                targetElement.pointValue += OnEnchantedTrigger(targetElement.isEnchanted);
            }

            if (OnUndoBonusTrigger != null)
            {
                targetElement.pointValue += OnUndoBonusTrigger();
            }

            if (OnScoutTrigger != null)
            {
                targetElement.pointValue += OnScoutTrigger(targetElement.colorName);
            }

            if (gameManager.currentTurn == gameManager.maxTurns - 1)
            {
                if (OnFirstTurn != null)
                {
                    targetElement.pointValue += OnFirstTurn();
                }
            }
        }

        float scoreIncrease = targetElement.pointValue * basePointDebuff;
        float scoreMulti = 0;

        if (gameManager.currentTurn == 0)
        {
            if (OnLastTurn != null)
            {
                scoreMulti += OnLastTurn();
            }
        }

        if(OnRogueCritTrigger != null)
        {
            scoreMulti += OnRogueCritTrigger();
        }

        int horizMatchLength = targetElement.horizMatchLength;
        int vertMatchLength = targetElement.vertMatchLength;

        // Large Match Bonus
        if (horizMatchLength > 3 || vertMatchLength > 3)
        {
            int matchSize = 0;
            if (vertMatchLength > horizMatchLength)
            {
                matchSize = vertMatchLength;

                if (matchSize > 4 && targetElement.vertMatchedElements[0] == targetElement)
                {
                    targetElement.spawnType = SpawnOnDestroy.Bomb;
                }
                else if (matchSize == 4 && targetElement.vertMatchedElements[0] == targetElement)
                {
                    targetElement.spawnType = SpawnOnDestroy.VertRocket;
                }
            }
            else
            {
                matchSize = horizMatchLength;

                if (matchSize > 4 && targetElement.horizMatchedElements[0] == targetElement)
                {
                    targetElement.spawnType = SpawnOnDestroy.Bomb;
                }
                else if (matchSize == 4 && targetElement.horizMatchedElements[0] == targetElement)
                {
                    targetElement.spawnType = SpawnOnDestroy.HorizRocket;
                }
            }

            scoreIncrease += (gameManager.largeMatchBonus * (matchSize - 3));
        }


        // Enchanted Tiles
        if (targetElement.isEnchanted)
        {
            scoreMulti += gameManager.enchantedTileMulti;
        }

        // only after all multis are considered
        if (scoreMulti == 0)
        {
            scoreMulti = 1;
        }

        return scoreIncrease * scoreMulti * matchStreak;
    }

    public void assignRandomTileMask()
    {
        if(availableTileMasks.Count == 0)
        {
            foreach (GameObject tm in tileMaskPrefabs)
            {
                availableTileMasks.Add(tm.GetComponent<Tilemap>());
            }
        }

        int rand = UnityEngine.Random.Range(0, availableTileMasks.Count);

        Debug.Log(rand);
        assignTileMask(availableTileMasks[rand]);

        availableTileMasks.RemoveAt(rand);
    }


    private void assignTileMask(Tilemap tileMask)
    {
        //disable masked tiles
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                allTiles[i, j].isMasked = false;
            }
        }


        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //Debug.Log("all tiles length: " + allTiles.Length);

                if(tileMask.GetTile(new Vector3Int(i, j, 0)) != null)
                {
                    allTiles[i, j].isMasked = (tileMask.GetTile(new Vector3Int(i, j, 0)).name == "tile_mask");
                    tilemapBase.SetTile(new Vector3Int(i, j, 0), null);
                }
                else
                {
                    tilemapBase.SetTile(new Vector3Int(i, j, 0), tilemapBasePrefab.GetTile(new Vector3Int(i, j, 0)));
                }
            }
        }
    }

    private IEnumerator spawnPopUpScore(Vector3 pos, float score, Color color)
    {
        yield return new WaitForSeconds(0.3f);

        GameObject newPopup = Instantiate(textPopup, pos, Quaternion.identity);
        newPopup.transform.parent = transform;
        newPopup.GetComponent<ScorePopup>().initialize(score, color);
    }

    public void allMatchesFound(bool matchesFound)
    {
        matchesToDestroy = matchesFound;
        findingMatches = false;
    }

    public void spawnPopup(Vector2 pos, string popupText, float fontSize, Color color, string audioName = "")
    {
        GameObject newPopup = Instantiate(popupPrefab, pos, Quaternion.identity);
        //newPopup.transform.parent = transform;
        newPopup.GetComponent<TextPopup>().initialize(popupText, fontSize, color, audioName);
    }

    #region State Switch Conditions
    private void determineState()
    {
        switch (currentState)
        {
            case GameState.Waiting:
                if (roundEnded)
                {
                    currentState = GameState.RoundEnded;
                }
                break;

            case GameState.SettingBoard:
                if (boardInitialized && tilesInitialized)
                {
                    boardReset = false;
                    tilesInitialized = false;
                    currentState = GameState.CreateElementalTiles;
                }
                break;

            case GameState.MovingTiles:
                if (findingMatches)
                {
                    //Debug.Log("Moving Tiles -> Check Matches");
                    currentState = GameState.CheckMatches;
                }
                break;

            case GameState.CheckMatches:
                if (!findingMatches && matchesToDestroy)
                {
                    //Debug.Log("Check Matches -> Destroy Matches");
                    destroyingMatches = true;
                    currentState = GameState.DestroyMatches;
                }
                if (!findingMatches && !matchesToDestroy)
                {
                    turnOver = true;
                    //Debug.Log("Check Matches -> Create Elemental Tiles");
                    currentState = GameState.CreateElementalTiles;
                }
                break;

            case GameState.DestroyMatches:
                if (!destroyingMatches)
                {
                    destroyMatchesInitialized = false;
                    refillingBoard = true;

                    if (matchStreak > 1)
                    {
                        Color col = Color.white;

                        if (matchStreak - 1 > popUpColors.Length)
                        {
                            col = popUpColors[popUpColors.Length - 1];
                        }
                        else
                        {
                            col = popUpColors[matchStreak - 1];
                        }

                        Vector2 pos = new Vector2(boardCenter.x + UnityEngine.Random.Range(-1f, 1f), boardCenter.y + UnityEngine.Random.Range(-1f, 1f));
                        spawnPopup(pos, matchStreak + "x Combo", comboFontSize, col);
                    }

                    matchStreak++;

                    //Debug.Log("Destroy Matches -> Refill Board");
                    currentState = GameState.RefillBoard;
                }
                break;

            case GameState.RefillBoard:
                if (!refillingBoard)
                {
                    //Debug.Log("Refill Board -> Check Matches");
                    boardRefilled = false;
                    findingMatches = true;
                    checkMatchesInitialized = false;
                    currentState = GameState.CheckMatches;
                }
                break;

            case GameState.CreateElementalTiles:
                if (enchantedTilesCreated && frozenTilesCreated)
                {
                    //Debug.Log("Create Elemental Tiles -> Waiting");
                    if (turnOver && turnStarted)
                    {
                        turnEnded();
                    }
                    reassignTileIDs();
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

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public enum TargetColor
{
    None,
    Red,
    Blue,
    Green,
    Purple,
    Yellow
}

public enum TileType
{
    Gem,
    Bomb,
    Rocket,
}

public enum SpawnOnDestroy
{
    None,
    Bomb,
    VertRocket,
    HorizRocket
}

public enum RocketFacing
{
    Left,
    Right,
    Up,
    Down
}

public class Element : MonoBehaviour
{
    [Header("Element Variables")]
    [SerializeField] private float swapSpeed;
    [SerializeField] private float fallSpeed;
    public bool beingSwiped = false;
    [SerializeField] private GameObject matchedIcon;
    [SerializeField] private GameObject enchantedEffect;
    [SerializeField] private GameObject frozenEffect;
    [SerializeField] private GameObject banishedIcon;
    [SerializeField] private GameObject lightFlash;
    [SerializeField] private GameObject rocketTrail;
    [SerializeField] private Material enchantedMaterial;
    [SerializeField] private SpriteRenderer gemSprite;
    [SerializeField] private SpriteRenderer iceFrontSprite;
    [SerializeField] private GameObject explosionParticles;
    public GameObject burstEffectPrefab;
    public string colorName;
    public int colorIndex;
    public TargetColor color;
    public TileType tileType;
    public SpawnOnDestroy spawnType;
    public bool isFrozen = false;
    public bool isEnchanted = false;
    public float pointValue = 0;
    public bool isVertRocket = false;
    public bool isHorizRocket = false;
    public RocketFacing rocketFacing;
    public bool isBanished = false;
    public bool markedByRogue = false;
    //public bool explodedByBomb = false;
    private bool bombSploded = false;
    private bool rocketSploded = false;


    [Header("Board Variables")]
    public int column;
    //public int prevColumn;
    public int row;
    //public int prevRow;
    public float targetX;
    public float targetY;
    public bool isFalling = false;
    private bool playedFallSound = true;
    private bool triggeredMatchEffect = false;

    private BoardManager board;
    private FindMatches findMatches;
    private GameManager gameManager;
    private GameObject otherElement;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    // Match variables
    public bool isMatched = false;
    public bool isScored = false;
    public bool isBombMatch = false;
    public bool isRocketMatch = false;
    public int horizMatchLength = 0;
    public int vertMatchLength = 0;
    public List<Element> horizMatchedElements = new List<Element>();
    public List<Element> vertMatchedElements = new List<Element>();
    public List<Element> specialMatchedElements = new List<Element>();

    [SerializeField] Sprite[] bombColors;
    private Dictionary<TargetColor, Sprite> bombColorDict;

    [SerializeField] Sprite[] rocketColors;
    private Dictionary<TargetColor, Sprite> rocketColorDict;

    public Color flashColor;

    [SerializeField] int minPointParticles = 5;
    [SerializeField] int maxPointParticles = 8;
    [SerializeField] GameObject pointParticlePrefab;

    [SerializeField] GameObject rogueEffect;
    [SerializeField] Animator rogueAnimator;

    [SerializeField] GameObject highlightRailPrefab;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        board = FindObjectOfType<BoardManager>();
        findMatches = FindObjectOfType<FindMatches>();
    }

    // Update is called once per frame
    void Update()
    {
        //FindMatches();

        if (isMatched)
        {
            //matchedIcon.SetActive(true);
            if (isEnchanted)
            {
                matchedIcon.GetComponent<SpriteRenderer>().color = Color.green;
            }

            if(tileType != TileType.Gem && !triggeredMatchEffect)
            {
                if(tileType == TileType.Bomb)
                {
                    Debug.Log("Bomb SPLODED");
                    // bomb effect

                    //for (int i1 = column - 1; i1 < column + 2; i1++)
                    //{
                    //    for (int j1 = row - 1; j1 < row + 2; j1++)
                    //    {
                    //        if(i1 >= 0 && i1 < board.width - 1 && j1 > 0 && j1 < board.height - 1)
                    //        {
                    //            for (int i2 = column - 1; i2 < column + 2; i2++)
                    //            {
                    //                for (int j2 = row - 1; j2 < row + 2; j2++)
                    //                {
                    //                    if (i2 >= 0 && i2 < board.width - 1 && j2 > 0 && j2 < board.height - 1)
                    //                    {

                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    for (int i = 0; i < board.height; i++)
                    {
                        if (!board.allTiles[column, i].isMasked)
                        {
                            Element elementRef = board.allElements[column, i].GetComponent<Element>();
                            elementRef.isMatched = true;
                            elementRef.isBombMatch = true;

                            for (int j = 0; j < board.height; j++)
                            {
                                if (!board.allTiles[column, j].isMasked)
                                {
                                    elementRef.specialMatchedElements.Add(board.allElements[column, j].GetComponent<Element>());
                                }
                            }
                        }
                    }

                    for (int i = 0; i < board.width; i++)
                    {
                        if (!board.allTiles[i, row].isMasked)
                        {
                            Element elementRef = board.allElements[i, row].GetComponent<Element>();
                            elementRef.isMatched = true;
                            elementRef.isBombMatch = true;

                            for (int j = 0; j < board.width; j++)
                            {
                                if (j != elementRef.column)
                                {
                                    if (!board.allTiles[j, row].isMasked)
                                    {
                                        elementRef.specialMatchedElements.Add(board.allElements[j, row].GetComponent<Element>());
                                    }
                                }
                            }
                        }
                    }

                    transform.DOPunchScale(new Vector3(2, 2, 2), 0.3f);
                    triggeredMatchEffect = true;
                }
                else if (tileType == TileType.Rocket && isVertRocket)
                {
                    //Debug.Log("Vert Rocket SPLODED");
                    // vert rocket effect

                    //transform.DOScale(new Vector3(2, 2, 2), 0.2f);

                    for (int i = 0; i < board.height; i++)
                    {
                        if (!board.allTiles[column, i].isMasked)
                        {
                            Element elementRef = board.allElements[column, i].GetComponent<Element>();
                            elementRef.isMatched = true;
                            elementRef.isRocketMatch = true;

                            for (int j = 0; j < board.height; j++)
                            {
                                if (!board.allTiles[column, j].isMasked)
                                {
                                    elementRef.specialMatchedElements.Add(board.allElements[column, j].GetComponent<Element>());
                                }
                            }
                        }
                    }

                    //if (rocketFacing == RocketFacing.Down) // face down
                    //{
                    //    Debug.Log("rocket sploded down");
                    //    targetY = board.height * -3f * board.ySpawnOffsetMult;
                    //}
                    //else if (rocketFacing == RocketFacing.Up) // face up
                    //{
                    //    Debug.Log("rocket sploded up");
                    //    targetY = board.height * 3f * board.ySpawnOffsetMult;
                    //}

                    //rocketTrail.SetActive(true);
                    fallSpeed *= 1.5f;
                    triggeredMatchEffect = true;

                    if (!rocketSploded)
                    {
                        StartCoroutine(RocketEffect());
                        rocketSploded = true;
                    }
                }
                else if (tileType == TileType.Rocket && isHorizRocket)
                {
                    //Debug.Log("Horiz Rocket SPLODED");
                    // horiz rocket effect

                    //transform.DOScale(new Vector3(2, 2, 2), 0.2f);

                    for (int i = 0; i < board.width; i++)
                    {
                        if (!board.allTiles[i, row].isMasked)
                        {
                            Element elementRef = board.allElements[i, row].GetComponent<Element>();
                            elementRef.isMatched = true;
                            elementRef.isRocketMatch = true;

                            for (int j = 0; j < board.width; j++)
                            {
                                if (!board.allTiles[j, row].isMasked)
                                {
                                    elementRef.specialMatchedElements.Add(board.allElements[j, row].GetComponent<Element>());
                                }
                            }
                        }
                    }

                    //if (rocketFacing == RocketFacing.Left) // face left
                    //{
                    //    Debug.Log("rocket sploded left");
                    //    targetX = board.width * -3f;
                    //}
                    //else if(rocketFacing == RocketFacing.Right) // face right
                    //{
                    //    Debug.Log("rocket sploded right");
                    //    targetX = board.width + (board.width * 3f);
                    //}

                    //rocketTrail.SetActive(true);
                    fallSpeed *= 1.5f;
                    triggeredMatchEffect = true;

                    if (!rocketSploded)
                    {
                        StartCoroutine(RocketEffect());
                        rocketSploded = true;
                    }
                }
            }
        }
        else
        {
            targetX = column * board.xSpawnOffsetMult;
            targetY = row * board.ySpawnOffsetMult;
        }

        if (beingSwiped)
        {
            float step = swapSpeed * Time.deltaTime;
            if (Mathf.Abs(targetX - transform.position.x) > 0.1)
            {
                // move towards target
                tempPosition = new Vector2(targetX, transform.position.y);
                //transform.position = Vector2.Lerp(transform.position, tempPosition, swapSpeed);
                transform.position = Vector2.MoveTowards(transform.position, tempPosition, step);
                if (board.allElements[column, row] != this.gameObject && !triggeredMatchEffect)
                {
                    board.allElements[column, row] = this.gameObject;
                }
            }
            else
            {
                // set pos
                tempPosition = new Vector2(targetX, transform.position.y);
                transform.position = tempPosition;
                beingSwiped = false;
                //board.allElements[column, row] = this.gameObject;
                //lightFlash.SetActive(false);
            }
            if (Mathf.Abs(targetY - transform.position.y) > 0.1)
            {
                // move towards target
                tempPosition = new Vector2(transform.position.x, targetY);
                //transform.position = Vector2.Lerp(transform.position, tempPosition, swapSpeed);
                transform.position = Vector2.MoveTowards(transform.position, tempPosition, step);
                if (board.allElements[column, row] != this.gameObject && !triggeredMatchEffect)
                {
                    board.allElements[column, row] = this.gameObject;
                }
            }
            else
            {
                // set pos
                tempPosition = new Vector2(transform.position.x, targetY);
                transform.position = tempPosition;
                beingSwiped = false;
                //board.allElements[column, row] = this.gameObject;
                //lightFlash.SetActive(false);
            }
        }
        else // falling
        {
            float step = fallSpeed * Time.deltaTime;
            if (Mathf.Abs(targetX - transform.position.x) > 0.1)
            {
                // move towards target
                tempPosition = new Vector2(targetX, transform.position.y);
                //transform.position = Vector2.Lerp(transform.position, tempPosition, fallSpeed);
                transform.position = Vector2.MoveTowards(transform.position, tempPosition, step);
                if (board.allElements[column, row] != this.gameObject && !triggeredMatchEffect)
                {
                    board.allElements[column, row] = this.gameObject;
                }
            }
            else
            {
                // set pos
                tempPosition = new Vector2(targetX, transform.position.y);
                transform.position = tempPosition;
                //board.allElements[column, row] = this.gameObject;
            }
            if (Mathf.Abs(targetY - transform.position.y) > 0.1)
            {
                isFalling = true;
                // move towards target
                tempPosition = new Vector2(transform.position.x, targetY);
                //transform.position = Vector2.Lerp(transform.position, tempPosition, fallSpeed);
                transform.position = Vector2.MoveTowards(transform.position, tempPosition, step);

                if (board.allElements[column, row] != this.gameObject && !triggeredMatchEffect)
                {
                    board.allElements[column, row] = this.gameObject;
                }
                if (playedFallSound && board.currentState != GameState.SettingBoard)
                {
                    playedFallSound = false;
                }
            }
            else
            {
                // set pos
                isFalling = false;
                tempPosition = new Vector2(transform.position.x, targetY);
                transform.position = tempPosition;
                //board.allElements[column, row] = this.gameObject;
                if(!playedFallSound && board.currentState != GameState.SettingBoard)
                {
                    //Debug.Log("sound");
                    FindObjectOfType<AudioManager>().Play("tile fall");
                    playedFallSound = true;
                }

                //row == board.height - 1 &&
            }
        }
    }

    private IEnumerator RocketEffect()
    {
        yield return new WaitForSeconds(0.1f);

        if (isHorizRocket)
        {
            Vector2 targetPosHoriz = new Vector2(board.width / 2 * board.xSpawnOffsetMult, row * board.ySpawnOffsetMult);
            GameObject horizTrail = Instantiate(highlightRailPrefab, targetPosHoriz, Quaternion.identity);
            Color col = board.gemColors[board.targetColorDict[color]];
            col.a = 0.5f;
            horizTrail.GetComponent<SpriteRenderer>().color = col;
            horizTrail.transform.eulerAngles = new Vector3(0, 0, 90);
        }

        if (isVertRocket)
        {
            Vector2 targetPosVert = new Vector2(column * board.xSpawnOffsetMult, board.height / 2 * board.ySpawnOffsetMult);
            GameObject vertTrail = Instantiate(highlightRailPrefab, targetPosVert, Quaternion.identity);
            Color col = board.gemColors[board.targetColorDict[color]];
            col.a = 0.5f;
            vertTrail.GetComponent<SpriteRenderer>().color = col;
        }

        if (rocketFacing == RocketFacing.Left) // face left
        {
            Debug.Log("rocket sploded left");
            targetX = board.width * -3f;
        }
        else if (rocketFacing == RocketFacing.Right) // face right
        {
            Debug.Log("rocket sploded right");
            targetX = board.width + (board.width * 3f);
        }
        else if (rocketFacing == RocketFacing.Down) // face down
        {
            Debug.Log("rocket sploded down");
            targetY = board.height * -3f * board.ySpawnOffsetMult;
        }
        else if (rocketFacing == RocketFacing.Up) // face up
        {
            Debug.Log("rocket sploded up");
            targetY = board.height * 3f * board.ySpawnOffsetMult;
        }

        transform.DOScale(new Vector3(1.7f, 1.7f, 1.7f), 0.2f);
        rocketTrail.SetActive(true);

        //yield return null;
    }

    public IEnumerator CheckMove()
    {
        yield return new WaitForSeconds(0f);

        board.startFindingMatches();
    }

    private void OnMouseDown()
    {
        if (board.currentState == GameState.Waiting)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.Waiting)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    void CalculateAngle() 
    {
        if(Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist ||
            Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            //Debug.Log(swipeAngle);
            MovePieces();
            //board.currentState = GameState.wait;
        }
    }

    void MovePieces()
    {
        float animDuration = 0.1f;

        //Debug.Log("Waiting -> Moving Tiles");
        FindObjectOfType<AudioManager>().Play("tile swap");
        board.currentState = GameState.MovingTiles;
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1 && noMaskFound(1,0)) // right swipe
        {
            otherElement = board.allElements[column + 1, row];
            otherElement.GetComponent<Element>().column -= 1;
            column += 1;

            transform.DOPunchScale(new Vector3(0.3f, 0.9f, 1), animDuration, 0, 0);
            beingSwiped = true;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1 && noMaskFound(0,1)) // up swipe
        {
            otherElement = board.allElements[column, row + 1];
            otherElement.GetComponent<Element>().row -= 1;
            row += 1;

            transform.DOPunchScale(new Vector3(0.9f, 0.3f, 1), animDuration, 0, 0);
            beingSwiped = true;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0 && noMaskFound(-1,0)) // left swipe
        {
            otherElement = board.allElements[column - 1, row];
            otherElement.GetComponent<Element>().column += 1;
            column -= 1;

            transform.DOPunchScale(new Vector3(0.3f, 0.9f, 1), animDuration, 0, 0);
            beingSwiped = true;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0 && noMaskFound(0, -1)) // dowm swipe
        {
            otherElement = board.allElements[column, row - 1];
            otherElement.GetComponent<Element>().row += 1;
            row -= 1;

            transform.DOPunchScale(new Vector3(0.3f, 0.6f, 1), animDuration, 0, 0);
            beingSwiped = true;
        }

        if (beingSwiped)
        {
            otherElement.GetComponent<Element>().beingSwiped = true;
            StartCoroutine(CheckMove());
            //findMatches.FindAllMatchesStart();
            gameManager.turnStarted();
        }
        else
        {
            board.currentState = GameState.Waiting;
        }
    } 

    public void flashSequence()
    {
        StartCoroutine(FlashSequence());
    }

    IEnumerator FlashSequence()
    {
        //Debug.Log("flashing");
        lightFlash.GetComponent<SpriteRenderer>().color = flashColor;


        float duration = 0.5f;
        float pause = 0.2f;
        float flashScaleFactor = 2f;
        float gemScaleFactor = 1.2f;

        Vector3 startScaleFlash = lightFlash.transform.localScale;
        Vector3 startScaleGem = gameObject.transform.localScale;

        Vector3 endScaleFlash = startScaleFlash * flashScaleFactor;
        Vector3 endScaleGem = startScaleGem * gemScaleFactor;

        lightFlash.SetActive(true);
        lightFlash.GetComponent<SpriteRenderer>().DOFade(0.5f, (duration - pause)/2);
        lightFlash.transform.DOScale(endScaleFlash, (duration - pause) / 2);
        gameObject.transform.DOScale(endScaleGem, (duration - pause) / 2);

        yield return new WaitForSeconds(((duration - pause) / 2) + pause);

        lightFlash.GetComponent<SpriteRenderer>().DOFade(0, (duration - pause) / 2);
        lightFlash.transform.DOScale(startScaleFlash, (duration - pause) / 2);
        gameObject.transform.DOScale(startScaleGem, (duration - pause) / 2);

        yield return new WaitForSeconds((duration - pause) / 2);

        lightFlash.SetActive(false);
    }

    public void freezeElement()
    {
        isFrozen = true;
        frozenEffect.SetActive(true);
    }

    public void enchantElement()
    {
        isEnchanted = true;
        enchantedEffect.SetActive(true);

        if (isFrozen)
        {
            iceFrontSprite.material = enchantedMaterial;
        }
        else
        {
            gemSprite.material = enchantedMaterial;
        }
    }

    public void banish()
    {
        isBanished = true;
        banishedIcon.SetActive(true);
    }

    public void initializeBomb(TargetColor colorRef, int colorIndexRef, string tagName)
    {
        if (bombColors.Length == 5)
        {
            bombColorDict = new Dictionary<TargetColor, Sprite>
            {
                {TargetColor.Red, bombColors[0]},
                {TargetColor.Blue, bombColors[1]},
                {TargetColor.Green, bombColors[2]},
                {TargetColor.Purple, bombColors[3]},
                {TargetColor.Yellow, bombColors[4]},
            };
        }

        color = colorRef;
        //GetComponent<SpriteRenderer>().color = FindObjectOfType<GameManager>().tileColors[colorIndexRef];
        gemSprite.sprite = bombColorDict[color];
        colorIndex = colorIndexRef;
        colorName = tagName;
        gameObject.tag = tagName;

        transform.DOPunchScale(new Vector3(1.2f, 1.2f, 1), 0.3f, 0, 0);
    }

    public void initializeRocket(TargetColor colorRef,int colorIndexRef, string tagName, bool isVert, bool isHoriz)
    {
        if (rocketColors.Length == 5)
        {
            rocketColorDict = new Dictionary<TargetColor, Sprite>
            {
                {TargetColor.Red, rocketColors[0]},
                {TargetColor.Blue, rocketColors[1]},
                {TargetColor.Green, rocketColors[2]},
                {TargetColor.Purple, rocketColors[3]},
                {TargetColor.Yellow, rocketColors[4]},
            };
        }

        isVertRocket = isVert;
        isHorizRocket = isHoriz;
        color = colorRef;
        Color elemColor = FindObjectOfType<GameManager>().tileColors[colorIndexRef];
        //GetComponent<SpriteRenderer>().color = elemColor;
        gemSprite.sprite = rocketColorDict[color];
        rocketTrail.GetComponent<TrailRenderer>().startColor = elemColor;
        elemColor.a = 0f;
        rocketTrail.GetComponent<TrailRenderer>().endColor = elemColor;
        colorIndex = colorIndexRef;
        colorName = tagName;
        gameObject.tag = tagName;


        if (isVert)
        {
            if (column >= FindObjectOfType<BoardManager>().height / 2) // face down
            {
                transform.eulerAngles = new Vector3(0, 0, 180);
                rocketFacing = RocketFacing.Down;
            }
            else // face up
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                rocketFacing = RocketFacing.Up;
            }
        }

        if (isHoriz)
        {
            if (column >= FindObjectOfType<BoardManager>().width / 2) // face left
            {
                transform.eulerAngles = new Vector3(0, 0, 90);
                rocketFacing = RocketFacing.Left;
            }
            else // face right
            {
                transform.eulerAngles = new Vector3(0, 0, -90);
                rocketFacing = RocketFacing.Right;
            }
        }

        transform.DOPunchScale(new Vector3(1.2f, 1.2f, 1), 0.3f, 0, 0);
    }

    private bool noMaskFound(int xChange, int yChange)
    {
        if (board.allTiles[column + xChange, row + yChange].isMasked)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void destroyThis()
    {
        StartCoroutine(destroyThisEnum());
    }

    public void rogueEffectStart()
    {
        StartCoroutine(rogueEffectEnum());
    }

    IEnumerator rogueEffectEnum()
    {
        rogueEffect.SetActive(true);

        int animHash = Animator.StringToHash("Base Layer.Rogue Action");

        rogueAnimator.Play(animHash, 0);

        float animLength = rogueAnimator.GetCurrentAnimatorClipInfo(0).Length;

        yield return new WaitForSeconds(animLength);

        rogueEffect.SetActive(false);
    }

    IEnumerator destroyThisEnum()
    {
        //if (markedByRogue)
        //{
        //    rogueEffect.SetActive(true);

        //    int animHash = Animator.StringToHash("Base Layer.Rogue Action");

        //    rogueAnimator.Play(animHash, 0);

        //    float animLength = rogueAnimator.GetCurrentAnimatorClipInfo(0).Length;

        //    yield return new WaitForSeconds(animLength);

        //    rogueEffect.SetActive(false);
        //}

        if (tileType == TileType.Gem)
        {
            int randAmount = Random.Range(minPointParticles, maxPointParticles + 1);

            for (int i = 0; i < randAmount; i++)
            {
                float randScale = Random.Range(0.1f, 0.2f);
                Vector3 newScale = new Vector3(randScale, randScale, randScale);

                float xOffset = Random.Range(-0.25f, 0.25f);
                float yOffset = Random.Range(-0.25f, 0.25f);

                Vector3 pos = new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, transform.position.z);
                GameObject particle = Instantiate(pointParticlePrefab, pos, Quaternion.identity);
                particle.transform.localScale = newScale;
                //Debug.Log("particle spawned");
            }
        }
        else if(tileType == TileType.Rocket)
        {
            //if (isHorizRocket)
            //{
            //    Vector2 targetPosHoriz = new Vector2(board.width / 2 * board.xSpawnOffsetMult, row * board.ySpawnOffsetMult);
            //    GameObject horizTrail = Instantiate(highlightRailPrefab, targetPosHoriz, Quaternion.identity);
            //    Color col = board.gemColors[board.targetColorDict[color]];
            //    col.a = 0.5f;
            //    horizTrail.GetComponent<SpriteRenderer>().color = col;
            //    horizTrail.transform.eulerAngles = new Vector3(0, 0, 90);
            //}

            //if (isVertRocket)
            //{
            //    Vector2 targetPosVert = new Vector2(column * board.xSpawnOffsetMult, board.height / 2 * board.ySpawnOffsetMult);
            //    GameObject vertTrail = Instantiate(highlightRailPrefab, targetPosVert, Quaternion.identity);
            //    Color col = board.gemColors[board.targetColorDict[color]];
            //    col.a = 0.5f;
            //    vertTrail.GetComponent<SpriteRenderer>().color = col;
            //}

            //if (rocketFacing == RocketFacing.Left) // face left
            //{
            //    Debug.Log("rocket sploded left");
            //    targetX = board.width * -3f;
            //}
            //else if (rocketFacing == RocketFacing.Right) // face right
            //{
            //    Debug.Log("rocket sploded right");
            //    targetX = board.width + (board.width * 3f);
            //}
            //else if (rocketFacing == RocketFacing.Down) // face down
            //{
            //    Debug.Log("rocket sploded down");
            //    targetY = board.height * -3f * board.ySpawnOffsetMult;
            //}
            //else if (rocketFacing == RocketFacing.Up) // face up
            //{
            //    Debug.Log("rocket sploded up");
            //    targetY = board.height * 3f * board.ySpawnOffsetMult;
            //}

            //transform.DOScale(new Vector3(1.7f, 1.7f, 1.7f), 0.2f);
            //rocketTrail.SetActive(true);

            //yield return new WaitForSeconds(0.5f);
        }
        //else if(tileType == TileType.Bomb)
        //{
        //    Vector3 pos = transform.position;
        //    Instantiate(explosionParticles, pos, Quaternion.identity);
        //}

        if (isBombMatch)
        {
            Vector3 pos = transform.position;
            Instantiate(explosionParticles, pos, Quaternion.identity);
        }

        Destroy(gameObject);
        yield return null;
    }
}

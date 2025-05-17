using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
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
                        Element elementRef = board.allElements[column, i].GetComponent<Element>();
                        elementRef.isMatched = true;
                        elementRef.isBombMatch = true;

                        for (int j = 0; j < board.height; j++)
                        {
                            elementRef.specialMatchedElements.Add(board.allElements[column, j].GetComponent<Element>());
                        }
                    }

                    for (int i = 0; i < board.width; i++)
                    {
                        Element elementRef = board.allElements[i, row].GetComponent<Element>();
                        elementRef.isMatched = true;
                        elementRef.isBombMatch = true;

                        for (int j = 0; j < board.width; j++)
                        {
                            if(j != elementRef.column)
                            {
                                elementRef.specialMatchedElements.Add(board.allElements[j, row].GetComponent<Element>());
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

                    transform.DOScale(new Vector3(2, 2, 2), 0.2f);

                    for (int i = 0; i < board.height; i++)
                    {
                        Element elementRef = board.allElements[column, i].GetComponent<Element>();
                        elementRef.isMatched = true;
                        elementRef.isRocketMatch = true;

                        for (int j = 0; j < board.height; j++)
                        {
                            elementRef.specialMatchedElements.Add(board.allElements[column, j].GetComponent<Element>());
                        }
                    }

                    if (rocketFacing == RocketFacing.Down) // face down
                    {
                        Debug.Log("rocket sploded down");
                        targetY = board.height * -3f * board.ySpawnOffsetMult;
                    }
                    else if (rocketFacing == RocketFacing.Up) // face up
                    {
                        Debug.Log("rocket sploded up");
                        targetY = board.height * 3f * board.ySpawnOffsetMult;
                    }

                    rocketTrail.SetActive(true);
                    //fallSpeed *= 2f;
                    triggeredMatchEffect = true;
                }
                else if (tileType == TileType.Rocket && isHorizRocket)
                {
                    //Debug.Log("Horiz Rocket SPLODED");
                    // horiz rocket effect

                    transform.DOScale(new Vector3(2, 2, 2), 0.2f);

                    for (int i = 0; i < board.width; i++)
                    {
                        Element elementRef = board.allElements[i, row].GetComponent<Element>();
                        elementRef.isMatched = true;
                        elementRef.isRocketMatch = true;

                        for (int j = 0; j < board.width; j++)
                        {
                            elementRef.specialMatchedElements.Add(board.allElements[j, row].GetComponent<Element>());
                        }
                    }

                    if (rocketFacing == RocketFacing.Left) // face left
                    {
                        Debug.Log("rocket sploded left");
                        targetX = board.width * -3f;
                    }
                    else if(rocketFacing == RocketFacing.Right) // face right
                    {
                        Debug.Log("rocket sploded right");
                        targetX = board.width + (board.width * 3f);
                    }

                    rocketTrail.SetActive(true);
                    //fallSpeed *= 2f;
                    triggeredMatchEffect = true;
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
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1) // right swipe
        {
            otherElement = board.allElements[column + 1, row];
            otherElement.GetComponent<Element>().column -= 1;
            column += 1;

            StartCoroutine(flashSequence("right", animDuration));
            transform.DOPunchScale(new Vector3(0.3f, 0.9f, 1), animDuration, 0, 0);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1) // up swipe
        {
            otherElement = board.allElements[column, row + 1];
            otherElement.GetComponent<Element>().row -= 1;
            row += 1;

            StartCoroutine(flashSequence("up", animDuration));
            transform.DOPunchScale(new Vector3(0.9f, 0.3f, 1), animDuration, 0, 0);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0) // left swipe
        {
            otherElement = board.allElements[column - 1, row];
            otherElement.GetComponent<Element>().column += 1;
            column -= 1;

            StartCoroutine(flashSequence("left",animDuration));
            transform.DOPunchScale(new Vector3(0.3f, 0.9f, 1), animDuration, 0, 0);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0) // dowm swipe
        {
            otherElement = board.allElements[column, row - 1];
            otherElement.GetComponent<Element>().row += 1;
            row -= 1;

            StartCoroutine(flashSequence("down", animDuration));
            transform.DOPunchScale(new Vector3(0.3f, 0.6f, 1), animDuration, 0, 0);
        }

        beingSwiped = true;
        otherElement.GetComponent<Element>().beingSwiped = true;
        StartCoroutine(CheckMove());
        //findMatches.FindAllMatchesStart();
        gameManager.turnStarted();
    } 

    IEnumerator flashSequence(string direction, float duration)
    {
        //DG.Tweening.Sequence fadeSequence = DOTween.Sequence();
        //fadeSequence.Append(lightFlash.GetComponent<SpriteRenderer>().DOFade(0.5f, duration / 2));
        //fadeSequence.Append(lightFlash.GetComponent<SpriteRenderer>().DOFade(0, duration / 2));
        //fadeSequence.Play();

        //lightFlash.SetActive(true);
        if (direction == "up" || direction == "down")
        {
            //if (direction == "up")
            //{
            //    lightFlash.transform.DOPunchPosition(new Vector3(lightFlash.transform.localPosition.x, lightFlash.transform.localPosition.y - 0.5f, lightFlash.transform.localPosition.z), duration);
            //}
            //else
            //{
            //    lightFlash.transform.DOPunchPosition(new Vector3(lightFlash.transform.localPosition.x, lightFlash.transform.localPosition.y + 0.5f, lightFlash.transform.localPosition.z), duration);
            //}
            //lightFlash.transform.DOPunchScale(new Vector3(0.1f, 1f, 1), duration, 0, 0);
            GetComponent<SpriteRenderer>().sortingOrder++;

            yield return new WaitForSeconds(duration);
        }
        else if (direction == "left" || direction == "right")
        {
            //if (direction == "left")
            //{
            //    lightFlash.transform.DOPunchPosition(new Vector3(lightFlash.transform.localPosition.x + 0.5f, lightFlash.transform.localPosition.y, lightFlash.transform.localPosition.z), duration);
            //}
            //else
            //{
            //    lightFlash.transform.DOPunchPosition(new Vector3(lightFlash.transform.localPosition.x - 0.5f, lightFlash.transform.localPosition.y, lightFlash.transform.localPosition.z), duration);
            //}
            //lightFlash.transform.DOPunchScale(new Vector3(1f, 0.1f, 1), duration, 0, 0);
            GetComponent<SpriteRenderer>().sortingOrder++;

            yield return new WaitForSeconds(duration);
        }
        GetComponent<SpriteRenderer>().sortingOrder--;
        //lightFlash.SetActive(false);
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
    }

    public void banish()
    {
        pointValue = 0;
        banishedIcon.SetActive(true);
    }

    public void initializeBomb(TargetColor colorRef, int colorIndexRef, string tagName)
    {
        color = colorRef;
        GetComponent<SpriteRenderer>().color = FindObjectOfType<GameManager>().tileColors[colorIndexRef];
        colorIndex = colorIndexRef;
        colorName = tagName;
        gameObject.tag = tagName;

        transform.DOPunchScale(new Vector3(1.2f, 1.2f, 1), 0.3f, 0, 0);
    }

    public void initializeRocket(TargetColor colorRef,int colorIndexRef, string tagName, bool isVert, bool isHoriz)
    {
        isVertRocket = isVert;
        isHorizRocket = isHoriz;
        color = colorRef;
        Color elemColor = FindObjectOfType<GameManager>().tileColors[colorIndexRef];
        GetComponent<SpriteRenderer>().color = elemColor;
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
}

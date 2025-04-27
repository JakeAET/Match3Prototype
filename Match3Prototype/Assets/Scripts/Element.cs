using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetColor
{
    none,
    red,
    blue,
    green,
    purple,
    yellow
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
    public GameObject burstEffectPrefab;
    public string colorName;
    public int colorIndex;
    public TargetColor color;
    public bool isFrozen = false;
    public bool isEnchanted = false;
    public float pointValue = 0;


    [Header("Board Variables")]
    public int column;
    //public int prevColumn;
    public int row;
    //public int prevRow;
    public float targetX;
    public float targetY;

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
    public int horizMatchLength = 0;
    public int vertMatchLength = 0;
    public List<Element> horizMatchedElements = new List<Element>();
    public List<Element> vertMatchedElements = new List<Element>();

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
            //SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            //mySprite.color = new Color(1f, 1f, 1f, 0.5f);
        }

        targetX = column * board.xSpawnOffsetMult;
        targetY = row * board.ySpawnOffsetMult;

        if (beingSwiped)
        {
            if (Mathf.Abs(targetX - transform.position.x) > 0.1)
            {
                // move towards target
                tempPosition = new Vector2(targetX, transform.position.y);
                transform.position = Vector2.Lerp(transform.position, tempPosition, swapSpeed);
                if (board.allElements[column, row] != this.gameObject)
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
            }
            if (Mathf.Abs(targetY - transform.position.y) > 0.1)
            {
                // move towards target
                tempPosition = new Vector2(transform.position.x, targetY);
                transform.position = Vector2.Lerp(transform.position, tempPosition, swapSpeed);
                if (board.allElements[column, row] != this.gameObject)
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
            }
        }
        else // falling
        {
            if (Mathf.Abs(targetX - transform.position.x) > 0.1)
            {
                // move towards target
                tempPosition = new Vector2(targetX, transform.position.y);
                transform.position = Vector2.Lerp(transform.position, tempPosition, fallSpeed);
                if (board.allElements[column, row] != this.gameObject)
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
                // move towards target
                tempPosition = new Vector2(transform.position.x, targetY);
                transform.position = Vector2.Lerp(transform.position, tempPosition, fallSpeed);
                if (board.allElements[column, row] != this.gameObject)
                {
                    board.allElements[column, row] = this.gameObject;
                }
            }
            else
            {
                // set pos
                tempPosition = new Vector2(transform.position.x, targetY);
                transform.position = tempPosition;
                //board.allElements[column, row] = this.gameObject;
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
        //Debug.Log("Waiting -> Moving Tiles");
        board.currentState = GameState.MovingTiles;
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1) // right swipe
        {
            otherElement = board.allElements[column + 1, row];
            otherElement.GetComponent<Element>().column -= 1;
            column += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1) // up swipe
        {
            otherElement = board.allElements[column, row + 1];
            otherElement.GetComponent<Element>().row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0) // left swipe
        {
            otherElement = board.allElements[column - 1, row];
            otherElement.GetComponent<Element>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0) // dowm swipe
        {
            otherElement = board.allElements[column, row - 1];
            otherElement.GetComponent<Element>().row += 1;
            row -= 1;
        }
        beingSwiped = true;
        otherElement.GetComponent<Element>().beingSwiped = true;
        StartCoroutine(CheckMove());
        //findMatches.FindAllMatchesStart();
        gameManager.turnStarted();
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


    //void FindMatches()
    //{
    //    if(column > 0 && column < board.width - 1)
    //    {
    //        GameObject leftElement1 = board.allElements[column - 1, row];
    //        GameObject rightElement1 = board.allElements[column + 1, row];
    //        if (leftElement1 != null && rightElement1 != null && leftElement1 != this.gameObject && rightElement1 != this.gameObject)
    //        {
    //            if (leftElement1.tag == this.gameObject.tag && rightElement1.tag == this.gameObject.tag)
    //            {
    //                leftElement1.GetComponent<Element>().isMatched = true;
    //                rightElement1.GetComponent<Element>().isMatched = true;
    //                isMatched = true;
    //            }
    //        }
    //    }
    //    if (row > 0 && row < board.height - 1)
    //    {
    //        GameObject upElement1 = board.allElements[column, row + 1];
    //        GameObject downElement1 = board.allElements[column, row - 1];
    //        if (upElement1 != null && downElement1 != null && upElement1 != this.gameObject && downElement1 != this.gameObject)
    //        {
    //            if (upElement1.tag == this.gameObject.tag && downElement1.tag == this.gameObject.tag)
    //            {
    //                upElement1.GetComponent<Element>().isMatched = true;
    //                downElement1.GetComponent<Element>().isMatched = true;
    //                isMatched = true;
    //            }
    //        }
    //    }
    //}
}

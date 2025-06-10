using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private BoardManager board;
    private GameManager gameManager;
    public List<GameObject> currentMatches = new List<GameObject>();
    public bool rogueCanTrigger = false;

    public delegate void RogueTrigger();
    public static event RogueTrigger OnRogueTrigger;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<BoardManager>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void FindAllMatchesStart()
    {
        StartCoroutine(FindAllMatches());
    }

    private IEnumerator FindAllMatches()
    {
        float waitTime = 0.1f;
        //yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentElement = board.allElements[i, j];
                if(currentElement != null)
                {
                    if(i > 0 && i < board.width - 1 && currentElement.GetComponent<Element>().horizMatchLength == 0)
                    {
                        GameObject leftElement = board.allElements[i - 1, j];
                        GameObject rightElement = board.allElements[i + 1, j];
                        if (leftElement != null && rightElement != null)
                        {
                            if(leftElement.tag == currentElement.tag && rightElement.tag == currentElement.tag)
                            {
                                List<Element> allElementsInMatch = new List<Element>();

                                if (!currentMatches.Contains(leftElement))
                                {
                                    currentMatches.Add(leftElement);
                                }
                                if (!currentMatches.Contains(rightElement))
                                {
                                    currentMatches.Add(rightElement);
                                }
                                if (!currentMatches.Contains(currentElement))
                                {
                                    currentMatches.Add(currentElement);
                                }
                                Element currentRef = currentElement.GetComponent<Element>();
                                currentRef.isMatched = true;
                                allElementsInMatch.Add(currentRef);
                                checkAdjacentForFrozen(i, j);

                                Element leftRef = leftElement.GetComponent<Element>();
                                leftRef.isMatched = true;
                                allElementsInMatch.Add(leftRef);
                                checkAdjacentForFrozen(i - 1, j);

                                Element rightRef = rightElement.GetComponent<Element>();
                                rightRef.isMatched = true;
                                allElementsInMatch.Add(rightRef);
                                checkAdjacentForFrozen(i + 1, j);

                                //check further left matches
                                int column = i - 2;
                                int row = j;

                                while (column >= 0)
                                {
                                    if (board.allElements[column, row].tag == board.allElements[column + 1, row].tag)
                                    {
                                        Element targetElement = board.allElements[column, row].GetComponent<Element>();
                                        targetElement.isMatched = true;
                                        allElementsInMatch.Add(targetElement);
                                        checkAdjacentForFrozen(column, row);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    column--;
                                }

                                //check further right matches
                                column = i + 2;
                                row = j;

                                while (column <= board.width - 1)
                                {
                                    if (board.allElements[column, row].tag == board.allElements[column - 1, row].tag)
                                    {
                                        Element targetElement = board.allElements[column, row].GetComponent<Element>();
                                        targetElement.isMatched = true;
                                        allElementsInMatch.Add(targetElement);
                                        checkAdjacentForFrozen(column, row);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    column++;
                                }

                                foreach (Element targetElement in allElementsInMatch)
                                {
                                    targetElement.horizMatchLength = allElementsInMatch.Count;

                                    foreach (Element e in allElementsInMatch)
                                    {
                                        targetElement.horizMatchedElements.Add(e);
                                    }

                                    if (targetElement.tileType == TileType.Rocket)
                                    {
                                        FindObjectOfType<AudioManager>().Play("firework");
                                        waitTime = 0.2f;
                                    }

                                    if (targetElement.tileType == TileType.Bomb)
                                    {
                                        FindObjectOfType<AudioManager>().Play("bomb");
                                        waitTime = 0.2f;
                                    }
                                }

                                Debug.Log("Horizontal match made: " + allElementsInMatch.Count + " tiles long");
                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1 && currentElement.GetComponent<Element>().vertMatchLength == 0)
                    {
                        GameObject upElement = board.allElements[i, j + 1];
                        GameObject downElement = board.allElements[i, j - 1];
                        if (upElement != null && downElement != null)
                        {
                            if (upElement.tag == currentElement.tag && downElement.tag == currentElement.tag)
                            {
                                List<Element> allElementsInMatch = new List<Element>();

                                if (!currentMatches.Contains(upElement))
                                {
                                    currentMatches.Add(upElement);
                                }
                                if (!currentMatches.Contains(downElement))
                                {
                                    currentMatches.Add(downElement);
                                }
                                if (!currentMatches.Contains(currentElement))
                                {
                                    currentMatches.Add(currentElement);
                                }

                                Element currentRef = currentElement.GetComponent<Element>();
                                currentRef.isMatched = true;
                                allElementsInMatch.Add(currentRef);
                                checkAdjacentForFrozen(i, j);

                                Element upRef = upElement.GetComponent<Element>();
                                upRef.isMatched = true;
                                allElementsInMatch.Add(upRef);
                                checkAdjacentForFrozen(i , j + 1);


                                Element downRef = downElement.GetComponent<Element>();
                                downRef.isMatched = true;
                                allElementsInMatch.Add(downRef);
                                checkAdjacentForFrozen(i, j - 1);

                                //check further down matches
                                int column = i;
                                int row = j - 2;

                                while (row >= 0)
                                {
                                    if(board.allElements[column,row].tag == board.allElements[column, row + 1].tag)
                                    {
                                        Element targetElement = board.allElements[column, row].GetComponent<Element>();
                                        targetElement.isMatched = true;
                                        allElementsInMatch.Add(targetElement);
                                        checkAdjacentForFrozen(column, row);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    row--;
                                }

                                //check further up matches
                                column = i;
                                row = j + 2;

                                while (row <= board.height - 1)
                                {
                                    if (board.allElements[column, row].tag == board.allElements[column, row - 1].tag)
                                    {
                                        Element targetElement = board.allElements[column, row].GetComponent<Element>();
                                        targetElement.isMatched = true;
                                        allElementsInMatch.Add(targetElement);
                                        checkAdjacentForFrozen(column, row);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    row++;
                                }

                                foreach (Element targetElement in allElementsInMatch)
                                {
                                    targetElement.vertMatchLength = allElementsInMatch.Count;

                                    foreach (Element e in allElementsInMatch)
                                    {
                                        targetElement.vertMatchedElements.Add(e);
                                    }

                                    if(targetElement.tileType == TileType.Rocket)
                                    {
                                        FindObjectOfType<AudioManager>().Play("firework");
                                        //waitTime = 0.3f;
                                    }

                                    if (targetElement.tileType == TileType.Bomb)
                                    {
                                        FindObjectOfType<AudioManager>().Play("bomb");
                                        //waitTime = 0.3f;
                                    }
                                }

                                Debug.Log("Vertical match made: " +  allElementsInMatch.Count + " tiles long");
                            }
                        } 
                    }
                }
            }
        }

        //Debug.Log("Count: " + currentMatches.Count);
        bool matchesFound = (currentMatches.Count > 0);

        // check for rogue effect
        if (rogueCanTrigger)
        {
            if (OnRogueTrigger != null)
            {
                OnRogueTrigger();
                matchesFound = true;
                rogueCanTrigger = false;
            }
        }

        if (matchesFound)
        {
            yield return new WaitForSeconds(waitTime);
        }
        else
        {
            yield return null;
        }
        board.allMatchesFound(matchesFound);
    }

    private void checkAdjacentForFrozen(int i, int j)
    {

        GameObject upElement = null;
        GameObject downElement = null;
        GameObject leftElement = null;
        GameObject rightElement = null;

        if (j > 0)
        {
             downElement = board.allElements[i, j - 1];
        }

        if (j < board.height - 1)
        {
            upElement = board.allElements[i, j + 1];
        }

        if (i > 0)
        {
            leftElement = board.allElements[i - 1, j];
        }

        if (i < board.width - 1)
        {
            rightElement = board.allElements[i + 1, j];
        }

        if (upElement != null)
        {
            if (upElement.GetComponent<Element>().isFrozen && !upElement.GetComponent<Element>().isMatched)
            {
                upElement.GetComponent<Element>().isMatched = true;
                checkAdjacentForFrozen(i, j + 1);
            }
        }

        if (downElement != null)
        {
            if (downElement.GetComponent<Element>().isFrozen && !downElement.GetComponent<Element>().isMatched)
            {
                downElement.GetComponent<Element>().isMatched = true;
                checkAdjacentForFrozen(i, j - 1);
            }
        }

        if (leftElement != null)
        {
            if (leftElement.GetComponent<Element>().isFrozen && !leftElement.GetComponent<Element>().isMatched)
            {
                leftElement.GetComponent<Element>().isMatched = true;
                checkAdjacentForFrozen(i - 1, j);
            }
        }

        if (rightElement != null)
        {
            if (rightElement.GetComponent<Element>().isFrozen && !rightElement.GetComponent<Element>().isMatched)
            {
                rightElement.GetComponent<Element>().isMatched = true;
                checkAdjacentForFrozen(i + 1, j);
            }
        }
    }
}

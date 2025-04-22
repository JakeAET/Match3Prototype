using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private BoardManager board;
    private GameManager gameManager;
    public List<GameObject> currentMatches = new List<GameObject>();

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
        //yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentElement = board.allElements[i, j];
                if(currentElement != null)
                {
                    if(i > 0 && i < board.width - 1)
                    {
                        GameObject leftElement = board.allElements[i - 1, j];
                        GameObject rightElement = board.allElements[i + 1, j];
                        if (leftElement != null && rightElement != null)
                        {
                            if(leftElement.tag == currentElement.tag && rightElement.tag == currentElement.tag)
                            {
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
                                leftElement.GetComponent<Element>().isMatched = true;
                                checkAdjacentForFrozen(i - 1, j);
                                rightElement.GetComponent<Element>().isMatched = true;
                                checkAdjacentForFrozen(i, j);
                                currentElement.GetComponent<Element>().isMatched = true;
                                checkAdjacentForFrozen(i + 1, j);

                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upElement = board.allElements[i, j + 1];
                        GameObject downElement = board.allElements[i, j - 1];
                        if (upElement != null && downElement != null)
                        {
                            if (upElement.tag == currentElement.tag && downElement.tag == currentElement.tag)
                            {
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
                                upElement.GetComponent<Element>().isMatched = true;
                                checkAdjacentForFrozen(i , j + 1);
                                downElement.GetComponent<Element>().isMatched = true;
                                checkAdjacentForFrozen(i, j - 1);
                                currentElement.GetComponent<Element>().isMatched = true;
                                checkAdjacentForFrozen(i, j);
                            }
                        } 
                    }
                }
            }
        }
        Debug.Log("Count: " + currentMatches.Count);
        bool matchesFound = (currentMatches.Count > 0);

        if (matchesFound)
        {
            yield return new WaitForSeconds(1f);
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

        if (j > 0 && j < board.height - 1)
        {
             upElement = board.allElements[i, j + 1];
             downElement = board.allElements[i, j - 1];
        }

        if (i > 0 && i < board.width - 1)
        {
             leftElement = board.allElements[i - 1, j];
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

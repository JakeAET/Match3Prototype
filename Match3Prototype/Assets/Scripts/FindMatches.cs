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
        yield return new WaitForSeconds(0.2f);
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
                                rightElement.GetComponent<Element>().isMatched = true;
                                currentElement.GetComponent<Element>().isMatched = true;
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
                                downElement.GetComponent<Element>().isMatched = true;
                                currentElement.GetComponent<Element>().isMatched = true;
                            }
                        } 
                    }
                }
            }
        }
        if (currentMatches.Count == 0)
        {
            //board.currentState = GameState.move;
            //gameManager.turnEnded();
        }
    }
}

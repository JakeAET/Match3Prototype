using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] float baseTargetScore = 0;
    [SerializeField] float targetScoreIncMult = 0;

    public float currentScore = 0;
    public float currentTargetScore = 0;

    public float baseElementValue = 0;
    public float streakValue = 1;

    [SerializeField] int maxTurns = 0;
    public int currentTurn = 0;

    private int currentRound = 0;

    private BoardManager board;

    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text streakText;
    [SerializeField] TMP_Text turnsText;
    [SerializeField] TMP_Text roundsText;
    [SerializeField] TMP_Text targetScoreText;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<BoardManager>();
        startRound();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void turnStarted()
    {
        currentTurn--;
        turnsText.text = "Turns Left: " + currentTurn;
    }

    public void turnEnded()
    {
        if (currentScore >= currentTargetScore) // game ends, won
        {
            roundEnded(true);
        }
        else if (currentTurn == 0) // game ends, lost
        {
            roundEnded(false);
        }
    }

    private void roundEnded(bool roundWon)
    {
        if (roundWon)
        {
            Debug.Log("Round Won");
            board.clearBoard();
            board.RefillBoard();
            startRound();
        }
        else
        {
            Debug.Log("Round Lost");
            gameOver();
        }
    }

    private void startRound()
    {
        currentScore = 0;
        currentTurn = maxTurns;
        currentRound++;

        currentTargetScore = baseTargetScore * targetScoreIncMult * currentRound;

        scoreText.text = "Score: " + currentScore;
        //streakText.text = "Streak: " + streakValue;
        turnsText.text = "Turns Left: " + currentTurn;
        roundsText.text = "Round: " + currentRound;
        targetScoreText.text = "Target Score: " + currentTargetScore;
    }

    private void gameOver()
    {
        currentScore = 0;
        currentTurn = 0;
        currentRound = 1;
    }

    public void IncreaseScore(float increaseAmount)
    {
        currentScore += increaseAmount;
        scoreText.text = "Score: " + currentScore;
    }

    public void increaseStreak()
    {
        streakValue++;
        //streakText.text = "Streak: " + streakValue;
    }

    private void updateUI()
    {
        scoreText.text = "Score: " + currentScore;
        //streakText.text = "Streak: " + streakValue;
        turnsText.text = "Turns Left: " + currentTurn;
        roundsText.text = "Round: " + currentRound;
        targetScoreText.text = "Target Score: " + currentTargetScore;
    }
}

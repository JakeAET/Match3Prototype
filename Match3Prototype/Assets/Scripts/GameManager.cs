using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using UnityEngine.UI;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    [SerializeField] float baseTargetScore = 0;
    [SerializeField] float targetScoreIncMult = 0;

    public float currentScore = 0;
    public float currentTargetScore = 0;

    public float baseElementValue = 0;
    public float streakValue = 1;

    public int maxTurns = 0;
    public int currentTurn = 0;

    public int maxUndos = 0;
    public int currentUndos = 0;
    public bool undoAllowed = false;

    public int currentRound = 0;

    private bool turnActive = false;

    private BoardManager board;
    private UIManager ui;

    //[SerializeField] TMP_Text scoreText;
    //[SerializeField] TMP_Text streakText;
    //[SerializeField] TMP_Text turnsText;
    //[SerializeField] TMP_Text roundsText;
    //[SerializeField] TMP_Text targetScoreText;


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<BoardManager>();
        ui = FindObjectOfType<UIManager>();
        startRound();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void turnStarted()
    {
        UnityEngine.Debug.Log("turn started");
        if (!turnActive)
        {
            currentTurn--;
            ui.updateTurns(currentTurn);
            turnActive = true;
        }
    }

    public void turnEnded()
    {
        if (turnActive)
        {
            UnityEngine.Debug.Log("turn " + (maxTurns - currentTurn) +  " ended - current score:  " + currentScore);
            board.currentState = GameState.move;
            board.reassignTileIDs();
            if (currentScore >= currentTargetScore) // game ends, won
            {
                roundEnded(true);
            }
            else if (currentTurn == 0) // game ends, lost
            {
                roundEnded(false);
            }

            if (!undoAllowed && currentUndos > 0)
            {
                undoAllowed = true;
                ui.checkToEnableUndo();
            }

            turnActive = false;
        }
    }

    private void roundEnded(bool roundWon)
    {
        undoAllowed = false;
        ui.disableUndo();

        if (roundWon)
        {
            UnityEngine.Debug.Log("Round Won");
            board.clearBoard();
            board.ResetBoard();
            ui.displayWinScreen();
            //startRound();
        }
        else
        {
            UnityEngine.Debug.Log("Round Lost");
            ui.displayLoseScreen();
            gameOver();
        }
    }

    public void startRound()
    {
        currentScore = 0;
        currentTurn = maxTurns;
        currentUndos = maxUndos;
        currentRound++;

        currentTargetScore = baseTargetScore * targetScoreIncMult * currentRound;

        //ui.updateScore(currentScore);
        ui.updateTurns(currentTurn);
        ui.updateRounds(currentRound);
        //ui.updateTargetScore(currentTargetScore);
        ui.updateScoreProgress( currentScore, currentTargetScore);
        ui.undoCountUpdate(currentUndos);

        undoAllowed = false;
        ui.disableUndo();

        //scoreText.text = "Score: " + currentScore;
        //streakText.text = "Streak: " + streakValue;
        //turnsText.text = "Turns Left: " + currentTurn;
        //roundsText.text = "Round: " + currentRound;
        //targetScoreText.text = "Target Score: " + currentTargetScore;
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
        ui.updateScoreProgress(currentScore, currentTargetScore);
    }

    public void increaseStreak()
    {
        streakValue++;
        //streakText.text = "Streak: " + streakValue;
    }

    public void increaseMaxTurns(int num)
    {
        maxTurns += num;
        ui.updateTurns(currentTurn);
    }

    public void increaseMaxUndos(int num)
    {
        maxUndos += num;
        currentUndos = maxUndos;
        ui.undoCountUpdate(currentUndos);
    }

    public void useUndoMove()
    {
        currentUndos--;
        board.undoLastMove();
        ui.undoCountUpdate(currentUndos);
        undoAllowed = false;
        ui.disableUndo();
    }

    //private void updateUI()
    //{
    //    scoreText.text = "Score: " + currentScore;
    //    //streakText.text = "Streak: " + streakValue;
    //    turnsText.text = "Turns Left: " + currentTurn;
    //    roundsText.text = "Round: " + currentRound;
    //    targetScoreText.text = "Target Score: " + currentTargetScore;
    //}
}

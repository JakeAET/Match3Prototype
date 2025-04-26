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

    public float baseElementValue;
    public float largeMatchBonus;
    public float streakValue = 1;
    public float enchantedTileMulti;

    public int maxTurns = 0;
    public int currentTurn = 0;

    public int maxUndos = 0;
    public int currentUndos = 0;
    public bool undoAllowed = false;

    public int currentRound = 0;
    public int currentGame = 1;

    public bool roundActive = false;
    public bool bossRound = false;

    private BoardManager board;
    private UIManager ui;

    [SerializeField] BossRound[] bossRounds;
    private List<BossRound> availableBossRounds = new List<BossRound>();
    public BossRound currentBossRound;

    //[SerializeField] TMP_Text scoreText;
    //[SerializeField] TMP_Text streakText;
    //[SerializeField] TMP_Text turnsText;
    //[SerializeField] TMP_Text roundsText;
    //[SerializeField] TMP_Text targetScoreText;


    // Start is called before the first frame update
    void Start()
    {
        foreach (BossRound br in bossRounds)
        {
            availableBossRounds.Add(br);
        }

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
        currentTurn--;
        ui.updateTurns(currentTurn);
    }

    public void turnEnded()
    {
        //UnityEngine.Debug.Log("turn " + (maxTurns - currentTurn) + " ended - current score:  " + currentScore);
        board.reassignTileIDs();
        if (currentScore >= currentTargetScore) // game ends, won
        {
            StartCoroutine(roundEnded(true));
        }
        else if (currentTurn == 0) // game ends, lost
        {
            StartCoroutine(roundEnded(false));
        }

        if (!undoAllowed && currentUndos > 0)
        {
            undoAllowed = true;
            ui.checkToEnableUndo();
        }
    }

    private IEnumerator roundEnded(bool roundWon)
    {
        board.roundOver(true);
        roundActive = false;
        undoAllowed = false;
        ui.disableUndo();
        yield return new WaitForSeconds(0.5f);

        if (roundWon)
        {
            //UnityEngine.Debug.Log("Round Won");
            board.clearBoard();

            if(currentRound == 4) // Boss Round
            {
                // determine which boss round (change to find this out at the start of the game instead later)
                determineBossCondition();
                // tell UI manager which boss round
                ui.displayBossInfoPanel(currentBossRound);

                bossRound = true;

                if (currentBossRound.constantEffect)
                {
                    currentBossRound.activateConstraint();
                }
            }
            else if (currentRound == 5)
            {
                // show boss win screen with stats
                // select from 5 patrons now

                bossRound = false;
                ui.displayWinScreen(true);

                if (currentBossRound.constantEffect)
                {
                    currentBossRound.deactivateConstraint();
                }
            }
            else
            {
                ui.displayWinScreen(false);
            }

            //startRound();
        }
        else
        {
            //UnityEngine.Debug.Log("Round Lost");
            ui.displayLoseScreen();
            gameOver();
        }
    }

    public void startRound()
    {
        board.roundOver(false);
        board.currentState = GameState.SettingBoard;
        roundActive = true;
        currentScore = 0;
        currentTurn = maxTurns;
        currentUndos = maxUndos;

        if(currentRound == 5)
        {
            currentRound = 1;
            currentGame++;
        }
        else
        {
            currentRound++;
        }

        currentTargetScore = (baseTargetScore + (baseTargetScore * targetScoreIncMult * 5 * (currentGame - 1))) * targetScoreIncMult * currentRound;

        //ui.updateScore(currentScore);
        ui.updateTurns(currentTurn);
        ui.updateRounds(currentRound);
        ui.updateGames(currentGame);
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

    private void determineBossCondition()
    {
        currentBossRound = availableBossRounds[Random.Range(0, availableBossRounds.Count - 1)];
        availableBossRounds.Remove(currentBossRound);
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

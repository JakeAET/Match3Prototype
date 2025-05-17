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
    [SerializeField] float roundScoreIncMult = 0;
    [SerializeField] float gameScoreIncMult = 0;
    public float extraHighPointMulti = 1;

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

    public int maxRefreshes = 0;
    public int currentRefreshes = 0;
    public bool refreshAllowed = false;

    public int maxSkips = 0;
    public int currentSkips= 0;
    public bool skipAllowed = false;

    public int currentRound = 0;
    public int currentGame = 1;

    public bool roundActive = false;
    public bool bossRound = false;

    private BoardManager board;
    private UIManager ui;
    private PatronManager patronManager;

    [SerializeField] BossRound[] bossRounds;
    private List<BossRound> availableBossRounds = new List<BossRound>();
    public BossRound currentBossRound;

    public Color[] tileColors;
    //0 = red
    //1 = blue
    //2 = green
    //3 = purple
    //4 = yellow

    //Stats
    public float bestScore;
    public int matchesMade;
    public int tilesCleared;
    public Dictionary<string, int> colorTilesCleared = new Dictionary<string, int>();

    void Start()
    {
        colorTilesCleared.Add("red", 0);
        colorTilesCleared.Add("blue", 0);
        colorTilesCleared.Add("green", 0);
        colorTilesCleared.Add("purple", 0);
        colorTilesCleared.Add("yellow", 0);

        //Application.targetFrameRate = 24;

        foreach (BossRound br in bossRounds)
        {
            availableBossRounds.Add(br);
        }

        board = FindObjectOfType<BoardManager>();
        ui = FindObjectOfType<UIManager>();

        if (maxSkips > 0)
        {
            currentSkips = maxSkips;
            skipAllowed = true;
            ui.toggleSkipInteract(skipAllowed);
            ui.skipCountUpdate(currentSkips);

        }

        if (maxRefreshes > 0)
        {
            currentRefreshes = maxRefreshes;
            refreshAllowed = true;
            ui.toggleRefreshInteract(refreshAllowed);
            ui.refreshCountUpdate(currentRefreshes);
        }

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
        if (currentScore >= currentTargetScore) // game ends, won
        {
            if(currentScore > bestScore)
            {
                bestScore = currentScore;
            }

            StartCoroutine(roundEnded(true));
        }
        else if (currentTurn == 0) // game ends, lost
        {
            if(bestScore == 0)
            {
                bestScore = currentScore;
            }

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
        ui.toggleUndoInteract(false);
        yield return new WaitForSeconds(0.5f);

        if (roundWon)
        {
            //UnityEngine.Debug.Log("Round Won");
            board.clearBoard();

            if(currentRound == 4) // Boss Round
            {
                // determine which boss round (change to find this out at the start of the game instead later)
                determineBossCondition();
                bossRound = true;
                ui.displayWinScreen(false);
            }
            else if (currentRound == 5)
            {
                // show boss win screen with stats
                // select from 5 patrons now

                if(currentGame == 10)
                {
                    ui.displayRunComplete();
                }
                else
                {
                    bossRound = false;
                    ui.displayWinScreen(true);

                    if (currentBossRound.constantEffect)
                    {
                        currentBossRound.deactivateConstraint();
                    }
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

        currentTargetScore = (baseTargetScore * (currentGame * (gameScoreIncMult - 1))) * (1 + (roundScoreIncMult * (currentRound - 1))) * extraHighPointMulti;

        //ui.updateScore(currentScore);
        ui.updateTurns(currentTurn);
        ui.updateRounds(currentRound);
        ui.updateGames(currentGame);
        //ui.updateTargetScore(currentTargetScore);
        ui.updateScoreProgress( currentScore, currentTargetScore);
        ui.undoCountUpdate(currentUndos);

        undoAllowed = false;
        ui.toggleUndoInteract(false);

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
        ui.toggleUndoInteract(false);
    }

    public void useSkip()
    {
        --currentSkips;
        if (currentSkips == 0)
        {
            skipAllowed = false;
        }
        ui.toggleSkipInteract(skipAllowed);
        ui.skipCountUpdate(currentSkips);
    }

    public void useRefresh()
    {
        --currentRefreshes;
        if (currentRefreshes == 0)
        {
            refreshAllowed = false;
        }
        ui.toggleRefreshInteract(refreshAllowed);
        ui.refreshCountUpdate(currentRefreshes);
    }

    private void determineBossCondition()
    {
        currentBossRound = availableBossRounds[Random.Range(0, availableBossRounds.Count)];
        availableBossRounds.Remove(currentBossRound);
    }

    public string mostMatchedTile()
    {
        int mostMatchedNum = 0;
        string mostMatchedColor = "";
        foreach (KeyValuePair<string, int> kvp in colorTilesCleared)
        {
            if(kvp.Value > mostMatchedNum)
            {
                mostMatchedNum = kvp.Value;
                mostMatchedColor = kvp.Key;
            }
        }
        return mostMatchedColor;
    }
}

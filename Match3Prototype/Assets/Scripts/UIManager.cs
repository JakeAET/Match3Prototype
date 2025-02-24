using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;

    [Header("Top Panel")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text streakText;
    [SerializeField] TMP_Text turnsText;
    [SerializeField] TMP_Text roundsText;
    [SerializeField] TMP_Text targetScoreText;

    [Header("Win Screen")]
    [SerializeField] GameObject winPanel;
    [SerializeField] TMP_Text winRoundText;

    [Header("Lose Screen")]
    [SerializeField] GameObject losePanel;
    [SerializeField] TMP_Text loseRoundText;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateScore(float num)
    {
        scoreText.text = "Score: " + num;
    }

    public void updateStreak(float num)
    {
        streakText.text = "Streak: " + num;
    }

    public void updateTurns(int num)
    {
        turnsText.text = "Turns Left: " + num;
    }

    public void updateRounds(int num)
    {
        roundsText.text = "Round: " + num;
    }

    public void updateTargetScore(float num)
    {
        targetScoreText.text = "Target Score: " + num;
    }

    public void displayWinScreen()
    {
        winPanel.SetActive(true);
        winRoundText.text = "Round " + gameManager.currentRound + " Complete";
    }

    public void displayLoseScreen()
    {
        losePanel.SetActive(true);
        loseRoundText.text = "Round " + gameManager.currentRound + " Failed";
    }

    public void startNextRound()
    {
        winPanel.SetActive(false);
        gameManager.startRound();
    }

    public void startAfterLoss()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

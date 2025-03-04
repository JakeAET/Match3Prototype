using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;
    private PatronManager patronManager;

    [Header("Top Panel")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text streakText;
    [SerializeField] TMP_Text turnsText;
    [SerializeField] TMP_Text roundsText;
    [SerializeField] TMP_Text targetScoreText;

    [Header("Win Screen")]
    [SerializeField] GameObject winPanel;
    [SerializeField] TMP_Text winRoundText;
    [SerializeField] Image patronImg1;
    [SerializeField] Image patronImg2;
    [SerializeField] Image patronImg3;
    private Patron patronOption1;
    private Patron patronOption2;
    private Patron patronOption3;

    [Header("Lose Screen")]
    [SerializeField] GameObject losePanel;
    [SerializeField] TMP_Text loseRoundText;

    [Header("Bottom Panel")]
    public Image[] patronLowerImgs;
    public TMP_Text[] patronLvls;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        patronManager = FindObjectOfType<PatronManager>();
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
        List<Patron> patronOptions = patronManager.select3Patrons();
        patronOption1 = patronOptions[0];
        patronOption2 = patronOptions[1];
        patronOption3 = patronOptions[2];
        patronImg1.sprite = patronOption1.sprite;
        patronImg2.sprite = patronOption2.sprite;
        patronImg3.sprite = patronOption3.sprite;

        winPanel.SetActive(true);
        winRoundText.text = "Round " + gameManager.currentRound + " Complete";
    }

    public void displayLoseScreen()
    {
        losePanel.SetActive(true);
        loseRoundText.text = "Round " + gameManager.currentRound + " Failed";
    }

    public void patronBttn1()
    {
        patronManager.selectNewPatron(patronOption1);
        
        winPanel.SetActive(false);
        gameManager.startRound();
    }

    public void patronBttn2()
    {
        patronManager.selectNewPatron(patronOption2);

        winPanel.SetActive(false);
        gameManager.startRound();
    }

    public void patronBttn3()
    {
        patronManager.selectNewPatron(patronOption3);

        winPanel.SetActive(false);
        gameManager.startRound();
    }

    //public void startNextRound()
    //{
    //    winPanel.SetActive(false);
    //    gameManager.startRound();
    //}

    public void startAfterLoss()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

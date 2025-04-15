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
    //[SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text streakText;
    [SerializeField] TMP_Text turnsText;
    [SerializeField] TMP_Text roundsText;
    [SerializeField] TMP_Text progressText;

    [SerializeField] TMP_Text undoCountText;
    [SerializeField] Button undoButton;
    //[SerializeField] TMP_Text targetScoreText;

    [Header("Win Screen")]
    [SerializeField] GameObject winPanel;
    [SerializeField] TMP_Text winRoundText;
    [SerializeField] Image patronImg1;
    [SerializeField] Image patronImg2;
    [SerializeField] Image patronImg3;
    private Patron patronOption1;
    private Patron patronOption2;
    private Patron patronOption3;
    [SerializeField] TMP_Text patron1Title;
    [SerializeField] TMP_Text patron2Title;
    [SerializeField] TMP_Text patron3Title;
    [SerializeField] TMP_Text patron1Desc;
    [SerializeField] TMP_Text patron2Desc;
    [SerializeField] TMP_Text patron3Desc;

    [Header("Lose Screen")]
    [SerializeField] GameObject losePanel;
    [SerializeField] TMP_Text loseRoundText;

    [Header("Patron Panel")]
    public Image[] patronUpperImgs;
    public GameObject[] patronSlots;
    public TMP_Text[] patronLvls;
    [SerializeField] Slider progressSlider;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        patronManager = FindObjectOfType<PatronManager>();

        if(gameManager.currentUndos == 0)
        {
            undoButton.interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateScoreProgress(float score, float target)
    {
        progressText.text = score + " / " + target;

        float sliderValue = score / target;

        if (sliderValue > 1)
        {
            sliderValue = 1;
        }

        progressSlider.value = sliderValue;
    }

    public void updateStreak(float num)
    {
        streakText.text = "Streak: " + num;
    }

    public void updateTurns(int num)
    {
        turnsText.text = "" + num;
    }

    public void updateRounds(int num)
    {
        roundsText.text = "" + num;
    }

    //public void updateTargetScore(float num)
    //{
    //    targetScoreText.text = "Target Score: " + num;
    //}

    public void displayWinScreen()
    {
        List<Patron> patronOptions = patronManager.select3Patrons();
        patronOption1 = patronOptions[0];
        patronOption2 = patronOptions[1];
        patronOption3 = patronOptions[2];

        patronImg1.sprite = patronOption1.sprite;
        patronImg1.color = patronOption1.color;
        patron1Desc.text = patronOption1.effectDescription;
        patron1Title.text = patronOption1.title;

        patronImg2.sprite = patronOption2.sprite;
        patronImg2.color = patronOption2.color;
        patron2Desc.text = patronOption2.effectDescription;
        patron2Title.text = patronOption2.title;

        patronImg3.sprite = patronOption3.sprite;
        patronImg3.color = patronOption3.color;
        patron3Desc.text = patronOption3.effectDescription;
        patron3Title.text = patronOption3.title;

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

    public void undoButtonPress()
    {
        if(gameManager.currentUndos > 0)
        {
            gameManager.useUndoMove();
        }
    }

    public void undoCountUpdate(int count)
    {
        undoCountText.text = "" + count;
    }

    public void checkToEnableUndo()
    {
        if (gameManager.currentUndos > 0 && !undoButton.IsInteractable())
        {
            undoButton.interactable = true;
        }
    }

    public void disableUndo()
    {
        undoButton.interactable = false;
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

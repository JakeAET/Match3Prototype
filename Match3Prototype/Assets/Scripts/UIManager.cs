using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build;
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
    [SerializeField] TMP_Text gamesText;
    [SerializeField] TMP_Text progressText;

    [SerializeField] TMP_Text undoCountText;
    [SerializeField] Button undoButton;
    //[SerializeField] TMP_Text targetScoreText;

    [Header("Win Screen")]
    [SerializeField] GameObject winPanel;
    [SerializeField] TMP_Text winRoundText;
    [SerializeField] GameObject patronChoicePanel;
    [SerializeField] GameObject patronChoicePrefab;
    public List<GameObject> currentChoicePrefabs;
    public List<Patron> selectedPatrons;
    public List<patronChoiceUI> patronUIRefs;
    public List<patronChoiceUI> selectedPatronUIRefs;
    [SerializeField] Button confirmPatronsButton;
    [SerializeField] TMP_Text confirmPatronsBttnText;
    private string confirmPatronsText;
    public int patronChoiceLimit;
    [SerializeField] TMP_Text skipCountText;
    [SerializeField] Button skipButton;
    [SerializeField] TMP_Text refreshCountText;
    [SerializeField] Button refreshButton;

    [Header("Run Completion Screen")]
    [SerializeField] GameObject runCompletePanel;

    //[SerializeField] Image patronImg1;
    //[SerializeField] Image patronImg2;
    //[SerializeField] Image patronImg3;
    //private Patron patronOption1;
    //private Patron patronOption2;
    //private Patron patronOption3;
    //[SerializeField] TMP_Text patron1Title;
    //[SerializeField] TMP_Text patron2Title;
    //[SerializeField] TMP_Text patron3Title;
    //[SerializeField] TMP_Text patron1Desc;
    //[SerializeField] TMP_Text patron2Desc;
    //[SerializeField] TMP_Text patron3Desc;

    [Header("Boss Info Screen")]
    [SerializeField] GameObject bossInfoPanel;
    [SerializeField] TMP_Text bossTitleText;
    [SerializeField] TMP_Text bossDescriptionText;

    [Header("Lose Screen")]
    [SerializeField] GameObject losePanel;
    [SerializeField] TMP_Text loseRoundText;

    [Header("Patron Panel")]
    public GameObject[] patronSlots;
    public List<PatronTopUI> patronSlotUIRefs;
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

        foreach (GameObject slot in patronSlots)
        {
            patronSlotUIRefs.Add(slot.GetComponent<PatronTopUI>());
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

    public void updateGames(int num)
    {
        gamesText.text = "" + num;
    }

    public void displayBossInfoPanel(BossRound br)
    {
        bossInfoPanel.GetComponent<RectTransform>().localScale = Vector3.zero;
        bossInfoPanel.SetActive(true);
        bossInfoPanel.GetComponent<RectTransform>().DOScale(Vector3.one, 0.3f);
        bossTitleText.text = br.title;
        bossDescriptionText.text = br.description;
        bossTitleText.color = br.titleTextColor;
    }

    //public void updateTargetScore(float num)
    //{
    //    targetScoreText.text = "Target Score: " + num;
    //}

    public void displayWinScreen(bool isBossRound)
    {
        confirmPatronsButton.interactable = false;
        int patronsToChoose = 0;

        if (isBossRound)
        {
            patronsToChoose = 5;
            patronChoiceLimit = 2;
            confirmPatronsText = "Choose two patrons";
        }
        else
        {
            patronsToChoose = 3;
            patronChoiceLimit = 1;
            confirmPatronsText = "Choose one patron";
        }

        confirmPatronsBttnText.text = confirmPatronsText;

        List<Patron> patronOptions = patronManager.selectPatrons(patronsToChoose);

        foreach (Patron p in patronOptions)
        {
            GameObject newPatronChoice = Instantiate(patronChoicePrefab, patronChoicePanel.transform);
            newPatronChoice.GetComponent<patronChoiceUI>().initialize(p);
        }

        winPanel.GetComponent<RectTransform>().localScale = Vector3.zero;
        winPanel.SetActive(true);
        winPanel.GetComponent<RectTransform>().DOScale(Vector3.one, 0.3f);

        winRoundText.text = "Round " + gameManager.currentRound + " Complete";
    }

    public void displayRunComplete()
    {
        runCompletePanel.GetComponent<RectTransform>().localScale = Vector3.zero;
        runCompletePanel.SetActive(true);
        runCompletePanel.GetComponent<RectTransform>().DOScale(Vector3.one, 0.3f);
    }

    public void displayLoseScreen()
    {
        losePanel.SetActive(true);
        loseRoundText.text = "Defeated on\nGame " + gameManager.currentGame + " - Round " + gameManager.currentRound;
    }

    public void patronToggle(patronChoiceUI thisPatronChoice, bool toggleOn)
    {
        FindObjectOfType<AudioManager>().Play("ui click");
        if (toggleOn)
        {
            if(selectedPatronUIRefs.Count == patronChoiceLimit) // already at limit
            {
                selectedPatronUIRefs.Add(thisPatronChoice);
                selectedPatronUIRefs[0].patronToggle.isOn = false;
            }
            else
            {
                selectedPatronUIRefs.Add(thisPatronChoice);
            }
        }
        else
        {
            selectedPatronUIRefs.Remove(thisPatronChoice);
            if(selectedPatronUIRefs.Count < patronChoiceLimit)
            {
                confirmPatronsButton.interactable = false;
                confirmPatronsBttnText.text = confirmPatronsText;
            }
        }

        if(selectedPatronUIRefs.Count == patronChoiceLimit)
        {
            confirmPatronsButton.interactable = true;
            confirmPatronsBttnText.text = "confirm";
        }
    }

    public void confirmPatronSelect()
    {
        winPanel.SetActive(false);

        foreach (patronChoiceUI patronUI in selectedPatronUIRefs)
        {
            patronManager.selectNewPatron(patronUI.patronRef);
        }

        for (int i = 0; i < currentChoicePrefabs.Count; i++)
        {
            GameObject objToDestroy = currentChoicePrefabs[i];
            Destroy(objToDestroy);
        }
        currentChoicePrefabs.Clear();
        patronUIRefs.Clear();
        selectedPatronUIRefs.Clear();

        gameManager.startRound();
    }

    public void refreshPatronSelect()
    {
        int patronsToChoose = currentChoicePrefabs.Count;

        for (int i = 0; i < currentChoicePrefabs.Count; i++)
        {
            GameObject objToDestroy = currentChoicePrefabs[i];
            Destroy(objToDestroy);
        }
        currentChoicePrefabs.Clear();
        patronUIRefs.Clear();
        selectedPatronUIRefs.Clear();

        List<Patron> patronOptions = patronManager.selectPatrons(patronsToChoose);

        foreach (Patron p in patronOptions)
        {
            GameObject newPatronChoice = Instantiate(patronChoicePrefab, patronChoicePanel.transform);
            newPatronChoice.GetComponent<patronChoiceUI>().initialize(p);
        }
    }

    public void skipPatronSelect()
    {
        winPanel.SetActive(false);
        for (int i = 0; i < currentChoicePrefabs.Count; i++)
        {
            GameObject objToDestroy = currentChoicePrefabs[i];
            Destroy(objToDestroy);
        }
        currentChoicePrefabs.Clear();
        patronUIRefs.Clear();
        selectedPatronUIRefs.Clear();

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

    public void refreshCountUpdate(int count)
    {
        refreshCountText.text = "" + count;
    }

    public void skipCountUpdate(int count)
    {
        skipCountText.text = "" + count;
    }

    public void checkToEnableUndo()
    {
        if (gameManager.currentUndos > 0 && !undoButton.IsInteractable())
        {
            undoButton.interactable = true;
        }
    }

    public void toggleUndoInteract(bool canInteract)
    {
        undoButton.interactable = canInteract;
    }

    public void toggleSkipInteract(bool canInteract)
    {
        skipButton.interactable = canInteract;
    }

    public void toggleRefreshInteract(bool canInteract)
    {
        refreshButton.interactable = canInteract;
    }

    public void refreshButtonPress()
    {
        if(gameManager.currentRefreshes > 0)
        {
            refreshPatronSelect();
            gameManager.useRefresh();
        }
    }

    public void skipButtonPress()
    {
        if (gameManager.currentSkips > 0)
        {
            skipPatronSelect();
            gameManager.useSkip();
        }
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

    public void playSound(string soundName)
    {
        FindObjectOfType<AudioManager>().Play(soundName);
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;
    private PatronManager patronManager;
    private AudioManager audioManager;

    [Header("Top Panel")]
    //[SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text streakText;
    [SerializeField] TMP_Text turnsText;
    [SerializeField] GameObject turnTextObj;
    [SerializeField] GameObject turnObj;
    [SerializeField] GameObject textPopupPrefab;
    [SerializeField] TMP_Text roundsText;
    [SerializeField] TMP_Text gamesText;
    [SerializeField] TMP_Text progressText;

    [SerializeField] TMP_Text undoCountText;
    [SerializeField] Button undoButton;
    //[SerializeField] TMP_Text targetScoreText;

    [Header("Win Screen")]
    public GameObject winPanel;
    [SerializeField] TMP_Text winRoundText;
    [SerializeField] TMP_Text choosePatronText;
    [SerializeField] GameObject patronChoicePanel;
    [SerializeField] GameObject patronChoicePrefab;
    public List<GameObject> currentChoicePrefabs;
    public List<Patron> selectedPatrons;
    public List<patronChoiceUI> patronUIRefs;
    public List<patronChoiceUI> selectedPatronUIRefs;
    [SerializeField] Button confirmPatronsButton;
    [SerializeField] TMP_Text confirmPatronsBttnText;
    //private string confirmPatronsText;
    public int patronChoiceLimit;
    //[SerializeField] TMP_Text skipCountText;
    //[SerializeField] Button skipButton;
    [SerializeField] TMP_Text refreshCountText;
    [SerializeField] Button refreshButton;

    [Header("Run Completion Screen")]
    [SerializeField] GameObject runCompletePanel;
    [SerializeField] TMP_Text winStatsText;

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
    [SerializeField] TMP_Text lossStatsText;

    [Header("Patron Panel")]
    [SerializeField] Slider progressSlider;
    [SerializeField] GameObject sliderObj;
    [SerializeField] Image sliderFill;
    private Color sliderBaseColor;
    private Color sliderBrightColor;

    public GameObject[] patronSlots;
    public List<PatronTopUI> patronSlotUIRefs;

    // Win Screen Patron Panel
    public GameObject[] patronSlotsWS;
    public List<PatronTopUI> patronSlotUIRefsWS;

    [Header("Patron Info Popup")]
    private Patron currentInfoPanelPatron;
    [SerializeField] GameObject patronInfoPanel;
    [SerializeField] TMP_Text infoPanelTitle;
    [SerializeField] TMP_Text infoPanelDesc;
    public GameObject removeButtonObj;

    [Header("Settings Panel")]
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Toggle musicToggle;
    [SerializeField] Toggle sfxToggle;

    [SerializeField] Color[] healthBarColors;

    [SerializeField] SkillTreeUI skillTree;

    [SerializeField] Animator turnCountAnimator;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        patronManager = FindObjectOfType<PatronManager>();
        audioManager = FindObjectOfType<AudioManager>();

        if (gameManager.currentUndos == 0)
        {
            undoButton.interactable = false;
        }

        foreach (GameObject slot in patronSlots)
        {
            patronSlotUIRefs.Add(slot.GetComponent<PatronTopUI>());
        }

        foreach (GameObject slot in patronSlotsWS)
        {
            patronSlotUIRefsWS.Add(slot.GetComponent<PatronTopUI>());
        }

        sliderBaseColor = sliderFill.color;
        sliderBrightColor = new Color(sliderBaseColor.r * 1.3f, sliderBaseColor.g * 1.3f, sliderBaseColor.b * 1.3f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void lowTurnEffect(bool active)
    {
        turnCountAnimator.SetBool("low turns", active);
    }

    public void updateScoreDirectly(float score, float target)
    {
        //progressText.text = score + " / " + target;
        progressText.text = target - score + " HP";

        //float sliderValue = score / target;
        float sliderValue = (target - score) / target;

        if (sliderValue < 0)
        {
            sliderValue = 0;
        }

        progressSlider.value = sliderValue;
    }

    public void updateSliderColor(int round)
    {
        sliderFill.color = healthBarColors[round - 1];

        sliderBaseColor = sliderFill.color;
        sliderBrightColor = new Color(sliderBaseColor.r * 1.3f, sliderBaseColor.g * 1.3f, sliderBaseColor.b * 1.3f);
    }

    public void updateScoreProgress(float score, float target)
    {
        //progressText.text = score + " / " + target;
        progressText.text = target - score + " HP";

        //float sliderValue = score / target;
        float sliderValue = (target - score) / target;

        //if (sliderValue > 1)
        //{
        //    sliderValue = 1;
        //}

        if (sliderValue < 0)
        {
            sliderValue = 0;
        }


        //DOTween.To(() => progressSlider.value, x => progressSlider.value = x, sliderValue, 0.3f);
        StartCoroutine(delayedSliderFlex(0.8f, sliderValue));
        //progressSlider.value = sliderValue;
    }

    IEnumerator delayedSliderFlex(float delay, float value)
    {
        sliderObj.GetComponent<RectTransform>().localScale = Vector3.one;

        yield return new WaitForSeconds(delay);

        sliderFill.color = sliderBaseColor;
        DOTween.To(() => progressSlider.value, x => progressSlider.value = x, value, 0.3f);

        Vector3 endScale = new Vector3(0, 0.1f, 0);
        sliderObj.GetComponent<RectTransform>().DOPunchScale(endScale, 0.3f, 0, 0);

        sliderFill.DOColor(sliderBrightColor, 0.15f);

        yield return new WaitForSeconds(0.15f);

        sliderFill.DOColor(sliderBaseColor, 0.15f);
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
        //confirmPatronsButton.interactable = false;
        int patronsToChoose = 0;

        if (isBossRound)
        {
            patronsToChoose = 5;
            patronChoiceLimit = 2;
            choosePatronText.text = "Choose " + patronChoiceLimit + " patrons";
            //confirmPatronsText = "Choose two patrons";
        }
        else
        {
            patronsToChoose = 3;
            patronChoiceLimit = 1;
            choosePatronText.text = "Choose " + patronChoiceLimit + " patron";
            //confirmPatronsText = "Choose one patron";
        }

        //confirmPatronsBttnText.text = confirmPatronsText;

        List<Patron> patronOptions = patronManager.selectPatrons(patronsToChoose);

        foreach (Patron p in patronOptions)
        {
            GameObject newPatronChoice = Instantiate(p.patronChoiceUIPrefab, patronChoicePanel.transform);
            newPatronChoice.GetComponent<patronChoiceUI>().initialize(p);
        }

        //foreach (PatronTopUI patronUI in patronSlotUIRefs)
        //{
        //    patronUI.removeButtonPanel.SetActive(true);
        //}

        checkPatronSelections();

        winPanel.GetComponent<RectTransform>().localScale = Vector3.zero;
        winPanel.SetActive(true);
        winPanel.GetComponent<RectTransform>().DOScale(Vector3.one, 0.3f);

        winRoundText.text = "Round " + gameManager.currentRound + " Complete";
    }

    public void displayRunComplete()
    {
        winStatsText.text =
            gameManager.bestScore + "\r\n" +
            gameManager.matchesMade + "\r\n" +
            gameManager.tilesCleared + "\r\n" +
            "<color=\"" + gameManager.mostMatchedTile() + "\">" + gameManager.mostMatchedTile();

        runCompletePanel.GetComponent<RectTransform>().localScale = Vector3.zero;
        runCompletePanel.SetActive(true);
        runCompletePanel.GetComponent<RectTransform>().DOScale(Vector3.one, 0.3f);
    }

    public void displayLoseScreen()
    {
        loseRoundText.text = "Defeated on\nGame " + gameManager.currentGame + " - Round " + gameManager.currentRound;
        lossStatsText.text = 
            gameManager.bestScore +"\r\n" +
            gameManager.matchesMade + "\r\n" +
            gameManager.tilesCleared + "\r\n" +
            "<color=\"" + gameManager.mostMatchedTile() + "\">" + gameManager.mostMatchedTile();

        losePanel.GetComponent<RectTransform>().localScale = Vector3.zero;
        losePanel.SetActive(true);
        losePanel.GetComponent<RectTransform>().DOScale(Vector3.one, 0.3f);
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
                selectedPatronUIRefs[0].patronToggleEffect();
            }
            else
            {
                selectedPatronUIRefs.Add(thisPatronChoice);
            }

            skillTree.openSkillTree(thisPatronChoice.patronRef, thisPatronChoice);
        }
        else
        {
            selectedPatronUIRefs.Remove(thisPatronChoice);
            if(selectedPatronUIRefs.Count < patronChoiceLimit)
            {
                //confirmPatronsButton.interactable = false;
                //confirmPatronsBttnText.text = confirmPatronsText;
            }
        }

        if(selectedPatronUIRefs.Count == patronChoiceLimit)
        {
            //confirmPatronsButton.interactable = true;
            //confirmPatronsBttnText.text = "confirm";
        }

        checkPatronSelections();
    }

    public void confirmPatronSelect()
    {
        FindObjectOfType<LevelProgress>().levelProgressStart();
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
            GameObject newPatronChoice = Instantiate(p.patronChoiceUIPrefab, patronChoicePanel.transform);
            newPatronChoice.GetComponent<patronChoiceUI>().initialize(p);
        }

        checkPatronSelections();
    }

    public void skipPatronSelect()
    {
        FindObjectOfType<LevelProgress>().levelProgressStart();
    }

    public void clearPatronOptions()
    {
        foreach (patronChoiceUI patronUI in selectedPatronUIRefs)
        {
            // determine which ability is selected
            patronManager.selectNewPatron(patronUI.patronRef, patronUI.currentSelectedAbility);
        }

        for (int i = 0; i < currentChoicePrefabs.Count; i++)
        {
            GameObject objToDestroy = currentChoicePrefabs[i];
            Destroy(objToDestroy);
        }
        currentChoicePrefabs.Clear();
        patronUIRefs.Clear();
        selectedPatronUIRefs.Clear();
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

    //public void skipCountUpdate(int count)
    //{
    //    skipCountText.text = "" + count;
    //}

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

    //public void toggleSkipInteract(bool canInteract)
    //{
    //    skipButton.interactable = canInteract;
    //}

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
        //if (gameManager.currentSkips > 0)
        //{
        //    skipPatronSelect();
        //    gameManager.useSkip();
        //}
        skipPatronSelect();
    }

    public void removePatronButtonPress()
    {
        GameObject targetPatron = patronManager.activePatrons[currentInfoPanelPatron.index].gameObject;
        Patron patronRefTemp = targetPatron.GetComponent<Patron>();

        //Debug.Log("removed");
        patronManager.removePatron(currentInfoPanelPatron.index);

        foreach(patronChoiceUI patron in patronUIRefs)
        {
            if(patron.patronRef == patronRefTemp)
            {
                foreach (Patron p in patronManager.potentialPatrons)
                {
                    if(p.title == patronRefTemp.title)
                    {
                        patronRefTemp = p;
                        break;
                    }
                }

                patron.initialize(patronRefTemp);
                break;
            }
        }

        checkPatronSelections();
        currentInfoPanelPatron = null;
        patronInfoPanelHide();
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

    private void checkPatronSelections()
    {

        List<patronChoiceUI> tempList = new List<patronChoiceUI>();

        foreach (patronChoiceUI patronUI in patronUIRefs)
        {
            tempList.Add(patronUI);
        }


        foreach (patronChoiceUI patronUI in tempList)
        {
            patronUI.toggleDisable(patronManager.canPatronBeChosen(patronUI.patronRef.index, patronUI.patronChoiceListIndex));
        }
    }

    public void patronInfoPanelShow(Patron patron)
    {
        currentInfoPanelPatron = patron;
        infoPanelTitle.text = patron.title + " - Lvl " + patron.level;
        infoPanelDesc.text = patron.currentDescription();
        patronInfoPanel.GetComponent<RectTransform>().localScale = Vector3.zero;
        patronInfoPanel.SetActive(true);
        patronInfoPanel.GetComponent<RectTransform>().DOScale(Vector3.one, 0.2f);

    }

    public void patronInfoPanelHide()
    {
        patronInfoPanel.SetActive(false);
    }

    public void masterVolumeUpdate()
    {
        audioManager.changeMasterVolume(masterVolumeSlider.value);
    }

    public void sfxToggleUpdate()
    {
        audioManager.muteSFX(!sfxToggle.isOn);
    }

    public void musicToggleUpdate()
    {
        audioManager.muteMusic(!musicToggle.isOn);
    }

    public void turnEffect(int turnNum)
    {
        float scaleValue = 0.3f;
        Vector3 punchScale = new Vector3(scaleValue, scaleValue, 0);
        turnTextObj.GetComponent<RectTransform>().DOPunchScale(punchScale, 0.3f, 0, 0);

        string text = "";

        if (turnNum >= 0)
        {
            text = "+" + turnNum;
        }
        else
        {
            text = "-" + (turnNum * -1);
        }

        //instantiate text effect
        GameObject popup = Instantiate(textPopupPrefab, turnObj.transform);
        popup.GetComponent<TMP_Text>().text = text;
        popup.SetActive(true);
        //assign string to effect
    }
}

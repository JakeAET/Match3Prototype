using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine;

public class LevelProgress : MonoBehaviour
{
    private GameManager gameManager;
    private UIManager ui;

    //[SerializeField] float arrowYIncrease;
    [SerializeField] float scaleIncrease;
    //private float arrowStartY;

    [SerializeField] GameObject levelProgressPanel;
    [SerializeField] Image background;
    [SerializeField] GameObject arrows;
    [SerializeField] GameObject[] levelObjs;
    [SerializeField] TMP_Text bossLevelText;
    [SerializeField] GameObject container;

    [SerializeField] Color[] levelColors;
    [SerializeField] Color[] gameColors;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        ui = FindObjectOfType<UIManager>();
        //arrowStartY = arrows.GetComponent<RectTransform>().localPosition.y;
        arrows.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void levelProgressStart()
    {
        StartCoroutine(levelProgressAnim());
    }

    private IEnumerator levelProgressAnim()
    {
        // turn on panel

        if (!(gameManager.currentGame == 1 && gameManager.currentRound == 0))
        {
            levelProgressPanel.transform.localScale = Vector3.zero;
            levelProgressPanel.GetComponent<RectTransform>().DOScale(Vector3.one, 0.3f);
        }
        
        levelProgressPanel.SetActive(true);

        // if level one: turn on arrows, assign color, assign boss type, activate level one
        if (gameManager.currentRound == 0 || gameManager.currentRound == 5)
        {
            RectTransform contRect = container.GetComponent<RectTransform>();
            contRect.transform.position = new Vector3(contRect.transform.position.x, -2000, contRect.transform.position.z);
            contRect.DOLocalMoveY(0, 0.5f);

            arrows.SetActive(false);
            arrows.GetComponent<RectTransform>().localScale = Vector3.one;
            arrows.GetComponent<RectTransform>().DOLocalMoveY(levelObjs[0].transform.localPosition.y, 0f);

            setLevelColors();
            background.color = gameColors[gameManager.currentGame - 1];
            bossLevelText.text = gameManager.currentBossRound.title;
            bossLevelText.color = gameManager.currentBossRound.titleTextColor;

            yield return new WaitForSeconds(1f);

            arrows.SetActive(true);
            levelObjs[0].GetComponent<Image>().color = levelColors[0];
            levelObjs[0].GetComponent<RectTransform>().DOScale(new Vector3(scaleIncrease, scaleIncrease, scaleIncrease), 0.3f);
            arrows.GetComponent<RectTransform>().DOScale(new Vector3(scaleIncrease, scaleIncrease, scaleIncrease), 0.3f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            Image img = levelObjs[gameManager.currentRound - 1].GetComponent<Image>();
            Color baseColor = new Color(levelColors[gameManager.currentRound - 1].r * 0.2f, levelColors[gameManager.currentRound - 1].g * 0.2f, levelColors[gameManager.currentRound - 1].b * 0.2f, 0.9f);
            img.color = baseColor;
            levelObjs[gameManager.currentRound - 1].GetComponent<RectTransform>().DOScale(Vector3.one, 0.2f);
            arrows.GetComponent<RectTransform>().DOScale(Vector3.one, 0.2f);

            yield return new WaitForSeconds(0.2f);

            arrows.GetComponent<RectTransform>().DOLocalMoveY(levelObjs[gameManager.currentRound].transform.localPosition.y, 0.3f);
            arrows.GetComponent<RectTransform>().DOScale(new Vector3(scaleIncrease, scaleIncrease, scaleIncrease), 0.3f);
            levelObjs[gameManager.currentRound].GetComponent<RectTransform>().DOScale(new Vector3(scaleIncrease, scaleIncrease, scaleIncrease), 0.3f);
            levelObjs[gameManager.currentRound].GetComponent<Image>().color = levelColors[gameManager.currentRound];
        }

        yield return new WaitForSeconds(3f);

        //else: shift arrows up, activate current level

        // turn off panel
        levelProgressPanel.SetActive(false);

        // trigger level start or boss level screen
        if (gameManager.currentRound == 4)
        {
            ui.displayBossInfoPanel(gameManager.currentBossRound);

            if (gameManager.currentBossRound.constantEffect)
            {
                gameManager.currentBossRound.activateConstraint();
            }
        }
        else
        {
            gameManager.startRound();
        }

        yield return null;
    }

    public void setLevelColors()
    {
        for (int i = 0; i < levelObjs.Length; i++)
        {
            Image img = levelObjs[i].GetComponent<Image>();
            Color baseColor = new Color(levelColors[i].r * 0.2f, levelColors[i].g * 0.2f, levelColors[i].b * 0.2f, 0.9f);
            img.color = baseColor;
        }
    }
}

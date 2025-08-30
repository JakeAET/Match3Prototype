using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeUI : MonoBehaviour
{
    [SerializeField] GameObject skillTreePanel;
    [SerializeField] Image patronImg;

    [SerializeField] GameObject skillTreeTierPrefab;
    public List<GameObject> currentSkillTreeTiers = new List<GameObject>();

    [SerializeField] GameObject confirmPatronsButton;
    [SerializeField] Button confirmSkillTreeButton;
    [SerializeField] Image confirmSTButtonImg;
    [SerializeField] TMP_Text confirmSTButtonTxt;
    [SerializeField] GameObject skillTreeContainer;
    [SerializeField] GameObject patronContainer;
    [SerializeField] GameObject infoPanel;
    [SerializeField] TMP_Text infoText;
    [SerializeField] float yInc;
    [SerializeField] GameObject lineRenderPrefab;

    public Patron currentPatronRef;
    public patronChoiceUI patronUI;
    public Ability currentSelectedAbility;

    public Ability currentHighlightedAbility;

    public GameObject confirmSkillButton;

    public GameObject activeLine;
    public List<GameObject> currentLines = new List<GameObject>();

    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject board;
    [SerializeField] GameObject skillTreeBG;
    [SerializeField] GameObject centeredPanel;


    // current skill tree tier reference

    // Start is called before the first frame update
    void Start()
    {
        skillTreePanel.SetActive(false);
        infoPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openSkillTree(Patron targetPatron, patronChoiceUI ui)
    {
        currentPatronRef = targetPatron;
        patronUI = ui;
        patronImg.sprite = targetPatron.sprite;
        confirmSkillTreeButton.interactable = false;

        Color col = confirmSTButtonImg.color;
        col.a = 0.3f;
        confirmSTButtonImg.color = col;

        col = confirmSTButtonTxt.color;
        col.a = 0.3f;
        confirmSTButtonTxt.color = col;

        winPanel.SetActive(false);
        centeredPanel.SetActive(false);
        skillTreeBG.SetActive(true);
        board.SetActive(false);
        confirmPatronsButton.SetActive(false);
        patronContainer.SetActive(false);
        skillTreePanel.SetActive(true);
        StartCoroutine(generateSkillTree());
    }

    public void closeSkillTree(bool confirmed)
    {
        if (confirmed)
        {
            patronUI.currentSelectedAbility = currentSelectedAbility;
            patronUI.updateDescription();
        }
        else
        {
            patronUI.patronToggle.isOn = false;
            patronUI.toggledPatron();
        }

        if(currentSelectedAbility == null)
        {
            patronUI.patronToggle.isOn = false;
            patronUI.toggledPatron();
        }

        currentSelectedAbility = null;
        currentPatronRef = null;
        patronUI = null;

        winPanel.SetActive(true);
        centeredPanel.SetActive(true);
        skillTreeBG.SetActive(false);
        board.SetActive(true);
        confirmPatronsButton.SetActive(true);
        skillTreePanel.SetActive(false);
        patronContainer.SetActive(true);
        infoPanel.SetActive(false);
        activeLine = null;
        resetSkillTree();
    }

    private IEnumerator generateSkillTree()
    {
        // set starting pos
        //float currentYInc = 0;
        int currentLevel = 1;

        if (currentPatronRef.allAbilityMatrix.Count == 0)
        {
            currentPatronRef.generateAbilityMatrix();
        }

        foreach (List<Ability> abilityList in currentPatronRef.allAbilityMatrix)
        {
            //instantiate tier prefab
            GameObject tier = Instantiate(skillTreeTierPrefab, skillTreeContainer.transform);
            tier.name = "Level " + currentLevel + " Tier";
            //Vector3 pos = tier.transform.position;
            //pos.y += yInc;
            //tier.transform.position = pos;

            //Debug.Log("Spawning level " + currentLevel + "for patron at level " + currentPatronRef.level);

            //initialize tier prefab
            SkillTreeTierUI tierUIRef = tier.GetComponent<SkillTreeTierUI>();
            tierUIRef.initialize(this, currentPatronRef, abilityList, currentLevel, currentLevel == currentPatronRef.level + 1);

            //add tier prefab to list
            currentSkillTreeTiers.Add(tier);

            // increment pos
            //currentYInc += yInc;

            while (!tierUIRef.initialized)
            {
                yield return null;
            }

            if(currentLevel > 1 && (currentPatronRef.level + 1) > 2 && currentLevel < (currentPatronRef.level + 1))
            {
                // determine start point
                Vector3 startPos = tierUIRef.chosenSkillObj.GetComponent<RectTransform>().transform.position;

                SkillTreeTierUI lowerTierUIRef = currentSkillTreeTiers[currentLevel - 2].GetComponent<SkillTreeTierUI>();

                // determine end point
                Vector3 endPos = lowerTierUIRef.chosenSkillObj.GetComponent<RectTransform>().transform.position;

                // create line renderer
                //CreateLine(lineRenderPrefab, startPos, endPos, Color.white);
                //Debug.Log("making line with " + tierUIRef.chosenSkillObj.name + " and " + lowerTierUIRef.chosenSkillObj.name);
                //Debug.Log(startPos + " - " + endPos);
            }

            currentLevel++;
        }
    }

    private void resetSkillTree()
    {
        //clear tree
        for (int i = 0; i < currentSkillTreeTiers.Count; i++)
        {
            GameObject target = currentSkillTreeTiers[i];
            Destroy(target);
        }

        for (int i = 0; i < currentLines.Count; i++)
        {
            GameObject target = currentLines[i];
            Destroy(target);
        }

        currentLines.Clear();
        currentSkillTreeTiers.Clear();
    }

    public void clickedOnChoice(Ability currentAbility)
    {
        //Debug.Log("clicked on " + currentAbility);
        currentHighlightedAbility = currentAbility;
        infoText.text = currentAbility.patronSelectDescription();
        infoPanel.SetActive(true);
    }

    public void clickedOffChoice(Ability currentAbility)
    {
        //if (currentHighlightedAbility == currentAbility && currentSelectedAbility == null)
        //{
        //    currentHighlightedAbility = null;
        //    infoPanel.SetActive(false);
        //}
    }
    
    public void toggleChanged(bool confirmAllowed, Ability currentAbility)
    {
        confirmSkillTreeButton.interactable = confirmAllowed;
        currentSelectedAbility = currentAbility;

        if (currentAbility == null)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            infoText.text = currentAbility.patronSelectDescription();
            infoPanel.SetActive(true);
        }

        if (confirmAllowed)
        {
            Color col = confirmSTButtonImg.color;
            col.a = 1;
            confirmSTButtonImg.color = col;

            col = confirmSTButtonTxt.color;
            col.a = 1f;
            confirmSTButtonTxt.color = col;
        }
        else
        {
            Color col = confirmSTButtonImg.color;
            col.a = 0.3f;
            confirmSTButtonImg.color = col;

            col = confirmSTButtonTxt.color;
            col.a = 0.3f;
            confirmSTButtonTxt.color = col;
        }
    }

    public void CreateLine(GameObject lineRender, Vector3 positionOne, Vector3 positionTwo, Color color)
    {
        GameObject lineInstance = Instantiate(lineRender, skillTreePanel.transform);

        Image m_image = lineInstance.GetComponent<Image>();
        RectTransform m_myTransform = lineInstance.GetComponent<RectTransform>();

        m_image.color = color;

        Vector2 point1 = new Vector2(positionTwo.x, positionTwo.y);
        Vector2 point2 = new Vector2(positionOne.x, positionOne.y);
        Vector2 midpoint = (point1 + point2) / 2f;

        //Debug.Log(point1 + " - " + point2);

        m_myTransform.position = midpoint;

        Vector2 dir = point1 - point2;
        m_myTransform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        m_myTransform.localScale = new Vector3(dir.magnitude, 1f, 1f);
    }
}

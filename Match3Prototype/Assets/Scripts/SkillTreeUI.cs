using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
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
    public SkillTreeChoice currentHighlightedRef;

    public GameObject confirmSkillButton;

    public GameObject activeLine;
    public List<GameObject> currentLines = new List<GameObject>();

    public GameObject activeForceField;
    public List<GameObject> currentForceFields = new List<GameObject>();

    [SerializeField] GameObject lineParticlePrefab;
    [SerializeField] GameObject forceFieldPrefab;

    public GameObject activeLineParticle;
    public List<GameObject> currentLineParticles = new List<GameObject>();

    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject board;
    [SerializeField] GameObject skillTreeBG;
    [SerializeField] GameObject centeredPanel;

    Vector2 treeStartPos;
    [SerializeField] float treeYGap;
    [SerializeField] float treeXGap;
    [SerializeField] float treeStaggerGap;
    [SerializeField] float treeYSpawnOffset;

    [SerializeField] touchCam tcam;

    [SerializeField] Color lrStartColor;
    [SerializeField] Color lrEndColor;


    // current skill tree tier reference

    // Start is called before the first frame update
    void Start()
    {
        skillTreePanel.SetActive(false);
        infoPanel.SetActive(false);

        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        if (cam != null)
        {
            treeStartPos = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
            treeStartPos.y += treeYSpawnOffset;
        }
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
        tcam.unlockCam();
        StartCoroutine(generateSkillTree());
    }

    public void closeSkillTree(bool confirmed)
    {
        winPanel.SetActive(true);
        tcam.lockCam();
        tcam.setBoundsCenter(tcam.defaultBoundsPos);

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

        if (currentSelectedAbility == null)
        {
            patronUI.patronToggle.isOn = false;
            patronUI.toggledPatron();
        }

        currentSelectedAbility = null;
        currentPatronRef = null;
        patronUI = null;

        centeredPanel.SetActive(true);
        skillTreeBG.SetActive(false);
        board.SetActive(true);
        confirmPatronsButton.SetActive(true);
        skillTreePanel.SetActive(false);
        patronContainer.SetActive(true);
        infoPanel.SetActive(false);
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

        float maxY = treeStartPos.y;
        float minY = treeStartPos.y;

        foreach (List<Ability> abilityList in currentPatronRef.allAbilityMatrix)
        {
            Vector2 currentPos = treeStartPos;
            float numToChoose = 0;

            if (currentLevel == currentPatronRef.level + 1)
            {
                numToChoose = 1;
            }
            else if(currentLevel < currentPatronRef.level + 1)
            {
                currentPos.y += (currentLevel - (currentPatronRef.level + 1)) * treeYGap;
            }
            else if (currentLevel > currentPatronRef.level + 1)
            {
                currentPos.y += (currentLevel - (currentPatronRef.level + 1)) * treeYGap;
            }


            if (currentPos.y < minY)
            {
                minY = currentPos.y;
            }
            else if (currentPos.y > maxY)
            {
                maxY = currentPos.y;
            }

            //instantiate tier prefab
            GameObject tier = Instantiate(skillTreeTierPrefab, currentPos, Quaternion.identity);
            tier.transform.SetParent(skillTreeContainer.transform, true);
            tier.name = "Level " + currentLevel + " Tier";
            //Vector3 pos = tier.transform.position;
            //pos.y += yInc;
            //tier.transform.position = pos;

            //Debug.Log("Spawning level " + currentLevel + "for patron at level " + currentPatronRef.level);

            //initialize tier prefab

            SkillTreeTierUI tierUIRef = tier.GetComponent<SkillTreeTierUI>();
            tierUIRef.initialize(this, currentPatronRef, abilityList, currentLevel, currentLevel == currentPatronRef.level + 1, treeXGap, treeStaggerGap, numToChoose);

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
                Vector3 startPos = tierUIRef.chosenSkillObj.transform.position;

                SkillTreeTierUI lowerTierUIRef = currentSkillTreeTiers[currentLevel - 2].GetComponent<SkillTreeTierUI>();

                // determine end point
                Vector3 endPos = lowerTierUIRef.chosenSkillObj.transform.position;

                // create line renderer
                CreateLine(lineRenderPrefab, startPos, endPos, false);
                //Debug.Log("making line with " + tierUIRef.chosenSkillObj.name + " and " + lowerTierUIRef.chosenSkillObj.name);
                //Debug.Log(startPos + " - " + endPos);
            }

            //currentPos.y += treeYGap;

            currentLevel++;
        }

        Vector3 tcamPos = new Vector3(treeStartPos.x, (Mathf.Abs(maxY) - Mathf.Abs(minY)) / 2);

        tcam.setBoundsCenter(tcamPos);
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

        for (int i = 0; i < currentLineParticles.Count; i++)
        {
            GameObject target = currentLineParticles[i];
            Destroy(target);
        }

        for (int i = 0; i < currentForceFields.Count; i++)
        {
            GameObject target = currentForceFields[i];
            Destroy(target);
        }


        if (activeLine != null)
        {
            Destroy(activeLine);
            activeLine = null;
        }

        if (activeLineParticle != null)
        {
            Destroy(activeLineParticle);
            activeLineParticle = null;
        }

        currentLines.Clear();
        currentForceFields.Clear();
        currentLineParticles.Clear();
        currentSkillTreeTiers.Clear();
    }

    public void clickedOnChoice(Ability currentAbility, SkillTreeChoice choice)
    {
        if(currentHighlightedRef == choice)
        {
            clickedOffChoice(currentAbility, choice);
        }
        else
        {
            currentHighlightedAbility = currentAbility;
            currentHighlightedRef = choice;
            infoText.text = currentAbility.patronSelectDescription();
            infoPanel.SetActive(true);
        }

        //StartCoroutine(clickedOnChoiceEnum(currentAbility, obj));
        //currentHighlightedAbility = currentAbility;
        //currentHighlightedRef = choice;
        //infoText.text = currentAbility.patronSelectDescription();
        //infoPanel.SetActive(true);
    }

    public void clickedOffChoice(Ability currentAbility, SkillTreeChoice choice)
    {
        currentHighlightedAbility = null;
        currentHighlightedRef = null;
        infoPanel.SetActive(false);
    }

    IEnumerator clickedOnChoiceEnum(Ability currentAbility, SkillTreeChoice choice)
    {
        yield return new WaitForSeconds(0.1f);

        currentHighlightedAbility = currentAbility;
        currentHighlightedRef = choice;
        infoText.text = currentAbility.patronSelectDescription();
        infoPanel.SetActive(true);
    }


    public void toggleChanged(bool confirmAllowed, Ability currentAbility, SkillTreeTierUI tierRef, SkillTreeChoice choice)
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

        if (currentAbility == null)
        {
            if(activeLine != null)
            {
                Destroy(activeLine);
                activeLine = null;
            }

            if (activeLineParticle != null)
            {
                Destroy(activeLineParticle);
                activeLineParticle = null;
            }
        }
        else
        {
            if(activeLine != null)
            {
                Destroy(activeLine);
                activeLine = null;
            }

            if (activeLineParticle != null)
            {
                Destroy(activeLineParticle);
                activeLineParticle = null;
            }

            if (tierRef.thisLevel - 2 >= 0)
            {

                SkillTreeTierUI lowerTierUIRef = currentSkillTreeTiers[tierRef.thisLevel - 2].GetComponent<SkillTreeTierUI>();

                if (lowerTierUIRef.chosenSkillObj != null)
                {
                    // determine start point
                    Vector3 startPos = choice.transform.position;

                    // determine end point
                    Vector3 endPos = lowerTierUIRef.chosenSkillObj.transform.position;

                    // create line renderer
                    CreateLine(lineRenderPrefab, startPos, endPos, true);
                    //Debug.Log("making line with " + tierUIRef.chosenSkillObj.name + " and " + lowerTierUIRef.chosenSkillObj.name);
                    //Debug.Log(startPos + " - " + endPos);
                }
            }
        }
    }

    public void CreateLine(GameObject lineRender, Vector3 positionOne, Vector3 positionTwo, bool tempLine)
    {
        GameObject lineInstance = Instantiate(lineRender, skillTreeContainer.transform);

        LineRenderer lr = lineInstance.GetComponent<LineRenderer>();
        Transform t = lineInstance.transform;

        lr.startColor = lrStartColor;
        lr.endColor = lrEndColor;

        lr.SetPosition(0, positionOne);
        lr.SetPosition(1, positionTwo);

        lineInstance.SetActive(true);

        GameObject lrParticle = Instantiate(lineParticlePrefab, skillTreeContainer.transform);
        //Vector2 point1 = new Vector2(positionTwo.x, positionTwo.y);
        //Vector2 point2 = new Vector2(positionOne.x, positionOne.y);
        //Vector2 dir = point1 - point2;
        lrParticle.transform.position = positionTwo;
        lrParticle.transform.LookAt(positionOne);
        //lrParticle.transform.rotation = Quaternion.Euler(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, lrParticle.transform.rotation.y, lrParticle.transform.rotation.z);

        //GameObject ffInstance = Instantiate(forceFieldPrefab, skillTreeContainer.transform);
        //ffInstance.transform.position = positionOne;
        //currentForceFields.Add(ffInstance);

        if (tempLine)
        {
            activeLine = lineInstance;
            activeLineParticle = lrParticle;
        }
        else
        {
            currentLines.Add(lineInstance);
            currentLineParticles.Add(lrParticle);
        }

        //Vector2 point1 = new Vector2(positionTwo.x, positionTwo.y);
        //Vector2 point2 = new Vector2(positionOne.x, positionOne.y);
        //Vector2 midpoint = (point1 + point2) / 2f;

        ////Debug.Log(point1 + " - " + point2);

        //t.position = midpoint;

        //Vector2 dir = point1 - point2;
        //t.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        //t.localScale = new Vector3(dir.magnitude, 1f, 1f);
    }
}

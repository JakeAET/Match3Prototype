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

    public Patron currentPatronRef;
    public patronChoiceUI patronUI;
    public Ability currentSelectedAbility;

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

        confirmPatronsButton.SetActive(false);
        patronContainer.SetActive(false);
        skillTreePanel.SetActive(true);
        generateSkillTree();
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
         
        confirmPatronsButton.SetActive(true);
        skillTreePanel.SetActive(false);
        patronContainer.SetActive(true);
        infoPanel.SetActive(false);
        resetSkillTree();
    }

    private void generateSkillTree()
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

        currentSkillTreeTiers.Clear();
    }
    
    public void toggleChanged(bool confirmAllowed, Ability currentAbility)
    {
        confirmSkillTreeButton.interactable = confirmAllowed;
        currentSelectedAbility = currentAbility;

        if(currentAbility == null)
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
}

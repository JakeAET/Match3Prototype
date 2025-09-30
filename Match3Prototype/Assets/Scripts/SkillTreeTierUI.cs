using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.UI;

public class SkillTreeTierUI : MonoBehaviour
{
    public SkillTreeUI skillTreeRef;
    [SerializeField] GameObject choicePrefab;
    [SerializeField] GameObject fillerPrefab;
    private List<SkillTreeChoice> skillTreeChoices = new List<SkillTreeChoice>();
    //private ToggleGroup tg;
    public Ability currentAbilityChoice;
    public GameObject chosenSkillObj;
    public bool initialized = false;
    public float numCanBeChosen = 0;
    public int thisLevel;
    public bool active;

    public List<SkillTreeChoice> selectedChoices = new List<SkillTreeChoice>();

    public void initialize(SkillTreeUI treeRef, Patron patronRef, List<Ability> abilities, int level, bool activeLevel, float xGap, float staggerGap, float canBeChosen)
    {
        skillTreeRef = treeRef;
        thisLevel = level;
        active = activeLevel;
        //tg = GetComponent<ToggleGroup>();

        Vector2 startPos = transform.position;
        float gap = xGap;

        if (level % 2 == 0)
        {
            startPos.x += staggerGap;
        }

        int alternator = 0;

        for (int i = 0; i < abilities.Count; i++)
        {
            Vector2 currentPos = startPos;
            bool isChosen = false;

            if (!activeLevel)
            {
                //Debug.Log(level - 1 + " - " + patronRef.abilitiesByLevel.Count);
                //if ((level - 1) <= patronRef.abilitiesByLevel.Count)
                //{
                //    Debug.Log("Ability checking " + abilities[i] + "v.s. patron ability" + patronRef.abilitiesByLevel[level - 1]);
                //    if (level <= patronRef.level && abilities[i] == patronRef.abilitiesByLevel[level - 1])
                //    {
                //        isChosen = true;
                //    }
                //}

                if(patronRef.abilitiesByLevel.Count > 0 && (level - 1) < patronRef.abilitiesByLevel.Count)
                {
                    //Debug.Log("Ability at this level = " + patronRef.abilitiesByLevel[level - 1].name + ", checked with " + abilities[i].name);
                }

                if (level - 1 < patronRef.level && abilities[i].title == patronRef.abilitiesByLevel[level - 1].title)
                {
                    isChosen = true;
                }
            }

            if(i != 0)
            {
                if (alternator < 2)
                {
                    gap *= -1f;
                    alternator++;
                }
                else
                {
                    currentPos = startPos;
                    gap *= 2f;
                    alternator = 0;
                }

                currentPos.x += gap;
            }

            GameObject choice = Instantiate(choicePrefab, currentPos, Quaternion.identity);
            choice.transform.SetParent(transform, true);
            SkillTreeChoice choiceRef = choice.GetComponent<SkillTreeChoice>();
            choiceRef.initialize(this, abilities[i], activeLevel, isChosen);
            skillTreeChoices.Add(choiceRef);

            if (isChosen)
            {
                chosenSkillObj = choice;
            }
        }

        //if (level % 2 != 0)
        //{
        //    Instantiate(fillerPrefab, transform);
        //}

        if (activeLevel)
        {
            currentAbilityChoice = currentChoice();
        }

        //Debug.Log(level + " - chosen object: " + chosenSkillObj);

        //Canvas.ForceUpdateCanvases();
        initialized = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Ability currentChoice()
    {
        Ability selectedChoice = null;

        foreach (SkillTreeChoice choice in skillTreeChoices)
        {
            if (choice.toggledOn)
            {
                selectedChoice = choice.ability;
            }
        }

        return selectedChoice;
    }

    public void toggledSkill(SkillTreeChoice targetChoice, bool isOn)
    {
        if (isOn)
        {
            foreach (SkillTreeChoice choice in skillTreeChoices)
            {
                if(choice != targetChoice)
                {
                    choice.toggledOn = false;
                    choice.unselect();
                    choice.toggleOff();
                }
            }

            currentAbilityChoice = targetChoice.ability;
            skillTreeRef.toggleChanged(true, targetChoice.ability, this, targetChoice);
        }
        else
        {
            currentAbilityChoice = null;
            skillTreeRef.toggleChanged(false, null, this, null);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeTierUI : MonoBehaviour
{
    private SkillTreeUI skillTreeRef;
    [SerializeField] GameObject choicePrefab;
    private List<SkillTreeChoice> skillTreeChoices = new List<SkillTreeChoice>();
    private ToggleGroup tg;
    public Ability currentAbilityChoice;

    public void initialize(SkillTreeUI treeRef, Patron patronRef, List<Ability> abilities, int level, bool activeLevel)
    {
        skillTreeRef = treeRef;
        tg = GetComponent<ToggleGroup>();

        for (int i = 0; i < abilities.Count; i++)
        {
            bool isChosen = false;

            if (!activeLevel)
            {
                //Debug.Log("Ability checking " + abilities[i] + "v.s. patron ability" + patronRef.abilitiesByLevel[level - 1]);
                if(level <= patronRef.level && abilities[i] == patronRef.abilitiesByLevel[level - 1])
                {
                    isChosen = true;
                }
            }

            GameObject choice = Instantiate(choicePrefab, transform);
            SkillTreeChoice choiceRef = choice.GetComponent<SkillTreeChoice>();
            choiceRef.initialize(this, abilities[i], activeLevel, isChosen, tg);
            skillTreeChoices.Add(choiceRef);
        }

        if (activeLevel)
        {
            currentAbilityChoice = currentChoice();
        }
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
            if (choice.toggle.isOn)
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
                    choice.toggle.isOn = false;
                }
            }

            currentAbilityChoice = targetChoice.ability;
            skillTreeRef.toggleChanged(true, targetChoice.ability);
        }
        else
        {
            currentAbilityChoice = null;
            skillTreeRef.toggleChanged(false, null);
        }
    }
}

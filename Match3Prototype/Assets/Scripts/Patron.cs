using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Patron : MonoBehaviour
{
    public string title;
    //public Ability[] abilitiesByLevel;

    public List<Ability> levelOneAbilities;
    public List<Ability> levelTwoAbilities;
    public List<Ability> levelThreeAbilities;
    public List<Ability> levelFourAbilities;
    public List<Ability> levelFiveAbilities;

    public List<List<Ability>> allAbilityMatrix = new List<List<Ability>>();
 
    public List<Ability> activeAbilites;
    public List<Ability> abilitiesByLevel;

    //public bool conditionalEffect;
    //public bool constantEffect;
    public int level = 1;
    public int maxLevel = 5;
    public int index = 0;
    public Sprite sprite;
    public Color color;
    public GameObject patronChoiceUIPrefab;

    public bool banished = false;

    //public bool matrixGenerated = false;

    public void generateAbilityMatrix()
    {
        if (levelOneAbilities.Count > 0)
        {
            allAbilityMatrix.Add(levelOneAbilities);

            if (levelTwoAbilities.Count > 0)
            {
                allAbilityMatrix.Add(levelTwoAbilities);

                if (levelThreeAbilities.Count > 0)
                {
                    allAbilityMatrix.Add(levelThreeAbilities);

                    if (levelFourAbilities.Count > 0)
                    {
                        allAbilityMatrix.Add(levelFourAbilities);

                        if (levelFiveAbilities.Count > 0)
                        {
                            allAbilityMatrix.Add(levelFiveAbilities);
                        }
                    }
                }
            }
        }

        //matrixGenerated = true;
    }

    public virtual void initialize()
    {
        determineMaxLevel();
        //generateAbilityMatrix();
    }

    public virtual bool conditionMet()
    {
        return false;
    }

    public virtual void triggerEffect()
    {

    }

    public virtual void levelUp(Ability ability)
    {
        if(level < maxLevel)
        {
            level++;
            FindObjectOfType<PatronManager>().updatePatronLvl(index, level);

            Ability targetAbility = existingAbility(ability);

            if (targetAbility != null) // already exists
            {
                targetAbility.levelUp();
                abilitiesByLevel.Add(targetAbility);

            }
            else // new ability
            {
                targetAbility = Instantiate(ability);
                targetAbility.transform.SetParent(gameObject.transform);
                activeAbilites.Add(targetAbility);
                abilitiesByLevel.Add(targetAbility);
                targetAbility.patron = this;
                targetAbility.initialize();
                targetAbility.levelUp();
            }

            //Debug.Log("Leveling up with " + targetAbility.name + " ability");
        }

        //Debug.Log("Level Up");
    }

    public virtual void reduceLevel(int levelNum)
    {
        for (int i = 0; i < levelNum; i++)
        {
            //Ability ability = abilitiesByLevel[level - 1];
            existingAbility(abilitiesByLevel[abilitiesByLevel.Count - (i + 1)]).undoAbility(1);

            level--;
            FindObjectOfType<PatronManager>().updatePatronLvl(index, level);
        }
    }

    public virtual void restoreLevel(int levelNum)
    {
        for (int i = 0; i < levelNum; i++)
        {
            levelUp(abilitiesByLevel[level]);
        }
    }

    public virtual string currentDescription()
    {
        string desc = "";

        foreach (Ability ability in activeAbilites)
        {
            desc += ability.description();
            desc += "\n";
        }

        return desc;
    }

    public Ability existingAbility(Ability ability)
    {
        Ability abilityRef = null;

        foreach (Ability a in activeAbilites)
        {
            if(a.title == ability.title)
            {
                abilityRef = a;
            }
        }

        // returns null if doesn't exist already
        return abilityRef;
    }

    public virtual void determineMaxLevel()
    {
        if (allAbilityMatrix.Count == 0)
        {
            generateAbilityMatrix();
        }

        maxLevel = allAbilityMatrix.Count;
    }
}

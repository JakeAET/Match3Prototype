using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Patron : MonoBehaviour
{
    public string title;
    public Ability[] abilitiesByLevel;
    public List<Ability> activeAbilites;
    //public bool conditionalEffect;
    //public bool constantEffect;
    public int level = 1;
    public int maxLevel = 5;
    public int index = 0;
    public Sprite sprite;
    public Color color;
    public GameObject patronChoiceUIPrefab;

    public virtual void initialize()
    {
        determineMaxLevel();
    }

    public virtual bool conditionMet()
    {
        return false;
    }

    public virtual void triggerEffect()
    {

    }

    public virtual void levelUp()
    {
        if(level < maxLevel)
        {
            level++;
            FindObjectOfType<PatronManager>().updatePatronLvl(index, level);

            Ability ability = existingAbility(abilitiesByLevel[level - 1]);

            if (ability != null) // already exists
            {
                ability.levelUp();
            }
            else // new ability
            {
                ability = Instantiate(abilitiesByLevel[level - 1]);
                ability.transform.SetParent(gameObject.transform);
                activeAbilites.Add(ability);
                ability.patron = this;
                ability.initialize();
                ability.levelUp();
            }
        }
    }

    public virtual void reduceLevel(int levelNum)
    {
        for (int i = 0; i < levelNum; i++)
        {
            Ability ability = abilitiesByLevel[level - 1];
            existingAbility(ability).undoAbility(1);

            level--;
            FindObjectOfType<PatronManager>().updatePatronLvl(index, level);
        }
    }

    public virtual void restoreLevel(int levelNum)
    {
        for (int i = 0; i < levelNum; i++)
        {
            levelUp();
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
        maxLevel = abilitiesByLevel.Length;
    }
}

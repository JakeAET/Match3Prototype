using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public string title;
    public bool conditionalEffect;
    public bool constantEffect;
    public bool canTrigger = true;
    public int level = 0;
    public int maxLevel = 0;
    public Patron patron;

    public virtual void initialize()
    {

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
        
    }

    public virtual void undoAbility(int levelNum)
    {

    }

    public virtual void restoreAbility(int levelNum)
    {

    }

    public virtual string description()
    {
        return "";
    }

    public virtual string patronSelectDescription()
    {
        return "";
    }

    public virtual void determineMaxLevel()
    {
        if (maxLevel == 0) // determine max level if not preset
        {
            foreach (Ability ability in patron.abilitiesByLevel)
            {
                if (ability.title == title)
                {
                    maxLevel++;
                }
            }
        }
    }
}

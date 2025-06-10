using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGainUndos : Ability
{
    public bool effectTriggered = false;
    public int undoIncrease;
    [SerializeField] GameManager gm;

    public override void initialize()
    {
        gm = FindObjectOfType<GameManager>();
        determineMaxLevel();
    }

    public override void triggerEffect()
    {
        if (canTrigger)
        {
            gm.increaseMaxUndos(undoIncrease);
            canTrigger = false;
        }
    }

    public override void levelUp()
    {
        if (level < maxLevel)
        {
            level++;
            canTrigger = true;
            triggerEffect();
        }
    }

    public override void undoAbility(int levelNum)
    {
        for (int i = 0; i < levelNum; i++)
        {
            gm.increaseMaxUndos(undoIncrease * -1);
            level--;
        }
    }

    public override void restoreAbility(int levelNum)
    {
        for (int i = 0; i < levelNum; i++)
        {
            levelUp();
        }
    }

    public override string description()
    {
        string desc = "- Grants " + "<color=\"green\">" + (level * undoIncrease) + "</color>" + " turn undo per round";

        return desc;
    }

    public override string patronSelectDescription()
    {
        string desc = "+ Additional " + "<color=\"green\">" + (undoIncrease) + "</color>" + " turn undo per round";

        return desc;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGainTurns : Ability
{
    public bool effectTriggered = false;
    public int turnIncrease;
    public int initialTurnIncrease;
    private int currentTurnIncrease;
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
            //gm.increaseMaxTurns(turnIncrease);
            canTrigger = false;

            if (level == 1)
            {
                gm.increaseMaxTurns(initialTurnIncrease);
                currentTurnIncrease += initialTurnIncrease;
            }
            else
            {
                gm.increaseMaxTurns(turnIncrease);
                currentTurnIncrease += turnIncrease;
            }
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
            //gm.increaseMaxTurns(turnIncrease * -1);
            level--;

            if (level == 1)
            {
                gm.increaseMaxTurns(initialTurnIncrease * -1);
                currentTurnIncrease -= initialTurnIncrease;
            }
            else
            {
                gm.increaseMaxTurns(turnIncrease * -1);
                currentTurnIncrease -= turnIncrease;
            }
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
        string desc = "- Grants " + "<color=\"green\">" + currentTurnIncrease + "</color>" + " additional turns per round";

        return desc;
    }

    public override string patronSelectDescription()
    {
        int increase = 0;

        if (level == 0)
        {
            increase = initialTurnIncrease;
        }
        else
        {
            increase = turnIncrease;
        }

        string desc = "+ Additional " + "<color=\"green\">" + (increase) + "</color>" + " turns per round";

        return desc;
    }
}

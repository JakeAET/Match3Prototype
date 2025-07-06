using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityLastTurn : Ability
{
    public int initialPointMulti;
    public int multiIncrease;
    private int currentPointMulti;
    private GameManager gm;

    public override void initialize()
    {
        gm = FindObjectOfType<GameManager>();
        determineMaxLevel();

        BoardManager.OnLastTurn += increaseAmount;
    }

    private void OnDisable()
    {
        BoardManager.OnLastTurn -= increaseAmount;
    }

    private float increaseAmount()
    {
        procEffect();
        return currentPointMulti;
    }

    public override void levelUp()
    {
        if (level < maxLevel)
        {
            level++;

            if (level == 1)
            {
                currentPointMulti += initialPointMulti;
            }
            else
            {
                currentPointMulti += multiIncrease;
            }
        }
    }

    public override void undoAbility(int levelNum)
    {
        for (int i = 0; i < levelNum; i++)
        {
            level--;

            if (level == 1)
            {
                currentPointMulti -= initialPointMulti;
            }
            else
            {
                currentPointMulti -= multiIncrease;
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
        string desc = "- Gain " + "<color=\"green\">" + currentPointMulti + "X</color> base score on final turn";

        return desc;
    }

    public override string patronSelectDescription()
    {
        int increase = 0;

        if (level == 0)
        {
            increase = initialPointMulti;
        }
        else
        {
            increase = multiIncrease;
        }

        string desc = "+ Gain <color=\"green\">" + increase + "X</color> base score on final turn";

        return desc;
    }
}

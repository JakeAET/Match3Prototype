using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFirstTurn : Ability
{
    public int initialPointIncrease;
    public int pointIncrease;
    private int currentPointIncrease;
    private GameManager gm;

    public override void initialize()
    {
        gm = FindObjectOfType<GameManager>();
        determineMaxLevel();

        BoardManager.OnFirstTurn += increaseAmount;
    }

    private void OnDisable()
    {
        BoardManager.OnFirstTurn -= increaseAmount;
    }

    private float increaseAmount()
    {
        procEffect();
        return currentPointIncrease;
    }

    public override void levelUp()
    {
        if (level < maxLevel)
        {
            level++;

            if (level == 1)
            {
                currentPointIncrease += initialPointIncrease;
            }
            else
            {
                currentPointIncrease += pointIncrease;
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
                currentPointIncrease -= initialPointIncrease;
            }
            else
            {
                currentPointIncrease -= pointIncrease;
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
        string desc = "- Gain " + "<color=\"green\">+" + currentPointIncrease + "</color> base score on first turn";

        return desc;
    }

    public override string patronSelectDescription()
    {
        int increase = 0;

        if (level == 0)
        {
            increase = initialPointIncrease;
        }
        else
        {
            increase = pointIncrease;
        }

        string desc = "+ Gain <color=\"green\">+" + increase + "</color> base score on first turn";

        return desc;
    }
}

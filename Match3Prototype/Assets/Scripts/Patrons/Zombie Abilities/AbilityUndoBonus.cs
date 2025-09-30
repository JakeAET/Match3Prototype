using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUndoBonus : Ability
{
    public int initialPointIncrease;
    public int pointIncrease;
    private int currentPointIncrease;
    private GameManager gm;

    public override void initialize()
    {
        gm = FindObjectOfType<GameManager>();
        determineMaxLevel();

        BoardManager.OnUndoBonusTrigger += increaseAmount;
    }

    private void OnDisable()
    {
        BoardManager.OnUndoBonusTrigger -= increaseAmount;

    }

    private float increaseAmount()
    {
        gm = FindObjectOfType<GameManager>();
        float amount = gm.currentUndos * currentPointIncrease;

        if (amount > 0)
        {
            procEffect();
        }

        return amount;
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
        string desc = "- Increase the base point score of all tiles by " + "<color=\"green\">+" + currentPointIncrease + "</color> for each remaining <color=\"green\">undo</color>";

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

        string desc = "- Increase the base point score of all tiles by " + "<color=\"green\">+" + increase + "</color> for each remaining <color=\"green\">undo</color>";

        //string desc = "+ Gain <color=\"green\">+" + increase + "</color> base point score on least matched color";

        return desc;
    }
}

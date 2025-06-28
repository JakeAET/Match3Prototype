using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityLeastScoredBonus : Ability
{
    public int initialPointIncrease;
    public int pointIncrease;
    private int currentPointIncrease;
    private GameManager gm;

    public override void initialize()
    {
        gm = FindObjectOfType<GameManager>();
        determineMaxLevel();

        BoardManager.OnScoutTrigger += increaseAmount;
    }

    private void OnDisable()
    {
        BoardManager.OnScoutTrigger -= increaseAmount;
    }

    private float increaseAmount(string colorName)
    {
        //Debug.Log("color name: " + colorName);

        //Debug.Log("scout activated");
        gm = FindObjectOfType<GameManager>();
        float amount = 0;

        string mostMatched = gm.mostMatchedTile();

        if (mostMatched != "")
        {

            int leastMatchedNum = gm.colorTilesCleared[gm.mostMatchedTile()]; // set to max first
            string leastMatchedColor = "";
            foreach (KeyValuePair<string, int> kvp in gm.colorTilesCleared)
            {
                if (kvp.Value <= leastMatchedNum)
                {
                    leastMatchedNum = kvp.Value;
                    leastMatchedColor = kvp.Key;
                }
            }

            //Debug.Log("least matched color: " + leastMatchedColor + " - current color: " + colorName + " - this color matched: " + gm.colorTilesCleared[colorName] + "  vs " + leastMatchedNum);

            if (gm.colorTilesCleared[colorName] <= leastMatchedNum)
            {
                amount += currentPointIncrease;
                //Debug.Log("scout found least matched color of " + colorName);
            }
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
        string desc = "- Increase the base point score of the least matched color tile by " + "<color=\"green\">+" + currentPointIncrease + "</color>";

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

        string desc = "+ Gain <color=\"green\">+" + increase + "</color> base point score on least matched color";

        return desc;
    }
}

using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCritChance : Ability
{
    public float initialCritChance;
    public float chanceIncrease;
    public float critMulti;
    private float currentCritChance;
    private GameManager gm;
    public Color popupColor;
    public float popupFontSize;

    public override void initialize()
    {
        gm = FindObjectOfType<GameManager>();
        determineMaxLevel();

        BoardManager.OnRogueCritTrigger += increaseAmount;
    }

    private void OnDisable()
    {
        BoardManager.OnRogueCritTrigger -= increaseAmount;
    }

    private float increaseAmount()
    {
        //Debug.Log("color name: " + colorName);

        //Debug.Log("scout activated");
        gm = FindObjectOfType<GameManager>();
        float amount = 0;

        float rand = UnityEngine.Random.Range(0, 1f);

        if (rand <= currentCritChance)
        {
            amount += critMulti;
            Debug.Log("Rogue Crit activated: Rand = " + rand);
        }

        if (amount > 0)
        {
            procEffect();
            Vector2 pos = new Vector2(BoardManager.GetInstance().boardCenter.x + UnityEngine.Random.Range(-1f, 1f), BoardManager.GetInstance().boardCenter.y + UnityEngine.Random.Range(-1f, 1f));
            BoardManager.GetInstance().spawnPopup(pos, "critical", popupFontSize, popupColor);
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
                currentCritChance += initialCritChance;
            }
            else
            {
                currentCritChance += chanceIncrease;
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
                currentCritChance -= initialCritChance;
            }
            else
            {
                currentCritChance -= chanceIncrease;
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

        string desc = "- Increase the chance of a tile <color=\"green\">Crit (" + critMulti + "X Score)</color> by " + "<color=\"green\">+" + currentCritChance * 100 + "%</color>";

        return desc;
    }

    public override string patronSelectDescription()
    {
        float increase = 0;

        if (level == 0)
        {
            increase = initialCritChance * 100;
        }
        else
        {
            increase = chanceIncrease * 100;
        }

        string desc = "+ Increase chance of a tile <color=\"green\">Crit (" + critMulti + "X Score)</color> by <color=\"green\">+" + increase + "%</color>";

        return desc;
    }
}

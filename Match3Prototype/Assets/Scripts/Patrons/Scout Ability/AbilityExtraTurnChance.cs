using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityExtraTurnChance : Ability
{
    public float initialChance;
    public float chanceIncrease;
    public int extraTurnAmount;
    private float currentChance;
    private GameManager gm;
    public Color popupColor;
    public float popupFontSize;
    //public string popupText;

    public override void initialize()
    {
        gm = FindObjectOfType<GameManager>();
        determineMaxLevel();

        GameManager.OnExtraTurnChance += extraTurns;
    }

    private void OnDisable()
    {
        GameManager.OnExtraTurnChance -= extraTurns;
    }

    private int extraTurns()
    {
        //Debug.Log("color name: " + colorName);

        //Debug.Log("scout activated");
        gm = FindObjectOfType<GameManager>();
        int amount = 0;

        float rand = UnityEngine.Random.Range(0, 1f);

        if (rand <= currentChance)
        {
            amount += extraTurnAmount;
            //Debug.Log("Extra Turn activated: Rand = " + rand);
        }

        if (amount > 0)
        {
            procEffect();
            Vector2 pos = new Vector2(BoardManager.GetInstance().boardCenter.x + UnityEngine.Random.Range(-1f, 1f), BoardManager.GetInstance().boardCenter.y + UnityEngine.Random.Range(-1f, 1f));
            BoardManager.GetInstance().spawnPopup(pos, "+ " + amount + " bonus turn", popupFontSize, popupColor);
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
                currentChance += initialChance;
            }
            else
            {
                currentChance += chanceIncrease;
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
                currentChance -= initialChance;
            }
            else
            {
                currentChance -= chanceIncrease;
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

        string desc = "- Increased chance of an extra <color=\"green\">" + extraTurnAmount + " turns</color> by " + "<color=\"green\">+" + currentChance * 100 + "%</color> each turn";

        return desc;
    }

    public override string patronSelectDescription()
    {
        float increase = 0;

        if (level == 0)
        {
            increase = initialChance * 100;
        }
        else
        {
            increase = chanceIncrease * 100;
        }

        string desc = "- Increased chance of an extra <color=\"green\">" + extraTurnAmount + " turns</color> by " + "<color=\"green\">+" + increase + "%</color> each turn";

        return desc;
    }
}

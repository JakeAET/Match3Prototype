using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityElementalBonus : Ability
{
    public ElementType targetElement;
    public int initialPointIncrease;
    public int pointIncrease;
    private int currentPointIncrease;
    private GameManager gm;

    public override void initialize()
    {
        gm = FindObjectOfType<GameManager>();
        determineMaxLevel();

        if (targetElement == ElementType.Enchanted)
        {
            BoardManager.OnEnchantedTrigger += increaseAmount;
        }

        if (targetElement == ElementType.Frozen)
        {
            BoardManager.OnFrozenTrigger += increaseAmount;
        }
    }

    private void OnDisable()
    {
        if (targetElement == ElementType.Enchanted)
        {
            BoardManager.OnEnchantedTrigger -= increaseAmount;
        }

        if (targetElement == ElementType.Frozen)
        {
            BoardManager.OnFrozenTrigger -= increaseAmount;
        }
    }

    private float increaseAmount(bool isElement)
    {
        gm = FindObjectOfType<GameManager>();
        float amount = 0;

        if (isElement)
        {
            amount += currentPointIncrease;
        }

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
        string desc = "";

        if (targetElement == ElementType.Enchanted)
        {
            desc = "- Increase the base point score of the \"<color=\\\"green\\\">Enchanted</color> tile by " + "<color=\"green\">+" + currentPointIncrease + "</color>";
        }

        if (targetElement == ElementType.Frozen)
        {
            desc = "- Increase the base point score of the \"<color=\\\"green\\\">Frozen</color> tile by " + "<color=\"green\">+" + currentPointIncrease + "</color>";
        }

        //string desc = "- Increase the base point score of the least matched color tile by " + "<color=\"green\">+" + currentPointIncrease + "</color>";

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

        string desc = "";

        if (targetElement == ElementType.Enchanted)
        {
            desc = "+ Gain <color=\"green\">+" + increase + "</color> base point score on <color=\"green\">Enchanted</color> tiles";
        }

        if (targetElement == ElementType.Frozen)
        {
            desc = "+ Gain <color=\"green\">+" + increase + "</color> base point score on <color=\"green\">Frozen</color> tiles";
        }

        //string desc = "+ Gain <color=\"green\">+" + increase + "</color> base point score on least matched color";

        return desc;
    }
}

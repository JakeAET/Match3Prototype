using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BaseTileIncreaseType
{
    Dwarf,
    Bomblin
}

public class AbilityBasePointIncrease : Ability
{
    public BaseTileIncreaseType type;
    public int initalLvlUpTileIncrease;
    public int lvlUpTileIncrease;
    [SerializeField] GameManager gm;
    private int currentBaseIncrease;
    private int targetTilesDestroyed;

    public override void initialize()
    {
        gm = FindObjectOfType<GameManager>();
        determineMaxLevel();

        if(type == BaseTileIncreaseType.Bomblin)
        {
            BoardManager.OnBombDestroyed += triggerEffect;
        }
        else if(type == BaseTileIncreaseType.Dwarf)
        {
            BoardManager.OnRocketDestroyed += triggerEffect;
        }
    }

    private void OnDisable()
    {
        if (type == BaseTileIncreaseType.Bomblin)
        {
            BoardManager.OnBombDestroyed -= triggerEffect;
        }
        else if (type == BaseTileIncreaseType.Dwarf)
        {
            BoardManager.OnRocketDestroyed -= triggerEffect;
        }
    }

    public override void triggerEffect()
    {
        targetTilesDestroyed++;
        gm.bonusBaseElementValue += currentBaseIncrease;
    }

    public override void levelUp()
    {
        if (level < maxLevel)
        {
            level++;

            gm.bonusBaseElementValue -= (currentBaseIncrease * targetTilesDestroyed);

            if (level == 1)
            {
                currentBaseIncrease += initalLvlUpTileIncrease;
            }
            else
            {
                currentBaseIncrease += lvlUpTileIncrease;
            }

            gm.bonusBaseElementValue += (currentBaseIncrease * targetTilesDestroyed);
        }
    }

    public override void undoAbility(int levelNum)
    {
        for (int i = 0; i < levelNum; i++)
        {
            level--;

            gm.bonusBaseElementValue -= (currentBaseIncrease * targetTilesDestroyed);

            if (level == 1)
            {
                currentBaseIncrease -= initalLvlUpTileIncrease;
            }
            else
            {
                currentBaseIncrease -= lvlUpTileIncrease;
            }

            gm.bonusBaseElementValue += (currentBaseIncrease * targetTilesDestroyed);
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

        if (type == BaseTileIncreaseType.Dwarf)
        {
            desc = "- <color=\"green\">+" + (currentBaseIncrease) + "</color>" + " base points for each rocket used. Current bonus: " + "<color=\"green\">+" + (currentBaseIncrease * targetTilesDestroyed) + "</color>";
        }

        if (type == BaseTileIncreaseType.Bomblin)
        {
            desc = "- <color=\"green\">+" + (currentBaseIncrease) + "</color>" + "  base points for each bomb used. Current bonus: " + "<color=\"green\">+" + (currentBaseIncrease * targetTilesDestroyed) + "</color>";
        }

        return desc;
    }

    public override string patronSelectDescription()
    {
        string desc = "";

        int increase = 0;

        if(level == 0)
        {
            increase = initalLvlUpTileIncrease;
        }
        else
        {
            increase = lvlUpTileIncrease;
        }

        if (type == BaseTileIncreaseType.Dwarf)
        {
            desc = "+ Gain " + "<color=\"green\">+" + (increase) + "</color>" + " base points for each rocket used";
        }

        if (type == BaseTileIncreaseType.Bomblin)
        {
            desc = "+ Gain " + "<color=\"green\">+" + (increase) + "</color>" + " base points for each bomb used";
        }

        return desc;
    }
}

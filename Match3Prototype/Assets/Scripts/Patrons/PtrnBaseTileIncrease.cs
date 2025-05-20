using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public enum BaseTileIncreaseType
{
    Dwarf,
    Bomblin
}

public class PtrnBaseTileIncrease : Patron
{
    public BaseTileIncreaseType type;
    public int initalLvlUpTileIncrease;
    public int lvlUpTileIncrease;
    private GameManager gameManager;
    private int currentBaseIncrease;
    private int targetTilesDestroyed;


    public override bool conditionMet()
    {
        return true;
    }

    public override void triggerEffect()
    {
        gameManager = FindObjectOfType<GameManager>();
        targetTilesDestroyed++;

        gameManager.bonusBaseElementValue += currentBaseIncrease;
    }

    public override void levelUp()
    {
        if (level < maxLevel)
        {
            level++;
            FindObjectOfType<PatronManager>().updatePatronLvl(index, level);
            gameManager = FindObjectOfType<GameManager>();

            gameManager.bonusBaseElementValue -= (currentBaseIncrease * targetTilesDestroyed);

            if (level == 1)
            {
                currentBaseIncrease += initalLvlUpTileIncrease;
            }
            else
            {
                currentBaseIncrease += lvlUpTileIncrease;
            }

            gameManager.bonusBaseElementValue += (currentBaseIncrease * targetTilesDestroyed);
        }
    }

    public override void reduceLevel(int levelNum)
    {
        for (int i = 0; i < levelNum; i++)
        {
            level--;
            FindObjectOfType<PatronManager>().updatePatronLvl(index, level);

            gameManager.bonusBaseElementValue -= (currentBaseIncrease * targetTilesDestroyed);

            if (level == 1)
            {
                currentBaseIncrease -= initalLvlUpTileIncrease;
            }
            else
            {
                currentBaseIncrease -= lvlUpTileIncrease;
            }

            gameManager.bonusBaseElementValue += (currentBaseIncrease * targetTilesDestroyed);
        }
    }

    public override void restoreLevel(int levelNum)
    {
        for (int i = 0; i < levelNum; i++)
        {
            levelUp();
        }
    }

    public override string currentDescription()
    {
        string desc = "";

        if (type == BaseTileIncreaseType.Dwarf)
        {
            desc = "Each rocket launched adds " + "<color=\"green\">+" + (currentBaseIncrease) + "</color>" + " to the base point score of all tiles. Current bonus: " + "<color=\"green\">+" + (currentBaseIncrease * targetTilesDestroyed) + "</color>";
        }

        if (type == BaseTileIncreaseType.Bomblin)
        {
            desc = "Each bomb exploded adds " + "<color=\"green\">+" + (currentBaseIncrease) + "</color>" + " to the base point score of all tiles. Current bonus: " + "<color=\"green\">+" + (currentBaseIncrease * targetTilesDestroyed) + "</color>";
        }

        return desc;
    }
}

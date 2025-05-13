using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public enum ElementType
{
    Enchanted,
    Frozen
}

public class PtrnElementalTile : Patron
{
    public ElementType spawnElement;
    public bool effectTriggered = false;
    public int initialTileIncrease;
    public int lvlUpTileIncrease;
    private BoardManager board;
    private int currentElemTiles;


    public override bool conditionMet()
    {
        return true;
    }

    public override void triggerEffect()
    {
        if (!effectTriggered)
        {
            //Debug.Log(spawnElement + " Elemental Tile Spawn Effect Triggered");
            board = FindObjectOfType<BoardManager>();

            if (spawnElement == ElementType.Frozen)
            {
                if (level == 1)
                {
                    board.maxFrozenTiles += initialTileIncrease;
                }
                else
                {
                    board.maxFrozenTiles += lvlUpTileIncrease;
                }
            }

            if (spawnElement == ElementType.Enchanted)
            {
                if (level == 1)
                {
                    board.maxEnchantedTiles += initialTileIncrease;
                }
                else
                {
                    board.maxEnchantedTiles += lvlUpTileIncrease;
                }
            }

            effectTriggered = true;
        }
    }

    public override void levelUp()
    {
        if (level < maxLevel)
        {
            level++;
            FindObjectOfType<PatronManager>().updatePatronLvl(index, level);

            effectTriggered = false;
            triggerEffect();
        }
    }

    public override void reduceLevel(int levelNum)
    {
        for (int i = 0; i < levelNum; i++)
        {
            level--;
            FindObjectOfType<PatronManager>().updatePatronLvl(index, level);

            if (spawnElement == ElementType.Frozen)
            {
                if (level == 1)
                {
                    board.maxFrozenTiles -= initialTileIncrease;
                }
                else
                {
                    board.maxFrozenTiles -= lvlUpTileIncrease;
                }
            }

            if (spawnElement == ElementType.Enchanted)
            {
                if (level == 1)
                {
                    board.maxEnchantedTiles -= initialTileIncrease;
                }
                else
                {
                    board.maxEnchantedTiles -= lvlUpTileIncrease;
                }
            }
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

        if (spawnElement == ElementType.Frozen)
        {
            desc = "Each turn, create " + "<color=\"green\">" + board.maxFrozenTiles + "</color>" + " frozen tiles that shatter when adjacent matches are made";
        }

        if (spawnElement == ElementType.Enchanted)
        {
            desc = "Each turn, create " + "<color=\"green\">" + board.maxEnchantedTiles + "</color>" + " enchanted tiles that score for double points";
        }

        return desc;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType
{
    Enchanted,
    Frozen
}

public class AbilityElementalTileSpawn : Ability
{
    public ElementType spawnElement;
    public int initialTileIncrease;
    public int lvlUpTileIncrease;
    private BoardManager board;

    public override void initialize()
    {
        board = FindObjectOfType<BoardManager>();
        determineMaxLevel();
    }

    public override void triggerEffect()
    {
        if (canTrigger)
        {
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

            canTrigger = false;
        }
    }

    public override void levelUp()
    {
        if (level < maxLevel)
        {
            level++;
            canTrigger = true;
            triggerEffect();
        }
    }

    public override void undoAbility(int levelNum)
    {
        for (int i = 0; i < levelNum; i++)
        {
            level--;

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

        if (spawnElement == ElementType.Frozen)
        {
            desc = "- Always have " + "<color=\"green\">" + board.maxFrozenTiles + "</color>" + " frozen tile(s) on the board";
        }

        if (spawnElement == ElementType.Enchanted)
        {
            desc = "- Always have " + "<color=\"green\">" + board.maxEnchantedTiles + "</color>" + " enchanted tile(s) on the board";
        }

        return desc;
    }

    public override string patronSelectDescription()
    {
        board = FindObjectOfType<BoardManager>();

        string desc = "";

        int tilesGained = 0;

        if (level == 0)
        {
            desc += "+ Always have ";
        }
        else
        {
            desc += "+ Additional ";
        }

        if (spawnElement == ElementType.Frozen)
        {

            if(level == 0)
            {
                tilesGained = initialTileIncrease;
            }
            else
            {
                tilesGained = lvlUpTileIncrease;
            }

            desc +=  "<color=\"green\">" + tilesGained + "</color>" + " frozen tile(s) on the board";
        }

        if (spawnElement == ElementType.Enchanted)
        {

            if (level == 0)
            {
                tilesGained = initialTileIncrease;
            }
            else
            {
                tilesGained = lvlUpTileIncrease;
            }
            desc += "<color=\"green\">" + tilesGained + "</color>" + " enchanted tile(s) on the board";
        }

        return desc;
    }
}

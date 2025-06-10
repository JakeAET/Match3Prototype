using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityColorSpawn : Ability
{
    public int spawnRateIncrease;
    public TargetColor targetColor;
    [SerializeField] BoardManager board;

    public override void initialize()
    {
        board = FindObjectOfType<BoardManager>();
        determineMaxLevel();
    }

    public override void triggerEffect()
    {
        if (canTrigger)
        {
            changeColorSpawn(spawnRateIncrease);
        }
    }

    public override void levelUp()
    {
        if (level < maxLevel)
        {
            level++;
            triggerEffect();
        }
    }

    public override void undoAbility(int levelNum)
    {
        for (int i = 0; i < levelNum; i++)
        {
            level--;
            changeColorSpawn(spawnRateIncrease * -1);
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

        if (targetColor == TargetColor.Red)
        {
            desc = "- Increase chance of red tiles by " + "<color=\"green\">" + board.redSpawnRate + "x" + "</color>";
        }
        if (targetColor == TargetColor.Blue)
        {
            desc = "- Increase chance of blue tiles by " + "<color=\"green\">" + board.blueSpawnRate + "x" + "</color>";
        }
        if (targetColor == TargetColor.Green)
        {
            desc = "- Increase chance of green tiles by " + "<color=\"green\">" + board.greenSpawnRate + "x" + "</color>";
        }
        if (targetColor == TargetColor.Purple)
        {
            desc = "- Increase chance of purple tiles by " + "<color=\"green\">" + board.purpleSpawnRate + "x" + "</color>";
        }
        if (targetColor == TargetColor.Yellow)
        {
            desc = "- Increase chance of yellow tiles by " + "<color=\"green\">" + board.yellowSpawnRate + "x" + "</color>";
        }

        return desc;
    }

    public override string patronSelectDescription()
    {
        board = FindObjectOfType<BoardManager>();

        string desc = "";

        if (level == 0)
        {
            desc += "+ Increase ";
        }
        else
        {
            desc += "+ Upgrade ";
        }

        if (targetColor == TargetColor.Red)
        {
            desc += "chance of red tiles to " + "<color=\"green\">" + (board.redSpawnRate + spawnRateIncrease) + "x" + "</color>";
        }
        if (targetColor == TargetColor.Blue)
        {
            desc += "chance of blue tiles to " + "<color=\"green\">" + (board.blueSpawnRate + spawnRateIncrease) + "x" + "</color>";
        }
        if (targetColor == TargetColor.Green)
        {
            desc += "chance of green tiles to " + "<color=\"green\">" + (board.greenSpawnRate + spawnRateIncrease) + "x" + "</color>";
        }
        if (targetColor == TargetColor.Purple)
        {
            desc += "chance of purple tiles to " + "<color=\"green\">" + (board.purpleSpawnRate + spawnRateIncrease) + "x" + "</color>";
        }
        if (targetColor == TargetColor.Yellow)
        {
            desc += "chance of yellow tiles to " + "<color=\"green\">" + (board.yellowSpawnRate + spawnRateIncrease) + "x" + "</color>";
        }

        return desc;
    }

    private void changeColorSpawn(int amount)
    {
        if (targetColor == TargetColor.Red)
        {
            board.redSpawnRate += amount;
        }
        if (targetColor == TargetColor.Blue)
        {
            board.blueSpawnRate += amount;
        }
        if (targetColor == TargetColor.Green)
        {
            board.greenSpawnRate += amount;
        }
        if (targetColor == TargetColor.Purple)
        {
            board.purpleSpawnRate += amount;
        }
        if (targetColor == TargetColor.Yellow)
        {
            board.yellowSpawnRate += amount;
        }
    }
}

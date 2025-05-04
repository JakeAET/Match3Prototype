using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PtrnColorSpawn : Patron
{
    public TargetColor targetColor;
    public bool effectTriggered = false;
    public int spawnRateIncrease;
    private BoardManager board;


    public override bool conditionMet()
    {
        return true;
    }

    public override void triggerEffect()
    {
        if (!effectTriggered)
        {
            //Debug.Log(targetColor + " spawn rate effect Triggered");
            board = FindObjectOfType<BoardManager>();
            if (targetColor == TargetColor.Red)
            {
                board.redSpawnRate = level * spawnRateIncrease;
            }
            if (targetColor == TargetColor.Blue)
            {
                board.blueSpawnRate = level * spawnRateIncrease;
            }
            if (targetColor == TargetColor.Green)
            {
                board.greenSpawnRate = level * spawnRateIncrease;
            }
            if (targetColor == TargetColor.Purple)
            {
                board.purpleSpawnRate = level * spawnRateIncrease;
            }
            if (targetColor == TargetColor.Yellow)
            {
                board.yellowSpawnRate = level * spawnRateIncrease;
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

            effectTriggered = false;
            triggerEffect();
        }
    }

    public override void restoreLevel(int levelNum)
    {
        for (int i = 0; i < levelNum; i++)
        {
            levelUp();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetColor
{
    red,
    blue,
    green,
    purple,
    yellow
}

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
            Debug.Log(targetColor + " spawn rate effect Triggered");
            board = FindObjectOfType<BoardManager>();
            if (targetColor == TargetColor.red)
            {
                board.redSpawnRate = level * spawnRateIncrease;
            }
            if (targetColor == TargetColor.blue)
            {
                board.blueSpawnRate = level * spawnRateIncrease;
            }
            if (targetColor == TargetColor.green)
            {
                board.greenSpawnRate = level * spawnRateIncrease;
            }
            if (targetColor == TargetColor.purple)
            {
                board.purpleSpawnRate = level * spawnRateIncrease;
            }
            if (targetColor == TargetColor.yellow)
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

            effectTriggered = false;
            triggerEffect();
        }
    }
}

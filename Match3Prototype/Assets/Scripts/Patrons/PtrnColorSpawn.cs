using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PtrnColorSpawn : Patron
{
    public TargetColor targetColor;
    public bool effectTriggered = false;
    public int spawnRateIncrease;
    public float pointIncrease;
    private BoardManager board;
    private GameManager gm;

    public override bool conditionMet()
    {
        return true;
    }

    public override void triggerEffect()
    {
        if (!effectTriggered)
        {
            //Debug.Log(targetColor + " spawn rate effect Triggered");
            //board = FindObjectOfType<BoardManager>();
            //if (targetColor == TargetColor.Red)
            //{
            //    board.redSpawnRate += spawnRateIncrease;
            //}
            //if (targetColor == TargetColor.Blue)
            //{
            //    board.blueSpawnRate += spawnRateIncrease;
            //}
            //if (targetColor == TargetColor.Green)
            //{
            //    board.greenSpawnRate += spawnRateIncrease;
            //}
            //if (targetColor == TargetColor.Purple)
            //{
            //    board.purpleSpawnRate += spawnRateIncrease;
            //}
            //if (targetColor == TargetColor.Yellow)
            //{
            //    board.yellowSpawnRate += spawnRateIncrease;
            //}

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

            //board = FindObjectOfType<BoardManager>();
            //if (targetColor == TargetColor.Red)
            //{
            //    board.redSpawnRate -= spawnRateIncrease;
            //}
            //if (targetColor == TargetColor.Blue)
            //{
            //    board.blueSpawnRate -= spawnRateIncrease;
            //}
            //if (targetColor == TargetColor.Green)
            //{
            //    board.greenSpawnRate -= spawnRateIncrease;
            //}
            //if (targetColor == TargetColor.Purple)
            //{
            //    board.purpleSpawnRate -= spawnRateIncrease;
            //}
            //if (targetColor == TargetColor.Yellow)
            //{
            //    board.yellowSpawnRate -= spawnRateIncrease;
            //}
        }
    }

    public override void restoreLevel(int levelNum)
    {
        for (int i = 0; i < levelNum; i++)
        {
            levelUp();
        }
    }

    //private void changeColorSpawn(int amount)
    //{

    //    board = FindObjectOfType<BoardManager>();
    //    if (targetColor == TargetColor.Red)
    //    {
    //        board.redSpawnRate += amount;
    //    }
    //    if (targetColor == TargetColor.Blue)
    //    {
    //        board.blueSpawnRate += amount;
    //    }
    //    if (targetColor == TargetColor.Green)
    //    {
    //        board.greenSpawnRate += amount;
    //    }
    //    if (targetColor == TargetColor.Purple)
    //    {
    //        board.purpleSpawnRate += amount;
    //    }
    //    if (targetColor == TargetColor.Yellow)
    //    {
    //        board.yellowSpawnRate += amount;
    //    }
    //}

    //private void changeColorPoints(float amount)
    //{
    //    gm.colorElementIncrease[targetColor] += amount;
    //}
}

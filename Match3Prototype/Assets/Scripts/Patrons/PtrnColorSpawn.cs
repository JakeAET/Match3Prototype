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

            // bad code please re-do
            if(level == 1 || level == 3 || level == 5)
            {
                changeColorSpawn(spawnRateIncrease);
            }
            else if (level == 2 || level == 4)
            {
                changeColorPoints(pointIncrease);
            }

            effectTriggered = true;
        }
    }

    public override void levelUp()
    {
        board = FindObjectOfType<BoardManager>();
        gm = FindAnyObjectByType<GameManager>();

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

            // bad code please re-do
            if (level == 1 || level == 3 || level == 5)
            {
                changeColorSpawn(spawnRateIncrease * -1);
            }
            else if (level == 2 || level == 4)
            {
                changeColorPoints(pointIncrease * -1);
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

        if (targetColor == TargetColor.Red)
        {
            desc = "Increase chance of red tiles by " + "<color=\"green\">" + board.redSpawnRate + "x" + "</color> and " + "<color=\"green\">+" + gm.colorElementIncrease[targetColor] + "</color> points for red tiles";
        }
        if (targetColor == TargetColor.Blue)
        {
            desc = "Increase chance of blue tiles by " + "<color=\"green\">" + board.blueSpawnRate + "x" + "</color> and " + "<color=\"green\">+" + gm.colorElementIncrease[targetColor] + "</color> points for blue tiles";
        }
        if (targetColor == TargetColor.Green)
        {
            desc = "Increase chance of green tiles by " + "<color=\"green\">" + board.greenSpawnRate + "x" + "</color> and " + "<color=\"green\">+" + gm.colorElementIncrease[targetColor] + "</color> points for green tiles";
        }
        if (targetColor == TargetColor.Purple)
        {
            desc = "Increase chance of purple tiles by " + "<color=\"green\">" + board.purpleSpawnRate + "x" + "</color> and " + "<color=\"green\">+" + gm.colorElementIncrease[targetColor] + "</color> points for purple tiles";
        }
        if (targetColor == TargetColor.Yellow)
        {
            desc = "Increase chance of yellow tiles by " + "<color=\"green\">" + board.yellowSpawnRate + "x" + "</color> and " + "<color=\"green\">+" + gm.colorElementIncrease[targetColor] + "</color> points for yellow tiles";
        }

        return desc;
    }

    private void changeColorSpawn(int amount)
    {

        board = FindObjectOfType<BoardManager>();
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

    private void changeColorPoints(float amount)
    {
        gm.colorElementIncrease[targetColor] += amount;
    }
}

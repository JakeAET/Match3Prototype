using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PtrnScout : Patron
{
    public int initialPointIncrease;
    public int pointIncrease;
    private int currentPointIncrease;
    private GameManager gm;


    public override bool conditionMet()
    {
        return true;
    }

    public float increaseAmount(string colorName)
    {
        gm = FindObjectOfType<GameManager>();
        float amount = 0;

        int leastMatchedNum = 0;
        string mostMatchedColor = "";
        foreach (KeyValuePair<string, int> kvp in gm.colorTilesCleared)
        {
            if (kvp.Value < leastMatchedNum)
            {
                leastMatchedNum = kvp.Value;
                mostMatchedColor = kvp.Key;
            }
        }

        if(gm.colorTilesCleared[colorName] <= leastMatchedNum)
        {
            amount += currentPointIncrease;
            Debug.Log("scout found least matched color of " +  colorName);
        }

        return amount;
    }

    public override void levelUp()
    {
        if (level < maxLevel)
        {
            level++;
            FindObjectOfType<PatronManager>().updatePatronLvl(index, level);

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

    public override void reduceLevel(int levelNum)
    {
        for (int i = 0; i < levelNum; i++)
        {
            level--;
            FindObjectOfType<PatronManager>().updatePatronLvl(index, level);

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

    public override void restoreLevel(int levelNum)
    {
        for (int i = 0; i < levelNum; i++)
        {
            levelUp();
        }
    }

    public override string currentDescription()
    {
        string desc = "Increases the base point score of the least matched color tile by " + "<color=\"green\">+" + currentPointIncrease + "</color>";

        return desc;
    }
}

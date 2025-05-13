using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PtrnOldWoman : Patron
{
    public bool effectTriggered = false;
    public int turnIncrease;
    private GameManager gm;


    public override bool conditionMet()
    {
        return true;
    }

    public override void triggerEffect()
    {
        if (!effectTriggered)
        {
            //Debug.Log("Old Woman (turn increase) effect Triggered");
            gm = FindObjectOfType<GameManager>();
            gm.increaseMaxTurns(turnIncrease);

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
            gm.increaseMaxTurns(turnIncrease * -1);
            level--;
            FindObjectOfType<PatronManager>().updatePatronLvl(index, level);
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
        string desc = "Grants " + "<color=\"green\">" + (level * turnIncrease) + "</color>" + " additional turns per round";

        return desc;
    }
}

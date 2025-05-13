using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PtrnZombie : Patron
{
    public bool effectTriggered = false;
    public int undoIncrease;
    private GameManager gm;


    public override bool conditionMet()
    {
        return true;
    }

    public override void triggerEffect()
    {
        if (!effectTriggered)
        {
            //Debug.Log("Zombie (undo increase) effect Triggered");
            gm = FindObjectOfType<GameManager>();
            gm.increaseMaxUndos(undoIncrease);

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
            gm.increaseMaxUndos(undoIncrease * -1);
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
        string desc = "Grants " + "<color=\"green\">" + gm.maxUndos + "</color>" + " turn undos per round";

        return desc;
    }
}

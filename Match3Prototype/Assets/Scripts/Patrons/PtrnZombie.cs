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
            Debug.Log("Zombie (undo increase) effect Triggered");
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

            effectTriggered = false;
            triggerEffect();
        }
    }
}

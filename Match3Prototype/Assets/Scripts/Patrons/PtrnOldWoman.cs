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
            Debug.Log("Old Woman (turn increase) effect Triggered");
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

            effectTriggered = false;
            triggerEffect();
        }
    }
}

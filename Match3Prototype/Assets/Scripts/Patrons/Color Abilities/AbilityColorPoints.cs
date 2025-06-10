using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityColorPoints : Ability
{
    public float pointIncrease;
    public TargetColor targetColor;
    [SerializeField] GameManager gm;

    public override void initialize()
    {
        gm = FindAnyObjectByType<GameManager>();
        determineMaxLevel();
    }

    public override void triggerEffect()
    {
        if (canTrigger)
        {
            changeColorPoints(pointIncrease);
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
            changeColorPoints(pointIncrease * -1);
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
            desc = "- Increase of " + "<color=\"green\">+" + gm.colorElementIncrease[targetColor] + "</color> points for red tiles";
        }
        if (targetColor == TargetColor.Blue)
        {
            desc = "- Increase of " + "<color=\"green\">+" + gm.colorElementIncrease[targetColor] + "</color> points for blue tiles";
        }
        if (targetColor == TargetColor.Green)
        {
            desc = "- Increase of " + "<color=\"green\">+" + gm.colorElementIncrease[targetColor] + "</color> points for green tiles";
        }
        if (targetColor == TargetColor.Purple)
        {
            desc = "- Increase of " + "<color=\"green\">+" + gm.colorElementIncrease[targetColor] + "</color> points for purple tiles";
        }
        if (targetColor == TargetColor.Yellow)
        {
            desc = "- Increase of " + "<color=\"green\">+" + gm.colorElementIncrease[targetColor] + "</color> points for yellow tiles";
        }

        return desc;
    }

    public override string patronSelectDescription()
    {
        gm = FindAnyObjectByType<GameManager>();

        string desc = "";

        if (level == 0)
        {
            desc += "+ Increase of ";
        }
        else
        {
            desc += "+ Upgraded to ";
        }

        if (targetColor == TargetColor.Red)
        {
            desc += "<color=\"green\">+" + (gm.colorElementIncrease[targetColor] + pointIncrease) + "</color> points for red tiles";
        }
        if (targetColor == TargetColor.Blue)
        {
            desc += "<color=\"green\">+" + (gm.colorElementIncrease[targetColor] + pointIncrease) + "</color> points for blue tiles";
        }
        if (targetColor == TargetColor.Green)
        {
            desc += "<color=\"green\">+" + (gm.colorElementIncrease[targetColor] + pointIncrease) + "</color> points for green tiles";
        }
        if (targetColor == TargetColor.Purple)
        {
            desc += "<color=\"green\">+" + (gm.colorElementIncrease[targetColor] + pointIncrease) + "</color> points for purple tiles";
        }
        if (targetColor == TargetColor.Yellow)
        {
            desc += "<color=\"green\">+" + (gm.colorElementIncrease[targetColor] + pointIncrease) + "</color> points for yellow tiles";
        }

        return desc;
    }

    private void changeColorPoints(float amount)
    {
        gm.colorElementIncrease[targetColor] += amount;
    }
}

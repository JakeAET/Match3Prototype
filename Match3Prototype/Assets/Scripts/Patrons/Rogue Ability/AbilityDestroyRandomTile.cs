using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDestroyRandomTile : Ability
{
    public int tilesDestroyedIncrease;
    private int currentTilesDestroyed;
    private BoardManager board;

    public override void initialize()
    {
        determineMaxLevel();
        board = FindObjectOfType<BoardManager>();

        FindMatches.OnRogueTrigger += triggerEffect;
    }

    private void OnDisable()
    {
        FindMatches.OnRogueTrigger -= triggerEffect;
    }

    public override void triggerEffect()
    {
        for (int i = 0; i < currentTilesDestroyed; i++)
        {
            Element targetElement = board.randomUnmaskedElement();
            while (targetElement.isMatched || board.allTiles[targetElement.column, targetElement.row].isMasked)
            {
                targetElement = board.randomUnmaskedElement();
            }
            targetElement.isMatched = true;
            targetElement.markedByRogue = true;
        }
        procEffect();
    }

    public override void levelUp()
    {
        if (level < maxLevel)
        {
            level++;
            currentTilesDestroyed += tilesDestroyedIncrease;
        }
    }

    public override void undoAbility(int levelNum)
    {
        for (int i = 0; i < levelNum; i++)
        {
            level--;
            currentTilesDestroyed -= tilesDestroyedIncrease;
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
        string desc = "- Destroy " + "<color=\"green\">" + currentTilesDestroyed + "</color>" + " random tile(s) each swap made";

        return desc;
    }

    public override string patronSelectDescription()
    {
        int increase = 0;

        string desc = "";

        if (level == 0)
        {
            increase = tilesDestroyedIncrease;
        }
        else
        {
            increase = currentTilesDestroyed;
        }

        desc += "<color=\"green\">+ " + increase + "</color>" + " random tile(s) destroyed each swap made";

        return desc;
    }
}

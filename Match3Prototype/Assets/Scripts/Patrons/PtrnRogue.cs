using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PtrnRogue : Patron
{
    public int tilesDestroyedIncrease;
    private int currentTilesDestroyed;


    public override bool conditionMet()
    {
        return true;
    }

    public override void triggerEffect()
    {
        BoardManager bm = FindObjectOfType<BoardManager>();

        for (int i = 0; i < currentTilesDestroyed; i++)
        {
            Element targetElement = bm.allElements[UnityEngine.Random.Range(0, bm.width - 1), UnityEngine.Random.Range(0, bm.height - 1)].GetComponent<Element>();
            while (targetElement.isMatched)
            {
                targetElement = bm.allElements[UnityEngine.Random.Range(0, bm.width - 1), UnityEngine.Random.Range(0, bm.height - 1)].GetComponent<Element>();
            }
            targetElement.isMatched = true;
        }
    }

    public override void levelUp()
    {
        if (level < maxLevel)
        {
            level++;
            FindObjectOfType<PatronManager>().updatePatronLvl(index, level);

            currentTilesDestroyed += tilesDestroyedIncrease;
        }
    }

    public override void reduceLevel(int levelNum)
    {
        for (int i = 0; i < levelNum; i++)
        {
            level--;
            FindObjectOfType<PatronManager>().updatePatronLvl(index, level);

            currentTilesDestroyed -= tilesDestroyedIncrease;
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
        string desc = "Destroy " + "<color=\"green\">" + currentTilesDestroyed + "</color>" + " random tile(s) each swap made";

        return desc;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BssRndColorRestrict : BossRound
{
    public TargetColor targetColor;
    private BoardManager board;

    public override void activateConstraint()
    {
        board = FindObjectOfType<BoardManager>();
        board.banishedType = targetColor;

        foreach (GameObject obj in board.allElements)
        {
            if(obj != null)
            {
                Element target = obj.GetComponent<Element>();
                if (target != null)
                {
                    if(target.color == targetColor)
                    {
                        target.banish();
                    }
                }
            }
        }
    }

    public override void deactivateConstraint()
    {
        board = FindObjectOfType<BoardManager>();
        board.banishedType = TargetColor.None;
    }
}

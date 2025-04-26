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
    }

    public override void deactivateConstraint()
    {
        board = FindObjectOfType<BoardManager>();
        board.banishedType = TargetColor.none;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BssRndPointDebuff : BossRound
{
    [SerializeField] float debuffMult;
    private BoardManager board;

    public override void activateConstraint()
    {
        board = FindObjectOfType<BoardManager>();
        board.basePointDebuff = debuffMult;
    }

    public override void deactivateConstraint()
    {
        board = FindObjectOfType<BoardManager>();
        board.basePointDebuff = 1;
    }
}

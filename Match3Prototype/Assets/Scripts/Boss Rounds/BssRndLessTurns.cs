using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BssRndLessTurns : BossRound
{
    [SerializeField] int numLessTurns;
    private int originalTurnCount;
    private GameManager gm;

    public override void activateConstraint()
    {
        gm = FindObjectOfType<GameManager>();
        originalTurnCount = gm.maxTurns;
        gm.maxTurns -= numLessTurns;
    }

    public override void deactivateConstraint()
    {
        gm = FindObjectOfType<GameManager>();
        gm.maxTurns = originalTurnCount;
    }
}

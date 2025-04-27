using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BssRndHighPoint : BossRound
{
    [SerializeField] float targetPointMulti;
    private GameManager gm;

    public override void activateConstraint()
    {
        gm = FindObjectOfType<GameManager>();
        gm.extraHighPointMulti = targetPointMulti;
    }

    public override void deactivateConstraint()
    {
        gm = FindObjectOfType<GameManager>();
        gm.extraHighPointMulti = 1;
    }
}

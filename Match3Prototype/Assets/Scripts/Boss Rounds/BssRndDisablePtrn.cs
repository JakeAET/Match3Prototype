using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BssRndDisablePtrn : BossRound
{
    private Patron affectedPatron;
    private int levelsReduced;
    private PatronManager pm;
    private UIManager ui;

    public override void activateConstraint()
    {
        pm = FindObjectOfType<PatronManager>();
        ui = FindObjectOfType<UIManager>();

        affectedPatron = pm.activePatrons[UnityEngine.Random.Range(0, pm.activePatrons.Count)];

        levelsReduced = affectedPatron.level;

        affectedPatron.reduceLevel(levelsReduced);

        affectedPatron.banished = true;

        ui.patronSlotUIRefs[affectedPatron.index].disabledIcon.SetActive(true);
    }

    public override void deactivateConstraint()
    {
        affectedPatron.banished = false;
        affectedPatron.restoreLevel(levelsReduced);
        ui = FindObjectOfType<UIManager>();
        ui.patronSlotUIRefs[affectedPatron.index].disabledIcon.SetActive(false);
    }
}

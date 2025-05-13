using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BssRndReduceLvl : BossRound
{
    [SerializeField] int levelsReduced;
    private List<Patron> affectedPatrons;
    private List<int> levelsReducedRef;
    private PatronManager pm;
    private UIManager ui;

    public override void activateConstraint()
    {
        pm = FindObjectOfType<PatronManager>();
        ui = FindObjectOfType<UIManager>();
        foreach (Patron p in pm.activePatrons)
        {
            if(p.level > 1)
            {
                int actualReduced = levelsReduced;

                if ((p.level - levelsReduced) < 1)
                {
                    actualReduced = p.level - 1;
                }

                //ui.patronSlotUIRefs[p.index].levelDownIcon.SetActive(true);

                affectedPatrons.Add(p);
                levelsReducedRef.Add(actualReduced);
                p.reduceLevel(actualReduced);
            }
        }
    }

    public override void deactivateConstraint()
    {
        pm = FindObjectOfType<PatronManager>();
        ui = FindObjectOfType<UIManager>();
        for (int i = 0; i < affectedPatrons.Count; i++)
        {
            affectedPatrons[i].restoreLevel(levelsReducedRef[i]);
            //ui.patronSlotUIRefs[affectedPatrons[i].index].levelDownIcon.SetActive(false);
        }
        affectedPatrons.Clear();
        levelsReducedRef.Clear();
    }
}

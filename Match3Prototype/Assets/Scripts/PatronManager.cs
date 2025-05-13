using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PatronManager : MonoBehaviour
{
    [SerializeField] Patron[] potentialPatrons;
    public List<Patron> activePatrons;
    private UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Patron> selectPatrons(int numPatrons)
    {
        List<Patron> chosenPatrons = new List<Patron>();
        List<Patron> possiblePatrons = new List<Patron>();

        foreach (Patron p in potentialPatrons)
        {
            bool canAdd = true;
            for (int i = 0; activePatrons.Count > i; i++)
            {
                if(activePatrons[i].title == p.title)
                {
                    if (activePatrons[i].level == activePatrons[i].maxLevel)
                    {
                        canAdd = false;
                    }
                }
            }

            if (canAdd)
            {
                possiblePatrons.Add(p);
            }
        }

        for (int i = 0; i < numPatrons; i++)
        {
            if(possiblePatrons.Count != 0)
            {
                Patron selectedPatron = possiblePatrons[UnityEngine.Random.Range(0, possiblePatrons.Count)];
                chosenPatrons.Add(selectedPatron);
                possiblePatrons.Remove(selectedPatron);
            }
            else
            {
                Debug.Log("Ran out of possible patrons, only " + chosenPatrons.Count + " were selected");
                break;
            }
            //Debug.Log(selectedPatron.title);
        }

        return chosenPatrons;
    }

    public void selectNewPatron(Patron patron)
    {
        Patron targetPatron = null;

        bool preExisting = false;

        int index = 0;

        for (int i = 0; i < activePatrons.Count; i++)
        {
            if (patron.title == activePatrons[i].title)
            {
                preExisting = true;
                targetPatron = activePatrons[i];
                break;
            }
            index++;
        }

        if (preExisting)
        {
            targetPatron.index = index;
            targetPatron.levelUp();
        }
        else
        {
            targetPatron = Instantiate(patron);
            activePatrons.Add(targetPatron);
            targetPatron.index = activePatrons.Count - 1;
            uiManager.patronSlotUIRefs[activePatrons.Count - 1].initialize(targetPatron);
            uiManager.patronSlotUIRefsWS[activePatrons.Count - 1].initialize(targetPatron);

            //if (targetPatron.constantEffect && targetPatron.conditionMet()) //activate constant effect immediately
            //{
            //    targetPatron.triggerEffect();
            //}

            targetPatron.levelUp();
        }
    }

    public void updatePatronLvl(int ptrnIndex, int lvl)
    {
        //uiManager.patronSlotUIRefs[ptrnIndex].lvlText.text = "" + lvl;
    }

    public void removePatron(int ptrnIndex)
    {
        GameObject targetPatron = activePatrons[ptrnIndex].gameObject;
        activePatrons.RemoveAt(ptrnIndex);
        Destroy(targetPatron);
        reassignTopUIPatrons();
    }

    public bool canPatronBeChosen(int ptrnIndex, int ptrnUIRefIndex)
    {

        bool thisPreExisting = false;

        for (int i = 0; i < activePatrons.Count; i++)
        {
            if (uiManager.patronUIRefs[ptrnUIRefIndex].patronRef.title == activePatrons[i].title)
            {
                thisPreExisting = true;
                break;
            }
        }

        int newPatrons = 0;

        foreach (patronChoiceUI patron in uiManager.selectedPatronUIRefs)
        {
            bool preExisting = false;
            for (int i = 0; i < activePatrons.Count; i++)
            {
                if (patron.patronRef.title == activePatrons[i].title)
                {
                    preExisting = true;
                    break;
                }
            }
            if (!preExisting)
            {
                newPatrons++;
            }
        }

        if (activePatrons.Count == 5)
        {
            if (thisPreExisting)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if(newPatrons + activePatrons.Count == 5)
            {
                if (uiManager.patronUIRefs[ptrnUIRefIndex].patronToggle.isOn) // is already selected
                {
                    return true;
                }
                else
                {
                    if (thisPreExisting)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return true;
            }
        }
    }

    public void reassignTopUIPatrons()
    {
        for (int i = 0; i < 5; i++)
        {
            if(i < activePatrons.Count)
            {
                activePatrons[i].index = i;
                uiManager.patronSlotUIRefs[i].reassign(activePatrons[i]);
                uiManager.patronSlotUIRefsWS[i].reassign(activePatrons[i]);
            }
            else
            {
                uiManager.patronSlotUIRefs[i].clear();
                uiManager.patronSlotUIRefsWS[i].clear();
            }
        }
    }
}

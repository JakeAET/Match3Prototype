using System;
using System.Collections;
using System.Collections.Generic;
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
            //Debug.Log("patron " + index + " upgraded");
            targetPatron.index = index;
            targetPatron.levelUp();
        }
        else
        {
            var newPatron = Instantiate(patron);
            //Debug.Log("new patron added");

            // TODO Instantiate new patron

            targetPatron = newPatron;
            activePatrons.Add(targetPatron);
            targetPatron.index = activePatrons.Count - 1;
            uiManager.patronSlots[activePatrons.Count - 1].SetActive(true);
            uiManager.patronSlotUIRefs[activePatrons.Count - 1].ptrnSprite.sprite = targetPatron.sprite;
            //uiManager.patronUpperImgs[activePatrons.Count - 1].color = targetPatron.color;

            if (targetPatron.constantEffect && targetPatron.conditionMet()) //activate constant effect immediately
            {
                targetPatron.triggerEffect();
            }
        }
    }

    public void updatePatronLvl(int ptrnIndex, int lvl)
    {
        uiManager.patronSlotUIRefs[ptrnIndex].lvlText.text = "" + lvl;
    }

    //private Patron copyPatron(Patron patronRef)
    //{
    //    Patron newPatron = new Patron;
    //    newPatron.title = patronRef.title;
    //    newPatron.effectDescription = patronRef.effectDescription;
    //    newPatron.conditionalEffect = patronRef.conditionalEffect;
    //    newPatron.constantEffect = patronRef.constantEffect;
    //    newPatron.level = patronRef.level;
    //    newPatron.maxLevel = patronRef.maxLevel;
    //    newPatron.sprite = patronRef.sprite;
    //    return newPatron;
    //}
}

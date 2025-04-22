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

    public List<Patron> select3Patrons()
    {
        List<Patron> chosenPatrons = new List<Patron>();
        List<Patron> possiblePatrons = new List<Patron>();

        foreach (Patron p in potentialPatrons)
        {
            possiblePatrons.Add(p);
        }

        for (int i = 0; i < 3; i++)
        {
            Patron selectedPatron = possiblePatrons[Random.Range(0, possiblePatrons.Count)];
            chosenPatrons.Add(selectedPatron);
            possiblePatrons.Remove(selectedPatron);
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
            Debug.Log("patron " + index + " upgraded");
            targetPatron.levelUp();
            uiManager.patronLvls[index].text = "" + targetPatron.level;

            if(targetPatron.level == targetPatron.maxLevel)
            {
                // Remove patron from selection list
            }

        }
        else
        {
            var newPatron = Instantiate(patron);
            //Debug.Log("new patron added");

            // TODO Instantiate new patron

            targetPatron = newPatron;
            activePatrons.Add(targetPatron);
            uiManager.patronSlots[activePatrons.Count - 1].SetActive(true);
            uiManager.patronUpperImgs[activePatrons.Count - 1].sprite = targetPatron.sprite;
            uiManager.patronUpperImgs[activePatrons.Count - 1].color = targetPatron.color;
            uiManager.patronUpperImgs[activePatrons.Count - 1].gameObject.SetActive(true);
            uiManager.patronLvls[activePatrons.Count - 1].gameObject.SetActive(true);

            if (targetPatron.constantEffect && targetPatron.conditionMet()) //activate constant effect immediately
            {
                targetPatron.triggerEffect();
            }
        }
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

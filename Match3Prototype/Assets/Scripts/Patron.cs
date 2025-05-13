using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Patron : MonoBehaviour
{
    public string title;
    public string[] effectDescriptions;
    public bool conditionalEffect;
    public bool constantEffect;
    public int level = 1;
    public int maxLevel = 5;
    public int index = 0;
    public Sprite sprite;
    public Color color;
    public GameObject patronChoiceUIPrefab;

    public virtual bool conditionMet()
    {
        return false;
    }

    public virtual void triggerEffect()
    {
        // 
    }

    public virtual void levelUp()
    {
        if(level < maxLevel)
        {
            level++;
            FindObjectOfType<PatronManager>().updatePatronLvl(index, level);
        }
    }

    public virtual void reduceLevel(int levelNum)
    {

    }

    public virtual void restoreLevel(int levelNum)
    {

    }

    public virtual string currentDescription()
    {
        return "";
    }
}

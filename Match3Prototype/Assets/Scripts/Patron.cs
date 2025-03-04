using UnityEngine;

[System.Serializable] public class Patron
{
    public string title;
    public string effectDescription;
    public bool conditionalEffect;
    public bool constantEffect;
    public int level = 1;
    public int maxLevel = 5;
    public Sprite sprite;

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
        }
    }
}

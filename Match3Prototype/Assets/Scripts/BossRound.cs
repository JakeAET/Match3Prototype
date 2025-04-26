using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public abstract class BossRound : MonoBehaviour
{
    public string title;
    public string description;
    public bool conditionalEffect;
    public bool constantEffect;
    public Color titleTextColor;

    public virtual void activateConstraint()
    {
        // 
    }

    public virtual void deactivateConstraint()
    {
        // 
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PatronTopUI : MonoBehaviour
{
    public Image ptrnSprite;
    public TMP_Text lvlText;
    public GameObject levelDownIcon;
    public GameObject disabledIcon;
    public GameObject removeButtonPanel;
    public GameObject patronSlotContainer;
    public Patron patronRef;

    public void initialize(Patron targetPatron)
    {
        patronRef = targetPatron;
        ptrnSprite.sprite = targetPatron.sprite;
        lvlText.text = "" + targetPatron.level;
        patronSlotContainer.SetActive(true);
    }

    public void reassign(Patron targetPatron)
    {
        patronRef = targetPatron;
        ptrnSprite.sprite = targetPatron.sprite;
        lvlText.text = "" + targetPatron.level;
    }

    public void clear()
    {
        patronRef = null;
        ptrnSprite.sprite = null;
        lvlText.text = "";
        patronSlotContainer.SetActive(false);
    }
}

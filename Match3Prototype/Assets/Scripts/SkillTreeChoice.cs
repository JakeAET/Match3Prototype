using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeChoice : MonoBehaviour
{
    private SkillTreeTierUI tierUIRef;
    public Ability ability;
    public Toggle toggle;
    [SerializeField] Image icon;
    [SerializeField] Image bg;
    [SerializeField] Image selectedHighlight;
    [SerializeField] Outline iconOutline;
    [SerializeField] Color disabledIconColor;

    public void initialize(SkillTreeTierUI tierUI, Ability targetAbility, bool active, bool chosen, ToggleGroup toggleGroup)
    {
        tierUIRef = tierUI;
        ability = targetAbility;
        toggle.group = toggleGroup;

        //assign abiltiy icon

        if (active)
        {
            toggle.interactable = true;
        }
        else
        {
            if (chosen)
            {
                toggle.isOn = true;
                toggle.interactable = true;
                toggle.enabled = false;

                Color col = bg.color;
                col.a = 0.5f;
                bg.color = col;

                col = selectedHighlight.color;
                col.a = 0.5f;
                selectedHighlight.color = col;
            }
            else
            {
                toggle.interactable = false;
                icon.color = disabledIconColor;
                iconOutline.effectColor = disabledIconColor;
            }
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    public void skillToggled()
    {
        tierUIRef.toggledSkill(this, toggle.isOn);
    }
}

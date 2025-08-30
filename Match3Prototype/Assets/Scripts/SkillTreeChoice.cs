using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    Color enabledIconOutline;
    Color enabledIconColor;
    [SerializeField] Color chosenIconOutline;
    [SerializeField] Color chosenIconColor;
    [SerializeField] GameObject touchZone;
    [SerializeField] GameObject selectedRing;
    [SerializeField] float enlargeScaleFactor;
    public bool isChosen;
    private Vector3 enlargedScale;
    private RectTransform rect;

    [SerializeField] Sprite disabledSprite;
    [SerializeField] Sprite enabledSprite;
    [SerializeField] Sprite chosenSprite;

    public void initialize(SkillTreeTierUI tierUI, Ability targetAbility, bool active, bool chosen, ToggleGroup toggleGroup)
    {
        tierUIRef = tierUI;
        ability = targetAbility;
        toggle.group = toggleGroup;
        isChosen = true;

        enabledIconColor = icon.color;
        enabledIconOutline = iconOutline.effectColor;

        enlargedScale = new Vector3(enlargeScaleFactor, enlargeScaleFactor, enlargeScaleFactor);
        rect = GetComponent<RectTransform>();

        //assign abiltiy icon

        if (active)
        {
            toggle.interactable = true;
            selectedHighlight.sprite = enabledSprite;
        }
        else
        {
            if (chosen)
            {
                toggle.isOn = true;
                toggle.interactable = false;
                //toggle.enabled = false;

                //Color col = bg.color;
                //col.a = 0.5f;
                //bg.color = col;
                selectedHighlight.sprite = chosenSprite;

                Color col = selectedHighlight.color;
                col.a = 0.5f;
                selectedHighlight.color = col;

                icon.color = chosenIconColor;
                iconOutline.effectColor = chosenIconOutline;
            }
            else
            {
                selectedHighlight.sprite = disabledSprite;
                toggle.interactable = false;
                icon.color = disabledIconColor;
                iconOutline.effectColor = disabledIconColor;
            }
        }

        gameObject.name = ability.name + " choice";
    }

    // Start is called before the first frame update
    void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement(tierUIRef.skillTreeRef.confirmSkillButton))
        {
            if (IsPointerOverUIElement(touchZone))
            {
                if (!toggle.interactable)
                {
                    //Debug.Log("touching " + ability.name);
                    selectedRing.SetActive(true);
                    tierUIRef.skillTreeRef.clickedOnChoice(ability);
                    rect.DOScale(enlargedScale, 0.3f);
                }
            }
            else
            {
                if (!toggle.interactable)
                {
                    selectedRing.SetActive(false);
                    tierUIRef.skillTreeRef.clickedOffChoice(ability);
                    rect.DOScale(Vector3.one, 0.3f);
                }
                else
                {
                    rect.DOScale(Vector3.one, 0.3f);
                }

            }
        }
}

    public void skillToggled()
    {
        tierUIRef.toggledSkill(this, toggle.isOn);
        //selectedRing.SetActive(toggle.isOn);

        if (toggle.isOn)
        {
            rect.DOScale(enlargedScale, 0.3f);
            selectedHighlight.sprite = chosenSprite;
            icon.color = chosenIconColor;
            iconOutline.effectColor = chosenIconOutline;
        }
        else
        {
            rect.DOScale(Vector3.one, 0.3f);
            selectedHighlight.sprite = enabledSprite;
            icon.color = enabledIconColor;
            iconOutline.effectColor = enabledIconOutline;
        }
    }

    public bool IsPointerOverUIElement(GameObject target)
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults(), target);
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults, GameObject target)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject == target)
                return true;
        }
        return false;
    }

    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}

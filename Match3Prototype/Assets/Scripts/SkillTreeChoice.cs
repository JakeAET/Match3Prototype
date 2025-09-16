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
    public bool toggledOn;
    public bool interactable;
    public bool selected;
    [SerializeField] SpriteRenderer icon;
    [SerializeField] SpriteRenderer bg;
    [SerializeField] SpriteRenderer selectedHighlight;
    [SerializeField] SpriteRenderer iconOutline;
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

    private CircleCollider2D col;

    public void initialize(SkillTreeTierUI tierUI, Ability targetAbility, bool active, bool chosen)
    {
        tierUIRef = tierUI;
        ability = targetAbility;
        //toggle.group = toggleGroup;
        isChosen = true;

        enabledIconColor = icon.color;
        enabledIconOutline = iconOutline.color;

        enlargedScale = new Vector3(enlargeScaleFactor, enlargeScaleFactor, enlargeScaleFactor);
        rect = GetComponent<RectTransform>();

        //assign abiltiy icon

        if (active)
        {
            interactable = true;
            selectedHighlight.sprite = enabledSprite;
        }
        else
        {
            if (chosen)
            {
                toggledOn = true;
                interactable = false;
                //toggle.enabled = false;

                //Color col = bg.color;
                //col.a = 0.5f;
                //bg.color = col;
                selectedHighlight.sprite = chosenSprite;

                Color col = selectedHighlight.color;
                col.a = 0.5f;
                selectedHighlight.color = col;

                icon.color = chosenIconColor;
                iconOutline.color = chosenIconOutline;
            }
            else
            {
                selectedHighlight.sprite = disabledSprite;
                interactable = false;
                icon.color = disabledIconColor;
                iconOutline.color = disabledIconColor;
            }
        }

        gameObject.name = ability.name + " choice";
    }

    // Start is called before the first frame update
    void Awake()
    {
        //toggle = GetComponent<Toggle>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            if (hit.collider != null && hit.collider.transform == transform)
            {
                //Debug.Log("clicked on");
                if (!interactable)
                {
                    if (this == tierUIRef.skillTreeRef.currentHighlightedRef)
                    {
                        selectedRing.SetActive(false);
                        tierUIRef.skillTreeRef.clickedOnChoice(ability, this);
                        transform.DOScale(Vector3.one, 0.3f);
                    }
                    else
                    {
                        selectedRing.SetActive(true);
                        tierUIRef.skillTreeRef.clickedOnChoice(ability, this);
                        transform.DOScale(enlargedScale, 0.3f);
                    }
                    //selected = !selected;
                    //Debug.Log("touching " + ability.name);
                }
                else
                {
                    toggledOn = !toggledOn;
                    tierUIRef.skillTreeRef.clickedOnChoice(ability, this);
                    skillToggled(toggledOn);
                }
            }
            else
            {
                selectedRing.SetActive(false);
                //Debug.Log("clicked off");
                if (!interactable)
                {
                    //tierUIRef.skillTreeRef.clickedOffChoice(ability, gameObject);
                    transform.DOScale(Vector3.one, 0.3f);
                }
                else
                {
                    //tierUIRef.skillTreeRef.clickedOffChoice(ability, gameObject);
                    transform.DOScale(Vector3.one, 0.3f);
                }

            }
        }

        //if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement(tierUIRef.skillTreeRef.confirmSkillButton))
        //{
        //    if (IsPointerOverUIElement(touchZone))
        //    {
        //        if (!toggle.interactable)
        //        {
        //            //Debug.Log("touching " + ability.name);
        //            selectedRing.SetActive(true);
        //            tierUIRef.skillTreeRef.clickedOnChoice(ability);
        //            rect.DOScale(enlargedScale, 0.3f);
        //        }
        //    }
        //    else
        //    {
        //        if (!toggle.interactable)
        //        {
        //            selectedRing.SetActive(false);
        //            tierUIRef.skillTreeRef.clickedOffChoice(ability);
        //            rect.DOScale(Vector3.one, 0.3f);
        //        }
        //        else
        //        {
        //            rect.DOScale(Vector3.one, 0.3f);
        //        }

        //    }
        //}
    }

    public void skillToggled(bool isToggledOn)
    {
        toggledOn = isToggledOn;

        tierUIRef.toggledSkill(this, toggledOn);
        selectedRing.SetActive(toggledOn);

        if (toggledOn)
        {
            transform.DOScale(enlargedScale, 0.3f);
            selectedHighlight.sprite = chosenSprite;
            icon.color = chosenIconColor;
            //iconOutline.effectColor = chosenIconOutline;
        }
        else
        {
            transform.DOScale(Vector3.one, 0.3f);
            selectedHighlight.sprite = enabledSprite;
            icon.color = enabledIconColor;
            //iconOutline.effectColor = enabledIconOutline;
        }
    }

    public void unselect()
    {

    }

    public void toggleOff()
    {
        transform.DOScale(Vector3.one, 0.3f);
        selectedHighlight.sprite = enabledSprite;
        icon.color = enabledIconColor;
        selectedRing.SetActive(false);
    }

    //public bool IsPointerOverUIElement(GameObject target)
    //{
    //    return IsPointerOverUIElement(GetEventSystemRaycastResults(), target);
    //}


    //Returns 'true' if we touched or hovering on Unity UI element.
    //private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults, GameObject target)
    //{
    //    for (int index = 0; index < eventSystemRaysastResults.Count; index++)
    //    {
    //        RaycastResult curRaysastResult = eventSystemRaysastResults[index];
    //        if (curRaysastResult.gameObject == target)
    //            return true;
    //    }
    //    return false;
    //}

    //static List<RaycastResult> GetEventSystemRaycastResults()
    //{
    //    PointerEventData eventData = new PointerEventData(EventSystem.current);
    //    eventData.position = Input.mousePosition;
    //    List<RaycastResult> raysastResults = new List<RaycastResult>();
    //    EventSystem.current.RaycastAll(eventData, raysastResults);
    //    return raysastResults;
    //}
}

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
    [SerializeField] GameObject touchZone;
    [SerializeField] GameObject selectedRing;
    [SerializeField] float enlargeScaleFactor;
    public bool isChosen;
    private Vector3 enlargedScale;
    private RectTransform rect;

    [SerializeField] SpriteRenderer sr;
    Sprite dormantSprite;
    Sprite selectableSprite;
    Sprite selectedSprite;
    Sprite chosenSprite;

    //private CircleCollider2D col;

    public void initialize(SkillTreeTierUI tierUI, Ability targetAbility, bool active, bool chosen)
    {
        tierUIRef = tierUI;
        ability = targetAbility;
        //toggle.group = toggleGroup;
        isChosen = true;

        enlargedScale = new Vector3(enlargeScaleFactor, enlargeScaleFactor, enlargeScaleFactor);
        rect = GetComponent<RectTransform>();

        dormantSprite = targetAbility.dormantSprite;
        selectableSprite = targetAbility.selectableSprite;
        selectedSprite = targetAbility.selectedSprite;
        chosenSprite = targetAbility.chosenSprite;

        //assign abiltiy icon

        if (active)
        {
            interactable = true;
            sr.sprite = selectableSprite;
        }
        else
        {
            if (chosen)
            {
                toggledOn = true;
                interactable = false;
                sr.sprite = chosenSprite;
            }
            else
            {
                interactable = false;
                sr.sprite = dormantSprite;
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
    }

    public void skillToggled(bool isToggledOn)
    {
        toggledOn = isToggledOn;

        tierUIRef.toggledSkill(this, toggledOn);
        selectedRing.SetActive(toggledOn);

        if (toggledOn)
        {
            transform.DOScale(enlargedScale, 0.3f);
            sr.sprite = selectedSprite;
        }
        else
        {
            transform.DOScale(Vector3.one, 0.3f);
            sr.sprite = selectableSprite;
        }
    }

    public void unselect()
    {

    }

    public void toggleOff()
    {
        transform.DOScale(Vector3.one, 0.3f);
        sr.sprite = selectableSprite;
        selectedRing.SetActive(false);
    }
}

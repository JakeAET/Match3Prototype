using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PatronTopUI : MonoBehaviour
{
    public Image ptrnSprite;
    public GameObject ptrnSpriteObj;
    public TMP_Text lvlText;
    //public GameObject levelDownIcon;
    public GameObject disabledIcon;
    public GameObject removeButtonPanel;
    public GameObject patronSlotContainer;
    public Toggle toggle;
    public Patron patronRef;
    public float largeScaleFactor;
    private Vector3 largeScale;
    private UIManager ui;

    public void initialize(Patron targetPatron)
    {
        ui = FindObjectOfType<UIManager>();
        patronRef = targetPatron;
        ptrnSprite.sprite = targetPatron.sprite;
        //lvlText.text = "" + targetPatron.level;
        patronSlotContainer.SetActive(true);
        largeScale = new Vector3(largeScaleFactor, largeScaleFactor, largeScaleFactor);
    }

    public void reassign(Patron targetPatron)
    {
        patronRef = targetPatron;
        ptrnSprite.sprite = targetPatron.sprite;
        //lvlText.text = "" + targetPatron.level;
    }
    void Update()
    {
        if (patronSlotContainer.activeInHierarchy && toggle.isOn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("mouse touch");
                if (!IsPointerOverUIElement(ptrnSpriteObj))
                {
                    toggle.isOn = false;
                    topUIPatronToggle();
                }
            }

            if (Input.touchCount > 0)
            {
                //Debug.Log("finger touch");
                if (!IsPointerOverUIElement(ptrnSpriteObj))
                {
                    toggle.isOn = false;
                    topUIPatronToggle();
                }
            }
        }
    }


    public void clear()
    {
        patronRef = null;
        ptrnSprite.sprite = null;
        //lvlText.text = "";
        patronSlotContainer.SetActive(false);
    }

    public void topUIPatronToggle()
    {
        if (toggle.isOn)
        {
            FindObjectOfType<AudioManager>().Play("ui pop");
            ptrnSpriteObj.GetComponent<RectTransform>().DOScale(largeScale, 0.1f);
            ptrnSpriteObj.GetComponent<Outline>().effectColor = Color.blue;
        }
        else
        {
            ptrnSpriteObj.GetComponent<RectTransform>().DOScale(Vector3.one, 0.1f);
            Color newColor = Color.black;
            newColor.a = 0.5f;
            ptrnSpriteObj.GetComponent<Outline>().effectColor = newColor;
        }

        if (!IsPointerOverUIElement(ui.removeButtonObj))
        {
            if (toggle.isOn)
            {
                ui.patronInfoPanelShow(patronRef);
                // enable info screen
            }
            else
            {
                ui.patronInfoPanelHide();
                // disable info screen
            }
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

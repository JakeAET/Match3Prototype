using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using DG.Tweening;

public class patronChoiceUI : MonoBehaviour
{
    [SerializeField] float tweenSpeed;

    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite selectedSprite;
    [SerializeField] Sprite disabledSprite;
    [SerializeField] Image backgroundImg;

    [SerializeField] TMP_Text titleTxt;
    [SerializeField] TMP_Text levelUpTxt;
    [SerializeField] TMP_Text descriptionTxt;

    //[SerializeField] Image patronImg;
    [SerializeField] GameObject patronImgObj;
    [SerializeField] GameObject disabledButtonCover;
    [SerializeField] GameObject disabledPatronCover;

    [SerializeField] GameObject backgroundObj;
    //[SerializeField] Color offColor;
    //[SerializeField] Color onColor;

    private Vector3 startPtrnScaleSize;
    private Vector3 startPtrnPos;

    [SerializeField] float endPtrnScaleFactor;
    [SerializeField] float endPtrnYFactor;

    [SerializeField] GameObject fullMask;
    [SerializeField] GameObject coveredMask;
    //[SerializeField] Image backgroundImg;
    public Patron patronRef;
    //public Ability selectedAbility;
    public Toggle patronToggle;
    private UIManager ui;
    public int patronChoiceListIndex;

    public Ability currentSelectedAbility;

    public void initialize(Patron patron)
    {
        DOTween.Init();
        //Color darkenColor = new Color(patronRef.color.r * 0.3f, patronRef.color.g * 0.3f, patronRef.color.b * 0.3f);
        //startScaleSize = patronImgObj.transform.localScale;
        //patronImg.sprite = patron.sprite;
        //backgroundImg.color = darkenColor;
        //titleTxt.color = patronRef.color;

        currentSelectedAbility = null;
        patronRef = patron;
        PatronManager patronManager = FindObjectOfType<PatronManager>();

        for (int i = 0; patronManager.activePatrons.Count > i; i++)
        {
            if (patronRef.title == patronManager.activePatrons[i].title)
            {
                patronRef = patronManager.activePatrons[i];
                break;
            }
        }

        titleTxt.text = patronRef.title;

        if (patronRef.level == 0)
        {
            levelUpTxt.text = "";
            levelUpTxt.gameObject.SetActive(false);
        }
        else
        {
            levelUpTxt.text = "Lvl " + patronRef.level + " -> Lvl " + (patronRef.level + 1);
        }

        //Ability nextAbility = patronRef.existingAbility(patronRef.abilitiesByLevel[patronRef.level]);

        descriptionTxt.text = "";

        //if (nextAbility != null)
        //{
        //    descriptionTxt.text = nextAbility.patronSelectDescription();
        //}
        //else
        //{
        //    descriptionTxt.text = patronRef.abilitiesByLevel[patronRef.level].patronSelectDescription();
        //}

        ui = FindObjectOfType<UIManager>();
        ui.currentChoicePrefabs.Add(gameObject);
        ui.patronUIRefs.Add(this);
        patronChoiceListIndex = ui.patronUIRefs.Count - 1;

        startPtrnPos = patronImgObj.transform.localPosition;
        startPtrnScaleSize = patronImgObj.transform.localScale;
        patronImgObj.transform.SetParent(coveredMask.transform);


        //patronToggle.onValueChanged.AddListener(() => ui.patronBttn(patron));
    }
    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    patronImgObj.GetComponent<RectTransform>().DOScale(largeScaleSize, 0.2f);
    //}
    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    patronImgObj.GetComponent<RectTransform>().DOScale(startScaleSize, 0.2f);
    //}

    public void toggledPatron()
    {
        ui.patronToggle(this, patronToggle.isOn);
        patronToggleEffect();
    }

    public void patronToggleEffect()
    {
        if (patronToggle.isOn)
        {
            Vector3 endPtrnScaleSize = new Vector3(endPtrnScaleFactor + startPtrnScaleSize.x, endPtrnScaleFactor + startPtrnScaleSize.y, endPtrnScaleFactor + startPtrnScaleSize.z);
            Vector3 endtPtrnPos = new Vector3(startPtrnPos.x, startPtrnPos.y + endPtrnYFactor, startPtrnPos.z);
            patronImgObj.GetComponent<RectTransform>().DOScale(endPtrnScaleSize, tweenSpeed);
            patronImgObj.GetComponent<RectTransform>().DOLocalMoveY(endtPtrnPos.y, tweenSpeed);
            // adjust gradient
            //backgroundImg.GetComponent<Image>().color = onColor;

            backgroundImg.sprite = selectedSprite;

            // change outline color
            //Outline outline = backgroundImg.GetComponent<Outline>();
            //Color newColor = outline.effectColor;
            //newColor.a = 1f;
            //outline.effectColor = newColor;
            //outline.effectDistance = new Vector2(8, -8);

            //StartCoroutine(delayedMaskSet(fullMask, patronImgObj, tweenSpeed));
            patronImgObj.transform.SetParent(fullMask.transform);
        }
        else
        {
            descriptionTxt.text = "";

            patronImgObj.GetComponent<RectTransform>().DOScale(startPtrnScaleSize, tweenSpeed);
            patronImgObj.GetComponent<RectTransform>().DOLocalMoveY(startPtrnPos.y, tweenSpeed);
            // adjust gradient
            //backgroundImg.GetComponent<Image>().color = offColor;

            backgroundImg.sprite = normalSprite;

            // change outline color
            //Outline outline = backgroundImg.GetComponent<Outline>();
            //Color newColor = outline.effectColor;
            //newColor.a = 0.3f;
            //outline.effectColor = newColor;
            //outline.effectDistance = new Vector2(3, -3);

            StartCoroutine(delayedMaskSet(coveredMask, patronImgObj, tweenSpeed));
            //patronImgObj.transform.parent = coveredMask.transform;
        }
    }

    private IEnumerator delayedMaskSet(GameObject mask, GameObject patronImg, float delay)
    {
        yield return new WaitForSeconds(delay);
        patronImg.transform.SetParent(mask.transform);
    }

    public void toggleDisable(bool isInteractable)
    {
        if (isInteractable)
        {
            disabledButtonCover.SetActive(false);
            disabledPatronCover.SetActive(false);
            patronToggle.interactable = true;

            if (patronToggle.isOn)
            {
                backgroundImg.sprite = selectedSprite;
            }
            else
            {
                backgroundImg.sprite = normalSprite;
            }
        }
        else
        {
            disabledButtonCover.SetActive(true);
            disabledPatronCover.SetActive(true);
            patronToggle.interactable = false;

            if (patronToggle.isOn)
            {
                patronToggle.isOn = false;
                ui.patronToggle(this, patronToggle.isOn);
            }

            backgroundImg.sprite = disabledSprite;
        }
    }

    public void updateDescription()
    {
        if(currentSelectedAbility != null)
        {
            descriptionTxt.text = currentSelectedAbility.patronSelectDescription();
        }
    }
}

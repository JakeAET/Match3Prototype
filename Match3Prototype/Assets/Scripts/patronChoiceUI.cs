using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using DG.Tweening;

public class patronChoiceUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text description;
    [SerializeField] Image patronImg;
    [SerializeField] GameObject patronImgObj;
    private Vector3 startScaleSize;
    [SerializeField] Vector3 largeScaleSize;
    [SerializeField] Image backgroundImg;
    public Patron patronRef;
    public Toggle patronToggle;
    private UIManager ui;

    public void initialize(Patron patron)
    {
        DOTween.Init();
        Color darkenColor = new Color(patron.color.r * 0.3f, patron.color.g * 0.3f, patron.color.b * 0.3f);
        startScaleSize = patronImgObj.transform.localScale;

        patronRef = patron;
        title.text = patron.title;
        title.color = patron.color;
        description.text = patron.effectDescription;
        patronImg.sprite = patron.sprite;
        backgroundImg.color = darkenColor;
        ui = FindObjectOfType<UIManager>();
        ui.currentChoicePrefabs.Add(gameObject);
        ui.patronUIRefs.Add(this);
        //patronToggle.onValueChanged.AddListener(() => ui.patronBttn(patron));
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        patronImgObj.GetComponent<RectTransform>().DOScale(largeScaleSize, 0.2f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        patronImgObj.GetComponent<RectTransform>().DOScale(startScaleSize, 0.2f);
    }

    public void toggledPatron()
    {
        ui.patronToggle(this, patronToggle.isOn);
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverFlex : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Vector3 flexScaleSize;
    private Vector3 startScaleSize;
    private Button thisButton;

    private void Start()
    {
        DOTween.Init();
        startScaleSize = gameObject.GetComponent<RectTransform>().localScale;
        thisButton = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (thisButton.interactable)
        {
            gameObject.GetComponent<RectTransform>().DOScale(flexScaleSize, 0.2f);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<RectTransform>().DOScale(startScaleSize, 0.2f);
    }
}

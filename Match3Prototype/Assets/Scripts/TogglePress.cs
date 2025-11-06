using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class TogglePress : MonoBehaviour, IPointerEnterHandler
{
    private Toggle thisToggle;
    private bool pointerOver = false;

    public string checkOnSound;
    public string checkOffSound;

    // Start is called before the first frame update
    void Start()
    {
        thisToggle = GetComponent<Toggle>();
        thisToggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    void OnToggleValueChanged(bool isOn)
    {
        if (pointerOver)
        {
            if (isOn)
            {
                if(checkOnSound != "")
                {
                    AudioManager.instance.Play(checkOnSound);
                }
            }
            else
            {
                if (checkOffSound != "")
                {
                    AudioManager.instance.Play(checkOffSound);
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerOver = false;
    }
}

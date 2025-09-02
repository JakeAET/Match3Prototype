using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonPressAnim : MonoBehaviour
{
    private Button button;
    [SerializeField] GameObject[] objectsToMove;
    [SerializeField] float amountMovedDown;

    private void Awake()
    {
        button = GetComponent<Button>();

        var pointerDown = new EventTrigger.Entry();
        var pointerUp = new EventTrigger.Entry();

        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((e) => buttonDown());
        trigger.triggers.Add(pointerDown);

        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((e) => buttonUp());
        trigger.triggers.Add(pointerUp);
    }

    void buttonDown()
    {
        //Debug.Log("pointer down");
        foreach (GameObject obj in objectsToMove)
        {
            Vector3 pos = obj.GetComponent<RectTransform>().position;
            pos.y -= amountMovedDown;
            obj.transform.position = pos;
        }
    }

    void buttonUp()
    {
        //Debug.Log("pointer up");
        foreach (GameObject obj in objectsToMove)
        {
            Vector3 pos = obj.GetComponent<RectTransform>().position;
            pos.y += amountMovedDown;
            obj.transform.position = pos;
        }
    }
}

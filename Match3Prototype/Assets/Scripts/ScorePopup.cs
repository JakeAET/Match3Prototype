using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class ScorePopup : MonoBehaviour
{
    [SerializeField] TMP_Text textRef;
    [SerializeField] float lifeTime;
    [SerializeField] float yIncrease;
    private float timer;
    private bool initialized = false;

    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            timer += Time.deltaTime;
            transform.position = new Vector2(transform.position.x, transform.position.y + (yIncrease * Time.deltaTime));
        }

        if (timer > lifeTime)
        {
            Destroy(gameObject);
        }
    }

    public void initialize(float pointNum, Color color)
    {
        textRef.text = "" + pointNum;
        textRef.color = color;
        initialized = true;
    }
}

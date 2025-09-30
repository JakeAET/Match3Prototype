using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static Coffee.UIExtensions.UIParticle;

public class SkillTreeLine : MonoBehaviour
{
    [SerializeField] float speed;
    private LineRenderer lr;
    private bool initialized;
    private Vector3 lrStartPos;
    private Vector3 lrEndPos;
    private float journeyLength;
    private float startTime;


    public void initialize(Vector3 startPos, Vector3 endPos, Color startColor, Color endColor)
    {
        lr = GetComponent<LineRenderer>();

        lr.startColor = startColor;
        lr.endColor = endColor;

        lrStartPos = startPos;
        lrEndPos = endPos;

        lr.SetPosition(0, startPos);

        startTime = Time.time;

        journeyLength = Vector3.Distance(startPos, endPos);

        lr.widthMultiplier = 0;
        DOTween.To(() => lr.widthMultiplier, x => lr.widthMultiplier = x, 0.6f, 0.4f);

        initialized = true;
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            if(lr.GetPosition(1) != lrEndPos)
            {
                float distCovered = (Time.time - startTime) * speed;
                float fractionOfJourney = distCovered / journeyLength;
                lr.SetPosition(1, Vector3.Lerp(lrStartPos, lrEndPos, fractionOfJourney));
            }
        }
    }
}

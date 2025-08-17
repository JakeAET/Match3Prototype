using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.GraphicsBuffer;

public class PointParticle : MonoBehaviour
{
    [SerializeField] TargetColor targetColor;
    [SerializeField] Sprite[] weaponSprites;
    private SpriteRenderer sr;
    [SerializeField] SpriteRenderer glowSR;

    public Vector3 targetPos = Vector3.zero;
    public float fracComplete;

    private Vector3 startPosition;
    //public AnimationCurve movementCurve;
    public float duration = 2f;

    private float startTime;
    private bool isMoving = true;

    private float randOffset;

    [SerializeField] bool isSpinning = false;
    [SerializeField] bool pointAtTarget = false;

    [SerializeField] float spinSpeed;

    public Dictionary<TargetColor, int> targetColorDict = new Dictionary<TargetColor, int>
    {
        {TargetColor.Red, 0},
        {TargetColor.Blue, 1},
        {TargetColor.Green, 2},
        {TargetColor.Purple, 3},
        {TargetColor.Yellow, 4},
    };

    private void Awake()
    {
        //GameObject target = GameObject.FindGameObjectWithTag("point particle target");

        //if(target != null)
        //{
        //    Camera cam = Camera.main;
        //    //targetPos = cam.ScreenToWorldPoint(cam.WorldToScreenPoint(target.GetComponent<RectTransform>().position));
        //    targetPos = target.GetComponent<RectTransform>().position;
        //    targetPos.z = 0;
        //}

        startTime = Time.time;
        startPosition = gameObject.transform.position;

        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.sprite = weaponSprites[targetColorDict[targetColor]];
        glowSR.sprite = weaponSprites[targetColorDict[targetColor]];
    }

    // Start is called before the first frame update
    void Start()
    {
        randOffset = Random.Range(-0.03f, 0.03f);
    }

    // Update is called once per frame
    void Update()
    {
        GameObject target = GameObject.FindGameObjectWithTag("point particle target");

        Vector3 pos = target.GetComponent<RectTransform>().position;

        if (System.Single.IsNaN(pos.x) || System.Single.IsNaN(pos.y) || System.Single.IsNaN(pos.y))
        {
            isMoving = false;
        }

        if (isMoving)
        {

            if (target != null)
            {
                Camera cam = Camera.main;
                //targetPos = cam.ScreenToWorldPoint(cam.WorldToScreenPoint(target.GetComponent<RectTransform>().position));
                targetPos = target.GetComponent<RectTransform>().position;
                targetPos.z = 0;
            }

            //float t = Mathf.Clamp01((Time.time - startTime) / duration);
            //float curveValue = movementCurve.Evaluate(t);
            //transform.position = Vector3.Lerp(startPosition, targetPos, curveValue);

            //if (t >= 1f)
            //{
            //    isMoving = false;
            //}

            Vector3 center = (startPosition + targetPos) * 0.5F;

            //float randOffset = Random.Range(-0.01f, 0.01f);

            center -= new Vector3(randOffset, 0, 0);

            Vector3 riseRelCenter = startPosition - center;
            Vector3 setRelCenter = targetPos - center;

            fracComplete = (Time.time - startTime) / duration;

            transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
            transform.position += center;

            if (fracComplete >= 1f)
            {
                isMoving = false;
            }

            if (isSpinning)
            {
                transform.Rotate(0, 0, spinSpeed, Space.Self);
            }

            if (pointAtTarget)
            {
                //transform.LookAt(targetPos);
                Vector3 lookPos = target.transform.position - transform.position;
                lookPos.z = 0;
                transform.up = lookPos;
            }
        }
        else
        {
            //Debug.Log("particle destroyed");

            Destroy(gameObject);
        }
    }
}

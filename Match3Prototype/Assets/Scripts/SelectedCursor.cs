using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class SelectedCursor : MonoBehaviour
{
    [SerializeField] GameObject pivot;
    [SerializeField] TrailRenderer[] trs;
    //public List<Color> trStartColors = new List<Color>();
    //public List<Color> trEndColors = new List<Color>();
    private List<float> trailLengths = new List<float>();
    [SerializeField] SpriteRenderer[] sprites;
    public List<Color> spriteColors = new List<Color>();

    [SerializeField] float rotationSpeed;
    private Vector3 startPos;
    private Quaternion startRotation;
    bool initialized;
    public bool isOn = false;

    private void Awake()
    {
        startPos = transform.position;
        startRotation = transform.rotation;
        initialized = true;

        foreach (TrailRenderer tr in trs)
        {
            //trStartColors.Add(tr.startColor);
            //trEndColors.Add(tr.endColor);
            trailLengths.Add(tr.time);
        }

        foreach (SpriteRenderer sr in sprites)
        {
            spriteColors.Add(sr.color);
        }

        for (int i = 0; i < trs.Length; i++)
        {
            //Color col = trs[i].startColor;
            //col.a = 0;
            //trs[i].startColor = col;

            //col = trs[i].endColor;
            //col.a = 0;
            //trs[i].endColor = col;

            trs[i].time = 0;

            trs[i].emitting = false;
        }

        for (int i = 0; i < sprites.Length; i++)
        {
            Color col = sprites[i].color;
            col.a = 0;
            sprites[i].color = col;
        }
    }

    //private void OnEnable()
    //{
    //    if (initialized)
    //    {
    //        //transform.position = startPos;
    //        //transform.rotation = startRotation;
    //        //tr.emitting = true;

    //        for (int i = 0; i < trs.Length; i++)
    //        {
    //            DOTween.To(() => trs[i].startColor, x => trs[i].startColor = x, trStartColors[i], 0.3f);
    //            DOTween.To(() => trs[i].endColor, x => trs[i].endColor = x, trEndColors[i], 0.3f);
    //        }

    //        for (int i = 0; i < sprites.Length; i++)
    //        {
    //            DOTween.To(() => sprites[i].color, x => sprites[i].color = x, spriteColors[i], 0.3f);
    //        }
    //    }
    //}

    //private void OnDisable()
    //{
    //    if (initialized)
    //    {
    //        //transform.position = startPos;
    //        //transform.rotation = startRotation;
    //        //tr.emitting = false;

    //        for (int i = 0; i < trs.Length; i++)
    //        {
    //            Color col = trs[i].startColor;
    //            col.a = 0;
    //            DOTween.To(() => trs[i].startColor, x => trs[i].startColor = x, col, 0.3f);

    //            col = trs[i].endColor;
    //            col.a = 0;
    //            DOTween.To(() => trs[i].endColor, x => trs[i].endColor = x, col, 0.3f);
    //        }

    //        for (int i = 0; i < sprites.Length; i++)
    //        {
    //            Color col = sprites[i].color;
    //            col.a = 0;
    //            DOTween.To(() => sprites[i].color, x => sprites[i].color = x, col, 0.3f);
    //        }
    //    }
    //}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(pivot.transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    public void toggleEffect(bool toggleOn)
    {
        if (initialized)
        {
            StopAllCoroutines();
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].DOKill();
            }

            if (toggleOn)
            {
                isOn = true;
                StartCoroutine(toggleEffectEnum(toggleOn));
            }
            else
            {
                isOn = false;
                StartCoroutine(toggleEffectEnum(toggleOn));
            }
        }
    }

    IEnumerator toggleEffectEnum(bool toggleOn)
    {
        if (toggleOn)
        {
            for (int i = 0; i < trs.Length; i++)
            {
                trs[i].time = trailLengths[i];
            }

            for (int i = 0; i < trs.Length; i++)
            {
                trs[i].emitting = true;
            }

            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].DOColor(spriteColors[i], 0.3f);
            }
        }
        else
        {
            for (int i = 0; i < trs.Length; i++)
            {
                //DOTween.To(() => trs[i].time, x => trs[i].time = x, 0, 0.3f);
                trs[i].time = 0.2f;
            }

            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].DOFade(0f, 0.3f);
            }

            yield return new WaitForSeconds(0.3f);

            for (int i = 0; i < trs.Length; i++)
            {
                trs[i].emitting = false;
            }
        }
    }
}

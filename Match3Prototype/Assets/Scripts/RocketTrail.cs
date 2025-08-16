using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTrail : MonoBehaviour
{
    [SerializeField] float lifeTime;
    [SerializeField] SpriteRenderer sprite;
    private float timer;

    private void Start()
    {
        StartCoroutine(RocketTrailEffect());
    }

    //void Update()
    //{
    //    timer += Time.deltaTime;

    //    if (timer > lifeTime)
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    IEnumerator RocketTrailEffect()
    {
        sprite.DOFade(0.8f, lifeTime / 2);

        yield return new WaitForSeconds(lifeTime/2);

        sprite.DOFade(0, lifeTime / 2);
        gameObject.transform.DOScaleX(0, lifeTime / 2);

        yield return new WaitForSeconds(lifeTime / 2);

        Destroy(gameObject);
    }
}

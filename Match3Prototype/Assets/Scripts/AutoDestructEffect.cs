using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestructEffect : MonoBehaviour
{
    [SerializeField] float lifeTime;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > lifeTime)
        {
            Destroy(gameObject);
        }
    }
}

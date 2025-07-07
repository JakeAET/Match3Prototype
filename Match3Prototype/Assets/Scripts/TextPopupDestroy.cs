using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPopupDestroy : MonoBehaviour
{
    [SerializeField] Animator animator;

    void Start()
    {
        Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

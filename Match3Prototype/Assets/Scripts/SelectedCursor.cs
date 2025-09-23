using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectedCursor : MonoBehaviour
{
    [SerializeField] GameObject pivot;
    [SerializeField] TrailRenderer tr;
    [SerializeField] float rotationSpeed;
    private Vector3 startPos;
    private Quaternion startRotation;
    bool initialized;

    private void Awake()
    {
        startPos = transform.position;
        startRotation = transform.rotation;
        initialized = true;
    }

    private void OnEnable()
    {
        if (initialized)
        {
            //transform.position = startPos;
            //transform.rotation = startRotation;
            //tr.emitting = true;
        }
    }

    private void OnDisable()
    {
        if (initialized)
        {
            //transform.position = startPos;
            //transform.rotation = startRotation;
            //tr.emitting = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(pivot.transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}

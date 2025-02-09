using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Tile : MonoBehaviour
{
    [SerializeField] GameObject[] tileElements;
    [SerializeField] float elementSpawnHeight;
    [SerializeField] float fallSpeed;
    private GameObject currentElement;
    private float step;
    public float spawnDelay = 0;

    private bool falling = false;


    // Start is called before the first frame update
    void Start()
    {
        //Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (falling)
        {
            step = fallSpeed * Time.deltaTime;
            currentElement.transform.position = Vector2.MoveTowards(currentElement.transform.position, transform.position, step);
            if(currentElement.transform.position == transform.position)
            {
                falling = false;
            }
        }
    }

    public void Initialize()
    {
        int elementToUse = Random.Range(0, tileElements.Length);
        Vector2 tempPos = new Vector2(transform.position.x, elementSpawnHeight);
        GameObject element = Instantiate(tileElements[elementToUse], tempPos, Quaternion.identity);
        element.transform.parent = transform;
        element.name = gameObject.name;
        currentElement = element;
        StartCoroutine(elementDropIn());
    }

    IEnumerator elementDropIn()
    {
        yield return new WaitForSeconds(spawnDelay);

        falling = true;
    }
}

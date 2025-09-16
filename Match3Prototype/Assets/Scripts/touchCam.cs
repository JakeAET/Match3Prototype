using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class touchCam : MonoBehaviour
{
    public static touchCam Instance;

    //public string currentPresetName;

    private Vector3 touchStart;
    [SerializeField] float zoomOutMin;
    [SerializeField] float zoomOutMax;
    [SerializeField] float zoomSensitivity;

    [SerializeField] Camera cam;
    [SerializeField] float groundZ = 0;

    [SerializeField] SpriteRenderer bounds;
    private float bgMinX;
    private float bgMinY;
    private float bgMaxX;
    private float bgMaxY;

    //dragging
    private GameObject dragManager;
    private bool dragActive;

    // zoom lock
    //[SerializeField] zoomPreset[] zoomPresets;
    private bool camLocked = true;
    //public bool presetZooming = false;
    //private float zoomTimer = 0;
    //private zoomPreset currentPreset;
    //private GameObject followTarget;

    private float startZoom;
    public Vector3 defaultBoundsPos;
    private Vector3 defaultCamPos;

    //private uiManager uiMang;

    private void Awake()
    {
        void Awake()
        {
            if (Instance != null && Instance != this)
            {

                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        bgMinX = bounds.transform.position.x - bounds.bounds.size.x / 2f;
        bgMaxX = bounds.transform.position.x + bounds.bounds.size.x / 2f;

        bgMinY = bounds.transform.position.y - bounds.bounds.size.y / 2f;
        bgMaxY = bounds.transform.position.y + bounds.bounds.size.y / 2f;

        //zoomPresets[0].endZoom = cam.orthographicSize;
        //zoomPresets[0].endPosition = cam.transform.position;
        //zoomPresets[0].lockCam = false;
    }

    private void Start()
    {
        //dragManager = GameObject.FindGameObjectWithTag("dragManager");
        //uiMang = GameObject.FindGameObjectWithTag("uiManager").GetComponent<uiManager>();
        startZoom = Camera.main.orthographicSize;
        defaultBoundsPos = bounds.transform.position;
        defaultCamPos = Camera.main.transform.position;
    }

    void Update()
    {
        //if (Input.GetKeyDown("d"))
        //{
        //    zoomLockCam("default");
        //}

        if (!camLocked)
        {
            edgeScroll();

            if (Input.GetMouseButtonDown(0))
            {
                touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            // camera panning
            if (Input.GetMouseButton(0))
            {
                //dragActive = dragManager.GetComponent<Dragging>().isDragging;

                if (dragActive == false && !TouchOnClicker())
                {
                    Vector3 direction = touchStart - GetWorldPosition(0);
                    cam.transform.position = ClampCamera(cam.transform.position + direction);
                    //Camera.main.transform.position += direction;
                }
            }

            //camera zoom
            if (Input.touchCount == 2 && !TouchOnClicker())
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                zoom(difference * zoomSensitivity);
            }
            else if (Input.GetMouseButton(0))
            {
                //dragActive = dragManager.GetComponent<Dragging>().isDragging;

                if (dragActive == false && !TouchOnClicker())
                {
                    Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Camera.main.transform.position += direction;
                }
            }
            zoom(Input.GetAxis("Mouse ScrollWheel"));
        }
    }

    private void zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
        cam.transform.position = ClampCamera(cam.transform.position);
    }

    private Vector3 GetWorldPosition(float z)
    {
        Ray mousePos = cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        ground.Raycast(mousePos, out distance);
        return mousePos.GetPoint(distance);
    }

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float minX = bgMinX + camWidth;
        float maxX = bgMaxX - camWidth;
        float minY = bgMinY + camHeight;
        float maxY = bgMaxY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, -10);
    }

    private void edgeScroll()
    {
        float moveAmount = 30f;
        float edgeSize = 50f;

        //dragActive = dragManager.GetComponent<Dragging>().isDragging;

        if (dragActive)
        {
            if (Input.mousePosition.x > Screen.width - edgeSize)
            {
                cam.transform.position = ClampCamera(cam.transform.position + new Vector3(moveAmount * Time.deltaTime, 0f, 0f));
            }
            if (Input.mousePosition.x < edgeSize)
            {
                cam.transform.position = ClampCamera(cam.transform.position - new Vector3(moveAmount * Time.deltaTime, 0f, 0f));
            }
            if (Input.mousePosition.y > Screen.height - edgeSize)
            {
                cam.transform.position = ClampCamera(cam.transform.position + new Vector3(0f, moveAmount * Time.deltaTime, 0f));
            }
            if (Input.mousePosition.y < edgeSize)
            {
                cam.transform.position = ClampCamera(cam.transform.position - new Vector3(0f, moveAmount * Time.deltaTime, 0f));
            }
        }
    }

    public bool TouchOnClicker()//if touch phase ends on this button 
    {                               //than the function will return true .
        if (Input.touchCount > 0)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            if (results.Count > 0)
            {
                if (results[0].gameObject.name.Equals("Clicker"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void lockCam()
    {
        camLocked = true;
        Camera.main.orthographicSize = startZoom;
        Camera.main.transform.position = defaultCamPos;
    }

    public void unlockCam()
    {
        camLocked = false;
    }

    public void setBoundsCenter(Vector3 pos)
    {
        bounds.transform.position = pos;

        bgMinX = bounds.transform.position.x - bounds.bounds.size.x / 2f;
        bgMaxX = bounds.transform.position.x + bounds.bounds.size.x / 2f;

        bgMinY = bounds.transform.position.y - bounds.bounds.size.y / 2f;
        bgMaxY = bounds.transform.position.y + bounds.bounds.size.y / 2f;
    }
}

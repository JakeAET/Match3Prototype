using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PatronTopUI : MonoBehaviour
{
    public Image ptrnSprite;
    public GameObject ptrnSpriteObj;
    public TMP_Text lvlText;
    //public GameObject levelDownIcon;
    public GameObject disabledIcon;
    public GameObject removeButtonPanel;
    public GameObject patronSlotContainer;
    public Toggle toggle;
    public Patron patronRef;
    public float largeScaleFactor;
    private Vector3 largeScale;
    private UIManager ui;
    public GameObject bg;
    public LayoutElement layoutElement;
    public GameObject patronMask;
    private bool patronEffectActive = false;

    [SerializeField] GameObject procVFX;
    [SerializeField] ParticleSystem streakParticles;
    //[SerializeField] ParticleSystem leftLightningParticles;
    //[SerializeField] ParticleSystem rightLightningParticles;
    [SerializeField] ParticleSystem starsParticles;
    [SerializeField] Image leftLightningImg;
    [SerializeField] Image rightLightningImg;
    [SerializeField] Animator leftLightningAnim;
    [SerializeField] Animator rightLightningAnim;
    [SerializeField] Outline leftLightningOutline;
    [SerializeField] Outline rightLightningOutline;
    [SerializeField] Canvas canvas;

    private void Awake()
    {
        streakParticles.Stop();
        starsParticles.Stop();
    }

    public void initialize(Patron targetPatron)
    {
        ui = FindObjectOfType<UIManager>();
        patronRef = targetPatron;
        ptrnSprite.sprite = targetPatron.sprite;
        //lvlText.text = "" + targetPatron.level;
        patronSlotContainer.SetActive(true);
        largeScale = new Vector3(largeScaleFactor, largeScaleFactor, largeScaleFactor);
        bg.SetActive(true);
        bg.GetComponent<Image>().color = targetPatron.color;
    }

    public void reassign(Patron targetPatron)
    {
        patronRef = targetPatron;
        ptrnSprite.sprite = targetPatron.sprite;
        //lvlText.text = "" + targetPatron.level;
    }
    void Update()
    {
        if (patronSlotContainer.activeInHierarchy && toggle.isOn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("mouse touch");
                if (!IsPointerOverUIElement(ptrnSpriteObj))
                {
                    toggle.isOn = false;
                    topUIPatronToggle();
                }
            }

            if (Input.touchCount > 0)
            {
                //Debug.Log("finger touch");
                if (!IsPointerOverUIElement(ptrnSpriteObj))
                {
                    toggle.isOn = false;
                    topUIPatronToggle();
                }
            }
        }

        if (patronRef != null)
        {
            if (Input.GetKeyDown("" + patronRef.index))
            {
                patronEffectTriggered(1f, 0.2f);
            }
        }
    }


    public void clear()
    {
        patronRef = null;
        ptrnSprite.sprite = null;
        //lvlText.text = "";
        patronSlotContainer.SetActive(false);
        bg.SetActive(false);
    }

    public void topUIPatronToggle()
    {
        if (toggle.isOn)
        {
            FindObjectOfType<AudioManager>().Play("ui pop");
            ptrnSpriteObj.GetComponent<RectTransform>().DOScale(largeScale, 0.1f);
            ptrnSpriteObj.GetComponent<Outline>().effectColor = Color.blue;
        }
        else
        {
            ptrnSpriteObj.GetComponent<RectTransform>().DOScale(Vector3.one, 0.1f);
            Color newColor = Color.black;
            newColor.a = 0f;
            ptrnSpriteObj.GetComponent<Outline>().effectColor = newColor;
        }

        if (!IsPointerOverUIElement(ui.removeButtonObj))
        {
            if (toggle.isOn)
            {
                ui.patronInfoPanelShow(patronRef);
                // enable info screen

                //patronEffectTriggered(1f, 0.5f);
            }
            else
            {
                ui.patronInfoPanelHide();
                // disable info screen
            }
        }
    }

    public bool IsPointerOverUIElement(GameObject target)
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults(), target);
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults, GameObject target)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject == target)
                return true;
        }
        return false;
    }

    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    public void patronEffectTriggered(float effectDuration, float tweenDuration)
    {
        if (!patronEffectActive)
        {
            patronEffectActive = true;
            StartCoroutine(patronEffect(effectDuration, tweenDuration));
        }
    }

    IEnumerator patronEffect(float effectDuration, float tweenDuration)
    {
        Color startColor = patronRef.color;
        float transparency = 0.2f;

        float lightenFactor = 2f;
        Color lighterColor = new Color(startColor.r * lightenFactor, startColor.g * lightenFactor, startColor.b * lightenFactor, transparency);

        float darkenFactor = 0.8f;
        Color darkerColor = new Color(startColor.r * darkenFactor, startColor.g * darkenFactor, startColor.b * darkenFactor, transparency);

        var streaksMain = streakParticles.main;
        streaksMain.startColor = new ParticleSystem.MinMaxGradient(lighterColor, darkerColor); ;

        float startSize = layoutElement.preferredWidth;
        float endSize = layoutElement.preferredWidth * 1.5f;

        RectTransform rect = patronMask.GetComponent<RectTransform>();
        RectTransform rectBG = bg.GetComponent<RectTransform>();

        Vector2 startScale = rect.localScale;
        Vector2 endScale = new Vector2(startScale.x * 1.1f, startScale.y);

        Vector2 startScaleBG = rectBG.localScale;
        Vector2 endScaleBG = new Vector2(startScaleBG.x * 1.1f, startScaleBG.y);

        streakParticles.Play();
        //leftLightningParticles.Play();
        //rightLightningParticles.Play();
        starsParticles.Play();

        leftLightningOutline.DOColor(startColor, tweenDuration);
        rightLightningOutline.DOColor(startColor, tweenDuration);
        leftLightningImg.DOFade(1f, tweenDuration);
        rightLightningImg.DOFade(1f, tweenDuration);
        leftLightningAnim.enabled = true;
        rightLightningAnim.enabled = true;
        canvas.sortingOrder = 1;

        if (toggle.isOn)
        {
            toggle.isOn = false;
            topUIPatronToggle();
        }

        DOTween.To(() => layoutElement.preferredWidth, x => layoutElement.preferredWidth = x, endSize, tweenDuration);
        rect.DOScale(endScale, tweenDuration);
        rectBG.DOScale(endScaleBG, tweenDuration);

        yield return new WaitForSeconds(effectDuration + tweenDuration);

        DOTween.To(() => layoutElement.preferredWidth, x => layoutElement.preferredWidth = x, startSize, tweenDuration);
        rect.DOScale(startScale, tweenDuration);
        rectBG.DOScale(startScaleBG, tweenDuration);

        streakParticles.Stop();
        starsParticles.Stop();

        yield return new WaitForSeconds(tweenDuration);

        patronEffectActive = false;

        leftLightningImg.DOFade(0, tweenDuration);
        rightLightningImg.DOFade(0, tweenDuration);
        leftLightningOutline.DOFade(1f, tweenDuration);
        rightLightningOutline.DOFade(1f, tweenDuration);
        leftLightningAnim.enabled = false;
        rightLightningAnim.enabled = false;
        canvas.sortingOrder = 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;

public class SceneItemActiveUI : MonoBehaviour
{

    [Header("Canvas Control: Don't Change!")]
    [SerializeField]
    GameObject canvas;

    // Start is called before the first frame update
    private Camera MainCamera;
    private CanvasGroup canvasGroup;
    private Transform canvasTransform;

    private float fullVisibleRange;
    private float initialVisibleRange;
    private void Awake()
    {
        MainCamera = FindObjectOfType<CameraManager>().GetComponent<Camera>();
    }

    void Start()
    {
        canvasGroup = canvas.GetComponent<CanvasGroup>();
        canvasTransform = canvas.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        canvasTransform.LookAt(MainCamera.transform.position + MainCamera.transform.forward);
        if (canvas.activeSelf)
        {
            canvasGroup.alpha = computeAlpha(computeDistance());
        }
    }

    private float computeDistance()
    {
        float distance = Vector3.Distance(gameObject.transform.position, MainCamera.transform.position);
        return distance;
    }

    public float computeAlpha(float distance)
    {
        if (fullVisibleRange < 0f || initialVisibleRange < 0f) { return 1f; }
        else { return (initialVisibleRange - distance) / (initialVisibleRange - fullVisibleRange); }
    }


    public void setActive(float initialVisibleRange, float fullVisibleRange)
    {
        canvas.SetActive(true);
        this.initialVisibleRange = initialVisibleRange;
        this.fullVisibleRange = fullVisibleRange;
        canvasGroup.alpha = computeAlpha(computeDistance());
        // print($"{computeDistance()}  {computeAlpha(computeDistance())}");
    }

    public void setInactive()
    {
        canvas.SetActive(false);
    }
}

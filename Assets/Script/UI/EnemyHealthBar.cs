using Assets.Script.Backend;
using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Canvas Control: Don't Change!")]
    [SerializeField]
    Slider healthBarSlider;
    [SerializeField]
    Image healthBarImage;
    [SerializeField]
    GameObject canvas;

    [Header("Behavior")]
    [SerializeField]
    Gradient colorGradient;
    [SerializeField]
    float startVisibleDistance = 10.0f;
    [SerializeField]
    float fullVisibleDistance = 8.0f;
    [SerializeField]
    float onTime = 3.0f;
    [SerializeField]
    float fadeOutSpeed = 1.0f;

    private CanvasGroup canvasGroup;
    private Transform canvasTransform;

    private float onTimeAlpha = 0.0f;
    private bool decreasingOnTimeAlpha = false;
    private float maxHitPoint;

    private bool _isFirstSetup = true;
    // Start is called before the first frame update
    void Start()
    {
        maxHitPoint = 100f;
        canvasGroup = canvas.GetComponent<CanvasGroup>();
        canvasTransform = canvas.GetComponent<Transform>();
    }

    private void LateUpdate()
    {
        canvasTransform.LookAt(transform.position + Camera.main.transform.forward);
        if (decreasingOnTimeAlpha)
        {
            onTimeAlpha -= Time.deltaTime * fadeOutSpeed;
        }
        float alphaToUse = Math.Max(computeDistanceAlpha(), onTimeAlpha);
        if (alphaToUse <= 0.0f) { canvas.SetActive(false); }
        canvasGroup.alpha = alphaToUse;
    }

    public void EnemyStatsChangeCallback(EnemyStats newStats)
    {
        var newHitPoint = newStats.CurrentHitPoint;
        maxHitPoint = newStats.MaxHitPoint;

        if(_isFirstSetup)
        {
            _isFirstSetup = false;
        }
        else
        {
            canvas.SetActive(true);
            onTimeAlpha = 1.0f;
            decreasingOnTimeAlpha = false;
            CancelInvoke();
            Invoke("disableVisualize", onTime);
        }

        healthBarImage.color = colorGradient.Evaluate(newHitPoint / maxHitPoint);
        healthBarSlider.value = newHitPoint;
    }

    float computeDistanceAlpha()
    {
        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        return 1f - Math.Clamp((distance - fullVisibleDistance) / (startVisibleDistance - fullVisibleDistance), 0, 1);
    }

    void disableVisualize()
    {
        decreasingOnTimeAlpha = true;
    }
}

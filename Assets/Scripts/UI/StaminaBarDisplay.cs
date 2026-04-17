using UnityEngine;

public class StaminaBarDisplay : MonoBehaviour
{
    [SerializeField] private float hideDelay = 2f;
    private CanvasGroup canvasGroup;
    private float hideTimer = 0f;
    private bool isVisible;
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    private void Update()
    {   
        StaminabarActive();
    } 
    private void StaminabarActive()
    {
        if (isVisible)
        {
            canvasGroup.alpha = 1f;
            hideTimer = hideDelay;
        } else
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0f)
            {
                canvasGroup.alpha = 0f;
            }
        }
    }

    public void SetVisble(bool visible)
    {
        isVisible = visible;
    }
}


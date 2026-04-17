using UnityEngine;

public class StaminaBarFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        SetStaminaBarPosition();
    }

    private void SetStaminaBarPosition()
    {
        Vector3 worldPosition = target.position + offset;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        rectTransform.position = screenPosition;
    } 
}

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform playerPosition;
    void LateUpdate()
    {
        transform.position = new Vector3(playerPosition.position.x, playerPosition.position.y, -10);
    }
}

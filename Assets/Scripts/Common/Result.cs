using UnityEngine;

public class Result : MonoBehaviour
{
    [SerializeField] private GameObject clearPanel, gameOverPanel;
    private void Start()
    {
        if(GameManager.isAllCollected)
        {
            clearPanel.SetActive(true);
            gameOverPanel.SetActive(false);
        } else
        {
            clearPanel.SetActive(false);
            gameOverPanel.SetActive(true);
        }
    }
}

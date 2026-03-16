using UnityEngine;

public class Result : MonoBehaviour
{
    [SerializeField] private GameObject clearPanel, gameOverPanel;
    void Start()
    {
        if(GameManager.isCleared)
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

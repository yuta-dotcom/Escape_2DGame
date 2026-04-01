using UnityEngine;

public class Goal : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && GameManager.isCleared)
        {
            SceneFader.instance.LoadScene("Result");
        }
    }
}

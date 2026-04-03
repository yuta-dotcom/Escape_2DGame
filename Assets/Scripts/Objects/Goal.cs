using UnityEngine;

public class Goal : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && GameManager.isAllCollected)
        {
            SceneFader.instance.LoadScene("Result");
        }
    }
}

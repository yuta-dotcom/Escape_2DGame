using UnityEngine;

public class Goal : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && GameManager.IsClear)
        {
            SceneFader.instance.LoadScene("Result");
        }
    }
}

using UnityEngine;

public class Gem : MonoBehaviour
{
    public void GetItem()
    {
        GameManager.instance.AddCollectible();
        DestroyItem();
    }

    public void DestroyItem()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"トリガー発火: 相手={collision.gameObject.name}, 親={collision.transform.root.name}, コライダー={collision.GetType()}");
        if (collision.gameObject.CompareTag("Player"))
        {
            GetItem();
        }
    }

}

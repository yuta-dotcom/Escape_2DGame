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
        if (collision.gameObject.CompareTag("Player"))
        {
            GetItem();
        }
    }

}

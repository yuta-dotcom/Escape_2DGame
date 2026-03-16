using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public static int playerHp;
    private const int maxHp = 3;
    public GameObject[] lifeArray = new GameObject[maxHp];

    [SerializeField] private float invincibleDuration = 1.5f;
    private float invincibleTimer = 0f;
    private bool isInvincible = false;

    void Start()
    {
        playerHp = maxHp;
        foreach(GameObject lifePoint in lifeArray)
        {
            lifePoint.SetActive(true);
        }
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
            {
                isInvincible = false;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isInvincible) return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            lifeArray[playerHp - 1].SetActive(false);
            playerHp--;
            if (playerHp == 0)
            {
                GameManager.instance.PlayerDead();
                return;
            }
            isInvincible = true;
            invincibleTimer = invincibleDuration;
        }
    }
}

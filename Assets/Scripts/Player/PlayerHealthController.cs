using System;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public static int playerHp;
    private const int maxHp = 3;
    public GameObject[] lifeArray = new GameObject[maxHp];

    [SerializeField] private float invincibleDuration = 1.5f;
    private float invincibleTimer = 0f;
    private bool isInvincible = false;

    [SerializeField] private Transform playerRespawnPoint;
    [SerializeField] private EnemyAI[] enemies;
    private Transform playerTransform;

    void Start()
    {
        playerTransform = transform.parent;
        Debug.Log(playerTransform.position);
        playerHp = maxHp;
        foreach (GameObject lifePoint in lifeArray)
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        //複数回当たり続かないようにする
        if (isInvincible) return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //プレイヤーを初期位置に戻す
            playerTransform.position = playerRespawnPoint.position;
            lifeArray[playerHp - 1].SetActive(false);
            playerHp--;
            if (playerHp == 0)
            {
                GameManager.instance.PlayerDead();
                return;
            }
            isInvincible = true;
            invincibleTimer = invincibleDuration;

            //敵を初期位置に戻す
            foreach (var enemy in enemies)
            {
                enemy.ResetToSpawn();
            }
        }

    }
}

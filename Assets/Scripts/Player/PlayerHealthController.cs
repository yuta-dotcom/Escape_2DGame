using System;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public static int playerHp;
    private const int maxHp = 3;
    [SerializeField] private GameObject[] lifeArray = new GameObject[maxHp];

    [SerializeField] private float invincibleDuration = 1.5f;
    private float invincibleTimer = 0f;
    private bool isInvincible = false;

    [SerializeField] private Transform playerRespawnPoint;
    [SerializeField] private EnemyAI[] enemies;

    private void Start()
    {
        InitPlayerStatus();
    }
    private void Update()
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

    private void InitPlayerStatus()
    {
        playerHp = maxHp;
        foreach (GameObject lifePoint in lifeArray)
        {
            lifePoint.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //複数回当たり続かないようにする
        if (isInvincible) return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //プレイヤーを初期位置に戻す
            transform.position = playerRespawnPoint.position;
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

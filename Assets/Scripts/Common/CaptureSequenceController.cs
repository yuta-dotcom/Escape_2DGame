using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CaptureSequenceController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image redOverLay;        //画面全体の赤Image
    [SerializeField] private Image BlackOverLay;      //画面全体の黒Image
    [Header("Capture Sequence Time")]
    [SerializeField] private float redFadeTime = 1.0f; //赤くなる時間 
    [SerializeField] private float blackFadeTime = 0.3f; //黒くなる時間
    [SerializeField] private float fadeInTime = 1.0f; //フェードインの時間

    [Header("References")]
    [SerializeField] private CaptureAnimPlayer captureAnimPlayer;
    [SerializeField] private PlayerHealthController playerHealthController;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform playerRespawnPoint;
    [SerializeField] private EnemyAI[] enemies;

    private bool isPlaying = false;

    public static CaptureSequenceController instance;
    private void Awake()    
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        //初期状態ではオーバーレイは非表示
        BlackOverLay.color = Color.clear;
    }

    /// <summary>
    /* 捕食シーケンスを開始するメソッド
       プレイヤーが敵に捕まったときに呼び出される
       画面が赤くなり、捕食演出が表示された後、プレイヤーの体力が減少し、一定時間待機してから画面が黒くなり、プレイヤーが初期位置に戻される
    */
    /// </summary>
    public void StartCaptureSequence()
    {
        if (!isPlaying)
        {
            StartCoroutine(CaptureSwquence());
        }
    }
    private IEnumerator CaptureSwquence()
    {
        isPlaying = true;
        playerController.StopPlayer(); //プレイヤーを停止
        //操作を一時的に無効化
        playerController.enabled = false;

        //捕食演出を表示
        yield return StartCoroutine(captureAnimPlayer.PlayCaptureAnimation());


        //画面をだんだん黒くする
        float elapsed = 0f;
        while (elapsed < blackFadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / blackFadeTime);
            BlackOverLay.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        //プレイヤーの体力を減らす
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(playerHealthController.TakeDamage());

        //体力が0ならゲームオーバー
        if (playerHealthController.IsPlayerDead())
        {
            GameManager.instance.GameOver();
            yield break;
        }

        //リスポーン処理
        playerController.ResetPlayerPosition(playerRespawnPoint.position);
        foreach (EnemyAI enemy in enemies)        {
            enemy.ResetToSpawn();
        }

        //フェードイン黒くなった画面をだんだん透明にする
        elapsed = 0f;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeInTime);
            BlackOverLay.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        BlackOverLay.color = Color.clear;

        //操作再開
        playerController.enabled = true;
        isPlaying = false;
    }
}

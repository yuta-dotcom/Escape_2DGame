using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CaptureAnimPlayer : MonoBehaviour
{
    private Animator animator;
    private Camera mainCamera;

    [SerializeField] private Image redOverLay;        //画面全体の赤Image
    [SerializeField] private float redFadeTime = 1.0f; //赤くなる時間 

    [SerializeField] private GameObject CaptureSequenceBackGround; //捕食シーケンスの背景Image

    [Header("Camera Shake")]
    [SerializeField] private float shakeIntensity = 0.4f; //カメラの揺れの強さ

    [Header("Camera Zoom")]
    [SerializeField] private float zoomInSize = 1f; //ズームの後のサイズ

    private float originalCameraSize; //カメラの元のサイズ
    private Vector3 originalCameraPosition; //カメラの元の位置

    private void Awake()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        gameObject.SetActive(false); //初期状態では非表示
        CaptureSequenceBackGround.SetActive(false); //捕食シーケンスの背景も非表示
        redOverLay.color = Color.clear; //赤オーバーレイを透明にしておく
    }
    /// <summary>
    /// 捕食アニメーション＋カメラ演出を再生し、完了まで待つ
    /// </summary>
    public IEnumerator PlayCaptureAnimation()
    {
        Debug.Log("捕食アニメーション開始");
        //カメラの元の状態を保存
        originalCameraSize = mainCamera.orthographicSize;
        originalCameraPosition = mainCamera.transform.position;

        CaptureSequenceBackGround.SetActive(true); //捕食シーケンスの背景を表示
        gameObject.SetActive(true); //捕食アニメーションを表示

        animator.SetTrigger("isCapture");
        //アニメーションが開始するまで1フレーム待機
        yield return null;

        //アニメーションの長さを取得
        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
        float elapsed = 0f;

        //アニメーションの再生と同時にカメラズームとシェイクを行う
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            //カメラズーム
            mainCamera.orthographicSize = Mathf.Lerp(originalCameraSize, zoomInSize, progress);

            //カメラシェイク
            Vector2 shake = Random.insideUnitCircle * shakeIntensity;
            mainCamera.transform.position = originalCameraPosition + new Vector3(shake.x, shake.y, 0);
            float alpha = Mathf.Lerp(0f,0.8f,elapsed / redFadeTime);
            redOverLay.color = new Color(1f, 0f, 0f, alpha);
            yield return null;
        }
        //アニメーション終了後、カメラを元の状態に戻す
        mainCamera.transform.position = originalCameraPosition;
        mainCamera.orthographicSize = originalCameraSize;
        gameObject.SetActive(false);
        CaptureSequenceBackGround.SetActive(false); //捕食シーケンスの背景を非表示
        redOverLay.color = Color.clear; //赤オーバーレイを透明にしておく
    }
}

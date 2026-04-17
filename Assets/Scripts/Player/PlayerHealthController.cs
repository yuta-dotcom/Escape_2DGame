using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : MonoBehaviour
{
    public int currentLife { get; private set; }
    private const int maxLife = 3;
    [SerializeField] private GameObject[] lifeArray = new GameObject[maxLife];
    [SerializeField] private float fadeTime = 1.0f;  //ライフのフェード時間
    [SerializeField] private float lifeLostWaitTime = 0.5f; //体力減少後の待ち時間

    private void Start()
    {
        InitPlayerStatus();
    }

    private void InitPlayerStatus()
    {
        currentLife = maxLife;
        foreach (GameObject lifePoint in lifeArray)
        {
            lifePoint.SetActive(false);
        }
    }

    public IEnumerator TakeDamage()
    {
        if (currentLife <= 0) yield break;
        currentLife--;
        yield return TakeDamageAnimation(currentLife);
    }

    IEnumerator TakeDamageAnimation(int index)
    {
        Image[] lifeImages = new Image[index + 1];

        for (int i = 0; i <= index; i++)
        {
            lifeImages[i] = lifeArray[i].GetComponent<Image>();
            lifeImages[i].color = new Color(1f, 1f, 1f, 0f);
            lifeArray[i].SetActive(true);
        }
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
            foreach (Image lifeImage in lifeImages)
            {
                lifeImage.color = new Color(1f, 1f, 1f, alpha);
            }
            yield return null;
        }
        //ダメージを受けたときの処理（点滅）
        for (int i = 0; i < 5; i++)
        {
            lifeArray[currentLife].SetActive(false);
            yield return new WaitForSeconds(0.2f);
            lifeArray[currentLife].SetActive(true);
            yield return new WaitForSeconds(0.2f);
        }
        lifeArray[currentLife].SetActive(false);
        yield return new WaitForSeconds(lifeLostWaitTime);

        elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            foreach (Image lifeImage in lifeImages)
            {
                lifeImage.color = new Color(1f, 1f, 1f, alpha);
            }
            yield return null;
        }

        for (int i = 0; i < index; i++)
        {
            lifeArray[i].SetActive(false);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            CaptureSequenceController.instance.StartCaptureSequence();
        }
    }

    public bool IsPlayerDead()
    {
        return currentLife <= 0;
    }
}

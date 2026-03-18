using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public static SceneFader instance;
    public Image fadeImage;
    public float fadeDuration = 1.0f;
    public bool IsFading { get; private set; }
    private System.Action onFadeInComplete;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    public void NotifyOnFadeInComplete(System.Action callback)
    {
        onFadeInComplete = callback;
    }

    public void LoadScene(string str)
    {
        StartCoroutine(FadeOutAndLoad(str));
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        IsFading = true;
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(t));
            yield return null;
        }

        yield return SceneManager.LoadSceneAsync(sceneName);

        t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(t));
            yield return null;
        }

        IsFading = false;
        onFadeInComplete?.Invoke();
        onFadeInComplete = null;
    }


}

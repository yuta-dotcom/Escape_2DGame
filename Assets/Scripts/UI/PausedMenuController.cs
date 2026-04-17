using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PausedMenuController : MonoBehaviour
{
    //ボタンオブジェクト
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private GameObject retryButton;
    [SerializeField] private GameObject titleBackButton;
    [SerializeField] private GameObject previousSelectedButton;

    //画面パネル
    [SerializeField] private GameObject pausePanel;
    public static bool isPaused { get; private set; }

  private void Update()
    {
        //カーソル音を再生するための処理
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != null && currentSelected != previousSelectedButton)
        {
            if (previousSelectedButton != null)
            {
                SoundManager.instance.PlaySfx("Cursor");
            }
            previousSelectedButton = currentSelected;
        }
    }
    public void Pause()
    {
        if (isPaused)
        {
            return;
        }

        isPaused = true;

        InputManager.instance.EnableUIMode(OnSubmit, OnCancel);

        //最初に選択状態になるボタン
        EventSystem.current.SetSelectedGameObject(retryButton);
        pausePanel.SetActive(true);

        //ゲームを止める
        Time.timeScale = 0f;
    }
    public void Resume()
    {
        if(!isPaused)
        {
            return;
        }

        isPaused = false;

        InputManager.instance.EnablePlayerMode();

        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void Retry()
    {
        InputManager.instance.EnablePlayerMode();

        pausePanel.SetActive(false);
        isPaused = false;

        Time.timeScale = 1f;
        SceneFader.instance.LoadScene("Stage");
    }

    void TitleBack()
    {
        InputManager.instance.UnregisterUIHandlers();

        isPaused = false;
        Time.timeScale = 1f;
        SceneFader.instance.LoadScene("Title");
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        if (EventSystem.current.currentSelectedGameObject == resumeButton)
        {
            SoundManager.instance.PlaySfx("Submit");            
            Resume();
        } else if(EventSystem.current.currentSelectedGameObject == retryButton)
        {
            SoundManager.instance.PlaySfx("Submit");
            Retry();
        } else if(EventSystem.current.currentSelectedGameObject == titleBackButton)
        {
            SoundManager.instance.PlaySfx("Submit");
            TitleBack();
        }
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        Resume();
    }
}

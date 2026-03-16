using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PausedMenuController : MonoBehaviour
{
    //ボタンオブジェクト
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private GameObject retryButton;
    [SerializeField] private GameObject titleBackButton;

    //画面パネル
    [SerializeField] private GameObject pausePanel;
    public static bool isPaused { get; private set; }

    public void Pause()
    {
        if (isPaused)
        {
            return;
        }

        isPaused = true;

        //UIイベント購読
        var input = InputManager.instance.inputActions;
        input.Player.Disable();
        input.UI.Enable();
        input.UI.Submit.started += OnSubmit;
        input.UI.Cancel.started += OnCancel;

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

        //イベント購読解除
        var input = InputManager.instance.inputActions;
        input.UI.Submit.started -= OnSubmit;
        input.UI.Cancel.started -= OnCancel;
        input.UI.Disable();
        input.Player.Enable();

        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void Retry()
    {
        var input = InputManager.instance.inputActions;
        input.UI.Submit.started -= OnSubmit;
        input.UI.Cancel.started -= OnCancel;
        input.UI.Disable();
        input.Player.Enable();

        pausePanel.SetActive(false);
        isPaused = false;

        Time.timeScale = 1f;
        SceneManager.LoadScene("Stage");
    }

    void TitleBack()
    {
        var input = InputManager.instance.inputActions;
        input.UI.Submit.started -= OnSubmit;
        input.UI.Cancel.started -= OnCancel;
        input.UI.Disable();

        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        if (EventSystem.current.currentSelectedGameObject == resumeButton)
        {
            Resume();
        } else if(EventSystem.current.currentSelectedGameObject == retryButton)
        {
            Retry();
        } else if(EventSystem.current.currentSelectedGameObject == titleBackButton)
        {
            TitleBack();
        }
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        Resume();
    }
}

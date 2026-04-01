using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PausedMenuController : MonoBehaviour
{
    //�{�^���I�u�W�F�N�g
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private GameObject retryButton;
    [SerializeField] private GameObject titleBackButton;

    //��ʃp�l��
    [SerializeField] private GameObject pausePanel;
    public static bool isPaused { get; private set; }

    public void Pause()
    {
        if (isPaused)
        {
            return;
        }

        isPaused = true;

        InputManager.instance.EnableUIMode(OnSubmit, OnCancel);

        //�ŏ��ɑI����ԂɂȂ�{�^��
        EventSystem.current.SetSelectedGameObject(retryButton);
        pausePanel.SetActive(true);

        //�Q�[�����~�߂�
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

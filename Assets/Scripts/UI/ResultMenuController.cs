using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ResultMenuController : MonoBehaviour
{

    [SerializeField] private GameObject firstSelectedButton;
    [SerializeField] private GameObject titleBackButton;
    private bool isReady = false;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        if (SceneFader.instance != null && SceneFader.instance.IsFading)
        {
            SceneFader.instance.NotifyOnFadeInComplete(() => isReady = true);
        }
        else
        {
            isReady = true;
        }
    }

    private void OnEnable()
    {
        InputManager.instance.EnableUIMode(OnSubmit, OnCancel);
    }
    private void OnDisable()
    {
        InputManager.instance.EnablePlayerMode();
    }
    private void OnSubmit(InputAction.CallbackContext context)
    {
        if (!isReady) return;
        if (EventSystem.current.currentSelectedGameObject == firstSelectedButton)
        {
            OnRetryButton();
        } else if (EventSystem.current.currentSelectedGameObject == titleBackButton)
        {
            TitleBack();
        }
    }

    private void OnCancel(InputAction.CallbackContext context)
    {

    }

    private void OnRetryButton()
    {
        InputManager.instance.EnablePlayerMode();
        SceneFader.instance.LoadScene("Stage");
    }

    private void TitleBack()
    {
        InputManager.instance.UnregisterUIHandlers();
        SceneFader.instance.LoadScene("Title");
    }
}

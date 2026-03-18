using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleMenuController : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton;
    private bool isReady = false;

    private void Start()
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
        var input = InputManager.instance.inputActions;
        input.Player.Disable();
        input.UI.Enable();
        input.UI.Submit.started += OnSubmit;
        input.UI.Cancel.started += OnCancel;
    }

    private void OnDisable()
    {
        var input = InputManager.instance.inputActions;
        input.UI.Submit.started -= OnSubmit;
        input.UI.Cancel.started -= OnCancel;
        input.UI.Disable();
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        if (!isReady) return;
        if (EventSystem.current.currentSelectedGameObject == firstSelectedButton)
        {
            OnStartButton();
        }
    }

    private void OnCancel(InputAction.CallbackContext context)
    {

    }

    public void OnStartButton()
    {
        isReady = false;
        SceneFader.instance.LoadScene("Stage");
    }

}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleMenuController : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton;
    

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
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
        if(EventSystem.current.currentSelectedGameObject == firstSelectedButton)
        {
            OnStartButton();
        }
    }

    private void OnCancel(InputAction.CallbackContext context)
    {

    }
    public void OnStartButton()
    {
        SceneFader.instance.LoadScene("Stage");
    }

}

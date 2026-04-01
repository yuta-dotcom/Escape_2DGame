using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1000)]
public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public GameActions inputActions;

    private Action<InputAction.CallbackContext> currentSubmitHandler;
    private Action<InputAction.CallbackContext> currentCancelHandler;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            inputActions = new GameActions();
            inputActions.Enable();
        } else
        {
            Destroy(gameObject);
        }
    }

    public void EnableUIMode(
        Action<InputAction.CallbackContext> onSubmit,
        Action<InputAction.CallbackContext> onCancel)
    {
        inputActions.Player.Disable();
        inputActions.UI.Enable();
        currentSubmitHandler = onSubmit;
        currentCancelHandler = onCancel;
        inputActions.UI.Submit.started += currentSubmitHandler;
        inputActions.UI.Cancel.started += currentCancelHandler;
    }

    public void EnablePlayerMode()
    {
        if (currentSubmitHandler != null) inputActions.UI.Submit.started -= currentSubmitHandler;
        if (currentCancelHandler != null) inputActions.UI.Cancel.started -= currentCancelHandler;
        currentSubmitHandler = null;
        currentCancelHandler = null;
        inputActions.UI.Disable();
        inputActions.Player.Enable();
    }

    public void UnregisterUIHandlers()
    {
        if (currentSubmitHandler != null) inputActions.UI.Submit.started -= currentSubmitHandler;
        if (currentCancelHandler != null) inputActions.UI.Cancel.started -= currentCancelHandler;
        currentSubmitHandler = null;
        currentCancelHandler = null;
    }
}

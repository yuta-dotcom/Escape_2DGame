using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TitleMenuController : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton;
    [SerializeField] private GameObject previousSelectedButton;
    private bool isReady = false;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);

        if (SceneFader.instance != null && SceneFader.instance.IsFading)
        {
            SceneFader.instance.NotifyOnFadeInComplete(() =>
            {
                isReady = true;
            });
        }
        else
        {
            bool isFading = SceneFader.instance != null && SceneFader.instance.IsFading;
            isReady = true;
        }
    }
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
            SoundManager.instance.PlaySfx("Submit");
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

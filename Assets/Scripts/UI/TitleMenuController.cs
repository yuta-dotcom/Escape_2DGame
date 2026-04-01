using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TitleMenuController : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton;
    private bool isReady = false;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);

        if (SceneFader.instance != null && SceneFader.instance.IsFading)
        {
            Debug.Log("フェード中 → コールバック登録");
            SceneFader.instance.NotifyOnFadeInComplete(() =>
            {
                Debug.Log("コールバック呼ばれた → isReady = true");
                isReady = true;
            });
        }
        else
        {
            bool isFading = SceneFader.instance != null && SceneFader.instance.IsFading;
            Debug.Log($"フェードなし → isReady = true (IsFading={isFading})");
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

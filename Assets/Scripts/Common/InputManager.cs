using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public GameActions inputActions;

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

    
}

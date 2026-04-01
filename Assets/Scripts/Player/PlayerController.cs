
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class PlayerController : MonoBehaviour
{
    private Vector2 move;
    private float walkSpeed =2.0f;
    private float dashSpeed = 5.0f;
    private float currentSpeed;
    private Rigidbody2D playerBody;
    private Animator anim;
    private Vector2 dir = Vector2.down;
    private bool isDash;
    [SerializeField] private PausedMenuController pauseMenuController;
    //ライト関連変数
    private FlashlightController flashlight;
    [SerializeField] private Image staminaGaugeImage;
    private float currentStamina;
    private float maxStamina = 150;

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        flashlight = GetComponentInChildren<FlashlightController>();
    }

    private void OnEnable()
    {
        var input = InputManager.instance.inputActions;
        input.Player.Enable();
        input.Player.Move.started += OnMove;
        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMove;
        input.Player.Pause.started += OnPause;
        input.Player.Dash.performed += OnDash;
        input.Player.Dash.canceled += OnDash;

    }

    private void OnDisable()
    {
        var input = InputManager.instance.inputActions;
        input.Player.Move.started -= OnMove;
        input.Player.Move.performed -= OnMove;
        input.Player.Move.canceled -= OnMove;
        input.Player.Pause.started -= OnPause;
        input.Player.Dash.performed -= OnDash;
        input.Player.Dash.canceled -= OnDash;
        input.Player.Disable();
    }

    private void Update()
    {
        if (move != Vector2.zero)
        {
            anim.SetBool("isWalking",true);
            dir =  move.normalized;
            flashlight.SetDirection(dir);
        } else
        {
            anim.SetBool("isWalking", false);
        }
        anim.SetFloat("X", dir.x);
        anim.SetFloat("Y",dir.y);

    }
    private void FixedUpdate()
    {
        if (move.sqrMagnitude > 1f)
        {
            move.Normalize();
        }
        currentSpeed = isDash ? dashSpeed : walkSpeed;
        playerBody.linearVelocity = move * currentSpeed;
    }

    private void UpdateStamina()
    {
        if (currentStamina <= maxStamina) return;
        if (isDash)
        { 
            currentStamina--;
        } else
        {
            currentStamina++;
        }
        staminaGaugeImage.fillAmount = currentStamina / maxStamina;
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        pauseMenuController.Pause();
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && currentStamina > 0)
        {
            isDash = true;
            Debug.Log("ダッシュ中");
        } else
        {
            isDash = false;
            Debug.Log("ダッシュ終了");
        }
    }
}

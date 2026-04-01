
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class PlayerController : MonoBehaviour
{
    private Vector2 move;
    [SerializeField] private float walkSpeed = 2.0f;
    [SerializeField] private float dashSpeed = 5.0f;
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
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float drainRate = 30f;  // 毎秒の消費量
    [SerializeField] private float regenRate = 10f;   // 毎秒の回復量

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        flashlight = GetComponentInChildren<FlashlightController>();
        currentStamina = maxStamina;
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
            anim.SetBool("isWalking", true);
            dir = move.normalized;
            flashlight.SetDirection(dir);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
        anim.SetFloat("X", dir.x);
        anim.SetFloat("Y", dir.y);
    }
    private void FixedUpdate()
    {
        UpdateStamina();
        currentSpeed = isDash ? dashSpeed : walkSpeed;
        playerBody.linearVelocity = (move.sqrMagnitude > 1f ? move.normalized : move) * currentSpeed;
    }

    private void UpdateStamina()
    {
        if (isDash && currentStamina > 0)
        {
            currentStamina -= drainRate * Time.deltaTime;
        }
        else if (isDash && currentStamina <= 0)
        {
            isDash = false;
        }

        if (!isDash && currentStamina < maxStamina)
        {
            currentStamina += regenRate * Time.deltaTime;
        }
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
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
        }
        else
        {
            isDash = false;
        }
    }
}


using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class PlayerController : MonoBehaviour
{
    //移動関連変数
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
    //スタミナ関連変数
    [SerializeField] private Image staminaGaugeImage;
    private float currentStamina;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float drainRate = 30f;  // 毎秒の消費量
    [SerializeField] private float regenRate = 10f;   // 毎秒の回復量

    // 歩行音関連変数
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float walkStepInterval = 0.45f; // 歩行音の再生間隔
    [SerializeField] private float dashStepInterval = 0.25f; // ダッシュ音の再生間隔
    private float stepTimer;
    //スタミナバー関連変数
    [SerializeField] private StaminaBarDisplay staminaBarDisplay;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
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

            float interval = isDash ? dashStepInterval : walkStepInterval;
            stepTimer += Time.deltaTime;
            if (stepTimer >= interval)
            {
                PlayFootstepSound();
                stepTimer = 0f;
            }
            
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
        staminaBarDisplay.SetVisble(isDash);
        if (!isDash && currentStamina < maxStamina)
        {
            currentStamina += regenRate * Time.deltaTime;
        }
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        staminaGaugeImage.fillAmount = currentStamina / maxStamina;
    }
    /// <summary>
    /// プレイヤーの位置をリセットするメソッド（捕食シーケンス完了後に呼び出される）
    /// </summary>
    /// <param name="position"></param>
    public void ResetPlayerPosition(Vector2 position)
    {
        transform.position = position;
    }

    public void StopPlayer()
    {
        move = Vector2.zero;
        playerBody.linearVelocity = Vector2.zero;
        anim.SetBool("isWalking", false);
        isDash = false;
        currentStamina = maxStamina;
    }

    private void PlayFootstepSound()
    {
        if (footstepClips.Length == 0) return;
        int index = Random.Range(0, footstepClips.Length);
        audioSource.PlayOneShot(footstepClips[index]);
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

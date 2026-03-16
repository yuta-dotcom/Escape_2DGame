using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerController : MonoBehaviour
{

    private Vector2 move;
    private float moveSpeed =6.0f;
    private Rigidbody2D playerBody;
    private Animator anim;
    private Vector2 dir = Vector2.down;

    [SerializeField] private PausedMenuController pauseMenuController;

    // 掴み関連の変数
    public LayerMask GrabMask;
    private float grabRange = 0.2f;

    [SerializeField] private float holdDistance = 0.8f;

    private Rigidbody2D grabObjectBody;
    [SerializeField] private Transform grabPoint;

    private Collider2D playerCol;
    private Collider2D grabObjectCol;
    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerCol = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {

        var input = InputManager.instance.inputActions;
        input.Player.Enable();
        input.Player.Move.started += OnMove;
        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMove;
        input.Player.Pause.started += OnPause;
        input.Player.Grab.performed += OnGrab;
        input.Player.Grab.canceled += OnGrab;
    }

    private void OnDisable()
    {
        // 掴んでいる途中にポーズ等で遷移する際に入力が残らないようにする
        ReleaseObject();
        var input = InputManager.instance.inputActions;
        input.Player.Move.started -= OnMove;
        input.Player.Move.performed -= OnMove;
        input.Player.Move.canceled -= OnMove;
        input.Player.Pause.started -= OnPause;
        input.Player.Grab.performed -= OnGrab;
        input.Player.Grab.canceled -= OnGrab;
        input.Player.Disable();
    }

    void Update()
    {
        if (move != Vector2.zero)
        {
            anim.SetBool("isWalking",true);
            dir =  move.normalized;
            grabPoint.localPosition = new Vector3(dir.x, dir.y, 0f) * holdDistance;
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
        playerBody.linearVelocity = move * moveSpeed;

        if(grabObjectBody != null)
        {
            Vector2 next = Vector2.Lerp(grabObjectBody.position, grabPoint.position, 0.8f);
            grabObjectBody.MovePosition(next);
        }
    }

    private void OnGrab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("掴んだ");
            Collider2D hit = Physics2D.OverlapCircle(grabPoint.position,grabRange,GrabMask);
            if (hit && hit.attachedRigidbody)
            {
                // 掴み対象の Rigidbody を取得・設定
                grabObjectBody = hit.attachedRigidbody;
                grabObjectBody.bodyType = RigidbodyType2D.Kinematic;
                grabObjectCol = hit;

                /*
                掴み対象のオブジェクトとのコライダーの衝突による移動防止のため
                掴み中のみ対象のオブジェクトとのコライダーを無視する
                 */
                if (playerCol && grabObjectCol)
                {
                    Physics2D.IgnoreCollision(playerCol,grabObjectCol,true);
                }
            }

        } else if (context.canceled)
        {
            ReleaseObject();
        }

    }

    void ReleaseObject()
    {
        Debug.Log("離した");
        if (grabObjectBody != null)
        {
            grabObjectBody.linearVelocity = Vector2.zero;
            grabObjectBody.angularVelocity = 0f;
            grabObjectBody.bodyType = RigidbodyType2D.Kinematic;

            if (playerCol && grabObjectCol)
            {
                Physics2D.IgnoreCollision(playerCol, grabObjectCol, false);
            }

            grabObjectCol = null;
            grabObjectBody = null;
        }
    }


    private void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        pauseMenuController.Pause();
    }

    void OnDrawGizmosSelected()
    {
        // 掴める範囲を表示
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(grabPoint.position, grabRange);
    }
}

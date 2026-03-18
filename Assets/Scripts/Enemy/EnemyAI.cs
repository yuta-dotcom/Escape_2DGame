using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    enum State { Patrol, Chase, Alert }

    [Header("Component")]
    private Rigidbody2D rb;
    private Seeker seeker;
    [SerializeField] private Transform player;
    private Animator anim;

    [Header("Vision")]
    [SerializeField] private float viewRadius = 6.0f;     // 視界半径
    [SerializeField] private float viewAngle = 90f;       // 視野角（片側°）
    [SerializeField] private float closeRadius = 1.0f;    // この距離以内は全方向検知
    private bool canSeePlayer;
    private Vector2 smoothFacingDir = Vector2.right;
    [SerializeField] private LayerMask wallLayer;

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 1.6f;
    [SerializeField] private float patrolPointDist = 0.2f;
    private int patrolPointIndex = 0;

    [Header("Chase")]
    private float lostSightTimer = 0f; // 見失ってからの経過時間
    [SerializeField] private float lostSightMaxTime = 2.0f;
    [SerializeField] private float chaseSpeed = 3.2f;

    [Header("Alert")]
    [SerializeField] private float alertMaxTime = 2.0f;
    private float alertTimer = 0f;

    [Header("State")]
    State currentState = State.Patrol;

    private Path currentPath;
    private int wayPointIndex = 0;
    [SerializeField] private float nextWaypointDist = 0.3f;
    private float repathRate = 0.4f;
    private float repathTimer = 0f;

    [SerializeField] private Transform respawnPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        seeker = GetComponent<Seeker>();
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void FixedUpdate()
    {
        canSeePlayer = CanSeePlayer();
        repathTimer += Time.fixedDeltaTime;

        switch (currentState)
        {
            case State.Patrol:
                if (canSeePlayer)
                {
                    currentState = State.Chase;
                    lostSightTimer = 0f;
                    currentPath = null;
                    break;
                }
                Patrol();
                break;
            case State.Chase:
                if (canSeePlayer)
                {
                    lostSightTimer = 0f;
                    Chase(player.position);
                }
                else
                {
                    lostSightTimer += Time.fixedDeltaTime;
                    // 一定時間以上見失った場合、警戒状態に移行
                    if (lostSightTimer >= lostSightMaxTime)
                    {
                        currentState = State.Alert;
                    }
                    MoveAlongPath(chaseSpeed);
                }
                break;
            case State.Alert:
                if (canSeePlayer)
                {
                    alertTimer = 0f;
                    currentState = State.Chase;
                    currentPath = null;
                    RequestPath(player.position);
                    break;
                }
                else
                {
                    alertTimer += Time.fixedDeltaTime;
                    rb.linearVelocity = Vector2.zero;
                    Vector2 toPlayer = ((Vector2)player.position - rb.position).normalized;
                    smoothFacingDir = Vector2.Lerp(smoothFacingDir, toPlayer, 0.02f).normalized;
                    if (alertTimer >= alertMaxTime)
                    {
                        alertTimer = 0f;
                        currentState = State.Patrol;
                        currentPath = null;
                        break;
                    }
                }
                break;
        }
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Vector2 target = patrolPoints[patrolPointIndex].position;

        // パトロールポイントから距離が以下になった場合
        if (Vector2.Distance(rb.position, target) <= patrolPointDist)
        {
            // 最後のポイントまで行ったら最初に戻る
            patrolPointIndex = (patrolPointIndex + 1) % patrolPoints.Length;
            RequestPath(patrolPoints[patrolPointIndex].position);
            return;
        }

        if (repathTimer >= repathRate)
        {
            RequestPath(target);
        }

        MoveAlongPath(patrolSpeed);
    }

    private void Chase(Vector2 targetPos)
    {
        // 視界内は短いインターバルでパスを更新してレスポンスを上げる
        float rate = canSeePlayer ? 0.1f : repathRate;
        if (repathTimer >= rate)
        {
            RequestPath(targetPos);
        }

        if (currentPath == null)
        {
            Vector2 dir = (targetPos - rb.position).normalized;
            rb.linearVelocity = dir * chaseSpeed;
            smoothFacingDir = Vector2.Lerp(smoothFacingDir, dir, 0.3f).normalized;
        }
        else
        {
            MoveAlongPath(chaseSpeed);
        }
    }

    private void RequestPath(Vector2 dest)
    {
        if (seeker.IsDone())
        {
            repathTimer = 0f;
            seeker.StartPath(rb.position, dest, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            currentPath = p;
            wayPointIndex = 0;
        }
    }

    private void MoveAlongPath(float speed)
    {
        if (currentPath == null)
        {
            return;
        }

        if (wayPointIndex >= currentPath.vectorPath.Count)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dir = ((Vector2)currentPath.vectorPath[wayPointIndex] - rb.position).normalized;
        rb.linearVelocity = dir * speed;
        smoothFacingDir = Vector2.Lerp(smoothFacingDir, dir, 0.3f).normalized;

        if (Vector2.Distance(rb.position, currentPath.vectorPath[wayPointIndex]) < nextWaypointDist)
        {
            wayPointIndex++;
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > viewRadius) return false;

        // 近距離は全方向検知
        if (dist <= closeRadius) return true;

        Vector2 dir = (player.position - transform.position).normalized;

        // 視野角チェック（smoothFacingDirでギズモと判定を一致させる）
        float angle = Vector2.Angle(smoothFacingDir, dir);
        if (angle > viewAngle) return false;

        // 中央・左右にずらした3本のRaycastで壁角の引っかかりを緩和
        float spreadAngle = 5f * Mathf.Deg2Rad;
        Vector2[] dirs = {
            dir,
            new(dir.x * Mathf.Cos(spreadAngle)  - dir.y * Mathf.Sin(spreadAngle),
                dir.x * Mathf.Sin(spreadAngle)  + dir.y * Mathf.Cos(spreadAngle)),
            new(dir.x * Mathf.Cos(-spreadAngle) - dir.y * Mathf.Sin(-spreadAngle),
                dir.x * Mathf.Sin(-spreadAngle) + dir.y * Mathf.Cos(-spreadAngle))
        };
        foreach (Vector2 d in dirs)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, d, viewRadius, wallLayer);
            if (hit.collider == null) return true;
        }
        return false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ResetToSpawn();
        }
    }

    public void ResetToSpawn()
    {
        transform.position = respawnPoint.position;
        rb.linearVelocity = Vector2.zero;
        currentState = State.Patrol;
        currentPath = null;
        wayPointIndex = 0;
        patrolPointIndex = 0;
        lostSightTimer = 0f;
        alertTimer = 0f;
        repathTimer = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        // 視界半径の円
        Gizmos.color = canSeePlayer ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        // 視野角の境界線
        Vector2 forward = Application.isPlaying ? smoothFacingDir : Vector2.right;
        float halfAngle = viewAngle * Mathf.Deg2Rad;
        Vector2 leftBound = new(
            forward.x * Mathf.Cos(halfAngle) - forward.y * Mathf.Sin(halfAngle),
            forward.x * Mathf.Sin(halfAngle) + forward.y * Mathf.Cos(halfAngle));
        Vector2 rightBound = new(
            forward.x * Mathf.Cos(-halfAngle) - forward.y * Mathf.Sin(-halfAngle),
            forward.x * Mathf.Sin(-halfAngle) + forward.y * Mathf.Cos(-halfAngle));

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, leftBound * viewRadius);
        Gizmos.DrawRay(transform.position, rightBound * viewRadius);

        // プレイヤーへのRaycast
        if (player != null)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            Gizmos.color = canSeePlayer ? Color.red : Color.green;
            Gizmos.DrawRay(transform.position, dir * viewRadius);
        }
    }
}

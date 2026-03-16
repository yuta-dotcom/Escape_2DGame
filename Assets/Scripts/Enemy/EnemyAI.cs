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
    [SerializeField] private float viewRadius = 6.0f; // 視界半径
    private bool canSeePlayer;
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
                if(canSeePlayer)
                {
                    currentState = State.Chase;
                    lostSightTimer = 0f;
                    RequestPath(player.position);
                    break;
                }
                Patrol();
                break;
            case State.Chase:
                if(canSeePlayer)
                {
                    lostSightTimer = 0f;
                    Chase(player.position);
                } else
                {
                    lostSightTimer += Time.fixedDeltaTime;
                    // 一定時間以上見失った場合、警戒状態に移行
                    if(lostSightTimer >= lostSightMaxTime)
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
                    break;
                }
                alertTimer += Time.fixedDeltaTime;
                if (alertTimer >= alertMaxTime)
                {
                    alertTimer = 0f;
                    currentState = State.Patrol;
                    currentPath = null;
                    break;
                }
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Vector2 target = patrolPoints[patrolPointIndex].position;

        // パトロールポイントから距離が以下になった場合
        if(Vector2.Distance(rb.position, target) <= patrolPointDist)
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
        if (repathTimer >= repathRate)
        {
            RequestPath(targetPos);
        }

        MoveAlongPath(chaseSpeed);
    }

    private void RequestPath(Vector2 dest)
    {
        if(seeker.IsDone())
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

        if (Vector2.Distance(rb.position, currentPath.vectorPath[wayPointIndex]) < nextWaypointDist)
        {
            wayPointIndex++;
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > viewRadius)
        {
            return false;
        }

        Vector2 dir = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, wallLayer);

        return hit.collider == null;
    }
}

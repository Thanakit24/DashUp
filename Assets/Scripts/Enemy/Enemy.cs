using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
   
    [Header("Stats")]
    [SerializeField] private float health;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float chaseSpeed;

    //-------------------------------------------------------;

    [Header("References")]
    private Rigidbody2D rb;
    [SerializeField] private Transform currentTarget;
    public Transform player;

    //-------------------------------------------------------;

    [Header("Patrol Behavior")]
    public bool canPatrol = false;
    public Transform[] patrolPoints;
    private int patrolIndex = 1;
    [SerializeField] private float nextWaypointDistance = 2f;
    [SerializeField] private bool waitAtPoint = false;
    [SerializeField] private float waitTime;

    //-------------------------------------------------------;

    [Header("WallCheck")]
    [SerializeField] private bool isFacingRight;
    [SerializeField] Transform wallCheckPos;
    [SerializeField] float wallCheckDistance;
    [SerializeField] private float fovAngle = 90f; // Adjust this angle to change the FOV
    [SerializeField] private float fovDetectDistance = 5f; // Adjust this distance to change the FOV range


    [SerializeField] private bool isChasing = false;
    void Start()
    {
        currentTarget = player.transform;
        isFacingRight = true;
        currentSpeed = moveSpeed;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPlayerInFOV();
            
        if (canPatrol && !isChasing)
        {
            Patrol();
        }
        else if (isChasing)
        {
            currentTarget = player;
            print("chase after player");
        }


        if (currentTarget.transform.position.x < transform.position.x && isFacingRight)
        {
            if (!waitAtPoint || !canPatrol)
                Flip();
        }

        else if (currentTarget.transform.position.x > transform.position.x && !isFacingRight)
        {
            if (!waitAtPoint|| !canPatrol)
                Flip();
        }
        //Write different function for patrol point logic *Done
        //Write raycast detection behavior with displaying cone effect *Done
        //Write what happens when player is detected *
    }

    private void FixedUpdate()
    {
        if (canPatrol && !waitAtPoint)
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            Vector2 moveVelocity = direction * currentSpeed;
            //rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            rb.velocity = new Vector2(moveVelocity.x, rb.velocity.y); //wtf is this
        }

        if (isHittingWall())
        {
            print("Hit Wall");
        }
    }

    private void Patrol()
    {
        currentTarget = patrolPoints[patrolIndex];
        if (patrolPoints.Length < 2)
        {
            Debug.LogWarning("Insufficient patrol points. Need at least 2.");
            return;
        }

        if (Vector3.Distance(transform.position, patrolPoints[patrolIndex].position) < nextWaypointDistance)
        {
            waitAtPoint = true;
            patrolIndex++;
            patrolIndex = patrolIndex % patrolPoints.Length; // modulo: remainder used to wrap to 0
            StartCoroutine(WaitAtPatrol(waitTime));
        }
    }

    IEnumerator WaitAtPatrol(float waitTime)
    {
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(waitTime);
        waitAtPoint = false;
    }

    bool isHittingWall()
    {
        bool foundWall = false;
        float castDistance = wallCheckDistance;
        //define cast distance for left and rightt
        
        if (!isFacingRight)
        {
            castDistance = -wallCheckDistance;
        }
        else
        {
            castDistance = wallCheckDistance;
        }

        //determine the target destination based on the cast distance
        Vector3 targetPos = wallCheckPos.position;
        targetPos.x += castDistance;

        if (Physics2D.Linecast(wallCheckPos.position, targetPos, 1 << LayerMask.NameToLayer("Ground")))
        {
            foundWall = true;
        }
        else
        {
            foundWall = false;
        }

        return foundWall;
    }

    void CheckForPlayerInFOV()
    {
        Vector3 directionToPlayer = PlayerDirection();
        float angleToPlayer = Vector3.Angle(transform.right, directionToPlayer);

        if (angleToPlayer < fovAngle * 0.5f)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, fovDetectDistance, LayerMask.GetMask("Player"));

            if (hit.collider != null)
            {
                // Check if there is an obstacle between the enemy and player
                RaycastHit2D obstacleHit = Physics2D.Raycast(transform.position, directionToPlayer, hit.distance, LayerMask.GetMask("Ground"));

                if (obstacleHit.collider == null)  //it will only stop chasing the player if it sees a wall and the player at the same time, this logic is wrong -> BUG
                {
                    Debug.Log("Player Detected!");
                    isChasing = true;
                    currentSpeed = chaseSpeed;
                    // Take appropriate action (e.g., chase the player)
                }
                else
                {
                    Debug.Log("Not Chasing");
                    isChasing = false;
                    currentSpeed = moveSpeed;
                    //Debug.Log("Wall Detected");
                    //NOTE: Need to set this logic outside of if laser hit player check
                }
            }
            else
            {
                isChasing = false;
                currentSpeed = moveSpeed;
            }
        }
    }

    Vector3 PlayerDirection()
    {
        return (player.position - transform.position).normalized;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    #region Gizmos
    void OnDrawGizmos()
    {
        DrawFOVGizmos();
    }

    void DrawFOVGizmos()
    {
        float halfFOV = fovAngle * 0.5f;
        float coneLength = Mathf.Tan(Mathf.Deg2Rad * halfFOV) * fovDetectDistance;

        Gizmos.color = Color.red;

        Vector3 coneStart = transform.position;
        Vector3 coneEnd = transform.position + (Quaternion.Euler(0, 0, halfFOV) * transform.right * fovDetectDistance);
        Gizmos.DrawLine(coneStart, coneEnd);

        coneEnd = transform.position + (Quaternion.Euler(0, 0, -halfFOV) * transform.right * fovDetectDistance);
        Gizmos.DrawLine(coneStart, coneEnd);

        Gizmos.DrawLine(coneStart, coneStart + transform.right * coneLength);
        Gizmos.DrawLine(coneStart, coneStart - transform.right * coneLength);
    }

    //private void OnDrawGizmos()
    //{
    //    //KongrooUtils.DrawGizmoCircle(transform.position, chaseRadius, Color.yellow);
    //    //KongrooUtils.DrawGizmoCircle(transform.position, attackRange, Color.red);
    //}
    #endregion
}

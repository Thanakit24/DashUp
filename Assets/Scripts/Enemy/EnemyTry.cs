using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTry : MonoBehaviour
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

    [Header("Detect & Chasing")]
    [SerializeField] private Transform castPos;
    [SerializeField] private float castDistance;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private bool canDetect;
    [SerializeField] private float detectRadius;    
    [SerializeField] private bool isChasing = false;
    [SerializeField] private BoxCollider2D detectCol;
   
    void Start()
    {
        currentTarget = player.transform;
        isFacingRight = true;
        currentSpeed = moveSpeed;
        detectCol = GetComponentInChildren<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        canDetect = Vector3.Distance(transform.position, player.position) <= detectRadius;
        
        if (!canDetect)
        {
            isChasing = false;
        }

        if (canPatrol && !isChasing)
        {
            Patrol();
        }
        //else if (isChasing)
        //{
        //    ChaseBehavior();
        //}


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
    }

    private void FixedUpdate()
    {
        if (canPatrol && !waitAtPoint)
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            Vector2 moveVelocity = direction * currentSpeed;
            //rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            rb.velocity = new Vector2(moveVelocity.x, rb.velocity.y); 
        }

        //if (isHittingWall())
        //{
        //    //print("Hit Wall");
        //}
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        //shoot a raycast here to check for walls later if true then return immediately
    //        //RaycastHit2D colliderHit = Physics2D.Raycast(transform.position, collision.gameObject.transform.position, 1000f, LayerMask.GetMask("Wall");
    //        isChasing = true;
    //        UpdatePath(player.transform);
    //        currentSpeed = chaseSpeed;
    //        //print("Chasing Player");
    //        //
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log(collision.gameObject.name);

                Vector3 rayDirection = collision.gameObject.transform.position - transform.position;
                Debug.DrawRay(castPos.position, rayDirection, Color.blue, 1f); // Draw the ray for debugging
               
                RaycastHit2D hit = Physics2D.Raycast(castPos.position, rayDirection, castDistance, wallLayer);

                if (hit.collider == null)
                {
                    print(hit.collider);
                    isChasing = true;
                    UpdatePath(player.transform);
                    currentSpeed = chaseSpeed;
                   // Debug.Log("Chasing Player");
                }
                else
                {
                    print(hit.collider);
                    //Debug.Log("Wall in the way, not chasing.");
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !canDetect)
        {
            {
                isChasing = false;
                currentSpeed = moveSpeed;
                print("Lost Player");
            }
        } 
    }


    // use box collider for detection instead 
    // if player is outside of box detection and range randius, then forget the player. 

    //RaycastHit2D playerHit = Physics2D.Raycast(castPos.position, transform.right, visionDistance, LayerMask.GetMask("Player"));
    //RaycastHit2D obstacleHit = Physics2D.Raycast(castPos.position, transform.right, visionDistance, LayerMask.GetMask("Ground"));

    //if (playerHit.collider != null)
    //{
    //    print(playerHit.collider);
    //    if (obstacleHit.collider != null)
    //        return;
    //    else
    //    {
    //        isChasing = true;
    //        UpdatePath(player.transform);
    //        currentSpeed = chaseSpeed;
    //    }
    //}



    private void UpdatePath(Transform targetLocation)
    {
        currentTarget = targetLocation;
        print(targetLocation);
        
    }

    private void Patrol()
    {
        currentSpeed = moveSpeed;
        UpdatePath(patrolPoints[patrolIndex]);
        //currentTarget = patrolPoints[patrolIndex];

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
 

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void OnDrawGizmos()
    {
        // Draw the chase radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);

        //// Draw the playerHit and obstacleHit raycasts
        //RaycastHit2D playerHit = Physics2D.Raycast(transform.position, transform.right, visionDistance, LayerMask.GetMask("Player"));
        //RaycastHit2D obstacleHit = Physics2D.Raycast(transform.position, transform.right, visionDistance, LayerMask.GetMask("Ground"));

        //// Draw playerHit ray
        ////Gizmos.color = Color.green;
        ////if (playerHit.collider != null)
        ////{
        ////    Gizmos.DrawLine(castPos.position, playerHit.point);
        ////    //Gizmos.DrawWireSphere(playerHit.point, 0.1f);
        ////}

        ////// Draw obstacleHit ray
        ////Gizmos.color = Color.blue;
        ////if (obstacleHit.collider != null)
        ////{
        ////    Gizmos.DrawLine(castPos.position, obstacleHit.point);
        ////    //Gizmos.DrawWireSphere(obstacleHit.point, 0.1f);
        ////}
    }


}

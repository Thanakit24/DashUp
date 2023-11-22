using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform currentTarget;

    [Header("Location Points")]
    [SerializeField] private float moveSpeed;
    public bool canMove = false;
    public Transform[] pathPoints;
    private int pathIndex = 1;
    [SerializeField] private float nextWaypointDistance = 2f;
    [SerializeField] private bool waitAtPoint = false;
    [SerializeField] private float waitTime;
 
    void Update()
    {
        if (canMove)
        {
            MoveToPoint();
        }
    }

    private void FixedUpdate()
    {
        if (canMove && !waitAtPoint)
        {
            //Move the platform
            transform.position = Vector2.MoveTowards(transform.position, currentTarget.transform.position, Time.deltaTime * moveSpeed);
        }
    }
    private void MoveToPoint()
    {
        UpdatePath(pathPoints[pathIndex]);

        if (pathPoints.Length < 2)
        {
            Debug.LogWarning("Insufficient patrol points. Need at least 2.");
            return;
        }

        if (Vector3.Distance(transform.position, pathPoints[pathIndex].position) < nextWaypointDistance)
        {
            waitAtPoint = true;
            pathIndex++;
            pathIndex = pathIndex % pathPoints.Length; // modulo: remainder used to wrap to 0
            StartCoroutine(WaitAtPoint(waitTime));
        }
    }

    private void UpdatePath(Transform targetLocation)
    {
        currentTarget = targetLocation;
        //print(targetLocation);
    }

    IEnumerator WaitAtPoint(float waitTime)
    {
        //rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(waitTime);
        waitAtPoint = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}

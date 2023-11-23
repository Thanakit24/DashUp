using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Location Points")]
    public bool canMove = false;
    [SerializeField] private float moveSpeed;
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
            transform.position = Vector2.MoveTowards(transform.position, pathPoints[pathIndex].transform.position, Time.deltaTime * moveSpeed);
        }
    }
    private void MoveToPoint()
    {
        if (pathPoints.Length < 2)
        {
            Debug.LogWarning("Insufficient patrol points. Need at least 2.");
            return;
        }

        if (Vector2.Distance(transform.position, pathPoints[pathIndex].position) < nextWaypointDistance)
        {
            waitAtPoint = true;
            pathIndex++;
            pathIndex = pathIndex % pathPoints.Length; // modulo: remainder used to wrap to 0
            StartCoroutine(WaitAtPoint(waitTime));
        }
    }
    IEnumerator WaitAtPoint(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        waitAtPoint = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            print("player landed on moving platform");
            collision.transform.SetParent(transform);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}

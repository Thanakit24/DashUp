using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float health;
    [SerializeField] private float moveSpeed;

    [SerializeField] private float detectRange;

    public bool canPatrol = false;
    public int currentPatrolPoints;
    public Transform[] patrolPoints;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Write different function for patrol point logic 
        //Write raycast detection behavior with displaying cone effect 
        //Write what happens when detected 
    }
}

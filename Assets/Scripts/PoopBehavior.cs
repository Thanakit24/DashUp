using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopBehavior : MonoBehaviour
{
    [SerializeField] private Vector2 velocity;
    [SerializeField] private float dropSpeed;
    [SerializeField] private float dropAcceleration;
    [SerializeField] private float poopDamage;
    [SerializeField] private float lifeTime;
    private Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        dropSpeed += dropAcceleration * Time.fixedDeltaTime;
        velocity.y = Mathf.MoveTowards(velocity.y, dropSpeed, dropAcceleration * Time.fixedDeltaTime);
        rb.velocity = -transform.up * velocity.y;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            print("destroy poop");
            //display explosive, on impact particles
            Destroy(gameObject);
        }
    }
}

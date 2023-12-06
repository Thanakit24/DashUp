using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopBehavior : MonoBehaviour
{
    private Vector2 velocity;
    [SerializeField] private float dropSpeed;
    [SerializeField] private float dropAcceleration;
    [SerializeField] private float poopDamage;
    [SerializeField] private float lifeTime;
    [SerializeField] private float destroyObjectTime;
    [SerializeField] private float explosionRadius;
    private TrailRenderer trail;
    public LayerMask groundMask;
    private Animator anim;
    private Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        trail = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        // Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        Destroy(gameObject, lifeTime);
        dropSpeed += dropAcceleration * Time.fixedDeltaTime;
        velocity.y = Mathf.MoveTowards(velocity.y, dropSpeed, dropAcceleration * Time.fixedDeltaTime);
        rb.velocity = -transform.up * velocity.y;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Obstacle"))
        {
            //print("destroy poop");
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            AudioManager.instance.Play("Poop");
            anim.SetTrigger("PoopExplode");
            //display explosive, on impact particles

        }

       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Breakable"))
        {
            bool obstacleHit = Physics2D.OverlapCircle(transform.position, explosionRadius, groundMask);
            if (obstacleHit)
            {
                print("found bbreakable obstacle");
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0;
                rb.constraints = RigidbodyConstraints2D.FreezePosition;
                AudioManager.instance.Play("Poop");
                anim.SetTrigger("PoopExplode");
                //Destroy(collision.gameObject, destroyObjectTime);
            }
        }
    }

    public void DisableTrail()
    {
        trail.enabled = false;
    }

    public void EnableTrail()
    {
        trail.enabled = true;
    }
    public void DestroyPoop() //called by animation event
    {
       
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

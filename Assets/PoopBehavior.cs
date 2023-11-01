using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopBehavior : MonoBehaviour
{
    [SerializeField] private float poopSpeed;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        rb.velocity = -transform.up * poopSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            print("destroy poop");
            Destroy(gameObject);
        }
    }
}

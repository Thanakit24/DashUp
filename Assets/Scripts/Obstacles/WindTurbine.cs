using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTurbine : MonoBehaviour
{
    [SerializeField] private float upwardForce;
    private BoxCollider2D boxCol;
    // Start is called before the first frame update
    void Start()
    {
        boxCol = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //print("Player Entered Wind");
            Rigidbody2D rbAffect = collision.gameObject.GetComponent<Rigidbody2D>();
            rbAffect.velocity = Vector2.zero;
            rbAffect.gravityScale = 0f;
            rbAffect.AddForce(Vector2.up * upwardForce, ForceMode2D.Force);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //print("Player Exit Wind");
            Rigidbody2D rbAffect = collision.gameObject.GetComponent<Rigidbody2D>();
            rbAffect.velocity = Vector2.zero;
            rbAffect.gravityScale = 1f;
            
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTurbine : MonoBehaviour
{
    [SerializeField] private float upwardForce;
    private BoxCollider2D boxCol;
    [SerializeField] private float reductSpeed;
    [SerializeField] private float reductJump;
    // Start is called before the first frame update
    void Start()
    {
        boxCol = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.flyUpwardSpeed /= reductSpeed;
            player.jumpPower -= reductJump;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //print("Player Entered Wind");
              PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            Rigidbody2D rbAffect = collision.gameObject.GetComponent<Rigidbody2D>();
            rbAffect.velocity = Vector2.zero;
            rbAffect.gravityScale = 0f;
            rbAffect.AddForce(-player.rb.transform.up * upwardForce, ForceMode2D.Force);
            //player.rb.transform.up
        }
    }
                                                                                                                                                    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //print("Player Exit Wind");                                                                                                                                                                                                                                                                                                                                
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            Rigidbody2D rbAffect = collision.gameObject.GetComponent<Rigidbody2D>();
            //rbAffect.velocity = Vector2.zero;
            player.frameVelocity.y = 0;                                                                                                                 
            player.jumpPower = 13.2f;
            player.flyUpwardSpeed = 250f;
            rbAffect.gravityScale = 1f;
            
        }
    }
}

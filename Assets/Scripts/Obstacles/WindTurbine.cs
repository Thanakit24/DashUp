using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Create a class to store original player values
[System.Serializable]
public class PlayerOriginalValues
{
    public float glideFallSpeed;
    public float glideFallAcceleration;
    public float fallAcceleration;
    public float flyUpwardSpeed;
    public float maxFlySpeed;
    public float jumpPower;
}

public class WindTurbine : MonoBehaviour
{
    [SerializeField] private float downwardForce;
    [SerializeField] private float upwardForce;
    [SerializeField] private float upwardForceMax;
    [SerializeField] private float impulseForce;
    [SerializeField] private bool facingUp = false;

    [Header("MOVEMENT")]
    private BoxCollider2D boxCol;
    [SerializeField] private float reductSpeed;
    [SerializeField] private float reductJump;

    // Create an instance of the class to store original player values
    private PlayerOriginalValues originalValues = new PlayerOriginalValues();

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

            // Store the original values when the player enters the trigger
            StoreOriginalValues(player);
            //if (!facingUp)
            //{
            //    player.rb.AddForce(-player.rb.transform.up * impulseForce, ForceMode2D.Impulse);
            //}
            //else
            //{
            //    player.rb.AddForce(player.rb.transform.up * impulseForce, ForceMode2D.Impulse);
            //}
           
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            Rigidbody2D rbAffect = collision.gameObject.GetComponent<Rigidbody2D>();
            rbAffect.velocity = Vector2.zero;
            rbAffect.gravityScale = 0f;

            if (!facingUp)
            {
                rbAffect.AddForce(-player.rb.transform.up * downwardForce, ForceMode2D.Force);
                //player.frameVelocity.y = Mathf.MoveTowards(player.frameVelocity.y, -upwardForce, upwardForceMax * Time.fixedDeltaTime);
                //player.flyUpwardSpeed = 125f;
                player.maxFlySpeed = 4f;
                //player.glideFallSpeed = 80f;
                //player.glideFallAcceleration = 250f;
            }
            else
            {
                //rbAffect.AddForce(player.rb.transform.up * upwardForce, ForceMode2D.Force);
                player.frameVelocity.y = Mathf.MoveTowards(player.frameVelocity.y, upwardForce, upwardForceMax * Time.fixedDeltaTime); //doing any actions in rb add upward force creates weird and buggy movement interactions
                player.flyUpwardSpeed = 400f; 
                player.glideFallAcceleration = 0;
                player.glideFallSpeed = 0;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            Rigidbody2D rbAffect = collision.gameObject.GetComponent<Rigidbody2D>();

            RestoreOriginalValues(player);
            player.jumpPower = originalValues.jumpPower;
            player.flyUpwardSpeed = originalValues.flyUpwardSpeed;
            rbAffect.gravityScale = 1f;
        }
    }

    // Function to store original player values
    private void StoreOriginalValues(PlayerController player)
    {
        originalValues.glideFallSpeed = player.glideFallSpeed;
        originalValues.glideFallAcceleration = player.glideFallAcceleration;
        originalValues.fallAcceleration = player.fallAcceleration;
        originalValues.flyUpwardSpeed = player.flyUpwardSpeed;
        originalValues.maxFlySpeed = player.maxFlySpeed;
        originalValues.jumpPower = player.jumpPower;
    }

    // Function to restore original player values
    private void RestoreOriginalValues(PlayerController player)
    {
        player.glideFallSpeed = originalValues.glideFallSpeed;
        player.glideFallAcceleration = originalValues.glideFallAcceleration;
        player.fallAcceleration = originalValues.fallAcceleration;
        player.flyUpwardSpeed = originalValues.flyUpwardSpeed;
        player.maxFlySpeed = originalValues.maxFlySpeed;
        player.jumpPower = originalValues.jumpPower;
    }
}

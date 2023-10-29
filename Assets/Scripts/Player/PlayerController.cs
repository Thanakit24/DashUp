using System;
using UnityEngine;

namespace unixel
{
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        public LayerMask playerLayer;
        private Rigidbody2D rb;
        private CapsuleCollider2D col;

        [Header("MOVEMENT")]
        private Vector2 moveDirection;
        public float maxSpeed;
        public float acceleration;
        public float groundDeceleration;
        public float airDeceleration;
        public float maxFallSpeed;
        public float fallAcceleration;
        public float groundingForce;

        [Header("GROUND & COLLISION CHECKS")]
        public LayerMask groundLayerMask;
        public float grounderDistance;
        public float JumpEndEarlyGravityModifier;
        private bool _cachedQueryStartInColliders;

        [Header("JUMPING")]
        public float jumpPower;
        private bool jumpToConsume;
        private float timeJumpWasPressed;
        private bool bufferedJumpUsable;
        public float jumpBuffer;
        private bool endedJumpEarly;
        private bool coyoteUsable;
        public float coyoteTime;

        [Header("GLIDING")]
        public float maxGlideSpeed;
        public float glideAcceleration;
        public float glideFallSpeed;
        public float glideFallAcceleration;
        public float glideGravityResistance;

        private Vector2 frameVelocity;
        private FrameInput frameInput;
        public bool isFacingRight = true;

        private float time;

        //Interface
        public Vector2 FrameInput => frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;

        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<CapsuleCollider2D>();

            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }

        // Update is called once per frame
        void Update()
        {
            time += Time.deltaTime;
            GatherInput();
            //if (frameInput.isGliding)
            //{
            //    print("in gliding state");
            //}
        }


        private void GatherInput()
        {
            frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump"),
                JumpHeld = Input.GetButton("Jump"),
                isGliding = Input.GetKey(KeyCode.LeftShift),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };

            if (frameInput.JumpDown)
            {
                jumpToConsume = true;
                timeJumpWasPressed = time;
            }

            FlipSprite();
        }

        private void FixedUpdate()
        {
            CheckCollisions();
            HandleJump();
            HandleDirection();
            HandleGravity();

            ApplyMovement();
        }

        private void HandleDirection()
        {
            if (frameInput.isGliding)
            {
                frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, frameInput.Move.x * maxGlideSpeed, glideAcceleration * Time.fixedDeltaTime);
            }

            else if  (frameInput.Move.x == 0)
            {
                var deceleration = grounded ? groundDeceleration : airDeceleration;
                // this line of code is setting the deceleration variable to either groundDeceleration or airDeceleration
                // based on whether the character is currently on the ground (grounded).
                // If grounded is true, it will use groundDeceleration; otherwise, it will use airDeceleration
                frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, frameInput.Move.x * maxSpeed, acceleration * Time.fixedDeltaTime);
            }
        }

        private void HandleGravity()
        {
            if (grounded && frameVelocity.y <= 0f)
            {
                frameVelocity.y = groundingForce;
            }
            else if (frameInput.isGliding)
            {
                frameVelocity.y /= glideGravityResistance;
                frameVelocity.y = Mathf.MoveTowards(frameVelocity.y, -glideFallSpeed, glideFallAcceleration * Time.fixedDeltaTime);
            }
            else
            {
                var inAirGravity = fallAcceleration;
                if (endedJumpEarly && frameVelocity.y > 0) inAirGravity *= JumpEndEarlyGravityModifier;
                frameVelocity.y = Mathf.MoveTowards(frameVelocity.y, -maxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
            
        }
        private void ApplyMovement() => rb.velocity = frameVelocity;


        #region Collisions

        private float frameLeftGrounded = float.MinValue;
        private bool grounded;

        private void CheckCollisions() //confusing on the GroundChanged?
        {
            Physics2D.queriesStartInColliders = false;

            // Ground and Ceiling
            bool groundHit = Physics2D.CapsuleCast(col.bounds.center, col.size, col.direction, 0, Vector2.down, grounderDistance, ~playerLayer);
            bool ceilingHit = Physics2D.CapsuleCast(col.bounds.center, col.size, col.direction, 0, Vector2.up, grounderDistance, ~playerLayer);

            // Hit a Ceiling
            if (ceilingHit) frameVelocity.y = Mathf.Min(0, frameVelocity.y);

            // Landed on the Ground
            if (!grounded && groundHit)
            {
                grounded = true;
                coyoteUsable = true;
                bufferedJumpUsable = true;
                endedJumpEarly = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(frameVelocity.y)); //idk wtf this is
            }
            // Left the Ground
            else if (grounded && !groundHit)
            {
                grounded = false;
                frameLeftGrounded = time;
                GroundedChanged?.Invoke(false, 0);
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;

        }
        #endregion
        #region Jumping

        [SerializeField] private bool HasBufferedJump => bufferedJumpUsable && time < timeJumpWasPressed + jumpBuffer; //buffering jump before performing next jump
        [SerializeField] private bool CanUseCoyote => coyoteUsable && !grounded && time < frameLeftGrounded + coyoteTime; 

        private void HandleJump() //Jumping conditions 
        {
            if (!endedJumpEarly && !grounded && !frameInput.JumpHeld && rb.velocity.y > 0) 
                endedJumpEarly = true;

            if (!jumpToConsume && !HasBufferedJump) 
                return;

            if (!frameInput.isGliding && jumpToConsume && grounded || CanUseCoyote) 
                ExecuteJump();

            jumpToConsume = false;
        }

        private void ExecuteJump() //Where jump happens
        {
            endedJumpEarly = false;
            timeJumpWasPressed = 0;
            bufferedJumpUsable = false;
            coyoteUsable = false;
            frameVelocity.y = jumpPower;
            Jumped?.Invoke();
        }

        #endregion
        private void FlipSprite()
        {
            if (isFacingRight && moveDirection.x < 0f || !isFacingRight && moveDirection.x > 0f)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }

    }
    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public bool isGliding;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;

        public event Action Jumped;
        public Vector2 FrameInput { get; }
    }
}

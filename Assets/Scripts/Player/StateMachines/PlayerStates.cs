using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStates : BaseState
{
    public PlayerController _pc;

    public PlayerStates(PlayerController pc) : base(pc)
    {
        _pc = pc;
    }
    // Start is called before the first frame update
}

public class GroundMoveState : PlayerStates
{
    public GroundMoveState(PlayerController pc) : base(pc) { }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if (!_pc.grounded && _pc.currentState is not JumpState)
        {
            _pc.ChangeState(new AirborneMoveState(_pc));
            return;
        }
        //HorizontalDirection on Ground
        if (_pc.frameInput.Move.x == 0)
        {
            _pc.frameVelocity.x = Mathf.MoveTowards(_pc.frameVelocity.x, 0, _pc.groundDeceleration * Time.fixedDeltaTime);
            _pc.anim.Play(PlayerController.IdleKey);
        }
        else
        {
            _pc.frameVelocity.x = Mathf.MoveTowards(_pc.frameVelocity.x, _pc.frameInput.Move.x * _pc.maxSpeed, _pc.acceleration * Time.fixedDeltaTime);
            _pc.anim.Play(PlayerController.MoveKey);
        }

        if (_pc.grounded && _pc.frameVelocity.y <= 0f)
            _pc.frameVelocity.y = _pc.groundingForce;

    }
}
public class JumpState : AirborneMoveState
{
    public JumpState(PlayerController pc) : base(pc)

    { duration = 0.3f; }

    public override void OnEnter()
    {
        //gets stuck in jump state for the second jump, setting duration fixes this
        //Debug.Log("Called JumpState");
        base.OnEnter();
        _pc.bufferedState = new AirborneMoveState(_pc);
        _pc.endedJumpEarly = false;
        _pc.timeJumpWasPressed = 0;
        _pc.bufferedJumpUsable = false;
        _pc.coyoteUsable = false;
        _pc.frameInput.isGliding = false;
        _pc.frameInput.isFlying = false;

        if (_pc.amountOfPoop > 0)
        {
            _pc.amountOfPoop--;
            _pc.frameVelocity.y = _pc.poopPower;
            _pc.anim.Play(PlayerController.BlackJumpkey);
            _pc.blackJump = true;
            GameObject poop = GameObject.Instantiate(_pc.poopPrefab, _pc.poopDropPos.position, Quaternion.identity);  

        }
        else
        {
            _pc.amountOfJumps--;
            _pc.frameVelocity.y = _pc.jumpPower;
            _pc.anim.Play(PlayerController.JumpKey);
        }
    }
}
public class AirborneMoveState : PlayerStates
{
    public AirborneMoveState(PlayerController pc) : base(pc) { }

    public override void OnEnter()
    {
        base.OnEnter();

    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!(_pc.currentState is GlideState || _pc.currentState is FlyState))
        {
            if (_pc.rb.velocity.y <= 0.01f)
            {
                if (_pc.blackJump)
                {
                    _pc.blackJump = false;
                    _pc.anim.Play(PlayerController.FallKey);
                }
                else
                {
                    _pc.anim.Play(PlayerController.FallKey);
                }
                
            }
        }

    }
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        HandleAirDirection();
        HandleGravity();
        if (_pc.grounded)
        {
            _pc.ChangeState(new GroundMoveState(_pc));
        }
    }

    private void HandleAirDirection()
    {
        //HorizontalDirection in Air -------------------
        if (_pc.currentState is GlideState || _pc.currentState is FlyState)
        {
            return;
        }
        else if (_pc.frameInput.Move.x == 0)
        {
            _pc.frameVelocity.x = Mathf.MoveTowards(_pc.frameVelocity.x, 0, _pc.airDeceleration * Time.fixedDeltaTime);
        }
        else
        {
            _pc.frameVelocity.x = Mathf.MoveTowards(_pc.frameVelocity.x, _pc.frameInput.Move.x * _pc.maxSpeed, _pc.acceleration * Time.fixedDeltaTime);
        }

    }

    private void HandleGravity()
    {
        ////Gravity ----------------------------------
        if (_pc.currentState is GlideState || _pc.currentState is FlyState)
        {
            return;
        }
        else
        {
            _pc.frameInput.isFlying = false;
            var inAirGravity = _pc.fallAcceleration;
            if (_pc.endedJumpEarly && _pc.frameVelocity.y > 0) inAirGravity *= _pc.JumpEndEarlyGravityModifier;
            _pc.frameVelocity.y = Mathf.MoveTowards(_pc.frameVelocity.y, -_pc.maxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }
}
public class GlideState : AirborneMoveState
{
    public GlideState(PlayerController pc) : base(pc) { }

    public override void OnEnter()
    {
        base.OnEnter();
        _pc.anim.Play(PlayerController.GlideKey);

        //set anim glide to play here
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!_pc.frameInput.isGliding)
        {
            _pc.ChangeState(new AirborneMoveState(_pc));
        }
    }
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if (_pc.currentState is JumpState)
            return;

        //_pc.currentEnergy -= _pc.depleteEnergy * Time.deltaTime;
        _pc.frameInput.isFlying = false;
        _pc.frameVelocity.x = Mathf.MoveTowards(_pc.frameVelocity.x, _pc.frameInput.Move.x * _pc.maxGlideSpeed, _pc.glideAcceleration * Time.fixedDeltaTime);
        _pc.frameVelocity.y /= _pc.glideGravityResistance;
        _pc.frameVelocity.y = Mathf.MoveTowards(_pc.frameVelocity.y, -_pc.glideFallSpeed, _pc.glideFallAcceleration * Time.fixedDeltaTime);
    }
}
public class FlyState : AirborneMoveState
{
    public FlyState(PlayerController pc) : base(pc) { }

    public override void OnEnter()
    {
        base.OnEnter();
        //set anim glide to play here
        _pc.anim.Play(PlayerController.FlyKey);

    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!_pc.frameInput.isFlying)
        {
            _pc.ChangeState(new AirborneMoveState(_pc));
        }
    }
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if (_pc.currentState is JumpState)
            return;

        //ui.buffIcons[i].color = new Color(1, 1, 1, KongrooUtils.RemapRange(buffs[buffDetails], 0, buffDetails.duration, 0, 1));

        _pc.currentEnergy -= _pc.depleteEnergy * Time.deltaTime;
        _pc.frameInput.isGliding = false;
        _pc.rb.velocity = Vector2.zero;
        _pc.frameVelocity.x = Mathf.MoveTowards(_pc.frameVelocity.x, _pc.frameInput.Move.x * _pc.maxFlySpeed, _pc.flyAcceleration * Time.fixedDeltaTime);
        _pc.frameVelocity.y /= _pc.airDownwardForce;
        _pc.frameVelocity.y = Mathf.MoveTowards(_pc.frameVelocity.y, _pc.flyUpwardSpeed, _pc.flyUpwardSpeed * Time.fixedDeltaTime);
    }

   
}

public class DeathState : PlayerStates
{
    public DeathState(PlayerController pc) : base(pc) { }

    public override void OnEnter()
    {
        base.OnEnter();
        _pc.sprite.color = new Color(255, 255, 255);
        _pc.energyBar.gameObject.SetActive(false);
        _pc.trail.emitting = false;
        _pc.rb.constraints = RigidbodyConstraints2D.FreezePosition;
        _pc.enabled = false;
        //add camera shake
        //explore different anims
        _pc.anim.Play(PlayerController.DeathKey);
        GameManager.instance.GameOver();

    }

}



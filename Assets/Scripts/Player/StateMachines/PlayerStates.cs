using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (!_pc.grounded)
        {
            _pc.ChangeState(new AirborneMoveState(_pc));
        }
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if (_pc.frameInput.Move.x == 0)
        {
            _pc.frameVelocity.x = Mathf.MoveTowards(_pc.frameVelocity.x, 0, _pc.groundDeceleration * Time.fixedDeltaTime);
        }
        else
        {
            _pc.frameVelocity.x = Mathf.MoveTowards(_pc.frameVelocity.x, _pc.frameInput.Move.x * _pc.maxSpeed, _pc.acceleration * Time.fixedDeltaTime);
        }

        if (_pc.grounded && _pc.frameVelocity.y <= 0f)
            _pc.frameVelocity.y = _pc.groundingForce;


        ApplyMovement();
    }

    private void ApplyMovement() => _pc.rb.velocity = _pc.frameVelocity;
}

public class JumpState : AirborneMoveState
{
    public JumpState(PlayerController pc) : base(pc) 
    { duration = 0.3f; }
   
    public override void OnEnter()
    {
        base.OnEnter();
        _pc.endedJumpEarly = false;
        _pc.timeJumpWasPressed = 0;
        _pc.bufferedJumpUsable = false;
        _pc.coyoteUsable = false;
        _pc.frameInput.isGliding = false;
        _pc.frameVelocity.y = _pc.jumpPower;
    }
}
public class AirborneMoveState : PlayerStates
{
    public AirborneMoveState(PlayerController pc) : base(pc) { }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (_pc.grounded)
        {
            _pc.ChangeState(new GroundMoveState(_pc));
        }
    }
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        //Horizontal Direction ----------------
        //if (_pc.rb.velocity.y <= -0.1f)
        //    _pc.ChangeState(new FallState(_pc));

        if (_pc.frameInput.isGliding)
            _pc.ChangeState(new GlideState(_pc));
        else
        {
            _pc.frameVelocity.x = Mathf.MoveTowards(_pc.frameVelocity.x, _pc.frameInput.Move.x * _pc.maxSpeed, _pc.acceleration * Time.fixedDeltaTime);
        }

        //Gravity ----------------------------------
        if (_pc.frameInput.isGliding)
            _pc.ChangeState(new GlideState(_pc));
        else
        {
            var inAirGravity = _pc.fallAcceleration;
            if (_pc.endedJumpEarly && _pc.frameVelocity.y > 0) inAirGravity *= _pc.JumpEndEarlyGravityModifier;
            _pc.frameVelocity.y = Mathf.MoveTowards(_pc.frameVelocity.y, -_pc.maxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }


        ApplyMovement();
    }

    private void ApplyMovement() => _pc.rb.velocity = _pc.frameVelocity;
}

//public class FallState : AirMoveState
//{
//    public FallState(PlayerController pc) : base(pc) { }

//    public override void OnUpdate()
//    {
//        base.OnUpdate();
//        if (_pc.grounded)
//        {
//            _pc.ChangeState(new GroundMoveState(_pc));
//        }
//    }
//}

public class GlideState : AirborneMoveState
{
    public GlideState(PlayerController pc) : base(pc) { }

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
        _pc.frameVelocity.x = Mathf.MoveTowards(_pc.frameVelocity.x, _pc.frameInput.Move.x * _pc.maxGlideSpeed, _pc.glideAcceleration * Time.fixedDeltaTime);
        _pc.frameVelocity.y /= _pc.glideGravityResistance;
        _pc.frameVelocity.y = Mathf.MoveTowards(_pc.frameVelocity.y, -_pc.glideFallSpeed, _pc.glideFallAcceleration * Time.fixedDeltaTime);

        ApplyMovement();
    }

    private void ApplyMovement() => _pc.rb.velocity = _pc.frameVelocity;
}


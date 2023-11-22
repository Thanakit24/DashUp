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

        //HorizontalDirection on Ground
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

        _pc.anim.SetFloat("xVelocity", Mathf.Abs(_pc.frameInput.Move.x));
        
    }
}

public class JumpState : AirborneMoveState
{
    public JumpState(PlayerController pc) : base(pc)
    { duration = 0.1f; }

    public override void OnEnter()
    {
        //gets stuck in jump state for the second jump
        Debug.Log("Called JumpState");
        base.OnEnter();
        _pc.bufferedState = new AirborneMoveState(_pc);
        _pc.amountOfJumps--;
        _pc.endedJumpEarly = false;
        _pc.timeJumpWasPressed = 0;
        _pc.bufferedJumpUsable = false;
        _pc.frameInput.isGliding = false;
        _pc.frameVelocity.y = _pc.jumpPower;
        _pc.jumpToConsume = false;
        //_pc.ChangeState(new AirborneMoveState(_pc));
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
        //_pc.anim.SetFloat("yVelocity", _pc.rb.velocity.y);
        HandleAirDirection();
        HandleGravity();
        _pc.anim.SetFloat("yVelocity", _pc.rb.velocity.y);
    }

    private void HandleAirDirection()
    {
        //HorizontalDirection in Air -------------------
        if (_pc.currentState is GlideState)
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
        if (_pc.currentState is GlideState)
        {
            return;
        }
        else
        {
            var inAirGravity = _pc.fallAcceleration;
            if (_pc.endedJumpEarly && _pc.frameVelocity.y > 0) inAirGravity *= _pc.JumpEndEarlyGravityModifier;
            _pc.frameVelocity.y = Mathf.MoveTowards(_pc.frameVelocity.y, -_pc.maxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }
}
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
        //Debug.Log(_pc.frameVelocity.x);
        //Debug.Log((_pc.frameInput.Move.x * _pc.maxGlideSpeed, _pc.glideAcceleration * Time.fixedDeltaTime));
        _pc.frameVelocity.y /= _pc.glideGravityResistance;
        _pc.frameVelocity.y = Mathf.MoveTowards(_pc.frameVelocity.y, -_pc.glideFallSpeed, _pc.glideFallAcceleration * Time.fixedDeltaTime);
    }

}


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
        //gets stuck in jump state for the second jump, setting duration fixes this
        //Debug.Log("Called JumpState");
        base.OnEnter();
        _pc.bufferedState = new AirborneMoveState(_pc);
        //_pc.amountOfJumps--;
        _pc.endedJumpEarly = false;
        _pc.timeJumpWasPressed = 0;
        _pc.bufferedJumpUsable = false;
        _pc.coyoteUsable = false;
        _pc.frameInput.isGliding = false;

        if (_pc.amountOfPoop > 0)
        {
            _pc.amountOfPoop--;
            _pc.frameVelocity.y = _pc.poopPower;
            GameObject poop = GameObject.Instantiate(_pc.poopPrefab, _pc.poopDropPos.position, Quaternion.identity);
        }
        else
        {
            _pc.amountOfJumps--;
            _pc.frameVelocity.y = _pc.jumpPower;
        }
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
        HandleAirDirection();
        HandleGravity();
        _pc.anim.SetFloat("yVelocity", _pc.rb.velocity.y);
    }

    private void HandleAirDirection()
    {
        //HorizontalDirection in Air -------------------
        if (_pc.currentState is FlightState)
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
        if (_pc.currentState is FlightState)
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
public class FlightState : AirborneMoveState
{
    public FlightState(PlayerController pc) : base(pc) { }

    public override void OnEnter()
    {
        base.OnEnter();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!_pc.frameInput.isGliding)
        {
            //_pc.rb.gravityScale = 1;
            _pc.ChangeState(new AirborneMoveState(_pc));
        }
    }
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        //Debug.Log(_pc.currentEnergy);
        if (_pc.currentState is JumpState)
            return;

        _pc.currentEnergy -= _pc.depleteEnergy * Time.deltaTime;
        
        if (_pc.frameInput.isFlying)
        {
            _pc.frameInput.isGliding = false;
            _pc.rb.velocity = Vector2.zero;
            _pc.frameVelocity.x = Mathf.MoveTowards(_pc.frameVelocity.x, _pc.frameInput.Move.x * _pc.maxFlySpeed, _pc.flyAcceleration * Time.fixedDeltaTime);
            _pc.frameVelocity.y /= _pc.airDownwardForce;
            _pc.frameVelocity.y = Mathf.MoveTowards(_pc.frameVelocity.y, _pc.flyUpwardSpeed, _pc.flyUpwardSpeed * Time.fixedDeltaTime);
        }
        else if (_pc.frameInput.isGliding)
        {
            _pc.frameInput.isFlying = false;
            _pc.frameVelocity.x = Mathf.MoveTowards(_pc.frameVelocity.x, _pc.frameInput.Move.x * _pc.maxGlideSpeed, _pc.glideAcceleration * Time.fixedDeltaTime);
            _pc.frameVelocity.y /= _pc.glideGravityResistance;
            _pc.frameVelocity.y = Mathf.MoveTowards(_pc.frameVelocity.y, -_pc.glideFallSpeed, _pc.glideFallAcceleration * Time.fixedDeltaTime);
        }
    }
}



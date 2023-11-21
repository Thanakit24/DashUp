using UnityEngine;

public class BaseState
{
    [HideInInspector] public StateMachine sm;
    [SerializeField] public string stateName = "Base State";
    public float duration = 0;
    public float age = float.MaxValue;

    protected float _elapsedTime => duration - age;

    public BaseState(StateMachine sm)
    {
        this.sm = sm;
        stateName = GetType().Name;
    }
    public virtual void Awake()
    {
    }
    public virtual void OnEnter()
    {
        if (duration != 0)
            age = duration;
    }
    public virtual void OnUpdate()
    {
        if (duration == 0) return;
        age -= Time.deltaTime;
        if (age <= 0)
        {
            if (sm.bufferedState == null)
                sm.ChangeState(sm.DefaultState());
            else
            {
                BaseState tempState = sm.bufferedState;
                sm.bufferedState = null;
                sm.ChangeState(tempState);
            }
        }
    }
    public virtual void OnFixedUpdate()
    {
    }

    public virtual void OnExit()
    {
    }
    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
    }
    //public virtual void OnTriggerEnter2D(Collider2D collision)
    //{
    //}
}
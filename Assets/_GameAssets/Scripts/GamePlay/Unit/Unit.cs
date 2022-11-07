
using System;
using DG.Tweening;
using Kryz.CharacterStats;
using Lean.Pool;
using MonsterLove.StateMachine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    #region fsm
    public class Driver
    {
        public StateEvent Update;
    }
    public enum State
    {
        Idle,
        Move,
        Attack,
        Dead,
    }
    public StateMachine<State, Driver> fsm;
    protected virtual void Awake()
    {
        fsm = new StateMachine<State, Driver>(this);
    }
    protected virtual void Update()
    {
        fsm.Driver.Update.Invoke();
    }
    protected virtual void Idle_Enter()
    {
    }
    protected virtual void Idle_Update()
    {
    }
    protected virtual void Move_Enter()
    {
    }
    protected virtual void Move_Update()
    {
    }
    protected virtual void Attack_Enter()
    {
    }
    protected virtual void Attack_Update()
    {
    }
    protected virtual void Dead_Enter()
    {
    }
    #endregion
    
    public TeamMgr team;
    [SerializeField] protected Transform hpSlider;
    [SerializeField,HideInInspector] private float hp;
    [ShowInInspector]
    public float Hp
    {
        get => hp;
        set
        {
            hp = Mathf.Clamp(value, 0, maxHp.Value);
            if (hpSlider != null)
            {
                hpSlider.gameObject.SetActive(hp < maxHp.Value);
                hpSlider.GetChild(0).DOScaleX(hp / maxHp.Value, 0.1f);
            }
        }
    }
    public CharacterStat atk = new CharacterStat(10);
    public CharacterStat atkRange = new CharacterStat(1);
    public CharacterStat atkSpeed = new CharacterStat(5);
    public CharacterStat maxHp = new CharacterStat(100);
    public virtual void DealDame(Unit source)
    {
        Hp -= source.atk.Value;
        FlyText.Spawn(hpSlider.position, source.atk.Value.ToString("00"));
        if (hp <= 0) fsm.ChangeState(State.Dead);
    }
}

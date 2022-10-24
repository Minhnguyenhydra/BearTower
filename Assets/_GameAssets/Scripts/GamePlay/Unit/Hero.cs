using System;
using Kryz.CharacterStats;
using Lean.Pool;
using Spine.Unity;
using UnityEngine;

public class Hero : Unit
{
    public enum AttackType
    {
        Melee,
        Ranged,
    }
    public Vector3 idlePosition;
    protected bool IsStandAtIdlePosition => Vector3.Distance(transform.position, idlePosition) < 0.1f;
    public AttackType attackType = AttackType.Melee;
    public CharacterStat moveSpeed = new CharacterStat(5);
    [SerializeField] private SkeletonAnimation _anim;
    [SpineAnimation] public string idle;
    [SpineAnimation] public string attack;
    [SpineAnimation] public string move;
    [SpineAnimation] public string hit;
    [SpineAnimation] public string die;
    
    protected override void Idle_Enter()
    {
        _anim.AnimationState.SetAnimation(0, idle, true);
        transform.GetChild(0).localScale = new Vector3(-team.castle.transform.GetChild(0).localScale.x, 1, 1);
    }
    protected override void Idle_Update()
    {
        if (team.fsm.State is TeamMgr.State.Back or TeamMgr.State.Defense && !IsStandAtIdlePosition
            || team.fsm.State is TeamMgr.State.Attack)
        {
            fsm.ChangeState(State.Move);
        }
    }
    protected override void Move_Enter()
    {
        _anim.AnimationState.SetAnimation(0, move, true);
    }

    protected override void Move_Update()
    {
        if (team.fsm.State is TeamMgr.State.Back or TeamMgr.State.Defense)
        {
            if (MoveToPos(idlePosition))
            {
                fsm.ChangeState(State.Idle);
            }
        }
        if (team.fsm.State is TeamMgr.State.Attack)
        {
            if (MoveToPos(team.enemyTeam.castle.transform.position))
            {
                fsm.ChangeState(State.Attack);
            }
        }
    }

    protected virtual bool MoveToPos(Vector3 pos)
    {
        var dir = pos - transform.position;
        var reached = dir.magnitude < 0.1f;
        if (!reached) transform.GetChild(0).localScale = new Vector3(-Mathf.Sign(dir.x), 1, 1);
        transform.position += dir.normalized * moveSpeed.Value * Time.deltaTime;
        if (reached) transform.position = pos;
        return reached;
    }
    
    protected override void Attack_Enter()
    {
        _anim.AnimationState.SetAnimation(0, attack, true);
    }
    protected override void Attack_Update()
    {
        if (team.fsm.State is not TeamMgr.State.Attack)
        {
            fsm.ChangeState(State.Move);
        }
    }
    protected override void Dead_Enter()
    {
        team.RemoveHero(this);
        _anim.AnimationState.SetAnimation(0, die, false);
        LeanPool.Despawn(this, 1f);
    }
    public override void DealDame(Unit source)
    {
        _anim.AnimationState.SetAnimation(1, hit, false);
        base.DealDame(source);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(idlePosition, Vector3.one);
        Gizmos.DrawWireSphere(transform.position, atkRange.Value);
    }
}

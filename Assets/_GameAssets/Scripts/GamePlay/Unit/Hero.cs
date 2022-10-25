using Kryz.CharacterStats;
using Lean.Pool;
using Sirenix.OdinInspector;
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
    [SerializeField] private SkeletonAnimation anim;
    [SpineAnimation,SerializeField] private string idle;
    [SpineAnimation,SerializeField] private string attack;
    [SpineAnimation,SerializeField] private string move;
    [SpineAnimation,SerializeField] private string hit;
    [SpineAnimation,SerializeField] private string die;
    protected Unit curTarget;

    [SerializeField] private float attackAnimDuration = 1.5f;
    [SerializeField,Range(0,1)] private float attackAnimEvent=0.6f;
    protected float lastTimeStartAttack;
    protected bool isAttacked;
    protected bool IsAttacking=>Time.time-lastTimeStartAttack<attackAnimDuration / atkSpeed.Value;
    protected override void Idle_Enter()
    {
        anim.timeScale = 1;
        anim.AnimationState.SetAnimation(0, idle, true);
        transform.GetChild(0).localScale = new Vector3(-team.FaceDirection, 1, 1);
    }
    protected override void Idle_Update()
    {
        curTarget = FindTarget();
        if (curTarget != null || !IsStandAtIdlePosition)
            fsm.ChangeState(State.Move);
    }
    protected override void Move_Enter()
    {
        anim.timeScale = 1;
        anim.AnimationState.SetAnimation(0, move, true);
    }

    protected override void Move_Update()
    {
        curTarget = FindTarget();
        if (curTarget != null && team.CurState is not TeamMgr.State.Back)
        {
            if (MoveToPos(curTarget.transform.position, atkRange.Value - 0.5f))
                fsm.ChangeState(State.Attack);
        }
        else
        {
            if (MoveToPos(idlePosition))
                fsm.ChangeState(State.Idle);
        }
    }

    protected override void Attack_Enter()
    {
        lastTimeStartAttack = Time.time - attackAnimDuration / atkSpeed.Value; //càng cao càng nhanh
    }

    protected override void Attack_Update()
    {
        if (IsAttacking)
        {
            ProcessAttack();
            return;
        }
        curTarget = FindTarget();
        if (curTarget != null && curTarget.Hp > 0 &&
            Vector3.Distance(transform.position, curTarget.transform.position) < atkRange.Value)
            ProcessAttack();
        else
            fsm.ChangeState(State.Move);
    }

    protected override void Dead_Enter()
    {
        team.RemoveHero(this);
        anim.AnimationState.SetAnimation(0, die, false);
        LeanPool.Despawn(this, 1f);
    }

    protected void ProcessAttack()
    {
        if (Time.time - lastTimeStartAttack >= attackAnimDuration / atkSpeed.Value)
        {
            anim.timeScale = atkSpeed.Value;
            anim.AnimationState.SetAnimation(0, attack, false);
            anim.AnimationState.AddAnimation(0, idle, true, 0);
            isAttacked = false;
            lastTimeStartAttack = Time.time;
        }

        if (!isAttacked && Time.time - lastTimeStartAttack >= attackAnimEvent*attackAnimDuration / atkSpeed.Value)
        {
            Attack();
            isAttacked = true;
        }
    }

    protected virtual void Attack()
    {
        if (curTarget != null && curTarget.Hp > 0)
            curTarget.DealDame(this);
    }
    
    protected virtual bool MoveToPos(Vector3 pos,float rangeReach=0.1f) //only move x, reach at range
    {
        var dir = pos - transform.position;
        var reached = dir.magnitude < rangeReach;
        if (!reached) transform.GetChild(0).localScale = new Vector3(-Mathf.Sign(dir.x), 1, 1);
        transform.position += dir.normalized * moveSpeed.Value * Time.deltaTime;
        if (reached && rangeReach <= 0.11f) transform.position = pos;
        return reached;
    }
    protected virtual Unit FindTarget()
    {
        if (team.EnemiesInRange.Count == 0) return null;
        Unit unitNearest = null;
        var minDistance = float.MaxValue;
        for (var i = 0; i < team.EnemiesInRange.Count; i++)
        {
            if (team.EnemiesInRange[i] == null) continue;
            var curDistance = Vector3.Distance(transform.position, team.EnemiesInRange[i].transform.position);
            if (!(curDistance < minDistance)) continue;
            unitNearest = team.EnemiesInRange[i];
            minDistance = curDistance;
        }
        return unitNearest;
    }

    public override void DealDame(Unit source)
    {
        anim.AnimationState.SetAnimation(1, hit, false);
        anim.AnimationState.AddEmptyAnimation(1, 0.2f, 0);
        base.DealDame(source);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(idlePosition, Vector3.one);
        Gizmos.DrawWireSphere(transform.position, atkRange.Value);
    }
}

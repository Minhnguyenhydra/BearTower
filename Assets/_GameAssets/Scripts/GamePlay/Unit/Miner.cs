using System.Linq;
using Kryz.CharacterStats;
using UnityEngine;
public class Miner : Hero
{
    public CharacterStat capacity = new CharacterStat(50);
    public float goldHolding;
    public GoldMine curGoldMine;
    private float _lastTimeAttack;
    protected override void Idle_Update()
    {
        if (team.fsm.State != TeamMgr.State.Back)
        {
            if (curGoldMine == null || curGoldMine.Gold==0) curGoldMine = FindGoldMine();
            if (curGoldMine != null) fsm.ChangeState(State.Move);
        }
    }

    protected override void Move_Update()
    {
        if (team.fsm.State != TeamMgr.State.Back)
        {
            if (goldHolding < capacity.Value && curGoldMine.Gold>0) //di ra mo vang
            {
                if (MoveToPos(curGoldMine.StandPositionOfMiner(this)))
                {
                    fsm.ChangeState(State.Attack);
                }
            }
            else
            {
                MoveToCastle();
            }
        }
        else //dang rut quan
        {
            MoveToCastle();
        }
    }

    protected override void Attack_Enter()
    {
        base.Attack_Enter();
        _lastTimeAttack = Time.time;
        var dir = curGoldMine.transform.position - transform.position;
        transform.GetChild(0).localScale = new Vector3(-Mathf.Sign(dir.x), 1, 1);
    }

    protected override void Attack_Update()
    {
        if (team.fsm.State != TeamMgr.State.Back)
        {
            if (Time.time-_lastTimeAttack > 10/atkSpeed.Value)
            {
                var goldAdd = Mathf.Min(atk.Value, capacity.Value - goldHolding, curGoldMine.Gold);
                goldHolding += goldAdd;
                curGoldMine.Gold -= goldAdd;
                if (goldHolding >= capacity.Value || curGoldMine.Gold == 0)
                    fsm.ChangeState(State.Move);
                _lastTimeAttack = Time.time;
            }
        }
        else //dang rut quan
        {
            fsm.ChangeState(State.Move);
        }
    }


    protected override void Dead_Enter()
    {
        if (curGoldMine != null) curGoldMine.slots.Remove(this);
        base.Dead_Enter();
    }
    private void MoveToCastle()
    {
        if (MoveToPos(team.castle.transform.position))
        {
            team.Gold += goldHolding;
            goldHolding = 0;
            fsm.ChangeState(State.Idle);
        }
    }
    private GoldMine FindGoldMine()
    {
        var res=team.goldMines.FirstOrDefault(goldMine => goldMine.Gold > 0 && goldMine.slots.Count < 2);
        if (res != null) res.slots.Add(this);
        return res;
    }

   
}

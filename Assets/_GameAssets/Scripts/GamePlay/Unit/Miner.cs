using System.Linq;
using Kryz.CharacterStats;
using UnityEngine;
public class Miner : Hero
{
    public CharacterStat capacity = new CharacterStat(50);
    private float _goldHolding;
    private GoldMine _curGoldMine;
    private float _lastTimeAttack;
    private Vector3 _goldMineStandPosition;
    protected override void Idle_Update()
    {
        if (team.CurState is not TeamMgr.State.Back)
        {
            if ((_curGoldMine == null || _curGoldMine.Gold == 0) && Time.frameCount % 60 == 0)
                _curGoldMine = FindGoldMine();
            if (_curGoldMine != null) fsm.ChangeState(State.Move);
        }
    }

    protected override void Move_Update()
    {
        if (team.CurState != TeamMgr.State.Back)
        {
            if (_goldHolding < capacity.Value && _curGoldMine.Gold>0) //di ra mo vang
            {
                if (MoveToPos(_goldMineStandPosition))
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
            MoveToCastle(); //move to spawnPos
        }
    }

    protected override void Attack_Enter()
    {
        base.Attack_Enter();
        var dir = _curGoldMine.transform.position - transform.position;
        transform.GetChild(0).localScale = new Vector3(-Mathf.Sign(dir.x), 1, 1);
    }

    protected override void Attack_Update()
    {
        if (IsAttacking)
        {
            ProcessAttack();
            return;
        }
        if (team.CurState != TeamMgr.State.Back)
        {
            ProcessAttack();
        }
        else
        {
            fsm.ChangeState(State.Move);
        }
    }

    protected override void Attack()
    {
        var goldAdd = Mathf.Min(atk.Value, capacity.Value - _goldHolding, _curGoldMine.Gold);
        _goldHolding += goldAdd;
        _curGoldMine.Gold -= goldAdd;
        if (_goldHolding >= capacity.Value || _curGoldMine.Gold == 0)
            fsm.ChangeState(State.Move);
    }

    protected override void Dead_Enter()
    {
        if (_curGoldMine != null) _curGoldMine.slots.Remove(this);
        base.Dead_Enter();
    }
    private void MoveToCastle()
    {
        if (MoveToPos(team.castle.transform.position))
        {
            team.Gold += _goldHolding;
            _goldHolding = 0;
            fsm.ChangeState(State.Idle);
        }
    }
    private GoldMine FindGoldMine()
    {
        var res=team.goldMines.FirstOrDefault(goldMine => goldMine.Gold > 0 && goldMine.slots.Count < 2);
        if (res != null)
        {
            res.slots.Add(this);
            _goldMineStandPosition = res.StandPositionOfMiner(this);
        }
        return res;
    }

   
}

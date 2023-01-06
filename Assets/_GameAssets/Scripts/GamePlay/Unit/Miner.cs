using System.Linq;
using Kryz.CharacterStats;
using UnityEngine;
public class Miner : Hero
{
    public CharacterStat capacity = new CharacterStat(50);
    private float _crystalHolding;
    private CrystalMine _curCrystalMine;
    private float _lastTimeAttack;
    private Vector3 _goldMineStandPosition;
    protected override void Idle_Update()
    {
        if (team.CurState is not TeamMgr.State.Back)
        {
            if ((_curCrystalMine == null || _curCrystalMine.Value == 0) && Time.frameCount % 60 == 0)
                _curCrystalMine = FindGoldMine();
            if (_curCrystalMine != null) fsm.ChangeState(State.Move);
        }
    }

    protected override void Move_Update()
    {
        if (team.CurState != TeamMgr.State.Back)
        {
            if (_crystalHolding < capacity.Value && _curCrystalMine.Value>0) //di ra mo vang
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
        var dir = _curCrystalMine.transform.position - transform.position;
        transform.GetChild(0).localScale = new Vector3(-Mathf.Sign(dir.x), 1, 1);
    }

    protected override void Attack_Update()
    {
        if (IsAttacking)
        {
            if (TriggerAttack()) Attack();
            return;
        }
        if (team.CurState != TeamMgr.State.Back)
            TriggerStartAttack();
        else
            fsm.ChangeState(State.Move);
    }
    protected override void Attack()
    {
        var goldAdd = Mathf.Min(atk.Value, capacity.Value - _crystalHolding, _curCrystalMine.Value);
        _crystalHolding += goldAdd;
        _curCrystalMine.Value -= goldAdd;
        if (_crystalHolding >= capacity.Value || _curCrystalMine.Value == 0)
            fsm.ChangeState(State.Move);
    }

    protected override void Dead_Enter()
    {
        if (_curCrystalMine != null) _curCrystalMine.slots.Remove(this);
        base.Dead_Enter();
    }
    private void MoveToCastle()
    {
        if (MoveToPos(team.castle.transform.position))
        {
            team.crystal.Value += _crystalHolding;
            _crystalHolding = 0;
            fsm.ChangeState(State.Idle);
        }
    }
    private CrystalMine FindGoldMine()
    {
        var res=team.goldMines.FirstOrDefault(mine => mine.Value > 0 && mine.slots.Count < 2);
        if (res != null)
        {
            res.slots.Add(this);
            _goldMineStandPosition = res.StandPositionOfMiner(this);
        }
        return res;
    }

   
}

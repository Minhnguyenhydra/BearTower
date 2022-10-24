using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MonsterLove.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

public class TeamMgr : MonoBehaviour
{
    public TeamMgr enemyTeam;
    public enum State
    {
        Back,
        Defense,
        TakeManaPool,
        Attack
    }
    public StateMachine<State> fsm;
    [SerializeField,HideInInspector] private float gold=500;
    public Action onGoldChange;
    [ShowInInspector]
    public float Gold
    {
        get => gold;
        set
        {
            gold = value;
            onGoldChange?.Invoke();
        }
    }
    [SerializeField,HideInInspector] private float mana;
    public Action onManaChange;
    [ShowInInspector]
    public float Mana
    {
        get => mana;
        set
        {
            mana = value;
            onManaChange?.Invoke();
        }
    }
    public List<Hero> listHero = new List<Hero>();
    public Action onListHeroChange;
    public int maxPopulation=50;

    public Castle castle;
    public GoldMine[] goldMines;
    public Fence fence;
    
    public List<TrainingProcess> trainingProcesses = new List<TrainingProcess>();
    [SerializeField] private HUDPanel hudPanel; //là tự điều khiển
    private ITeamControl TeamControl => hudPanel;

    private void Awake()
    {
        fsm = new StateMachine<State>(this);
        fsm.ChangeState(State.Back);
        fsm.Changed+=TeamStateChanged;
    }

    private void OnDestroy()
    {
        fsm.Changed-=TeamStateChanged;
    }

    private void TeamStateChanged(State curState)
    {
        SetupIdlePosition();
    }

    public void Setup()
    {
        foreach (var heroConfig in DBM.Config.HeroConfigs.Values)
            trainingProcesses.Add(new TrainingProcess(this, heroConfig));
        SpawnInitMiner(2);
        if (TeamControl != null)
        {
            TeamControl.Team = this;
            TeamControl.Setup();
        }
        fsm.ChangeState(State.Defense);
        AutoIncMana();
    }

    private void AutoIncMana()
    {
        DOTween.Sequence().AppendInterval(1).AppendCallback(() => Mana += 10).SetLoops(-1);

    }

    private void SpawnInitMiner(int count)
    {
        for (var i = 0; i < count; i++)
            trainingProcesses[0].SpawnHero();
    }

    public void AddHero(Hero hero)
    {
        listHero.Add(hero);
        OnListHeroChange();
    }

    public void RemoveHero(Hero hero)
    {
        listHero.Remove(hero);
        OnListHeroChange();
    }

    private void OnListHeroChange()
    {
        SetupIdlePosition();
        onListHeroChange?.Invoke();
    }

    private void SetupIdlePosition()
    {
        if (fsm.State == State.Defense)
        {
            var dictHero = new Dictionary<Hero.AttackType, List<Hero>>
            {
                {Hero.AttackType.Melee, new List<Hero>()},
                {Hero.AttackType.Ranged, new List<Hero>()}
            };
            foreach (var hero in listHero)
            {
                if (hero is Miner) hero.idlePosition = castle.transform.position;
                else dictHero[hero.attackType].Add(hero);
            }
            const int maxCol = 5;
            const int spaceCol = 2;
            const int spaceRow = 3;
            var curRow = 0;
            foreach (var (_, heroes) in dictHero)
            {
                var numOfRow = Mathf.CeilToInt(1f*heroes.Count / maxCol);
                for (var row = 0; row < numOfRow; row++)
                {

                    var numOfCol = row == numOfRow - 1 && heroes.Count % maxCol > 0
                        ? heroes.Count % maxCol
                        : maxCol;

                    for (var col = 0; col < numOfCol; col++)
                    {
                        var offSet = new Vector3(-(curRow+2) * spaceRow+col*spaceCol/2f,  maxCol * spaceCol*(1f*col  / numOfCol-0.5f), 0);
                        heroes[row * maxCol + col].idlePosition = fence.transform.position + fence.transform.GetChild(0).localScale.x * offSet;
                    }
                    curRow++;
                }
            }
        }
        else
        {
            foreach (var hero in listHero)
                hero.idlePosition = castle.transform.position;
        }
    }
}

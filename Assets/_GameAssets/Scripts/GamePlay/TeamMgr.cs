using System;
using System.Collections.Generic;
using System.Linq;
using _GameAssets.Scripts;
using DG.Tweening;
using MonsterLove.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

public class TeamMgr : MonoBehaviour
{
    public TeamMgr enemyTeam;
    public Color color = Color.blue;
    public enum State
    {
        Back,
        Defense,
        TakeManaPool,
        Attack
    }
    public StateMachine<State> fsm;

    [ShowInInspector]
    public State CurState
    {
        get => fsm!=null?fsm.State:State.Back;
        set => fsm.ChangeState(value);
    }

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
    
    public List<UpgradeHeroItem> dataUpgradeHero=new List<UpgradeHeroItem>();
    
    [SerializeField] private HUDPanel hudPanel; //là tự điều khiển
    private ITeamControl _teamControl;
    public ITeamControl TeamControl
    {
        get
        {
            if (_teamControl == null) _teamControl = hudPanel != null ? hudPanel : GetComponent<AIHandle>();
            return _teamControl;
        }
    }

    public readonly Dictionary<State, Vector2> RangeFindEnemy = new Dictionary<State, Vector2>();
    public readonly List<Unit> EnemiesInRange = new List<Unit>();
    public float FaceDirection => castle.transform.GetChild(0).localScale.x;

    private void Awake()
    {
        fsm = new StateMachine<State>(this);
        fsm.ChangeState(State.Back);
        fsm.Changed += TeamStateChanged;
    }

    private void Update()
    {
        if (RangeFindEnemy.Count != 0 && Time.frameCount % 20 == 0)
        {
            EnemiesInRange.Clear();
            foreach (var hero in enemyTeam.listHero.Where(
                hero => hero.InRange(RangeFindEnemy[CurState])))
                EnemiesInRange.Add(hero);
            if (enemyTeam.fence!=null && enemyTeam.fence.Hp > 0 && enemyTeam.fence.InRange(RangeFindEnemy[CurState]))
                EnemiesInRange.Add(enemyTeam.fence);
            if (enemyTeam.castle!=null && enemyTeam.castle.Hp > 0 && enemyTeam.castle.InRange(RangeFindEnemy[CurState]))
                EnemiesInRange.Add(enemyTeam.castle);
        }
    }

    private void OnDestroy()
    {
        fsm.Changed-=TeamStateChanged;
    }

    private void TeamStateChanged(State curState)
    {
        EnemiesInRange.Clear();
        SetupIdlePosition();
    }

    public void Setup()
    {
        TeamControl.Init(this);
        foreach (var upgradeInfo in dataUpgradeHero.Where(upgradeInfo => upgradeInfo.level>0))
            trainingProcesses.Add(new TrainingProcess(this, upgradeInfo));
        TeamControl.Setup();
        SpawnInitMiner(2);
        InitRangeFindEnemy();
        fsm.ChangeState(State.Defense);
    }

    private void InitRangeFindEnemy()
    {
        foreach (var state in Enum.GetValues(typeof(State)).Cast<State>())
        {
            var p1 = castle.transform.position.x;
            var manaPool = GamePlayMgr.Instance.manaPool;
            var p2 = state switch
            {
                State.Defense => manaPool.transform.position.x - manaPool.range * FaceDirection,
                State.TakeManaPool => manaPool.transform.position.x + manaPool.range * FaceDirection,
                _ => enemyTeam.castle.transform.position.x
            };
            RangeFindEnemy.Add(state, new Vector2(Mathf.Min(p1, p2), Mathf.Max(p1, p2)));
            Debug.Log(state+":"+ RangeFindEnemy[state]);
        }
    }

    private void SpawnInitMiner(int count)
    {
        var minerProcess = trainingProcesses.Find(t => t.upgradeInfo.heroId == "Miner");
        for (var i = 0; i < count; i++)
            minerProcess.SpawnHero();
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
        if (CurState is State.Defense or State.TakeManaPool)
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
                        var offSet =
                            new Vector3(-(curRow + 2) * spaceRow + FaceDirection*col * spaceCol / 2f,
                                maxCol * spaceCol * (1f * col / numOfCol - 0.5f), 0);
                        heroes[row * maxCol + col].idlePosition = (CurState is State.TakeManaPool ? GamePlayMgr.Instance.manaPool.transform.position : fence.transform.position) + FaceDirection * offSet;
                    }

                    curRow++;
                }
            }
        }
        else if (CurState is State.Back)
        {
            foreach (var hero in listHero)
                hero.idlePosition = castle.transform.position;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (RangeFindEnemy.Count > 0 && fsm!=null)
        {
            var size = RangeFindEnemy[CurState].y - RangeFindEnemy[CurState].x;
            Gizmos.color = name.Contains("Ally") ? Color.blue : Color.red;
            Gizmos.DrawWireCube(new Vector3(size / 2+RangeFindEnemy[CurState].x, 0, 0), new Vector3(size, 10, 0));
        }
    }
}

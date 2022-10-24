
using System;
using System.Collections;
using System.Linq;
using Lean.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrainingProcess
{
    public TrainingProcess(TeamMgr team, HeroConfig heroConfig)
    {
        this.team = team;
        this.heroConfig = heroConfig;
    }
    public TeamMgr team;
    public HeroConfig heroConfig;
    private int _queueCount;
    public Action onQueueCountChanged;
    public int QueueCount
    {
        get => _queueCount;
        private set
        {
            _queueCount = value;
            onQueueCountChanged?.Invoke();
        }
    }

    public int TotalQueueCount => team.trainingProcesses.Sum(p => p.QueueCount);

    private float _coolDown;
    public Action onCoolDownChanged;
    public float CoolDown
    {
        get => _coolDown;
        private set
        {
            _coolDown = value;
            onCoolDownChanged?.Invoke();
        }
    }
    public Action onNotEnoughGold;
    public Action onNotEnoughMana;
    public Action onMaxPopulation;
    public bool AddTrain()
    {
        if (team.Gold < heroConfig.priceGoldTraining)
        {
            onNotEnoughGold?.Invoke();
        }
        else if (team.Mana < heroConfig.priceManaTraining)
        {
            onNotEnoughMana?.Invoke();
        }
        else if (team.listHero.Count + TotalQueueCount >= team.maxPopulation)
        {
            onMaxPopulation?.Invoke();
        }
        else
        {
            team.Gold -= heroConfig.priceGoldTraining;
            team.Mana -= heroConfig.priceManaTraining;
            QueueCount++;
            if (QueueCount == 1) _trainingCor = team.StartCoroutine(Train());
            return true;
        }

        return false;
    }
    public void RemoveTrain()
    {
        team.Gold += heroConfig.priceGoldTraining;
        team.Mana += heroConfig.priceManaTraining;
        QueueCount--;
        if (QueueCount == 0) team.StopCoroutine(_trainingCor);
    }
    private Coroutine _trainingCor;
    private IEnumerator Train()
    {
        while (QueueCount > 0)
        {
            CoolDown = heroConfig.timeTraining;
            while (CoolDown>0)
            {
                yield return null;
                CoolDown -= Time.deltaTime;
            }
            CoolDown = 0;
            SpawnHero();
            QueueCount--;
        }
    }

    public void SpawnHero()
    {
        var hero = LeanPool.Spawn(heroConfig.prefab, team.castle.transform.position+new Vector3(Random.Range(-1,1),Random.Range(-1,1),0), Quaternion.identity);
        hero.Setup(team, 100);
        team.AddHero(hero);
    }
}
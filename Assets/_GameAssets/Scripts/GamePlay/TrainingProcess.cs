
using System;
using System.Collections;
using System.Linq;
using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;
[Serializable]
public class TrainingProcess
{
    public TrainingProcess(TeamMgr team, UpgradeHeroItem upgradeInfo)
    {
        this.team = team;
        this.upgradeInfo = upgradeInfo;
        this.heroConfig = DBM.Config.HeroConfigs[upgradeInfo.heroId];
    }
    public TeamMgr team;
    public UpgradeHeroItem upgradeInfo;
    public HeroConfig heroConfig;
    public NotiVar<int> queueCount = new NotiVar<int>();
    public int TotalQueueCount => team.trainingProcesses.Sum(p => p.queueCount.Value);
    public NotiVar<float> coolDown = new NotiVar<float>();
    public Action onMaxPopulation;
    public Action onNotEnoughCrystal;
    public void AddTrain()
    {
        if (team.crystal.Value < heroConfig.priceCrystalTraining)
        {
            onNotEnoughCrystal?.Invoke();
        }
        else if (team.listHero.Count + TotalQueueCount >= team.maxPopulation)
        {
            onMaxPopulation?.Invoke();
        }
        else
        {
            team.crystal.Value -= heroConfig.priceCrystalTraining;
            queueCount.Value++;
            if (queueCount.Value == 1) _trainingCor = team.StartCoroutine(Train());
        }
    }
    public void RemoveTrain()
    {
        team.crystal.Value += heroConfig.priceCrystalTraining;
        queueCount.Value--;
        if (queueCount.Value == 0) team.StopCoroutine(_trainingCor);
    }
    private Coroutine _trainingCor;
    private IEnumerator Train()
    {
        while (queueCount.Value > 0)
        {
            coolDown.Value = heroConfig.timeTraining;
            while (coolDown.Value>0)
            {
                yield return null;
                coolDown.Value -= Time.deltaTime;
            }
            coolDown.Value = 0;
            SpawnHero();
            queueCount.Value--;
        }
    }
    [Button]
    public void SpawnHero()
    {
        var hero = LeanPool.Spawn(heroConfig.prefab, team.castle.transform.position+new Vector3(Random.Range(-1,1),Random.Range(-1,1),0), Quaternion.identity);
        hero.Setup(team, upgradeInfo);
        team.AddHero(hero);
    }
}
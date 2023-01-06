using System;
using System.Collections.Generic;
using System.Linq;
using _GameAssets.Scripts;
using DG.Tweening;
using Kryz.CharacterStats;
using Lean.Gui;
using Lean.Transition;
using TMPro;
using UnityEngine;
public class HUDPanel : Panel,ITeamControl
{
    public Transform trainingBtnContainer;
    [SerializeField] private TMP_Text txtCrystal;
    [SerializeField] private TMP_Text txtPopulation;
    [SerializeField] private List<LeanToggle> toggleStates;
    public TeamMgr Team { get; set; }

    public static int SelectedMap;
    [SerializeField] private GamePlayMgr[] maps;

    private void Start()
    {
        var map=Instantiate(maps[SelectedMap]);
        map.teams[0].hudPanel = this;
    }

    public void Init(TeamMgr team)
    {
        this.Team = team;
        Team.dataUpgradeHero = DBM.UserData.upgrade.DictHero.Values.ToList();
    }

    public void Setup()
    {
        trainingBtnContainer.FillData<TrainingProcess, ButtonTraining>(Team.trainingProcesses, (data, btn, idx) =>
        {
            btn.Setup(data);
            data.onNotEnoughCrystal += NotEnoughGold;
            data.onMaxPopulation += MaxPopulation;
        });
        Team.fsm.Changed += FsmOnChanged;
        Team.onListHeroChange += ListHeroChanged;
        Team.crystal.onValueChanged += CrystalChanged;
        FsmOnChanged(Team.CurState);
        ListHeroChanged();
        CrystalChanged();
    }

    private void FsmOnChanged(TeamMgr.State state)
    {
        toggleStates[(int) state].TurnOn();
    }
    private void ListHeroChanged()
    {
        txtPopulation.text = $"{Team.listHero.Count}/{Team.maxPopulation}";
    }
    private void NotEnoughGold()
    {
        Toast.Show("Not Enough Crystal");
        txtCrystal.DOColor(Color.red, 0.2f).From(new Color(0.82f,0.82f,0.82f)).SetLoops(2, LoopType.Yoyo);
    }
    private void MaxPopulation()
    {
        Toast.Show("Reach max population");
        txtPopulation.DOColor(Color.red, 0.2f).From(new Color(0.82f,0.82f,0.82f)).SetLoops(2, LoopType.Yoyo);
    }
    private void CrystalChanged()
    {
        txtCrystal.text = Team.crystal.ToString();
    }

    public void OnClickChangeState(int state)
    {
        Team.fsm.ChangeState((TeamMgr.State) state);
    }

    public void OnClickSkill(int idxSkill)
    {
        switch (idxSkill)
        {
            case 0:
                var modSpeedMine = new StatModifier(1f, StatModType.PercentAdd);
                foreach (var miner in Team.listHero.Where(h => h is Miner))
                {
                    miner.atkSpeed.AddModifier(modSpeedMine);
                }
                this.EventTransition(() =>
                {
                    foreach (var miner in Team.listHero.Where(h => h is Miner))
                        miner.atkSpeed.RemoveModifier(modSpeedMine);
                }, 10);
                break;
            case 1:
                foreach (var h in Team.listHero)
                {
                    h.Heal();
                }
                break;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Team.fsm.Changed -= FsmOnChanged;
        Team.onListHeroChange -= ListHeroChanged;
        Team.crystal.onValueChanged -= CrystalChanged;
        foreach (var data in Team.trainingProcesses)
        {
            data.onNotEnoughCrystal -= NotEnoughGold;
            data.onMaxPopulation -= MaxPopulation;
        }
    }
    public override void BackClick()
    {
        Open<PausePanel>();
    }

}

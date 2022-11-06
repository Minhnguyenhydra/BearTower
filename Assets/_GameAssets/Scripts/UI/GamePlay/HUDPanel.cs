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
    [SerializeField] private TMP_Text txtGold;
    [SerializeField] private TMP_Text txtMana;
    [SerializeField] private TMP_Text txtPopulation;
    [SerializeField] private List<LeanToggle> toggleStates;
    public TeamMgr Team { get; set; }
    public void Setup()
    {
        trainingBtnContainer.FillData<TrainingProcess, ButtonTraining>(Team.trainingProcesses, (data, btn, idx) =>
        {
            btn.Setup(data);
            data.onNotEnoughGold += NotEnoughGold;
            data.onNotEnoughMana += NotEnoughMana;
            data.onMaxPopulation += MaxPopulation;
        });
        Team.fsm.Changed += FsmOnChanged;
        Team.onListHeroChange += ListHeroChanged;
        Team.onGoldChange += GoldChanged;
        Team.onManaChange += ManaChanged;
        FsmOnChanged(Team.CurState);
        ListHeroChanged();
        GoldChanged();
        ManaChanged();
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
        Toast.Show("Not Enough Gold");
        txtGold.DOColor(Color.red, 0.2f).From(Color.yellow).SetLoops(2, LoopType.Yoyo);
    }
    private void NotEnoughMana()
    {
        Toast.Show("Not Enough Mana");

        txtMana.DOColor(Color.red, 0.2f).From(Color.blue).SetLoops(2, LoopType.Yoyo);
    }
    private void MaxPopulation()
    {
        Toast.Show("Reach max population");
        txtPopulation.DOColor(Color.red, 0.2f).From(Color.gray).SetLoops(2, LoopType.Yoyo);
    }
    private void GoldChanged()
    {
        txtGold.text = Team.Gold.ToString();
    }
    private void ManaChanged()
    {
        txtMana.text = Team.Mana.ToString();
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
        Team.onGoldChange -= GoldChanged;
        Team.onManaChange -= ManaChanged;
        foreach (var data in Team.trainingProcesses)
        {
            data.onNotEnoughGold -= NotEnoughGold;
            data.onNotEnoughMana -= NotEnoughMana;
        }
    }
    public override void BackClick()
    {
        Time.timeScale = 0;
        Popup.Show("Pause","Continue?","Continue", () =>
        {
            Time.timeScale = 1;
        },forceChoose:true);
    }

}

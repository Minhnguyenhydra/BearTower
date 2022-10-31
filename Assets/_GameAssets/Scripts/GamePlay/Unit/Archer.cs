using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Kryz.CharacterStats;
using Lean.Pool;
using Spine;
using Spine.Unity;
using UnityEngine;

public class Archer : Hero
{
    public CharacterStat arrowSpeed = new CharacterStat(30);
    [SerializeField] private Arrow arrowPrefab;
    [SerializeField] private Transform arrowSpawnPos;
    [SpineBone,SerializeField] private string target;
    private Bone _boneTarget;
    private Bone BoneTarget => _boneTarget ??= anim.skeleton.FindBone(target);

    protected override void Attack()
    {
        if (curTarget != null && curTarget.Hp > 0)
        {
            var cachePos = curTarget.transform.position;
            var arrow = LeanPool.Spawn(arrowPrefab, new Vector3(arrowSpawnPos.position.x,transform.position.y,0), Quaternion.identity);
            arrow.transform.GetChild(0).position = arrowSpawnPos.position;
            var dis = Vector3.Distance(cachePos, transform.position);
            var duration = dis / arrowSpeed.Value;
            arrow.transform.DOMove(cachePos, duration).SetEase(Ease.Linear);
            var lasPos = arrow.transform.GetChild(0).position;
            arrow.transform.GetChild(0).DOLocalMoveY(dis / 5f, duration / 2).SetRelative(true).SetLoops(2, LoopType.Yoyo)
                .OnUpdate(() =>
                {
                    arrow.transform.GetChild(0).right = arrow.transform.GetChild(0).position - lasPos;
                    lasPos = arrow.transform.GetChild(0).position;
                }).OnComplete(() =>
                {
                    if (curTarget != null && curTarget.Hp > 0)
                        curTarget.DealDame(this);
                    LeanPool.Despawn(arrow);
                });
        }
    }
}

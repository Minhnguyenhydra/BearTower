using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;

public class Archer : Hero
{
    [SerializeField] private Arrow arrowPrefab;
    protected override void Attack()
    {
        if (curTarget != null && curTarget.Hp > 0)
        {
            var arrow = LeanPool.Spawn(arrowPrefab, transform.position - Vector3.right * transform.GetChild(0).localScale.x, Quaternion.identity);
            var cachePos = curTarget.transform.position;
            arrow.transform.DOMove(cachePos, 0.5f);
            arrow.transform.GetChild(0).right = cachePos+Vector3.up - arrow.transform.GetChild(0).transform.position;
            arrow.transform.GetChild(0).DOLocalMoveY(3f, 0.25f).From(1.5f).SetLoops(2,LoopType.Yoyo).OnComplete(() =>
            {
                if (curTarget != null && curTarget.Hp > 0)
                {
                    curTarget.DealDame(this);
                }
                LeanPool.Despawn(arrow);
            });
        }
            
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GoldMine : MonoBehaviour
{
    [SerializeField] private float maxGold = 1000;
    private float gold;
    [ShowInInspector]
    public float Gold
    {
        get => gold;
        set
        {
            gold = value;
            transform.localScale = Vector3.one * gold / maxGold;
        }
    }
    public List<Miner> slots = new List<Miner>();

    private void Awake()
    {
        gold = maxGold;
    }

    public Vector3 StandPositionOfMiner(Miner miner)
    {
        var idx = slots.IndexOf(miner);
        return transform.position + new Vector3(idx == 0 ? -3 : 3, -0.1f, 0);
    }
}

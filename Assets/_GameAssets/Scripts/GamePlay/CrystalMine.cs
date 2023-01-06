using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CrystalMine : MonoBehaviour
{
    [SerializeField] private float maxValue = 1000;
    [SerializeField] private Sprite[] mineSprites;
    [SerializeField] private SpriteRenderer mineVisual;
    private float _value;
    [ShowInInspector]
    public float Value
    {
        get => _value;
        set
        {
            this._value = value;
            var range=maxValue / mineSprites.Length;
            var visualIdx = Mathf.Clamp((int)(_value / range), 0, mineSprites.Length - 1);
            //mineVisual.sprite = mineSprites[visualIdx];
            transform.localScale = Vector3.one * _value / maxValue;
        }
    }
    public List<Miner> slots = new List<Miner>();
    private void Awake()
    {
        _value = maxValue;
    }

    public Vector3 StandPositionOfMiner(Miner miner)
    {
        var idx = slots.IndexOf(miner);
        return transform.position + new Vector3(idx == 0 ? -3 : 3, -0.1f, 0);
    }
}

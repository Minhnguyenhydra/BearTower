using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPool : MonoBehaviour
{
    public float range = 15;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector2(range*2, 10));
    }
}

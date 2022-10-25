using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : Unit
{
    protected override void Dead_Enter()
    {
        gameObject.SetActive(false);
    }
}

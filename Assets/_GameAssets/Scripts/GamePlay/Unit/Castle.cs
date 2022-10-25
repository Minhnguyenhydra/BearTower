using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : Unit
{
    protected override void Dead_Enter()
    {
        GamePlayMgr.Instance.EndGame();
    }
}

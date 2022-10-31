using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _GameAssets.Scripts;
using UnityEngine;

public class ManaPool : MonoBehaviour
{
    public float range = 15;
    public float manaAdd = 10;
    public float speedAddMana = 1;
    public float lastTimeAddMana = float.MinValue;
    public TeamMgr curTeam;
    public List<Unit> UnitsInRange = new List<Unit>();
    public SpriteRenderer spriteRenderer;
    private void Update()
    {
        UnitsInRange.Clear();
        var occupied = true;
        foreach (var team in GamePlayMgr.Instance.teams)
        {
            foreach (var hero in team.listHero)
            {
                if (hero.InRange(new Vector2(transform.position.x - range, transform.position.x + range)))
                {
                    UnitsInRange.Add(hero);
                    if (hero.team==curTeam)
                    {
                        occupied = false;
                    }
                }
            }
        }
        if (occupied && UnitsInRange.Count > 0)
        {
            curTeam = UnitsInRange.First().team;
            spriteRenderer.color = curTeam.color;
        }

        if (curTeam != null && Time.time - lastTimeAddMana > speedAddMana)
        {
            curTeam.Mana += manaAdd;
            lastTimeAddMana = Time.time;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector2(range*2, 10));
    }

}

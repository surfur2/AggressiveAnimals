﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : BehaviorBase {

    private BotSensorySystem target;

    public float radiusForAttack;
    public float attackCooldown;
    public int minAttackDamage;
    public int maxAttackDamage;
    public float chanceToHit;
    private float lastAttack;

    protected override void CalculateBehavior()
    {
        if (target != null)
        {
            if (lastAttack < 0.0f)
            {
                if (IsInAttackRadius())
                {
                    AttackEnemy();
                }
            }
            else
            {
                lastAttack -= Time.deltaTime;
            }
        }
    }

    public void SetTarget(GameObject _target)
    {
        target = null;

        if (_target != null)
            target = _target.GetComponent<BotSensorySystem>();
    }

    private void AttackEnemy()
    {
        if (Random.Range(0, 101) < chanceToHit * 100)
        {
            target.TakeDamage(Random.Range(minAttackDamage, maxAttackDamage + 1));
        }
        lastAttack = attackCooldown;
    }

    private bool IsInAttackRadius()
    {
        return (target.gameObject.transform.position - transform.position).magnitude < radiusForAttack ? true : false;
    }
}

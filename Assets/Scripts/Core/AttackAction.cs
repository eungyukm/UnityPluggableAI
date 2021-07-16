using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="PluggableAI/Actions/Attack")]
public class AttackAction : ActionSO
{
    public override void Act(StateController controller)
    {
        Attack(controller);
    }

    private void Attack(StateController controller)
    {
        RaycastHit hit;

        Debug.DrawRay(controller.eyes.position, controller.eyes.forward.normalized * controller.enemyStates.attackRange, Color.red);

        if (Physics.SphereCast(controller.eyes.position, controller.enemyStates.lookSphereCastRadius, controller.eyes.forward, out hit, controller.enemyStates.attackRange)
            && hit.collider.CompareTag("Player"))
        {
            if (controller.CheckIfCountDownElapsed(controller.enemyStates.attackRange))
            {
                controller.tankShooting.Fire(controller.enemyStates.attackForce, controller.enemyStates.attackRange);
            }
        }
    }
}

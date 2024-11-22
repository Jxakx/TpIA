using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public override void EnterState(PlayerEnemies enemy)
    {
        
    }

    public override void UpdateState(PlayerEnemies enemy)
    {
        GameManager.Instance.alert = true;
        GameManager.Instance.alertGameObject = enemy.gameObject.name;

        if (!enemy.IsPlayerInSight())
        {           
            // Si el jugador ya no está visible, utiliza A* para dirigirse al último nodo conocido
            if (enemy.lastVisitedNode != null)
            {
                enemy.StateMachine.ChangeState(new PatrolAStar(), enemy);
            }
            else
            {
                enemy.StateMachine.ChangeState(new PatrolState(), enemy);
            }
            return;
        }

        if (GameManager.Instance.skullsInTravel.Count == 0)
        {
            GameManager.Instance.alert = false;
            GameManager.Instance.alertGameObject = "";
        }

        enemy.MoveTowards(enemy.Player.position);
    }


    public override void ExitState(PlayerEnemies enemy)
    {
        
    }
}

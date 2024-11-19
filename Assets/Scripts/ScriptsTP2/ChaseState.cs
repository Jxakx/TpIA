using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public override void EnterState(PlayerEnemies enemy)
    {
        Debug.Log("Iniciando Persecución");
    }

    public override void UpdateState(PlayerEnemies enemy)
    {
        if (!enemy.IsPlayerInSight())
        {
            if (enemy.lastVisitedNode != null) // Si hay un nodo visitado
            {
                enemy.StateMachine.ChangeState(new PatrolState(enemy.lastVisitedNode), enemy);
            }
            else
            {
                enemy.StateMachine.ChangeState(new PatrolState(), enemy); // Patrullaje normal si no hay nodo guardado
            }
            return;
        }

        // Perseguir al jugador
        enemy.MoveTowards(enemy.Player.position);
    }

    public override void ExitState(PlayerEnemies enemy)
    {
        Debug.Log("Saliendo de Persecución");
    }
}

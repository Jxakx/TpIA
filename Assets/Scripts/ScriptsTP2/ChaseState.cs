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
            enemy.StateMachine.ChangeState(new PatrolState(), enemy);
            return;
        }

        // Moverse hacia el jugador con rotación
        enemy.MoveTowards(enemy.Player.position);
    }

    public override void ExitState(PlayerEnemies enemy)
    {
        Debug.Log("Saliendo de Persecución");
    }
}

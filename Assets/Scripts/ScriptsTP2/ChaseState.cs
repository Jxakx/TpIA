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

        Vector3 direction = enemy.Player.position - enemy.transform.position;
        enemy.transform.position += direction.normalized * enemy.Speed * Time.deltaTime;
    }

    public override void ExitState(PlayerEnemies enemy)
    {
        Debug.Log("Saliendo de Persecución");
    }
}


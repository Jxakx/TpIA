using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    private int _currentWaypointIndex = 0;

    public override void EnterState(PlayerEnemies enemy)
    {
        // Inicia el patrullaje
        Debug.Log("Iniciando Patrullaje");
    }

    public override void UpdateState(PlayerEnemies enemy)
    {
        if (enemy.Waypoints.Count == 0) return;

        // Moverse hacia el siguiente waypoint
        Transform waypoint = enemy.Waypoints[_currentWaypointIndex];
        Vector3 direction = waypoint.position - enemy.transform.position;
        enemy.transform.position += direction.normalized * enemy.Speed * Time.deltaTime;

        // Si alcanza el waypoint, pasa al siguiente
        if (direction.magnitude < 0.5f)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % enemy.Waypoints.Count;
        }

        // Cambiar de estado si detecta al jugador
        if (enemy.IsPlayerInSight())
        {
            enemy.StateMachine.ChangeState(new ChaseState(), enemy);
        }
    }

    public override void ExitState(PlayerEnemies enemy)
    {
        Debug.Log("Saliendo de Patrullaje");
    }
}


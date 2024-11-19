using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    private int _currentWaypointIndex = 0;

    public override void EnterState(PlayerEnemies enemy)
    {
        Debug.Log("Iniciando Patrullaje");
    }

    public override void UpdateState(PlayerEnemies enemy)
    {
        if (enemy.Waypoints.Count == 0) return;

        // Moverse hacia el waypoint actual con rotación
        Transform waypoint = enemy.Waypoints[_currentWaypointIndex];
        enemy.MoveTowards(waypoint.position);

        // Cambiar al siguiente waypoint si alcanza el actual
        if (Vector3.Distance(enemy.transform.position, waypoint.position) < 0.5f)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % enemy.Waypoints.Count;
        }

        // Cambiar al estado de persecución si detecta al jugador
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    private int _currentWaypointIndex = 0;
    private Node startNode;


    public override void EnterState(PlayerEnemies enemy)
    {
        Debug.Log("Iniciando Patrullaje");

        if (startNode != null) // Si hay un nodo inicial específico
        {
            // Encuentra el índice del nodo más cercano al último nodo visitado
            for (int i = 0; i < enemy.Waypoints.Count; i++)
            {
                if (enemy.Waypoints[i].position == startNode.transform.position)
                {
                    _currentWaypointIndex = i;
                    break;
                }
            }
        }
    }
    public PatrolState(Node startNode = null)
    {
        this.startNode = startNode;
    }

    


    public override void UpdateState(PlayerEnemies enemy)
    {
        if (enemy.Waypoints.Count == 0) return;

        // Moverse hacia el waypoint actual con rotación
        Transform waypoint = enemy.Waypoints[_currentWaypointIndex];
        enemy.MoveTowards(waypoint.position);

        // Si alcanza el waypoint actual, actualiza el último nodo visitado
        if (Vector3.Distance(enemy.transform.position, waypoint.position) < 0.5f)
        {
            // Actualiza el último nodo visitado
            enemy.lastVisitedNode = Pathfinding.Instance.getClosestNode(waypoint.position);

            // Cambia al siguiente waypoint
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

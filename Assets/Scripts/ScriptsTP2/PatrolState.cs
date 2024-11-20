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

        if (startNode != null) // Si hay un nodo inicial espec�fico
        {
            // Encuentra el �ndice del nodo m�s cercano al �ltimo nodo visitado
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

        // Moverse hacia el waypoint actual con rotaci�n
        Transform waypoint = enemy.Waypoints[_currentWaypointIndex];
        enemy.MoveTowards(waypoint.position);

        // Si alcanza el waypoint actual, actualiza el �ltimo nodo visitado
        if (Vector3.Distance(enemy.transform.position, waypoint.position) < 0.5f)
        {
            // Actualiza el �ltimo nodo visitado
            enemy.lastVisitedNode = Pathfinding.Instance.getClosestNode(waypoint.position);

            // Cambia al siguiente waypoint
            _currentWaypointIndex = (_currentWaypointIndex + 1) % enemy.Waypoints.Count;
        }

        // Cambiar al estado de persecuci�n si detecta al jugador
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

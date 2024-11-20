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

        // Calcula la ruta hacia el nodo más cercano al waypoint de patrullaje
        if (enemy.lastVisitedNode != null)
        {
            _currentWaypointIndex = 0;  // Suponiendo que el patrullaje siempre comienza en el primer waypoint
            enemy._path = Pathfinding.Instance.GetPath(
            enemy.lastVisitedNode,
            Pathfinding.Instance.getClosestNode(enemy.Waypoints[_currentWaypointIndex].position)
            );
        }
    }
    public PatrolState(Node startNode = null)
    {
        this.startNode = startNode;
    }




    public override void UpdateState(PlayerEnemies enemy)
    {
        if (enemy._path.Count > 0)
        {
            enemy.MoveTowards(enemy._path[0].transform.position);

            // Cuando llegue al nodo actual, quítalo de la lista y actualiza `lastVisitedNode`
            if (Vector3.Distance(enemy.transform.position, enemy._path[0].transform.position) < 0.5f)
            {
                enemy.lastVisitedNode = enemy._path[0];
                enemy._path.RemoveAt(0);
            }
        }
        else
        {
            // Cuando llegue al final del camino, cambia al siguiente waypoint
            _currentWaypointIndex = (_currentWaypointIndex + 1) % enemy.Waypoints.Count;
            enemy._path = Pathfinding.Instance.GetPath(
            enemy.lastVisitedNode,
            Pathfinding.Instance.getClosestNode(enemy.Waypoints[_currentWaypointIndex].position)
            );
        }
    }


    public override void ExitState(PlayerEnemies enemy)
    {
        Debug.Log("Saliendo de Patrullaje");
    }
}

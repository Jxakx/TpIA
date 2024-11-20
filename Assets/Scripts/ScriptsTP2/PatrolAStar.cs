using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAStar : State
{
    public Node startNode;
    public Node finalNode;
    private List<Node> path = new List<Node>();
    public int _currentWaypointIndex = 0;
    private bool inStar = true;
    public override void EnterState(PlayerEnemies enemy)
    {
        Debug.Log("Entre a clase Star");
        startNode = enemy.pathFinding.getClosestNode(enemy.transform.position);
        finalNode = enemy.pathFinding.getClosestNode(enemy.player.position);
        path = enemy.pathFinding.GetPath(startNode, finalNode);
        Debug.Log("Path creado");
        foreach(var item in path)
        {
            Debug.Log(item);
        }
    }
    
    public override void ExitState(PlayerEnemies enemy)
    {
        
    }

    public override void UpdateState(PlayerEnemies enemy)
    {
        if(inStar == true)
        {
            idaAstar(enemy);
        }
        else
        {
            retornoAstar(enemy);
        }
    }

    public void idaAstar(PlayerEnemies enemy)
    {
        if (path.Count == 0) return;

        Transform waypoint = path[_currentWaypointIndex].transform;
        enemy.MoveTowards(waypoint.position);

        if (Vector3.Distance(enemy.transform.position, waypoint.position) < 0.5f)
        {
            if (_currentWaypointIndex == path.Count - 1) // Si es el último nodo, cambia el estado
            {
                inStar = false;
                return;
            }

            enemy.lastVisitedNode = Pathfinding.Instance.getClosestNode(waypoint.position);
            _currentWaypointIndex++;
        }
    }

    public void retornoAstar(PlayerEnemies enemy)
    {
        if (path.Count == 0)
        {
            // Recalcula el camino hacia el primer nodo de la ruta de patrullaje
            enemy.PathFindingState(); // Usa A* para volver a patrullar
            _currentWaypointIndex = 0;
        }

        Transform waypoint = path[_currentWaypointIndex].transform;
        enemy.MoveTowards(waypoint.position);

        if (Vector3.Distance(enemy.transform.position, waypoint.position) < 0.5f)
        {
            if (_currentWaypointIndex == path.Count - 1)
            {
                // Vuelve al patrullaje normal
                enemy.StateMachine.ChangeState(new PatrolState(), enemy);
                return;
            }

            enemy.lastVisitedNode = Pathfinding.Instance.getClosestNode(waypoint.position);
            _currentWaypointIndex++;
        }
    }

}

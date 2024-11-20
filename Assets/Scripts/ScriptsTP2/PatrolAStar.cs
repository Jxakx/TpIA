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

        // Moverse hacia el waypoint actual con rotación
        Transform waypoint = path[_currentWaypointIndex].transform;
        enemy.MoveTowards(waypoint.position);

        // Si alcanza el waypoint actual, actualiza el último nodo visitado
        if (Vector3.Distance(enemy.transform.position, waypoint.position) < 0.5f)
        {
            if (path[_currentWaypointIndex] == finalNode)
            {
                //estrella en reversa
                inStar = false;
                return;
            }
            // Actualiza el último nodo visitado
            enemy.lastVisitedNode = Pathfinding.Instance.getClosestNode(waypoint.position);

            // Cambia al siguiente waypoint
            _currentWaypointIndex = (_currentWaypointIndex + 1);
        }

    }

    public void retornoAstar(PlayerEnemies enemy)
    {
        if (path.Count == 0) return;
        _currentWaypointIndex = 0;
        path.Reverse();
        
        // Moverse hacia el waypoint actual con rotación
        Transform waypoint = path[_currentWaypointIndex].transform;
        enemy.MoveTowards(waypoint.position);

        // Si alcanza el waypoint actual, actualiza el último nodo visitado
        if (Vector3.Distance(enemy.transform.position, waypoint.position) < 0.5f)
        {
            if (path[_currentWaypointIndex] == startNode)
            {
                //estrella en reversa
                enemy.StateMachine.ChangeState(new PatrolAStar(), enemy);
            }
            // Actualiza el último nodo visitado
            enemy.lastVisitedNode = Pathfinding.Instance.getClosestNode(waypoint.position);

            // Cambia al siguiente waypoint
            _currentWaypointIndex = (_currentWaypointIndex + 1);
        }
    }
}

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

    public bool oneTime = true;
    public bool isBack = false;

    public override void EnterState(PlayerEnemies enemy)
    {
        startNode = enemy.funcionesPaths.getClosestNode(enemy.transform.position);
        finalNode = enemy.funcionesPaths.getClosestNode(enemy.player.position);
        path = enemy.pathFinding.AStar(startNode, finalNode);
        path.Reverse();
        Debug.Log("XXXXXXXXXXX" + enemy.gameObject.name + " Desde" + startNode.gameObject.name + " hasta " + finalNode.gameObject.name);
    }
    
    public override void ExitState(PlayerEnemies enemy)
    {
        
    }

    public override void UpdateState(PlayerEnemies enemy)
    {
        if(inStar == true)
        {
            RecorrerStar(enemy);
        }
        else
        {
            if(oneTime == true)
            {
                path.Reverse();
                _currentWaypointIndex = 0;
                oneTime = false;
                isBack = true;
            }
            
            RecorrerStar(enemy);
        }
    }

    public void RecorrerStar(PlayerEnemies enemy)
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

            if(isBack == true && _currentWaypointIndex == path.Count - 1)
            {
                Debug.Log("Volviendo a Patrol");
                GameManager.Instance.skullsInTravel.Remove(enemy.gameObject);
                enemy.StateMachine.ChangeState(new PatrolAStar(), enemy);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private HashSet<Node> _closedNodes = new HashSet<Node>();
    private PriorityQueue _openNodes = new PriorityQueue();

    public static Pathfinding Instance;

    public List<Node> nodes = new List<Node>();

    public LayerMask _obstacleMask;

    private void Awake()
    {
        Instance = this;
    }

    public List<Node> GetPath(Node starNode, Node endNode)
    {
        _closedNodes.Clear();
        _openNodes.Clear();

        starNode.Heuristic = 0;

        var actualNode = starNode;

        while (actualNode != null && actualNode != endNode)
        {
            foreach (var neighbour in actualNode.Neighbours)
            {
                if (_closedNodes.Contains(neighbour)) continue;

                var heuristic = actualNode.Heuristic + 1 + Vector3.Distance(neighbour.transform.position, endNode.transform.position);

                if(neighbour.Heuristic > heuristic)
                {
                    neighbour.Heuristic = heuristic;
                    neighbour.previousNode = actualNode;
                }

                _openNodes.Enqueue(neighbour, neighbour.Heuristic);
            }

            _closedNodes.Add(actualNode);
            actualNode = _openNodes.Dequeue();
        }

        var finalPath = new List<Node>();
        actualNode = endNode;
        var actualPreviousNode = actualNode.previousNode;
        finalPath.Add(actualNode);

        while(actualNode != null && actualNode != starNode && actualNode.previousNode != null && actualPreviousNode.previousNode != null)
        {
            if(OnSight(actualNode.transform.position, actualPreviousNode.previousNode.transform.position, _obstacleMask))
            {
                actualPreviousNode = actualPreviousNode.previousNode;
            }
            else
            {
                finalPath.Add(actualNode);
                actualNode = actualNode.previousNode;
                actualPreviousNode = actualPreviousNode.previousNode;
            }
            
        }

        finalPath.Reverse();
        return finalPath;
    }

    public Node getClosestNode(Vector3 position)
    {
        var closestNode = nodes[0];
        var closestDistance = Vector3.Distance(closestNode.transform.position, position);

        for (var i = 1; i < nodes.Count; i++)
        {
            if (closestDistance > Vector3.Distance(position, nodes[i].transform.position) && OnSight(nodes[i].transform.position,position, _obstacleMask))
            {
                closestNode = nodes[i];
                closestDistance = Vector3.Distance(position, closestNode.transform.position);     
            }
        }

        return closestNode;
    }

    //Line of sight
    public static bool OnSight(Vector3 from, Vector3 to, LayerMask obstacleMask)
    {
        var dir = to - from;
        return !Physics.Raycast(from, dir, dir.magnitude, obstacleMask);
    }

    //Field of view
    public static bool FieldOfView(Vector3 from, Vector3 forward, Vector3 target, float viewAngle, LayerMask obstacleMask)
    {
        var dir = target - from;

        if(Vector3.Angle(forward, dir) < viewAngle / 2 && OnSight(from, target, obstacleMask))
        {
            return true;
        }

        return false;
    }
    
}

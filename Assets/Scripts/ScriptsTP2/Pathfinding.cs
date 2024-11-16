using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private HashSet<Node> _closedNodes = new HashSet<Node>();
    private PriorityQueue _openNodes = new PriorityQueue();

    public List<Node> GetPath(Node starNode, Node endNode)
    {
        _closedNodes.Clear();
        _openNodes.Clear();

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

        while(actualNode != null && actualNode != starNode && actualNode.previousNode != null)
        {
            finalPath.Add(actualNode);
            actualNode = actualNode.previousNode;
        }

        finalPath.Reverse();
        return finalPath;
    }
}

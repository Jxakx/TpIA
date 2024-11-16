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

                var heuristic = actualNode.heuristic + 1 + Vector3.Distance(neighbour.transform.position, endNode.transform.position);
            }
        }
    }
}

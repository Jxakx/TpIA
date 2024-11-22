using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding2
{
    public List<Node> nodes = new List<Node>();
    public List<Node> AStar(Node start, Node goal)
    {
        if (start == null || goal == null) return null;

        PriorityQueue2 frontier = new PriorityQueue2();
        frontier.Put(start, 0);

        Dictionary<Node, Node> camefrom = new Dictionary<Node, Node>();
        camefrom.Add(start, null);

        Dictionary<Node, float> costSoFar = new Dictionary<Node, float>();
        costSoFar.Add(start, 0);

        while (frontier.AllCostCount > 0)
        {
            Node current = frontier.Get();

            if (current == goal)
            {
                List<Node> path = new List<Node>();
                Node nodeToAdd = current;

                while (nodeToAdd != null)
                {
                    path.Add(nodeToAdd);
                    nodeToAdd = camefrom[nodeToAdd];
                }
                return path;
            }

            foreach (Node nodeAround in current.GetNeighborsNodes()) //Me da los nodos vecinos
            {

                float newCost = costSoFar[current] + nodeAround.cost;
                float dist = (goal.transform.position - nodeAround.transform.position).magnitude;
                float priority = newCost + dist;

                if (!camefrom.ContainsKey(nodeAround))
                {
                    frontier.Put(nodeAround, priority);
                    camefrom.Add(nodeAround, current);
                    costSoFar.Add(nodeAround, newCost);
                }
                else if (newCost < costSoFar[nodeAround])
                {
                    frontier.Put(nodeAround, priority);
                    camefrom[nodeAround] = current;
                    costSoFar[nodeAround] = newCost;
                }
            }
        }
            
        return null;
    }    
}

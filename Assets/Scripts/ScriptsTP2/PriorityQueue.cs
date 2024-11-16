using System.Collections.Generic;

public class PriorityQueue
{
    private Dictionary<Node, float> _priorityQueue = new Dictionary<Node, float>();

    //Agregar objetos a la Queue
    public void Enqueue (Node node, float heuristic)
    {
        if (!_priorityQueue.ContainsKey(node))
        {
            _priorityQueue[node] = heuristic;
        }
        else
        {
            _priorityQueue.Add(node, heuristic);
        }
    }

    //Quitar objetos a la Queue
    public Node Dequeue()
    {
        if (_priorityQueue.Count == 0)
        {
            return null;
        }

        Node minNode = null;

        foreach(var item in _priorityQueue)
        {
            if (minNode == null)
            {
                minNode = item.Key;
            }
            else if (item.Value < _priorityQueue[minNode])
            {
                minNode = item.Key;
            }
        }

        _priorityQueue.Remove(minNode);

        return minNode;
    }

    public int Count()
    {
        return _priorityQueue.Count;
    }
    public void Clear()
    {
        _priorityQueue.Clear();
    }

    public bool Contains(Node node)
    {
        return _priorityQueue.ContainsKey(node);
    }


}
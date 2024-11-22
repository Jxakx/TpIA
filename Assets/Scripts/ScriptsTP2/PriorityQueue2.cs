using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue2 
{
    Dictionary<Node, float> _allCost = new Dictionary<Node, float>();

    public int AllCostCount
    {
        get
        {
            return _allCost.Count;
        }
    }

    public void Put(Node node, float cost)
    {
        if (_allCost.ContainsKey(node))
        {
            _allCost[node] = cost;
        }
        else
        {
            _allCost.Add(node, cost);
        }
    }

    public Node Get()
    {
        Node node = null;
        float loweCost = Mathf.Infinity;

        foreach (var item in _allCost)
        {
            if (item.Value < loweCost)
            {
                node = item.Key;
                loweCost = item.Value;
            }
        }

        _allCost.Remove(node);

        return node;
    }
}

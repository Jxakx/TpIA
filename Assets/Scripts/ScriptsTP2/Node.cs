using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Node : MonoBehaviour
{
    public float heuristic;

    public float Heuristic 
    { 
        set { heuristic = value; } 
        get { return heuristic; }
    }

    [SerializeField] private List<Node> _neighbours = new();
    public List<Node> Neighbours { get { return _neighbours; } }

    public Node previousNode;

    public int cost = 1;

    public List<Node> GetNeighborsNodes()
    {
        return _neighbours;
    }



    private void Awake()
    {
        heuristic = 9999;
    }

    private void Start()
    {
        FuncionesPaths.Instance.nodes.Add(this);
    }

    public void SetHeuristic(Vector3 from, Vector3 target, float previousheuristic)
    {
        heuristic = previousheuristic + Vector3.Distance(from, transform.position) + 
            Vector3.Distance(transform.position, target);
    }

    private void OnDrawGizmos()
    {
        if(_neighbours.Count > 0)
        {
            foreach(var neighbour  in _neighbours)
            {
                Gizmos.DrawLine(transform.position, neighbour.transform.position);
            }
        }
    }

}

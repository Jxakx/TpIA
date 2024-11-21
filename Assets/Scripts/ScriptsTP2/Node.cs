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

    [SerializeField] private float detectionRadius;
    public List<Node> Neighbours { get { return _neighbours; } }

    public Node previousNode;

    public int cost = 1;

    public List<Node> GetNeighborsNodes()
    {
        return _neighbours;
    }



    private void Awake()
    {
        var colliders = Physics.OverlapSphere(transform.position, detectionRadius, LayerMask.GetMask("Node"));

        heuristic = 9999;

        foreach (var collider in colliders)
        {
            var node = collider.GetComponent<Node>();
            
            if(node != null && node != this && Pathfinding.OnSight(transform.position,node.transform.position,Pathfinding.Instance._obstacleMask))
            {
                _neighbours.Add(node);
            }
        }
    }

    private void Start()
    {
        Pathfinding.Instance.nodes.Add(this);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

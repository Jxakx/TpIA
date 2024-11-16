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

    private void Awake()
    {
        var colliders = Physics.OverlapSphere(transform.position, detectionRadius, LayerMask.GetMask("Node"));

        foreach (var collider in colliders)
        {
            var node = collider.GetComponent<Node>();
            
            if(node != null && node != this)
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
        Gizmos.DrawSphere(transform.position, detectionRadius);
    }
}

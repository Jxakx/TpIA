using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Node : MonoBehaviour
{
    public float heuristic { get; private set; }

    [SerializeField] private List<Node> _neighbours = new();
    [SerializeField] private float detectionRadius;

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

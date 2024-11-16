using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnemies : MonoBehaviour
{
    [SerializeField] private float _obstacleDist;
    [SerializeField] private LayerMask _obstacleMask;
    private Vector3 _avoidanceDir;

    [SerializeField] private Transform _target; // Target (Transform)
    private Vector3 _desiredDir;

    [SerializeField] private float rotationSpeed;
    [SerializeField] private float speed;

    [SerializeField, Range(0f, 1f)] private float seekWeight; // "0.5"
    [SerializeField, Range(0f, 1f)] private float obstacleWeight; // "0.743"

    private Action followingAction = delegate { };

    private void Update()
    {
        followingAction();
        
        if(Input.GetKey(KeyCode.Q))
        {
            followingAction = ObstacleAvoidanceState;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            followingAction = PathfindingState;
        }
    }

    private void PathFindingState()
    {
        
    }

    public void ObstacleAvoidanceState()
    {
        _desiredDir = Seek().normalized * seekWeight + ObstacleAvoidance().normalized * obstacleWeight;
        transform.forward = Vector3.Lerp(transform.forward, _desiredDir, rotationSpeed * Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public Vector3 Seek()
    {
        var seekVector = _target.position - transform.position;
        seekVector.y = 0;
        return seekVector;
    }

    public Vector3 ObstacleAvoidance()
    {
        _avoidanceDir = Vector3.zero;
        var obstacles = Physics.OverlapSphere(transform.position, _obstacleDist, (int)_obstacleMask);

        if (obstacles.Length > 0)
        {
            foreach (var obstacle in obstacles)
            {
                var dir = transform.position - obstacle.transform.position;
                _avoidanceDir += dir.normalized * (_obstacleDist - dir.magnitude);
            }
        }

        _avoidanceDir.y = 0;
        return _avoidanceDir;
    }

}

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

    private List<Node> _path = new List<Node>();

    [SerializeField] private float viewRange;
    [SerializeField] private float viewAngle;
    private void Update()
    {
        followingAction();
        
        if(Input.GetKey(KeyCode.Q))
        {
            followingAction = ObstacleAvoidanceState;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            followingAction = PathFindingState;
        }
    }

    private void PathFindingState()
    {
        if (Vector3.Distance(transform.position, _target.position) < .5f)
        {
            return;
        }

        if(_path.Count <= 0)
        {
            _path = Pathfinding.Instance.GetPath(Pathfinding.Instance.getClosestNode(transform.position), Pathfinding.Instance.getClosestNode(_target.position));

            if (_path.Count == 0)
            {
                return;
            }
        }

        

        var dir = _path[0].transform.position - transform.position;
        transform.position += dir.normalized * speed * Time.deltaTime;

        if (dir.magnitude < .5f)
        {
            _path.RemoveAt(0);
        }
    }

    #region Obstacle Region

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
        var obstacles = Physics.OverlapSphere(transform.position, _obstacleDist, _obstacleMask);

        if (obstacles.Length > 0)
        {
            foreach (var obstacle in obstacles)
            {
                var dir = transform.position - obstacle.transform.position;
                _avoidanceDir += dir.normalized;
            }
        }

        _avoidanceDir.y = 0;
        return _avoidanceDir;
    }


    #endregion

    #region OnSight
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _obstacleDist);

        if (Pathfinding.OnSight(transform.position, _target.position, LayerMask.GetMask("Obstacle")))
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        //Gizmos.DrawLine(transform.position, _target.position);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRange);

        var dirA = DirFromAngle(viewAngle / 2 + transform.eulerAngles.y);
        var dirB = DirFromAngle(-viewAngle / 2 + transform.eulerAngles.y);

        Gizmos.DrawLine(transform.position, transform.position + dirA * viewRange);
        Gizmos.DrawLine(transform.position, transform.position + dirB * viewRange);

        if(Pathfinding.FieldOfView(transform.position, transform.forward, _target.position, viewAngle, LayerMask.GetMask("Obstacle"))
            && Vector3.Distance(transform.position, _target.position) < viewRange)
        {
            Gizmos.color = Color.blue;
        }
        else
        {
            Gizmos.color = Color.yellow;
        }

        Gizmos.DrawLine(transform.position, _target.position);
    }

    private Vector3 DirFromAngle(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, MathF.Cos(angle * Mathf.Deg2Rad));
    }
    #endregion


}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnemies : MonoBehaviour
{
    #region Variables
    [SerializeField] private float _obstacleDist;
    [SerializeField] private LayerMask _obstacleMask;

    [SerializeField] private Transform _target;
    [SerializeField] private float _speed;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private float viewRange = 10f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private List<Transform> _waypoints;

    public List<Node> _path = new List<Node>();
    public StateMachine StateMachine { get; private set; } = new StateMachine();
    public Transform Player => _target;
    public float Speed => _speed;
    public List<Transform> Waypoints => _waypoints;

    public Node lastVisitedNode;


    public PathFinding2 pathFinding = new PathFinding2();
    public FuncionesPaths funcionesPaths;

    public Transform player;

    #endregion
    private void Start()
    {
        lastVisitedNode = FuncionesPaths.Instance.getClosestNode(transform.position);
        StateMachine.ChangeState(new PatrolState(), this);
    }

    private void Update()
    {
        StateMachine.Update(this);
    }

    public bool IsPlayerInSight()
    {
        Vector3 dirToPlayer = (_target.position - transform.position).normalized;

        // Verificar si el jugador está dentro del ángulo de visión
        if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _target.position);

            // Si no hay un obstáculo bloqueando la visión, devolver true (jugador está visible)
            if (distanceToPlayer <= viewRange && !Physics.Raycast(transform.position, dirToPlayer, distanceToPlayer, _obstacleMask))
            {
                return true;
            }
            else if (distanceToPlayer <= viewRange && Physics.Raycast(transform.position, dirToPlayer, distanceToPlayer, _obstacleMask))
            {
                StateMachine.ChangeState(new PatrolState(), this);
            }
        }

        return false; // No ve al jugador ni hay razón para continuar el estado actual
    }

    public void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;

        if (direction.magnitude > 0.01f)
        {
            RotateTowards(direction);
            transform.position += direction.normalized * Speed * Time.deltaTime;
        }
    }

    private void RotateTowards(Vector3 direction)
    {
        if (direction.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
 
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireSphere(transform.position, viewRange);

        // Dibuja las líneas del ángulo de visión
        Vector3 dirA = DirFromAngle(viewAngle / 2 + transform.eulerAngles.y);
        Vector3 dirB = DirFromAngle(-viewAngle / 2 + transform.eulerAngles.y);

        Gizmos.DrawLine(transform.position, transform.position + dirA * viewRange);
        Gizmos.DrawLine(transform.position, transform.position + dirB * viewRange);

        // Cambia de color si el jugador está en el rango de visión
        Gizmos.color = IsPlayerInSight() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _obstacleDist);
    }

    private Vector3 DirFromAngle(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}

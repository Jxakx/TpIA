using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnemies : MonoBehaviour
{
    // Variables principales
    [SerializeField] private float _obstacleDist;
    [SerializeField] private LayerMask _obstacleMask;
    private Vector3 _avoidanceDir;

    [SerializeField] private Transform _target;
    [SerializeField] private float _speed;
    [SerializeField] private float rotationSpeed; // Velocidad de rotación
    [SerializeField, Range(0f, 1f)] private float seekWeight = 0.5f;
    [SerializeField, Range(0f, 1f)] private float obstacleWeight = 0.743f;

    [SerializeField] private float viewRange = 10f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private List<Transform> _waypoints;

    private Vector3 _desiredDir;
    public List<Node> _path = new List<Node>();
    public StateMachine StateMachine { get; private set; } = new StateMachine();
    public Transform Player => _target;
    public float Speed => _speed;
    public List<Transform> Waypoints => _waypoints;

    public Node lastVisitedNode; // Nodo visitado más recientemente


    public PathFinding2 pathFinding = new PathFinding2();
    public Pathfinding funcionesPaths;

    public Transform player;

    private void Start()
    {
        lastVisitedNode = Pathfinding.Instance.getClosestNode(transform.position);
        StateMachine.ChangeState(new PatrolState(), this);
    }

    private void Update()
    {
        StateMachine.Update(this);
    }


    public bool IsPlayerInSight()
    {
        Vector3 dirToPlayer = (_target.position - transform.position).normalized;

        if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
        {
            
            float distanceToPlayer = Vector3.Distance(transform.position, _target.position);

            if(distanceToPlayer <= 0.5f) //Distancia de choque con el player
            {
                StateMachine.ChangeState(new PatrolState(), this); //Cambie el PatrolAStar por el PatrolState
            }

            if (distanceToPlayer <= viewRange &&
                !Physics.Raycast(transform.position, dirToPlayer, distanceToPlayer, _obstacleMask))
            {
                return true;
            }
        }

        return false;
    }

    public void MoveTowards(Vector3 targetPosition)
    {
        // Calcula la dirección hacia el objetivo
        Vector3 direction = targetPosition - transform.position;

        if (direction.magnitude > 0.01f) // Evita cálculos innecesarios
        {
            // Rota hacia la dirección del objetivo
            RotateTowards(direction);

            // Mueve hacia el objetivo
            transform.position += direction.normalized * Speed * Time.deltaTime;
        }
    }

    /// <summary>
    /// Rota suavemente hacia una dirección.
    /// </summary>
    private void RotateTowards(Vector3 direction)
    {
        if (direction.magnitude > 0.01f) // Solo rota si hay una dirección significativa
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    //public void PathFindingState()
    //{
    //    // Paso 1: Verificar si estamos cerca del objetivo
    //    if (Vector3.Distance(transform.position, _target.position) < 0.5f)
    //    {
    //        return; // Si ya estamos cerca del objetivo, no hacemos nada
    //    }

    //    // Paso 2: Comprobar si necesitamos calcular un nuevo camino
    //    if (_path.Count <= 0)
    //    {
    //        // Paso 3: Calcular el camino desde el nodo más cercano a la posición actual al nodo más cercano al objetivo
    //        _path = Pathfinding.Instance.GetPath(
    //            Pathfinding.Instance.getClosestNode(transform.position),
    //            Pathfinding.Instance.getClosestNode(_target.position)
    //        );

    //        if (_path.Count == 0)
    //        {
    //            return; // Si no se encontró un camino, salir
    //        }
    //    }

    //    // Paso 4: Moverse hacia el primer nodo de la ruta calculada
    //    if (_path.Count > 0)
    //    {
    //        Transform nextNode = _path[0].transform;

    //        // Mueve hacia el nodo objetivo
    //        MoveTowards(nextNode.position);

    //        // Si alcanza el nodo actual, remuévelo de la lista
    //        if (Vector3.Distance(transform.position, nextNode.position) < 0.5f)
    //        {
    //            _path.RemoveAt(0); // Eliminar el nodo alcanzado de la lista
    //        }
    //    }
    //}


    /// <summary>
    /// Evasión de obstáculos.
    /// </summary>
    public void ObstacleAvoidanceState()
    {
        _desiredDir = Seek().normalized * seekWeight + ObstacleAvoidance().normalized * obstacleWeight;
        MoveTowards(transform.position + _desiredDir);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        // Dibuja el rango de visión
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

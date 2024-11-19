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

    [SerializeField] private Transform _target; // Jugador o objetivo
    [SerializeField] private float _speed; // Velocidad de movimiento
    [SerializeField] private float rotationSpeed; // Velocidad de rotaci�n
    [SerializeField, Range(0f, 1f)] private float seekWeight = 0.5f;
    [SerializeField, Range(0f, 1f)] private float obstacleWeight = 0.743f;

    [SerializeField] private float viewRange = 10f; // Distancia de visi�n configurable
    [SerializeField] private float viewAngle = 90f; // �ngulo de visi�n configurable
    [SerializeField] private List<Transform> _waypoints; // Nodos para patrullaje

    private Action _followingAction = delegate { };
    private List<Node> _path = new List<Node>();
    private Vector3 _desiredDir;

    // M�quina de estados
    public StateMachine StateMachine { get; private set; } = new StateMachine();

    // Propiedades p�blicas
    public Transform Player => _target; // Referencia al objetivo
    public float Speed => _speed; // Velocidad de movimiento
    public List<Transform> Waypoints => _waypoints; // Nodos para patrullaje

    private void Start()
    {
        // Inicializa la m�quina de estados en el estado de patrullaje
        StateMachine.ChangeState(new PatrolState(), this);
    }

    private void Update()
    {
        // Actualizar la l�gica del FSM en cada frame
        StateMachine.Update(this);
    }

    /// <summary>
    /// Comprueba si el jugador est� en el campo de visi�n del NPC.
    /// </summary>
    /// <returns>True si el jugador est� visible y dentro de la distancia y �ngulo configurados, false en caso contrario.</returns>
    public bool IsPlayerInSight()
    {
        // Calcula la direcci�n hacia el jugador
        Vector3 dirToPlayer = (_target.position - transform.position).normalized;

        // Comprueba si el jugador est� dentro del �ngulo de visi�n
        if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
        {
            // Calcula la distancia al jugador
            float distanceToPlayer = Vector3.Distance(transform.position, _target.position);

            // Comprueba si est� dentro del rango de visi�n y sin obst�culos
            if (distanceToPlayer <= viewRange &&
                !Physics.Raycast(transform.position, dirToPlayer, distanceToPlayer, _obstacleMask))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// L�gica del estado de pathfinding.
    /// </summary>
    private void PathFindingState()
    {
        if (Vector3.Distance(transform.position, _target.position) < 0.5f)
        {
            return;
        }

        if (_path.Count <= 0)
        {
            _path = Pathfinding.Instance.GetPath(
                Pathfinding.Instance.getClosestNode(transform.position),
                Pathfinding.Instance.getClosestNode(_target.position)
            );

            if (_path.Count == 0)
            {
                return;
            }
        }

        Vector3 dir = _path[0].transform.position - transform.position;
        transform.position += dir.normalized * Speed * Time.deltaTime;

        if (dir.magnitude < 0.5f)
        {
            _path.RemoveAt(0);
        }
    }

    /// <summary>
    /// L�gica del estado de evasi�n de obst�culos.
    /// </summary>
    public void ObstacleAvoidanceState()
    {
        _desiredDir = Seek().normalized * seekWeight + ObstacleAvoidance().normalized * obstacleWeight;
        transform.forward = Vector3.Lerp(transform.forward, _desiredDir, rotationSpeed * Time.deltaTime);
        transform.position += transform.forward * Speed * Time.deltaTime;
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

        // Dibuja el rango de visi�n
        Gizmos.DrawWireSphere(transform.position, viewRange);

        // Dibuja las l�neas del �ngulo de visi�n
        Vector3 dirA = DirFromAngle(viewAngle / 2 + transform.eulerAngles.y);
        Vector3 dirB = DirFromAngle(-viewAngle / 2 + transform.eulerAngles.y);

        Gizmos.DrawLine(transform.position, transform.position + dirA * viewRange);
        Gizmos.DrawLine(transform.position, transform.position + dirB * viewRange);

        // Cambia de color si el jugador est� en el rango de visi�n
        Gizmos.color = IsPlayerInSight() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRange);
    }

    private Vector3 DirFromAngle(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}

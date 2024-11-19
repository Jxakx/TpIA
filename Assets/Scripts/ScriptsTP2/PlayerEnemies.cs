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
    [SerializeField] private float rotationSpeed; 
    [SerializeField, Range(0f, 1f)] private float seekWeight = 0.5f;
    [SerializeField, Range(0f, 1f)] private float obstacleWeight = 0.743f;

    [SerializeField] private float viewRange = 10f; 
    [SerializeField] private float viewAngle = 90f; 
    [SerializeField] private List<Transform> _waypoints;

    private Vector3 _desiredDir;

    // M�quina de estados
    public StateMachine StateMachine { get; private set; } = new StateMachine();

    // Propiedades p�blicas
    public Transform Player => _target; 
    public float Speed => _speed; 
    public List<Transform> Waypoints => _waypoints; 

    private void Start()
    {
        StateMachine.ChangeState(new PatrolState(), this);
    }

    private void Update()
    {
        StateMachine.Update(this);
    }


    //Comprueba si el jugador est� en el campo de visi�n del NPC.
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
    

    //evasi�n de obst�culos.
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

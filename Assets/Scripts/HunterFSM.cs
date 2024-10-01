using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterFSM : MonoBehaviour
{
    enum HunterState { Patrol, Chase, Recharge }
    HunterState currentState = HunterState.Patrol;

    public Transform[] waypoints;  // Puntos de patrullaje
    public float speed = 3f;  // Velocidad de movimiento del cazador
    public float detectionRange = 5f;  // Rango de detección de los boids
    public float energy = 100f;  // Nivel de energía del cazador
    public float energyDecayRate = 0.5f;  // Velocidad de pérdida de energía
    public float rechargeRate = 5f;  // Velocidad de recarga de energía
    public Transform rechargeStation;  // Lugar de recarga

    private int currentWaypointIndex = 0;

    void Update()
    {
        switch (currentState)
        {
            case HunterState.Patrol:
                Patrol();
                break;
            case HunterState.Chase:
                Chase();
                break;
            case HunterState.Recharge:
                Recharge();
                break;
        }

        // Reducir energía a lo largo del tiempo, excepto durante la recarga
        if (currentState != HunterState.Recharge)
        {
            energy -= energyDecayRate * Time.deltaTime;
            if (energy <= 0)
            {
                ChangeState(HunterState.Recharge);
            }
        }
    }

    void Patrol()
    {
        // Patrullaje entre waypoints
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        MoveTowards(targetWaypoint.position);

        // Si llega al waypoint, cambia al siguiente
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }

        // Si detecta un boid, cambia a estado de persecución
        Boid closestBoid = DetectBoids();
        if (closestBoid != null)
        {
            ChangeState(HunterState.Chase);
        }
    }

    void Chase()
    {
        Boid closestBoid = DetectBoids();
        if (closestBoid != null)
        {
            MoveTowards(closestBoid.transform.position);
        }
        else
        {
            ChangeState(HunterState.Patrol);
        }
    }

    void Recharge()
    {
        // Moverse hacia la estación de recarga
        MoveTowards(rechargeStation.position);

        // Si llega a la estación, recargar energía
        if (Vector3.Distance(transform.position, rechargeStation.position) < 1f)
        {
            energy += rechargeRate * Time.deltaTime;
            if (energy >= 100f)
            {
                energy = 100f;
                ChangeState(HunterState.Patrol);
            }
        }
    }

    Boid DetectBoids()
    {
        // Detectar boids dentro del rango de detección
        Boid[] boids = FindObjectsOfType<Boid>();
        foreach (Boid boid in boids)
        {
            if (Vector3.Distance(transform.position, boid.transform.position) < detectionRange)
            {
                return boid;
            }
        }
        return null;
    }

    void MoveTowards(Vector3 targetPosition)
    {
        // Moverse hacia la posición objetivo
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    void ChangeState(HunterState newState)
    {
        currentState = newState;
    }
}

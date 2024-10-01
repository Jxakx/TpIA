using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterFSM : MonoBehaviour
{
    enum HunterState { Patrol, Chase, Recharge }
    HunterState currentState = HunterState.Patrol;

    public Transform[] waypoints; // Puntos de patrullaje
    public float speed = 3f;
    public float detectionRange = 5f; // Rango de detección de los boids
    public float energy;
    public float energyDecayRate = 0.5f;
    public float rechargeRate = 5f;
    public Transform rechargeStation; // Lugar donde se recarga energía

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

        // Reducir energía en todo momento excepto cuando recarga
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

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }

        // Cambiar a persecución si detecta a un boid
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
        MoveTowards(rechargeStation.position);
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
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    void ChangeState(HunterState newState)
    {
        currentState = newState;
    }
}

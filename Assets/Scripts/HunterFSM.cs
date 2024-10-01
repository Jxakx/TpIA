using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterFSM : MonoBehaviour
{
    enum HunterState { Patrol, Chase, Recharge }
    HunterState currentState = HunterState.Patrol;

    public Transform[] waypoints;
    public float speed = 5f;
    public float detectionRange = 10f;
    public float energy = 100f;
    public float energyDecayRate = 0.5f;
    public float rechargeRate = 5f;
    public Transform rechargeStation;

    private int currentWaypointIndex = 0;
    private bool movingForward = true;  // Variable para saber si está avanzando o retrocediendo en los waypoints

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

        // Pérdida de energía
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
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        MoveTowards(targetWaypoint.position);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 1f)
        {
            // Si está avanzando, incrementa el índice, si está retrocediendo, decrementa
            if (movingForward)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Length)  // Si llega al último waypoint, cambia de dirección
                {
                    currentWaypointIndex = waypoints.Length - 1;
                    movingForward = false;
                }
            }
            else
            {
                currentWaypointIndex--;
                if (currentWaypointIndex < 0)  // Si llega al primer waypoint, cambia de dirección
                {
                    currentWaypointIndex = 0;
                    movingForward = true;
                }
            }
        }

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
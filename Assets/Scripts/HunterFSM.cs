
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterFSM : MonoBehaviour
{
    private enum HunterState { Patrol, Chase, Recharge }
    private HunterState currentState = HunterState.Patrol;

    public Transform[] waypoints;
    public float speed = 5f;
    public float detectionRange = 10f;
    public float energy = 100f;
    public float energyDecayRate = 0.5f;
    public float rechargeRate = 5f;
    public Transform rechargeStation;

    private int currentWaypointIndex = 0;
    private bool movingForward = true;

    void Update()
    {
        HandleState();
        HandleEnergyDecay();
    }

    private void HandleState()
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
    }

    private void Patrol()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        MoveTowards(targetWaypoint.position);

        if (IsNear(targetWaypoint.position))
        {
            UpdateWaypointIndex();
        }

        Boid closestBoid = DetectBoid();
        if (closestBoid != null)
        {
            ChangeState(HunterState.Chase);
        }
    }

    private void UpdateWaypointIndex()
    {
        if (movingForward)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = waypoints.Length - 1;
                movingForward = false;
            }
        }
        else
        {
            currentWaypointIndex--;
            if (currentWaypointIndex < 0)
            {
                currentWaypointIndex = 0;
                movingForward = true;
            }
        }
    }

    private void Chase()
    {
        Boid closestBoid = DetectBoid();
        if (closestBoid != null)
        {
            MoveTowards(closestBoid.transform.position);
        }
        else
        {
            ChangeState(HunterState.Patrol);
        }
    }

    private void Recharge()
    {
        MoveTowards(rechargeStation.position);
        if (IsNear(rechargeStation.position))
        {
            energy += rechargeRate * Time.deltaTime;
            if (energy >= 100f)
            {
                energy = 100f;
                ChangeState(HunterState.Patrol);
            }
        }
    }

    private Boid DetectBoid()
    {
        Boid[] boids = FindObjectsOfType<Boid>();
        Boid closestBoid = null;
        float closestDistance = Mathf.Infinity;

        foreach (Boid boid in boids)
        {
            float distance = Vector3.Distance(transform.position, boid.transform.position);
            if (distance < detectionRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestBoid = boid;
            }
        }

        return closestBoid;
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    private bool IsNear(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) < 1f;
    }

    private void HandleEnergyDecay()
    {
        if (currentState != HunterState.Recharge)
        {
            energy -= energyDecayRate * Time.deltaTime;
            if (energy <= 0)
            {
                energy = 0;
                ChangeState(HunterState.Recharge);
            }
        }
    }

    private void ChangeState(HunterState newState)
    {
        currentState = newState;
    }
}

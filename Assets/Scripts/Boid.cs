
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float speed = 5f;
    public float acceleration = 10f;
    public float evadeBoost = 1.5f; // Boost multiplier when fleeing
    public float neighborDistance = 10f;
    public float cohesionStrength = 1f;
    public float separationDistance = 2f;
    public float separationStrength = 1.5f;
    public float alignmentStrength = 1f;
    public Transform hunter;
    public float fleeDistance = 10f;
    public float fleeStrength = 2f;
    private Vector3 velocity;
    private Vector3 desiredVelocity;

    public float mapWidth = 50f;
    public float mapDepth = 50f;

    public float foodDetectionRange = 15f;
    private Transform nearestFood;
    public float arriveRadius = 1.5f;
    public float foodEatenDistance = 1f;

    private List<Boid> neighbors;
    private List<Boid> closeNeighbors;

    private float decisionCooldown = 0.3f;
    private float lastDecisionTime;

    private Vector3 baseDirection;
    private float baseDirectionChangeCooldown = 2f;
    private float lastBaseDirectionChangeTime;

    void Start()
    {
        velocity = RandomDirection() * speed;
        neighbors = new List<Boid>();
        closeNeighbors = new List<Boid>();
        baseDirection = RandomDirection();
    }

    void Update()
    {
        if (Time.time - lastDecisionTime > decisionCooldown)
        {
            lastDecisionTime = Time.time;
            UpdateBoidBehavior();
        }

        SmoothMovement();
        WrapPosition();
        CheckFoodProximity();
    }

    private void SmoothMovement()
    {
        velocity = Vector3.Lerp(velocity, desiredVelocity, acceleration * Time.deltaTime).normalized * speed;
        transform.position += velocity * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    private void WrapPosition()
    {
        float buffer = 0.1f;
        if (transform.position.x > mapWidth / 2)
            transform.position = new Vector3(-mapWidth / 2 + buffer, transform.position.y, transform.position.z);
        else if (transform.position.x < -mapWidth / 2)
            transform.position = new Vector3(mapWidth / 2 - buffer, transform.position.y, transform.position.z);

        if (transform.position.z > mapDepth / 2)
            transform.position = new Vector3(transform.position.x, transform.position.y, -mapDepth / 2 + buffer);
        else if (transform.position.z < -mapDepth / 2)
            transform.position = new Vector3(transform.position.x, transform.position.y, mapDepth / 2 - buffer);
    }

    private void CheckFoodProximity()
    {
        if (nearestFood != null && Vector3.Distance(transform.position, nearestFood.position) < foodEatenDistance)
        {
            Destroy(nearestFood.gameObject);
            nearestFood = null;
        }
    }

    void UpdateBoidBehavior()
    {
        UpdateNeighbors();
        DetectNearestFood();

        Vector3 cohesion = Cohesion() * cohesionStrength;
        Vector3 separation = Separation() * separationStrength;
        Vector3 alignment = Alignment() * alignmentStrength;
        Vector3 flee = Vector3.zero;

        // If the hunter is close, prioritize fleeing with an evade boost
        if (Vector3.Distance(transform.position, hunter.position) < fleeDistance)
        {
            flee = FleeFromHunter() * fleeStrength * evadeBoost;
            desiredVelocity = flee.normalized * speed; // Evade overrides other behaviors
            return; // Skip further behavior calculations during flee
        }

        Vector3 moveToFood = Vector3.zero;
        if (nearestFood != null && Vector3.Distance(hunter.position, nearestFood.position) > fleeDistance)
            moveToFood = ArriveAtFood(nearestFood.position);

        if (Time.time - lastBaseDirectionChangeTime > baseDirectionChangeCooldown)
        {
            baseDirection = RandomDirection();
            lastBaseDirectionChangeTime = Time.time;
        }

        desiredVelocity = (cohesion + separation + alignment + moveToFood + baseDirection * 0.5f).normalized * speed;
    }

    Vector3 ArriveAtFood(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        float distance = direction.magnitude;

        if (distance < arriveRadius)
            return direction.normalized * speed * (distance / arriveRadius);
        return direction.normalized * speed;
    }

    void UpdateNeighbors()
    {
        neighbors.Clear();
        closeNeighbors.Clear();
        foreach (Boid boid in FindObjectsOfType<Boid>())
        {
            if (boid != this)
            {
                float distance = Vector3.Distance(transform.position, boid.transform.position);
                if (distance < neighborDistance)
                {
                    neighbors.Add(boid);
                    if (distance < separationDistance)
                        closeNeighbors.Add(boid);
                }
            }
        }
    }

    Vector3 Cohesion()
    {
        if (neighbors.Count == 0) return Vector3.zero;

        Vector3 center = Vector3.zero;
        foreach (Boid neighbor in neighbors)
            center += neighbor.transform.position;

        center /= neighbors.Count;
        return (center - transform.position).normalized;
    }

    Vector3 Separation()
    {
        if (closeNeighbors.Count == 0) return Vector3.zero;

        Vector3 force = Vector3.zero;
        foreach (Boid neighbor in closeNeighbors)
        {
            Vector3 direction = transform.position - neighbor.transform.position;
            force += direction.normalized / direction.magnitude;
        }
        return force.normalized;
    }

    Vector3 Alignment()
    {
        if (neighbors.Count == 0) return Vector3.zero;

        Vector3 avgVelocity = Vector3.zero;
        foreach (Boid neighbor in neighbors)
            avgVelocity += neighbor.velocity;

        avgVelocity /= neighbors.Count;
        return avgVelocity.normalized;
    }

    void DetectNearestFood()
    {
        GameObject[] foodItems = GameObject.FindGameObjectsWithTag("Food");
        float closestDistance = Mathf.Infinity;
        nearestFood = null;

        foreach (GameObject food in foodItems)
        {
            float distance = Vector3.Distance(transform.position, food.transform.position);
            if (distance < closestDistance && distance < foodDetectionRange)
            {
                closestDistance = distance;
                nearestFood = food.transform;
            }
        }
    }

    Vector3 FleeFromHunter()
    {
        Vector3 fleeDirection = (transform.position - hunter.position).normalized;
        Vector3 randomOffset = Random.insideUnitSphere * 0.5f;
        fleeDirection += new Vector3(randomOffset.x, 0, randomOffset.z).normalized;
        return fleeDirection.normalized;
    }

    private Vector3 RandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }
}

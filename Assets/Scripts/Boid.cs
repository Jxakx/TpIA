using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float speed = 2f;
    public float neighborDistance = 3f; // Distancia para detectar otros boids
    public float cohesionStrength = 0.5f; // Fuerza de la cohesión
    public float separationDistance = 1.5f; // Distancia mínima antes de separarse
    public float separationStrength = 0.5f; // Fuerza de la separación
    public float alignmentStrength = 0.5f; // Fuerza de la alineación
    public Vector2 velocity;

    private List<Boid> neighbors;

    void Start()
    {
        velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * speed;
        neighbors = new List<Boid>();
    }

    Vector2 Flee(Vector2 threatPosition)
    {
        return ((Vector2)transform.position - threatPosition).normalized;
    }

    void Update()
    {
        UpdateNeighbors();

        // Verificar si el cazador está cerca
        HunterFSM hunter = FindObjectOfType<HunterFSM>();
        if (Vector2.Distance(transform.position, hunter.transform.position) < hunter.detectionRange)
        {
            // Activar evasión si el cazador está cerca
            velocity = Flee(hunter.transform.position) * speed;
        }
        else
        {
            // Continuar con el comportamiento de flocking
            Vector2 cohesion = Cohesion() * cohesionStrength;
            Vector2 separation = Separation() * separationStrength;
            Vector2 alignment = Alignment() * alignmentStrength;

            Vector2 flockingDirection = (cohesion + separation + alignment).normalized;
            velocity = flockingDirection * speed;
        }

        // Movimiento
        transform.position += (Vector3)velocity * Time.deltaTime;
    }
    void UpdateNeighbors()
    {
        neighbors.Clear();
        foreach (Boid boid in FindObjectsOfType<Boid>())
        {
            if (boid != this && Vector2.Distance(transform.position, boid.transform.position) < neighborDistance)
            {
                neighbors.Add(boid);
            }
        }
    }

    Vector2 Cohesion()
    {
        Vector2 centerOfMass = Vector2.zero;
        foreach (Boid neighbor in neighbors)
        {
            centerOfMass += (Vector2)neighbor.transform.position;
        }
        if (neighbors.Count > 0)
        {
            centerOfMass /= neighbors.Count;
            return (centerOfMass - (Vector2)transform.position).normalized;
        }
        return Vector2.zero;
    }

    Vector2 Separation()
    {
        Vector2 separationForce = Vector2.zero;
        foreach (Boid neighbor in neighbors)
        {
            if (Vector2.Distance(transform.position, neighbor.transform.position) < separationDistance)
            {
                separationForce -= ((Vector2)neighbor.transform.position - (Vector2)transform.position).normalized;
            }
        }
        return separationForce.normalized;
    }

    Vector2 Alignment()
    {
        Vector2 avgVelocity = Vector2.zero;
        foreach (Boid neighbor in neighbors)
        {
            avgVelocity += neighbor.velocity;
        }
        if (neighbors.Count > 0)
        {
            avgVelocity /= neighbors.Count;
            return avgVelocity.normalized;
        }
        return Vector2.zero;
    }
}

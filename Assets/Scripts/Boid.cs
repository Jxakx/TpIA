using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float speed = 5f;  // Velocidad de movimiento
    public float neighborDistance = 10f;  // Distancia para detectar otros boids
    public float cohesionStrength = 1f;  // Fuerza que atrae a los boids
    public float separationDistance = 2f;  // Distancia mínima para evitar colisiones
    public float separationStrength = 1.5f;  // Fuerza que repele a los boids
    public float alignmentStrength = 1f;  // Fuerza que alinea la dirección de los boids
    public Transform hunter;  // Referencia al cazador
    public float fleeDistance = 10f;  // Distancia mínima para que los boids huyan del cazador
    public float fleeStrength = 2f;  // Fuerza con la que los boids huyen del cazador
    public Vector3 velocity;  // Vector de velocidad

    public float mapWidth = 50f;  // Ancho del área de movimiento
    public float mapDepth = 50f;  // Profundidad del área de movimiento

    // Para la detección de comida
    public float foodDetectionRange = 15f;  // Rango para detectar la comida
    private Transform nearestFood;  // La comida más cercana

    private List<Boid> neighbors;

    void Start()
    {
        velocity = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * speed;
        neighbors = new List<Boid>();
    }

    void Update()
    {
        UpdateNeighbors();
        DetectNearestFood();  // Detectar la comida más cercana

        Vector3 cohesion = Cohesion() * cohesionStrength;
        Vector3 separation = Separation() * separationStrength;
        Vector3 alignment = Alignment() * alignmentStrength;
        Vector3 flee = Vector3.zero;

        // Si el cazador está cerca, los boids huyen
        if (Vector3.Distance(transform.position, hunter.position) < fleeDistance)
        {
            flee = FleeFromHunter() * fleeStrength;
        }

        Vector3 moveToFood = Vector3.zero;
        if (nearestFood != null)  // Si hay comida cercana, moverse hacia ella
        {
            moveToFood = MoveTowardsFood() * speed;
        }

        // Sumamos las fuerzas de flocking, evasión, y moverse hacia la comida
        Vector3 flockingDirection = (cohesion + separation + alignment + flee + moveToFood).normalized;
        velocity = flockingDirection * speed;

        // Movemos el boid en la dirección calculada
        transform.position += velocity * Time.deltaTime;

        // Limitar el movimiento dentro del área especificada
        float x = Mathf.Clamp(transform.position.x, -mapWidth / 2, mapWidth / 2);
        float z = Mathf.Clamp(transform.position.z, -mapDepth / 2, mapDepth / 2);
        transform.position = new Vector3(x, transform.position.y, z);

        // Mantener el movimiento en el plano XZ
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    void UpdateNeighbors()
    {
        neighbors.Clear();
        foreach (Boid boid in FindObjectsOfType<Boid>())
        {
            if (boid != this && Vector3.Distance(transform.position, boid.transform.position) < neighborDistance)
            {
                neighbors.Add(boid);
            }
        }
    }

    Vector3 Cohesion()
    {
        Vector3 centerOfMass = Vector3.zero;
        foreach (Boid neighbor in neighbors)
        {
            centerOfMass += neighbor.transform.position;
        }
        if (neighbors.Count > 0)
        {
            centerOfMass /= neighbors.Count;
            return (centerOfMass - transform.position).normalized;
        }
        return Vector3.zero;
    }

    Vector3 Separation()
    {
        Vector3 separationForce = Vector3.zero;
        foreach (Boid neighbor in neighbors)
        {
            if (Vector3.Distance(transform.position, neighbor.transform.position) < separationDistance)
            {
                separationForce -= (neighbor.transform.position - transform.position).normalized;
            }
        }
        return separationForce.normalized;
    }

    Vector3 Alignment()
    {
        Vector3 avgVelocity = Vector3.zero;
        foreach (Boid neighbor in neighbors)
        {
            avgVelocity += neighbor.velocity;
        }
        if (neighbors.Count > 0)
        {
            avgVelocity /= neighbors.Count;
            return avgVelocity.normalized;
        }
        return Vector3.zero;
    }

    // Detectar la comida más cercana
    void DetectNearestFood()
    {
        GameObject[] foodItems = GameObject.FindGameObjectsWithTag("Food");
        float closestDistance = Mathf.Infinity;
        nearestFood = null;

        foreach (GameObject food in foodItems)
        {
            float distanceToFood = Vector3.Distance(transform.position, food.transform.position);
            if (distanceToFood < closestDistance && distanceToFood < foodDetectionRange)
            {
                closestDistance = distanceToFood;
                nearestFood = food.transform;
            }
        }
    }

    // Moverse hacia la comida más cercana
    Vector3 MoveTowardsFood()
    {
        if (nearestFood != null)
        {
            return (nearestFood.position - transform.position).normalized;
        }
        return Vector3.zero;
    }

    // Huir del cazador
    Vector3 FleeFromHunter()
    {
        return (transform.position - hunter.position).normalized;
    }

    // Método de colisión: destruir comida al tocarla
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            Destroy(other.gameObject);  // Destruir la comida
            Debug.Log("Comida recogida");
        }
    }
}

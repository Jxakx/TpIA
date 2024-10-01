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

    // Parámetros para suavizar decisiones
    private float decisionCooldown = 0.5f;  // Medio segundo entre decisiones
    private float lastDecisionTime;

    void Start()
    {
        velocity = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * speed;
        neighbors = new List<Boid>();
    }

    void Update()
    {
        // Usamos un cooldown para suavizar los movimientos
        if (Time.time - lastDecisionTime > decisionCooldown)
        {
            lastDecisionTime = Time.time;
            UpdateBoidBehavior();
        }

        // Movemos el boid en la dirección calculada
        transform.position += velocity * Time.deltaTime;

        // Limitar el movimiento dentro del área especificada
        float x = Mathf.Clamp(transform.position.x, -mapWidth / 2, mapWidth / 2);
        float z = Mathf.Clamp(transform.position.z, -mapDepth / 2, mapDepth / 2);
        transform.position = new Vector3(x, transform.position.y, z);

        // Mantener el movimiento en el plano XZ
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    void UpdateBoidBehavior()
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
        if (nearestFood != null && Vector3.Distance(hunter.position, nearestFood.position) > fleeDistance)  // Priorizar huir del cazador sobre ir a la comida
        {
            moveToFood = MoveTowardsFood() * speed;
        }

        // Introducimos una dirección base para que siempre se muevan un poco
        Vector3 baseDirection = new Vector3(1f, 0f, 1f).normalized;

        // Sumamos las fuerzas de flocking, evasión, movimiento hacia la comida y una base de dirección
        Vector3 flockingDirection = (cohesion + separation + alignment + flee + moveToFood + baseDirection * 0.1f).normalized;
        velocity = flockingDirection * speed;
    }

    // Método de colisión: destruir comida al tocarla
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            Destroy(other.gameObject);
            Debug.Log("Comida destruida al tocarla");
        }
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
            float distance = Vector3.Distance(transform.position, neighbor.transform.position);
            if (distance < separationDistance)
            {
                Vector3 fleeDirection = (transform.position - neighbor.transform.position).normalized;
                separationForce += fleeDirection / distance;  // Mayor fuerza cuanto más cerca estén
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
}

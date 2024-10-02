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
    public float arriveRadius = 1.5f;  // Distancia para que el Boid desacelere al llegar a la comida
    public float foodEatenDistance = 1f;  // Distancia mínima para considerar que el boid ha llegado a la comida

    private List<Boid> neighbors;

    // Parámetros para suavizar decisiones
    private float decisionCooldown = 0.3f;  // Tiempo entre decisiones
    private float lastDecisionTime;

    // Nueva variable para almacenar la dirección base persistente
    private Vector3 baseDirection;

    // Tiempo entre cambios de dirección base
    private float baseDirectionChangeCooldown = 2f;
    private float lastBaseDirectionChangeTime;

    void Start()
    {
        velocity = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * speed;
        neighbors = new List<Boid>();

        // Iniciar la dirección base aleatoria
        baseDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

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

        // Comportamiento de "wrapping": Si el boid sale de los límites, reaparece en el lado opuesto, pero ligeramente dentro del mapa
        float buffer = 0.1f;  // Pequeño desplazamiento para evitar quedarse trancado en el borde

        if (transform.position.x > mapWidth / 2)
        {
            transform.position = new Vector3(-mapWidth / 2 + buffer, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -mapWidth / 2)
        {
            transform.position = new Vector3(mapWidth / 2 - buffer, transform.position.y, transform.position.z);
        }

        if (transform.position.z > mapDepth / 2)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -mapDepth / 2 + buffer);
        }
        else if (transform.position.z < -mapDepth / 2)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, mapDepth / 2 - buffer);
        }

        // Mantener el boid a nivel del suelo (plano XZ)
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        // Verificar si está lo suficientemente cerca de la comida para destruirla
        if (nearestFood != null && Vector3.Distance(transform.position, nearestFood.position) < foodEatenDistance)
        {
            Destroy(nearestFood.gameObject);
            nearestFood = null;  // Para evitar que intente seguir la comida destruida
        }
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
            moveToFood = ArriveAtFood(nearestFood.position);  // Aplicamos Arrive
        }

        // Cambiar la dirección base cada cierto tiempo (para evitar titubeos)
        if (Time.time - lastBaseDirectionChangeTime > baseDirectionChangeCooldown)
        {
            baseDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
            lastBaseDirectionChangeTime = Time.time;
        }

        // Sumamos las fuerzas de flocking, evasión, movimiento hacia la comida y la dirección base
        Vector3 flockingDirection = (cohesion + separation + alignment + flee + moveToFood + baseDirection * 0.5f).normalized;
        velocity = flockingDirection * speed;
    }

    Vector3 ArriveAtFood(Vector3 targetPosition)
    {
        Vector3 directionToTarget = targetPosition - transform.position;
        float distance = directionToTarget.magnitude;

        if (distance < arriveRadius)
        {
            float reduceSpeedFactor = distance / arriveRadius;
            return directionToTarget.normalized * speed * reduceSpeedFactor;
        }
        else
        {
            return directionToTarget.normalized * speed;
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

    Vector3 FleeFromHunter()
    {
        Vector3 fleeDirection = (transform.position - hunter.position).normalized;

        // Añadir un poco de aleatoriedad a la dirección de huida
        Vector3 randomOffset = new Vector3(
            Random.Range(-0.5f, 0.5f),
            0,
            Random.Range(-0.5f, 0.5f)
        ).normalized;

        fleeDirection += randomOffset;
        return fleeDirection.normalized;
    }
}

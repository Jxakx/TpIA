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
    private float decisionCooldown = 0.3f;  // Medio segundo entre decisiones
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

        // Verificar si está lo suficientemente cerca de la comida para destruirla
        if (nearestFood != null && Vector3.Distance(transform.position, nearestFood.position) < foodEatenDistance)
        {
            Destroy(nearestFood.gameObject);
            nearestFood = null; // Para evitar que intente seguir la comida destruida
            Debug.Log("Comida destruida al acercarse");
        }

        // Forzar alejamiento de los bordes del mapa
        ForceAwayFromMapEdges();
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

        // Introducimos una dirección base para que siempre se muevan un poco
        Vector3 baseDirection = new Vector3(1f, 0f, 1f).normalized;

        // Sumamos las fuerzas de flocking, evasión, movimiento hacia la comida y una base de dirección
        Vector3 flockingDirection = (cohesion + separation + alignment + flee + moveToFood + baseDirection * 0.1f).normalized;
        velocity = flockingDirection * speed;
    }

    // Función para la llegada suave a la comida (Arrive)
    Vector3 ArriveAtFood(Vector3 targetPosition)
    {
        Vector3 directionToTarget = targetPosition - transform.position;
        float distance = directionToTarget.magnitude;

        if (distance < arriveRadius)
        {
            // Desacelerar cuando se acerque a la comida
            float reduceSpeedFactor = distance / arriveRadius;
            return directionToTarget.normalized * speed * reduceSpeedFactor;
        }
        else
        {
            // Ir a la velocidad normal si aún está lejos
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

    // Huir del cazador
    Vector3 FleeFromHunter()
    {
        return (transform.position - hunter.position).normalized;
    }

    // Método para alejar a los boids de los límites del mapa
    void ForceAwayFromMapEdges()
    {
        float edgeThreshold = 5f;  // Distancia mínima para empezar a aplicar la repulsión en los bordes
        Vector3 repulsionForce = Vector3.zero;

        if (transform.position.x > mapWidth / 2 - edgeThreshold)
        {
            repulsionForce += Vector3.left;  // Repulsión hacia la izquierda
        }
        else if (transform.position.x < -mapWidth / 2 + edgeThreshold)
        {
            repulsionForce += Vector3.right;  // Repulsión hacia la derecha
        }

        if (transform.position.z > mapDepth / 2 - edgeThreshold)
        {
            repulsionForce += Vector3.back;  // Repulsión hacia abajo (atrás)
        }
        else if (transform.position.z < -mapDepth / 2 + edgeThreshold)
        {
            repulsionForce += Vector3.forward;  // Repulsión hacia arriba (adelante)
        }

        // Aplicar la fuerza de repulsión si está cerca de los bordes
        if (repulsionForce != Vector3.zero)
        {
            velocity += repulsionForce.normalized * speed * 0.5f;  // Ajusta la magnitud de la repulsión según sea necesario
        }
    }
}

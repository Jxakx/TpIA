using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Boid : MonoBehaviour
{
    public float speed = 5f;  
    public float neighborDistance = 10f;  
    public float cohesionStrength = 1f;  // Fuerza que atrae a los boids
    public float separationDistance = 2f;  // Distancia mínima para evitar colisiones
    public float separationStrength = 1.5f;  // Fuerza que repele a los boids
    public float alignmentStrength = 1f;  // Fuerza que alinea la dirección de los boids
    public Transform hunter; 
    public float fleeDistance = 10f;  // Distancia mínima para que los boids huyan del cazador
    public float fleeStrength = 2f;  // Fuerza con la que los boids huyen del cazador
    public Vector3 velocity;
    public float mapWidth = 50f; 
    public float mapDepth = 50f;  

    // Para la detección de comida
    public float foodDetectionRange = 15f; 
    private Transform nearestFood;  
    public float arriveRadius = 1.5f;  // Distancia para que el Boid desacelere al llegar a la comida
    public float foodEatenDistance = 1f;  // Distancia mínima para considerar que el boid ha llegado a la comida

    private List<Boid> neighbors;

    // Parámetros para suavizar decisiones
    private float decisionCooldown = 0.3f; 
    private float lastDecisionTime;

   
    private Vector3 baseDirection;

    // Tiempo entre cambios de dirección (para que nunca se quedan estaticos)
    private float baseDirectionChangeCooldown = 2f;
    private float lastBaseDirectionChangeTime;

    void Start()
    {
        // Movimiento colectivo aleatorio 
        velocity = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * speed;
        neighbors = new List<Boid>();

        // Movimiento individual aleatorio 
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

        transform.position += velocity * Time.deltaTime;

        // Si el boid sale de los límites, reaparece en el lado opuesto
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

        // Mantener el boid a nivel del suelo (Traspasaba el suelo y volaba)
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
        DetectNearestFood();  // Detecta la comida más cercana

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
            moveToFood = ArriveAtFood(nearestFood.position);  // Arrive
        }

        // Cambiar la dirección base cada cierto tiempo (es para evitar que titubeen)
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
                separationForce += fleeDirection / distance;  // Mayor fuerza cuanto más cerca estén (es que se chocan mucho)
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

        // Aleatoriedad al momento de huida
        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f)).normalized;

        fleeDirection += randomOffset;
        return fleeDirection.normalized;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float speed = 2f;  // Velocidad de movimiento de los boids
    public float neighborDistance = 3f;  // Distancia para detectar otros boids
    public float cohesionStrength = 0.5f;  // Fuerza que atrae a los boids entre s�
    public float separationDistance = 1.5f;  // Distancia m�nima para evitar colisiones
    public float separationStrength = 0.5f;  // Fuerza que repele a los boids
    public float alignmentStrength = 0.5f;  // Fuerza que alinea la direcci�n de los boids
    public Vector3 velocity;  // Vector que representa la velocidad de movimiento

    private List<Boid> neighbors;  // Lista de boids vecinos

    void Start()
    {
        // Asignamos una velocidad inicial aleatoria en 3D
        velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * speed;
        neighbors = new List<Boid>();  // Inicializamos la lista de vecinos
    }

    void Update()
    {
        // Actualizamos la lista de boids vecinos
        UpdateNeighbors();

        // Calculamos las fuerzas de cohesi�n, separaci�n y alineaci�n
        Vector3 cohesion = Cohesion() * cohesionStrength;
        Vector3 separation = Separation() * separationStrength;
        Vector3 alignment = Alignment() * alignmentStrength;

        // Sumamos todas las fuerzas para obtener la direcci�n final
        Vector3 flockingDirection = (cohesion + separation + alignment).normalized;

        // Asignamos la velocidad final del boid
        velocity = flockingDirection * speed;

        // Movemos el boid en la direcci�n calculada
        transform.position += velocity * Time.deltaTime;
    }

    // M�todo para actualizar la lista de vecinos cercanos
    void UpdateNeighbors()
    {
        neighbors.Clear();  // Limpiamos la lista de vecinos

        // Buscamos todos los boids en la escena
        foreach (Boid boid in FindObjectsOfType<Boid>())
        {
            // Si el boid est� cerca, lo a�adimos a la lista de vecinos
            if (boid != this && Vector3.Distance(transform.position, boid.transform.position) < neighborDistance)
            {
                neighbors.Add(boid);
            }
        }
    }

    // Fuerza de cohesi�n: atrae al boid hacia el centro del grupo
    Vector3 Cohesion()
    {
        Vector3 centerOfMass = Vector3.zero;  // Calculamos el centro de masa del grupo
        foreach (Boid neighbor in neighbors)
        {
            centerOfMass += neighbor.transform.position;
        }
        if (neighbors.Count > 0)
        {
            // Promediamos la posici�n de todos los vecinos
            centerOfMass /= neighbors.Count;
            return (centerOfMass - transform.position).normalized;  // Direcci�n hacia el centro de masa
        }
        return Vector3.zero;
    }

    // Fuerza de separaci�n: repele a los boids para evitar colisiones
    Vector3 Separation()
    {
        Vector3 separationForce = Vector3.zero;
        foreach (Boid neighbor in neighbors)
        {
            // Si est�n demasiado cerca, aplicamos una fuerza de repulsi�n
            if (Vector3.Distance(transform.position, neighbor.transform.position) < separationDistance)
            {
                separationForce -= (neighbor.transform.position - transform.position).normalized;
            }
        }
        return separationForce.normalized;
    }

    // Fuerza de alineaci�n: alinea la direcci�n del boid con la del grupo
    Vector3 Alignment()
    {
        Vector3 avgVelocity = Vector3.zero;
        foreach (Boid neighbor in neighbors)
        {
            avgVelocity += neighbor.velocity;
        }
        if (neighbors.Count > 0)
        {
            // Promediamos la velocidad de todos los vecinos
            avgVelocity /= neighbors.Count;
            return avgVelocity.normalized;  // Alineamos la direcci�n del boid
        }
        return Vector3.zero;
    }
}

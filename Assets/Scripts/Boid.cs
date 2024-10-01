using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float speed = 2f;  // Velocidad de movimiento de los boids
    public float neighborDistance = 3f;  // Distancia para detectar otros boids
    public float cohesionStrength = 0.5f;  // Fuerza que atrae a los boids entre sí
    public float separationDistance = 1.5f;  // Distancia mínima para evitar colisiones
    public float separationStrength = 0.5f;  // Fuerza que repele a los boids
    public float alignmentStrength = 0.5f;  // Fuerza que alinea la dirección de los boids
    public Vector2 velocity;  // Vector que representa la velocidad de movimiento

    private List<Boid> neighbors;  // Lista de boids vecinos

    // Método que se ejecuta al iniciar el juego
    void Start()
    {
        // Asignamos una velocidad inicial aleatoria
        velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * speed;
        neighbors = new List<Boid>();  // Inicializamos la lista de vecinos
    }

    // Método que se ejecuta en cada frame del juego
    void Update()
    {
        // Actualizamos la lista de boids vecinos
        UpdateNeighbors();

        // Calculamos las fuerzas de cohesión, separación y alineación
        Vector2 cohesion = Cohesion() * cohesionStrength;
        Vector2 separation = Separation() * separationStrength;
        Vector2 alignment = Alignment() * alignmentStrength;

        // Sumamos todas las fuerzas para obtener la dirección final
        Vector2 flockingDirection = (cohesion + separation + alignment).normalized;

        // Asignamos la velocidad final del boid
        velocity = flockingDirection * speed;

        // Movemos el boid en la dirección calculada
        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    // Método para actualizar la lista de vecinos cercanos
    void UpdateNeighbors()
    {
        neighbors.Clear();  // Limpiamos la lista de vecinos

        // Buscamos todos los boids en la escena
        foreach (Boid boid in FindObjectsOfType<Boid>())
        {
            // Si el boid está cerca, lo añadimos a la lista de vecinos
            if (boid != this && Vector2.Distance(transform.position, boid.transform.position) < neighborDistance)
            {
                neighbors.Add(boid);
            }
        }
    }

    // Fuerza de cohesión: atrae al boid hacia el centro del grupo
    Vector2 Cohesion()
    {
        Vector2 centerOfMass = Vector2.zero;  // Calculamos el centro de masa del grupo
        foreach (Boid neighbor in neighbors)
        {
            centerOfMass += (Vector2)neighbor.transform.position;
        }
        if (neighbors.Count > 0)
        {
            // Promediamos la posición de todos los vecinos
            centerOfMass /= neighbors.Count;
            return (centerOfMass - (Vector2)transform.position).normalized;  // Dirección hacia el centro de masa
        }
        return Vector2.zero;
    }

    // Fuerza de separación: repele a los boids para evitar colisiones
    Vector2 Separation()
    {
        Vector2 separationForce = Vector2.zero;
        foreach (Boid neighbor in neighbors)
        {
            // Si están demasiado cerca, aplicamos una fuerza de repulsión
            if (Vector2.Distance(transform.position, neighbor.transform.position) < separationDistance)
            {
                separationForce -= ((Vector2)neighbor.transform.position - (Vector2)transform.position).normalized;
            }
        }
        return separationForce.normalized;
    }

    // Fuerza de alineación: alinea la dirección del boid con la del grupo
    Vector2 Alignment()
    {
        Vector2 avgVelocity = Vector2.zero;
        foreach (Boid neighbor in neighbors)
        {
            avgVelocity += neighbor.velocity;
        }
        if (neighbors.Count > 0)
        {
            // Promediamos la velocidad de todos los vecinos
            avgVelocity /= neighbors.Count;
            return avgVelocity.normalized;  // Alineamos la dirección del boid
        }
        return Vector2.zero;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boid"))  // Asegúrate de que los boids tengan el tag "Boid"
        {
            // Destruir la comida cuando un Boid la toque
            Destroy(gameObject);
        }
    }

}

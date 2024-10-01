using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDestroyer : MonoBehaviour
{
    // Este script debe estar en cada prefab de comida.

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Boid"))  // Asegúrate de que los boids tengan el tag "Boid"
        {
            // Destruir la comida cuando un Boid la toque
            Destroy(gameObject);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLimitsRaycast : MonoBehaviour
{
    public float mapWidth = 10f;  // Ancho del mapa
    public float mapHeight = 5f;  // Altura del mapa
    public LayerMask limitLayer;  // Capa que representa los límites del mapa

    void Update()
    {
        // Hacer raycasts en todas las direcciones para detectar los límites
        CheckMapBounds();
    }

    void CheckMapBounds()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, Mathf.Infinity, limitLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, Mathf.Infinity, limitLayer);
        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, Mathf.Infinity, limitLayer);
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, limitLayer);

        // Asegurarse de que los boids no superen el límite izquierdo
        if (hitLeft.distance < mapWidth / 2f)
        {
            transform.position = new Vector3(hitLeft.point.x + 0.1f, transform.position.y, transform.position.z);
        }

        // Asegurarse de que los boids no superen el límite derecho
        if (hitRight.distance < mapWidth / 2f)
        {
            transform.position = new Vector3(hitRight.point.x - 0.1f, transform.position.y, transform.position.z);
        }

        // Asegurarse de que los boids no superen el límite superior
        if (hitUp.distance < mapHeight / 2f)
        {
            transform.position = new Vector3(transform.position.x, hitUp.point.y - 0.1f, transform.position.z);
        }

        // Asegurarse de que los boids no superen el límite inferior (suelo)
        if (hitDown.distance < mapHeight / 2f)
        {
            transform.position = new Vector3(transform.position.x, hitDown.point.y + 0.1f, transform.position.z);
        }
    }

    // Visualizar los límites del mapa
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapWidth, mapHeight, 1f));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLimitsRaycast : MonoBehaviour
{
    public float mapWidth = 10f;  // Ancho del mapa
    public float mapHeight = 5f;  // Altura del mapa
    public float mapDepth = 10f;  // Profundidad del mapa
    public LayerMask limitLayer;  // Capa que representa los límites del mapa

    void Update()
    {
        // Hacer raycasts en todas las direcciones para detectar los límites
        CheckMapBounds();
    }

    void CheckMapBounds()
    {
        RaycastHit hitLeft, hitRight, hitUp, hitDown, hitForward, hitBackward;

        // Raycast hacia la izquierda
        if (Physics.Raycast(transform.position, Vector3.left, out hitLeft, Mathf.Infinity, limitLayer))
        {
            if (hitLeft.distance < mapWidth / 2f)
            {
                transform.position = new Vector3(hitLeft.point.x + 0.1f, transform.position.y, transform.position.z);
            }
        }

        // Raycast hacia la derecha
        if (Physics.Raycast(transform.position, Vector3.right, out hitRight, Mathf.Infinity, limitLayer))
        {
            if (hitRight.distance < mapWidth / 2f)
            {
                transform.position = new Vector3(hitRight.point.x - 0.1f, transform.position.y, transform.position.z);
            }
        }

        // Raycast hacia arriba
        if (Physics.Raycast(transform.position, Vector3.up, out hitUp, Mathf.Infinity, limitLayer))
        {
            if (hitUp.distance < mapHeight / 2f)
            {
                transform.position = new Vector3(transform.position.x, hitUp.point.y - 0.1f, transform.position.z);
            }
        }

        // Raycast hacia abajo
        if (Physics.Raycast(transform.position, Vector3.down, out hitDown, Mathf.Infinity, limitLayer))
        {
            if (hitDown.distance < mapHeight / 2f)
            {
                transform.position = new Vector3(transform.position.x, hitDown.point.y + 0.1f, transform.position.z);
            }
        }

        // Raycast hacia adelante (eje Z)
        if (Physics.Raycast(transform.position, Vector3.forward, out hitForward, Mathf.Infinity, limitLayer))
        {
            if (hitForward.distance < mapDepth / 2f)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, hitForward.point.z - 0.1f);
            }
        }

        // Raycast hacia atrás (eje Z negativo)
        if (Physics.Raycast(transform.position, Vector3.back, out hitBackward, Mathf.Infinity, limitLayer))
        {
            if (hitBackward.distance < mapDepth / 2f)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, hitBackward.point.z + 0.1f);
            }
        }
    }

    // Visualizar los límites del mapa
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapWidth, mapHeight, mapDepth));
    }
}
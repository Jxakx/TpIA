using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLimitsRaycast : MonoBehaviour
{
    public float mapWidth = 50f;  // Ancho del mapa
    public float mapHeight = 50f;  // Altura del mapa
    public float mapDepth = 50f;  // Profundidad del mapa
    public LayerMask limitLayer;  // Capa que representa los límites del mapa

    void Update()
    {
        // Detectar los límites del mapa con raycasts
        CheckMapBounds();
    }

    void CheckMapBounds()
    {
        RaycastHit hitLeft, hitRight, hitUp, hitDown, hitForward, hitBackward;

        // Raycast para la izquierda
        if (Physics.Raycast(transform.position, Vector3.left, out hitLeft, mapWidth, limitLayer))
        {
            if (hitLeft.distance < mapWidth / 2f)
            {
                transform.position = new Vector3(hitLeft.point.x + 0.1f, transform.position.y, transform.position.z);
            }
        }

        // Raycast para la derecha
        if (Physics.Raycast(transform.position, Vector3.right, out hitRight, mapWidth, limitLayer))
        {
            if (hitRight.distance < mapWidth / 2f)
            {
                transform.position = new Vector3(hitRight.point.x - 0.1f, transform.position.y, transform.position.z);
            }
        }

        // Raycast hacia arriba
        if (Physics.Raycast(transform.position, Vector3.up, out hitUp, mapHeight, limitLayer))
        {
            if (hitUp.distance < mapHeight / 2f)
            {
                transform.position = new Vector3(transform.position.x, hitUp.point.y - 0.1f, transform.position.z);
            }
        }

        // Raycast hacia abajo
        if (Physics.Raycast(transform.position, Vector3.down, out hitDown, mapHeight, limitLayer))
        {
            if (hitDown.distance < mapHeight / 2f)
            {
                transform.position = new Vector3(transform.position.x, hitDown.point.y + 0.1f, transform.position.z);
            }
        }

        // Raycast hacia adelante
        if (Physics.Raycast(transform.position, Vector3.forward, out hitForward, mapDepth, limitLayer))
        {
            if (hitForward.distance < mapDepth / 2f)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, hitForward.point.z - 0.1f);
            }
        }

        // Raycast hacia atrás
        if (Physics.Raycast(transform.position, Vector3.back, out hitBackward, mapDepth, limitLayer))
        {
            if (hitBackward.distance < mapDepth / 2f)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, hitBackward.point.z + 0.1f);
            }
        }
    }

    // Visualizar los límites del mapa en el editor de Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapWidth, mapHeight, mapDepth));
    }
}

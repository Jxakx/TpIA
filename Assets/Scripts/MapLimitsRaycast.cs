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
        RaycastHit hit;

        // Raycast para la izquierda y derecha
        if (Physics.Raycast(transform.position, Vector3.left, out hit, mapWidth / 2f, limitLayer))
        {
            transform.position = new Vector3(hit.point.x + 0.1f, transform.position.y, transform.position.z);
        }
        if (Physics.Raycast(transform.position, Vector3.right, out hit, mapWidth / 2f, limitLayer))
        {
            transform.position = new Vector3(hit.point.x - 0.1f, transform.position.y, transform.position.z);
        }

        // Raycast hacia adelante y atrás
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, mapDepth / 2f, limitLayer))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, hit.point.z - 0.1f);
        }
        if (Physics.Raycast(transform.position, Vector3.back, out hit, mapDepth / 2f, limitLayer))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, hit.point.z + 0.1f);
        }
    }

    // Visualizar los límites del mapa en el editor de Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapWidth, mapHeight, mapDepth));
    }
}

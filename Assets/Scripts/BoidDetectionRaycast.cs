using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidDetectionRaycast : MonoBehaviour
{
    public Transform cazador;  // Transform del cazador
    public float detectionRange = 10f;  // Rango de detección del raycast
    public LayerMask cazadorLayer;  // Layer del cazador para el raycast
    public LayerMask obstacleLayer;  // Layer de obstáculos que podrían bloquear el raycast

    void Update()
    {
        // Dirección hacia el cazador
        Vector3 directionToCazador = (cazador.position - transform.position).normalized;

        // Hacer un raycast en la dirección del cazador
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToCazador, out hit, detectionRange, cazadorLayer | obstacleLayer))
        {
            // Si el raycast detecta al cazador sin obstáculos en el medio
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Cazador"))
            {
                // Activar la evasión si el cazador es detectado
                FleeFromCazador(hit.point);
            }
        }
    }

    void FleeFromCazador(Vector3 cazadorPosition)
    {
        Vector3 fleeDirection = (transform.position - cazadorPosition).normalized;
        transform.position += fleeDirection * Time.deltaTime * 5f;  // Ajusta la velocidad de huida
    }

    // Visualizar el raycast en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (cazador.position - transform.position).normalized * detectionRange);
    }
}

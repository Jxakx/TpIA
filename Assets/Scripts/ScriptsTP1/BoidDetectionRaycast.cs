using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidDetectionRaycast : MonoBehaviour
{
    public Transform cazador;
    public float detectionRange = 10f;  // Rango de detección del raycast
    public LayerMask cazadorLayer;  
    public LayerMask obstacleLayer; 

    void Update()
    {
        Vector3 directionToCazador = (cazador.position - transform.position).normalized;

        // Hacer un raycast en la dirección del cazador
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToCazador, out hit, detectionRange, cazadorLayer | obstacleLayer))
        {
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (cazador.position - transform.position).normalized * detectionRange);
    }
}

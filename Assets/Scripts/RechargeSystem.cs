using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeSystem : MonoBehaviour
{
    public float maxEnergy;  // Energía máxima del cazador
    public float currentEnergy;  // Energía actual
    public float energyDecayRate = 1f;  // Velocidad de pérdida de energía por segundo
    public float rechargeRate = 20f;  // Velocidad de recarga por segundo
    public Transform rechargePoint;  // Punto de recarga en el mapa
    private bool isRecharging = false;  // Estado del cazador, si está recargando o no
    private Vector3 initialPosition;  // Posición inicial del cazador para restaurar tras la recarga

    void Start()
    {
        currentEnergy = maxEnergy;  // Inicializamos la energía al máximo
        initialPosition = transform.position;  // Guardamos la posición inicial
    }

    void Update()
    {
        // Si el cazador no está recargando, pierde energía gradualmente
        if (!isRecharging)
        {
            currentEnergy -= energyDecayRate * Time.deltaTime;
        }

        // Si la energía baja de un cierto umbral, cambiar al modo de recarga
        if (currentEnergy <= 20f && !isRecharging)
        {
            StartCoroutine(MoveToRecharge());
        }
    }

    // Método que mueve al cazador hacia el punto de recarga
    IEnumerator MoveToRecharge()
    {
        isRecharging = true;

        // Mover al cazador hacia el punto de recarga
        while (Vector3.Distance(transform.position, rechargePoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, rechargePoint.position, Time.deltaTime * 5f);  // Ajusta la velocidad de movimiento si es necesario
            yield return null;
        }

        // Cuando llega al punto de recarga, comienza a recargar energía
        yield return StartCoroutine(RechargeEnergy());
    }

    // Método para recargar la energía
    IEnumerator RechargeEnergy()
    {
        while (currentEnergy < maxEnergy)
        {
            currentEnergy += rechargeRate * Time.deltaTime;
            yield return null;
        }

        // Cuando termina de recargar, vuelve a su posición inicial
        isRecharging = false;
        transform.position = initialPosition;  // Vuelve a su posición de patrullaje
    }

    // Método para visualizar el punto de recarga
    private void OnDrawGizmos()
    {
        if (rechargePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(rechargePoint.position, 1f);
        }
    }
}

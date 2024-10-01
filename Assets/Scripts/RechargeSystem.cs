using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeSystem : MonoBehaviour
{
    public float maxEnergy;  // Energ�a m�xima del cazador
    public float currentEnergy;  // Energ�a actual
    public float energyDecayRate = 1f;  // Velocidad de p�rdida de energ�a por segundo
    public float rechargeRate = 20f;  // Velocidad de recarga por segundo
    public Transform rechargePoint;  // Punto de recarga en el mapa
    private bool isRecharging = false;  // Estado del cazador, si est� recargando o no
    private Vector3 initialPosition;  // Posici�n inicial del cazador para restaurar tras la recarga
    private bool cazadorDentroDelRango = false; // Verifica si el cazador est� en el rango de recarga

    void Start()
    {
        currentEnergy = maxEnergy;  // Inicializamos la energ�a al m�ximo
        initialPosition = transform.position;  // Guardamos la posici�n inicial
    }

    void Update()
    {
        // Si el cazador no est� recargando, pierde energ�a gradualmente
        if (!isRecharging)
        {
            currentEnergy -= energyDecayRate * Time.deltaTime;
        }

        // Si la energ�a baja de un cierto umbral, cambiar al modo de recarga
        if (currentEnergy <= 20f && !isRecharging)
        {
            StartCoroutine(MoveToRecharge());
        }
    }

    // M�todo que mueve al cazador hacia el punto de recarga
    IEnumerator MoveToRecharge()
    {
        isRecharging = true;

        // Mover al cazador hacia el punto de recarga
        while (Vector3.Distance(transform.position, rechargePoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, rechargePoint.position, Time.deltaTime * 5f);  // Ajusta la velocidad de movimiento si es necesario
            yield return null;
        }

        // Cuando llega al punto de recarga, marca que el cazador est� en el rango
        cazadorDentroDelRango = true;
        yield return StartCoroutine(RechargeEnergy());
    }

    // M�todo para recargar la energ�a
    IEnumerator RechargeEnergy()
    {
        while (currentEnergy < maxEnergy && cazadorDentroDelRango)
        {
            currentEnergy += rechargeRate * Time.deltaTime;
            yield return null;
        }

        // Cuando termina de recargar o se aleja, vuelve a su posici�n inicial
        isRecharging = false;
        cazadorDentroDelRango = false;  // El cazador ya no est� recargando
        transform.position = initialPosition;  // Vuelve a su posici�n de patrullaje
    }

    // M�todo para detener la recarga si el cazador se aleja demasiado del punto de recarga
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hunter"))  // Aseg�rate de que el cazador tenga este tag
        {
            cazadorDentroDelRango = false;  // El cazador sali� del rango de recarga
        }
    }

    // M�todo para visualizar el punto de recarga
    private void OnDrawGizmos()
    {
        if (rechargePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(rechargePoint.position, 1f);
        }
    }
}

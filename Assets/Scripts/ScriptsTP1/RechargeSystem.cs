using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeSystem : MonoBehaviour
{
    public float maxEnergy;  
    public float currentEnergy; 
    public float energyDecayRate = 1f;  // Velocidad de pérdida de energía por segundo
    public float rechargeRate = 20f;  // Velocidad de recarga por segundo
    public Transform rechargePoint; 
    private bool isRecharging = false; 
    private Vector3 initialPosition;  // Posición inicial del cazador para restaurar tras la recarga
    private bool cazadorDentroDelRango = false; // Verifica si el cazador está en el rango de recarga

    void Start()
    {
        currentEnergy = maxEnergy;
        initialPosition = transform.position;
    }

    void Update()
    {
        // Si el cazador no está recargando, pierde energía gradualmente
        if (!isRecharging)
        {
            currentEnergy -= energyDecayRate * Time.deltaTime;
        }

        // Si la energía baja, cambia al modo de recarga
        if (currentEnergy <= 20f && !isRecharging)
        {
            StartCoroutine(MoveToRecharge());
        }
    }

    // El cazador va hacia el punto de recarga
    IEnumerator MoveToRecharge()
    {
        isRecharging = true;

        while (Vector3.Distance(transform.position, rechargePoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, rechargePoint.position, Time.deltaTime * 5f);  // Ajusta la velocidad de movimiento si es necesario
            yield return null;
        }

        cazadorDentroDelRango = true;
        yield return StartCoroutine(RechargeEnergy());
    }

    
    IEnumerator RechargeEnergy()
    {
        while (currentEnergy < maxEnergy && cazadorDentroDelRango)
        {
            currentEnergy += rechargeRate * Time.deltaTime;
            yield return null;
        }

        // Cuando termina de recargar o se aleja, vuelve a su posición inicial
        isRecharging = false;
        cazadorDentroDelRango = false;
        transform.position = initialPosition;  // Vuelve a su posición de patrullaje
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hunter"))  
        {
            cazadorDentroDelRango = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (rechargePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(rechargePoint.position, 1f);
        }
    }
}

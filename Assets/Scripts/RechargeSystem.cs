
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeSystem : MonoBehaviour
{
    public float maxEnergy = 100f;
    public float currentEnergy = 100f;
    public float energyDecayRate = 1f;
    public float rechargeRate = 20f;
    public Transform rechargePoint;
    private bool isRecharging = false;

    void Update()
    {
        if (!isRecharging)
        {
            currentEnergy -= energyDecayRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
        }

        if (currentEnergy <= 20f && !isRecharging)
        {
            StartCoroutine(RechargeAtPoint());
        }
    }

    private IEnumerator RechargeAtPoint()
    {
        isRecharging = true;

        while (Vector3.Distance(transform.position, rechargePoint.position) > 0.1f)
        {
            MoveTowards(rechargePoint.position);
            yield return null;
        }

        while (currentEnergy < maxEnergy)
        {
            currentEnergy += rechargeRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
            yield return null;
        }

        isRecharging = false;
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 5f);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5f; 

    private Vector3 currentVelocity;

    private void Update()
    {        
        float horizontalInput = Input.GetAxisRaw("Horizontal"); 
        float verticalInput = Input.GetAxisRaw("Vertical");  
       
        Vector3 targetDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        
        if (targetDirection.magnitude > 0)
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetDirection * speed, 100 * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, 1000 * Time.deltaTime);
        }
        
        transform.Translate(currentVelocity * Time.deltaTime, Space.World);
    }
}

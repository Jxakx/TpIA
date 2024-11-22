using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5f; 

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); 
        float verticalInput = Input.GetAxis("Vertical");
        
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);
        
        movement = movement.normalized * speed * Time.deltaTime;
        
        transform.Translate(movement, Space.World);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;      
    public Vector3 spawnAreaSize = new Vector3(50f, 0f, 50f);  
    public int foodCount = 10;         
    public float spawnInterval = 1.5f;  

    void Start()
    {
        // Spawnear cantidad de comida 
        for (int i = 0; i < foodCount; i++)
        {
            SpawnFood();
        }

        // Iniciar la corrutina para spawnear comida
        StartCoroutine(SpawnFoodPeriodically());
    }

    void SpawnFood()
    {
        // Generar una posición aleatoria dentro del área de spawn (para que spawnee siempre a la altura de los boid)
        Vector3 randomPosition = new Vector3(Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2), 0f, Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2));

        Instantiate(foodPrefab, randomPosition, Quaternion.identity);
    }

    IEnumerator SpawnFoodPeriodically()//Spawnear prefab cada 1.5 seg
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnFood();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}

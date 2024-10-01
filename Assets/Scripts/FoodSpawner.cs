using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;      // Prefab de la comida
    public Vector3 spawnAreaSize = new Vector3(50f, 0f, 50f);  // Tamaño del área donde spawneará la comida
    public int foodCount = 10;         // Cantidad de comida a spawnear inicialmente

    void Start()
    {
        // Spawnear la cantidad de comida especificada
        for (int i = 0; i < foodCount; i++)
        {
            SpawnFood();
        }
    }

    void SpawnFood()
    {
        // Generar una posición aleatoria dentro del área de spawn, con Y siempre en 0
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2), // Eje X
            0f, // Mantén la comida en el plano Y=0
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)  // Eje Z
        );

        // Instanciar la comida en la posición generada, asegurando que se mantenga en el plano XZ
        Instantiate(foodPrefab, randomPosition, Quaternion.identity);
    }

    // Visualizar el área de spawn en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}

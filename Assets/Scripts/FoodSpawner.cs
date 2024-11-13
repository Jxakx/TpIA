
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;
    public Vector3 spawnAreaSize = new Vector3(50f, 0f, 50f);
    public int maxFoodCount = 10;
    public float respawnTime = 5f;

    private List<GameObject> spawnedFood;

    void Start()
    {
        spawnedFood = new List<GameObject>();
        for (int i = 0; i < maxFoodCount; i++)
        {
            SpawnFood();
        }
    }

    void Update()
    {
        // Continuously check if food count is below the max and respawn if needed
        for (int i = spawnedFood.Count - 1; i >= 0; i--)
        {
            // Remove null entries for food objects that were destroyed
            if (spawnedFood[i] == null)
            {
                spawnedFood.RemoveAt(i);
                StartCoroutine(RespawnFoodAfterDelay());
            }
        }
    }

    public void SpawnFood()
    {
        Vector3 randomPosition = GenerateRandomPosition();
        GameObject food = Instantiate(foodPrefab, randomPosition, Quaternion.identity);
        spawnedFood.Add(food);
    }

    private IEnumerator RespawnFoodAfterDelay()
    {
        yield return new WaitForSeconds(respawnTime);
        if (spawnedFood.Count < maxFoodCount) // Re-check to avoid over-spawning
        {
            SpawnFood();
        }
    }

    private Vector3 GenerateRandomPosition()
    {
        return new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            0f,
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        ) + transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}

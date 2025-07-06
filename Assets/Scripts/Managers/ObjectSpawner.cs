using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    private List<GameObject> prefabsToSpawn;
    private float spawnRate;
    private int maxObjectCount; 
    private Camera mainCamera;
    private Coroutine spawnCoroutine;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    public void ConfigureForLevel(LevelData levelData)
    {
        prefabsToSpawn = levelData.absorbablePrefabs;
        spawnRate = levelData.generationRate;
        maxObjectCount = levelData.maxObjects; 

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / spawnRate);

            if (PhysicsOrbitalMovement.ActiveObjectCount < maxObjectCount)
            {
                if (prefabsToSpawn != null && prefabsToSpawn.Count > 0)
                {
                    GameObject prefab = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Count)];
                    Instantiate(prefab, GetSpawnPosition(), Quaternion.identity);
                }
            }
        }
    }

    private Vector2 GetSpawnPosition()
    {
        float height = mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;

        float spawnPadding = 2f;
        Vector2 spawnPoint = Vector2.zero;

        if (Random.Range(0, 2) == 0)
        {
            spawnPoint.x = (Random.Range(0, 2) == 0) ? -width - spawnPadding : width + spawnPadding;
            spawnPoint.y = Random.Range(-height, height);
        }
        else
        {
            spawnPoint.x = Random.Range(-width, width);
            spawnPoint.y = (Random.Range(0, 2) == 0) ? -height - spawnPadding : height + spawnPadding;
        }

        return spawnPoint;
    }
}
using System.Collections;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject fishPrefab;
    public int numberOfFish = 10;

    void Start()
    {
        SpawnFish();
    }

    private void SpawnFish()
    {
        Camera mainCamera = Camera.main;
        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        for (int i = 0; i < numberOfFish; i++)
        {
            Vector2 randomPosition = new Vector2(
                Random.Range(-cameraWidth, cameraWidth),
                Random.Range(-cameraHeight, cameraHeight)
            );

            GameObject newFish = Instantiate(fishPrefab, randomPosition, Quaternion.identity);
        }
    }
}
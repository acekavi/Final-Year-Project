using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracking : MonoBehaviour
{
    private ARTrackedImageManager trackedImages;
    public GameObject[] ArPrefabs;
    private Dictionary<string, GameObject> prefabMap = new Dictionary<string, GameObject>();

    private List<GameObject> spawnedObjects = new List<GameObject>();

    private GameManager gameManager; // Reference to the GameManager

    private void Awake()
    {
        trackedImages = GetComponent<ARTrackedImageManager>();
        gameManager = FindObjectOfType<GameManager>(); // Find the GameManager in the scene

        // Populate the prefab map for faster lookup
        foreach (var prefab in ArPrefabs)
        {
            prefabMap[prefab.name] = prefab;
        }
    }

    private void OnEnable()
    {
        trackedImages.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImages.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            SpawnPrefab(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            UpdatePrefab(trackedImage);
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            RemovePrefab(trackedImage);
        }
    }

    private void SpawnPrefab(ARTrackedImage trackedImage)
    {
        if (prefabMap.ContainsKey(trackedImage.referenceImage.name))
        {
            GameObject prefab = prefabMap[trackedImage.referenceImage.name];
            GameObject spawnedObject = Instantiate(prefab, trackedImage.transform.position, trackedImage.transform.rotation, transform);
            spawnedObjects.Add(spawnedObject);

            // Pass the spawn point to GameManager if it's available
            if (gameManager != null)
            {
                Transform spawnPoint = trackedImage.transform.Find("SpawnPoint");
                if (spawnPoint != null)
                {
                    gameManager.planetSpawnPoint = spawnPoint;
                }
            }
        }
    }

    private void UpdatePrefab(ARTrackedImage trackedImage)
    {
        foreach (var spawnedObject in spawnedObjects)
        {
            if (spawnedObject.name == trackedImage.referenceImage.name)
            {
                spawnedObject.SetActive(trackedImage.trackingState == TrackingState.Tracking);
            }
        }
    }

    private void RemovePrefab(ARTrackedImage trackedImage)
    {
        spawnedObjects.RemoveAll(obj => obj.name == trackedImage.referenceImage.name);
    }
}
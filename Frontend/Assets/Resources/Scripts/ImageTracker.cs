using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracking : MonoBehaviour
{
    [SerializeField]
    private ARTrackedImageManager trackedImageManager;

    // Prefabs for the planets.
    public GameObject[] planetPrefabs;

    // Mapping of planet names to their prefabs.
    private Dictionary<string, GameObject> planetPrefabsMap = new Dictionary<string, GameObject>();

    // Reference to the GameManager script.
    private GameManager gameManager;
    private GameObject currentSpawnedPlanet = null;

    private void Awake()
    {
        // Initialize the planet prefabs map.
        foreach (GameObject prefab in planetPrefabs)
        {
            planetPrefabsMap[prefab.name] = prefab;
        }

        // Find the GameManager in the scene.
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            // Handle newly detected images.
            UpdateTrackedImage(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            // Handle updated images.
            UpdateTrackedImage(trackedImage);
        }

        // Optionally handle removed images, if necessary.
    }

    private void UpdateTrackedImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;

        // Check if the detected planet is the one currently asked for.
        if (gameManager.GetCurrentQuestionPlanet() == name)
        {
            gameManager.CorrectPlanetScanned();
            SpawnPlanet(name, trackedImage.transform.position);
        }
        else
        {
            gameManager.WrongPlanetScanned(name); // Pass the wrongly scanned planet name for feedback.
        }
    }

    private void SpawnPlanet(string planetName, Vector3 position)
    {
        // Destroy the previously spawned planet, if any.
        if (currentSpawnedPlanet != null)
        {
            Destroy(currentSpawnedPlanet);
        }

        if (planetPrefabsMap.ContainsKey(planetName))
        {
            currentSpawnedPlanet = Instantiate(planetPrefabsMap[planetName], position, Quaternion.identity);
        }
    }
}
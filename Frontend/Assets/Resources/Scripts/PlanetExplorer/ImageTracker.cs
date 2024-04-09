using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    [SerializeField]
    private ARTrackedImageManager trackedImageManager;

    // Prefabs for the planets.
    public GameObject[] planetPrefabs;

    // Mapping of planet names to their prefabs.
    private Dictionary<string, GameObject> planetPrefabsMap = new Dictionary<string, GameObject>();

    // Reference to the GameManager script.
    private GameManager gameManager;

    // Currently spawned planets mapped by the ARTrackedImage names.
    private Dictionary<string, GameObject> spawnedPlanets = new Dictionary<string, GameObject>();

    private void Awake()
    {
        // Initialize the planet prefabs map.
        foreach (GameObject prefab in planetPrefabs)
        {
            planetPrefabsMap[prefab.name] = prefab;
        }

        gameManager = FindObjectOfType<GameManager>(); // Updated to find the GameManager in the scene.
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
            UpdateTrackedImage(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateTrackedImage(trackedImage);
        }

        // Handle removed images
        foreach (var trackedImage in eventArgs.removed)
        {
            if (spawnedPlanets.TryGetValue(trackedImage.referenceImage.name, out GameObject planet))
            {
                Destroy(planet);
                spawnedPlanets.Remove(trackedImage.referenceImage.name);
            }
        }
    }

    private void UpdateTrackedImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;

        // Before checking the current question answer, ensure a question is being asked.
        if (!gameManager.IsAnsweringQuestion()) return;

        if (trackedImage.trackingState == TrackingState.Tracking && gameManager.IsAnsweringQuestion())
        {
            if (gameManager.GetCurrentQuestionAnswer() == name)
            {
                // Correctly identified the planet for the current question
                gameManager.CorrectAnswerSelected();
                SpawnOrUpdatePlanet(name, trackedImage);
            }
            else
            {
                // Incorrectly identified the planet for the current question
                gameManager.WrongAnswerSelected();
            }
        }
    }

    private void SpawnOrUpdatePlanet(string planetName, ARTrackedImage trackedImage)
    {
        // Check if the planet is already spawned and just update its position.
        if (!spawnedPlanets.TryGetValue(planetName, out GameObject currentSpawnedPlanet))
        {
            // Instantiate the correct planet prefab at the detected image's position.
            if (planetPrefabsMap.TryGetValue(planetName, out GameObject planetPrefab))
            {
                currentSpawnedPlanet = Instantiate(planetPrefab, trackedImage.transform);
                spawnedPlanets[planetName] = currentSpawnedPlanet;
            }
        }
        else
        {
            // Update the planet's position to match the tracked image.
            currentSpawnedPlanet.transform.position = trackedImage.transform.position;
        }
    }

    // Individual planets are now managed in the OnTrackedImagesChanged event.
    public void ClearAllSpawnedPlanets()
    {
        foreach (var planet in spawnedPlanets.Values)
        {
            Destroy(planet);
        }
        spawnedPlanets.Clear();
    }
}
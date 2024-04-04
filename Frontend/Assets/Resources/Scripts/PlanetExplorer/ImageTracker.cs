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
    private GameObject currentSpawnedPlanet = null;

    private void Awake()
    {
        // Initialize the planet prefabs map.
        foreach (GameObject prefab in planetPrefabs)
        {
            // Using prefab.name as the key might be prone to errors if the prefab's name doesn't exactly match the answer.
            // Consider using a more reliable mapping if necessary.
            planetPrefabsMap[prefab.name] = prefab;
        }

        gameManager = GetComponent<GameManager>();
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

        // Handle removed images if necessary.
        foreach (var trackedImage in eventArgs.removed)
        {
            // Consider removing the spawned planet if it corresponds to the removed image.
            // You might also want to handle this case differently based on your game's requirements.
            UpdateTrackedImage(trackedImage);
        }
    }

    private void UpdateTrackedImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;

        // Before checking the current question answer, ensure a question is being asked.
        if (!gameManager.IsAnsweringQuestion()) return;

        if (gameManager.GetCurrentQuestionAnswer() == name)
        {
            // Correctly identified the planet for the current question
            gameManager.CorrectAnswerSelected();
            SpawnPlanet(name, trackedImage.transform.position, trackedImage);
        }
    }

    private void SpawnPlanet(string planetName, Vector3 position, ARTrackedImage trackedImage)
    {
        // Destroy the previously spawned planet, if any.
        if (currentSpawnedPlanet != null)
        {
            Destroy(currentSpawnedPlanet);
        }

        // Instantiate the correct planet prefab at the detected image's position.
        if (planetPrefabsMap.TryGetValue(planetName, out GameObject planetPrefab))
        {
            currentSpawnedPlanet = Instantiate(planetPrefab, position, Quaternion.identity);
        }
    }

    public void ClearSpawnedPlanet()
    {
        if (currentSpawnedPlanet != null)
        {
            Destroy(currentSpawnedPlanet);
        }
    }
}
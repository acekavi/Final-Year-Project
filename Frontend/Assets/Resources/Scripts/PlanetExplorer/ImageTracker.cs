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
            if (currentSpawnedPlanet != null)
            {
                currentSpawnedPlanet.SetActive(trackedImage.trackingState == TrackingState.Tracking);
            }
            else
            {
                UpdateTrackedImage(trackedImage);
            }
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
            SpawnPlanet(name, trackedImage);
        }
        else
        {
            // Incorrectly identified the planet for the current question
            gameManager.WrongAnswerSelected();
        }
    }

    private void SpawnPlanet(string planetName, ARTrackedImage trackedImage)
    {
        // Destroy the previously spawned planet, if any.
        ClearSpawnedPlanet();

        // Instantiate the correct planet prefab at the detected image's position.
        if (planetPrefabsMap.TryGetValue(planetName, out GameObject planetPrefab))
        {
            currentSpawnedPlanet = Instantiate(planetPrefab, trackedImage.transform);
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
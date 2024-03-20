using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracking : MonoBehaviour
{
    private ARTrackedImageManager trackedImages;
    public GameObject[] ArPrefabs;

    List<GameObject> ArObjects = new List<GameObject>();

    private void Awake()
    {
        trackedImages = FindObjectOfType<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        trackedImages.trackedImagesChanged += OntrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImages.trackedImagesChanged -= OntrackedImagesChanged;
    }

    private void OntrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            foreach (var prefab in ArPrefabs)
            {
                if (trackedImage.referenceImage.name == prefab.name)
                {
                    GameObject ArObject = Instantiate(prefab, trackedImage.transform);
                    ArObjects.Add(ArObject);
                }
            }
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            foreach (var gameObject in ArObjects)
            {
                if (gameObject.name == trackedImage.name)
                {
                    gameObject.SetActive(trackedImage.trackingState == TrackingState.Tracking);
                }
            }
        }
    }
}

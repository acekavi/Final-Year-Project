using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FunFactsDisplay : MonoBehaviour
{
    [SerializeField] private PlanetProperties planetProperties;
    [SerializeField] private TMP_Text factText;
    [SerializeField] private GameObject funFactsPanel;
    private int currentIndex = 0;
    private AudioSource audioSource;

    // Initialize lists only once
    public void ShowFunFactPanel()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        funFactsPanel.SetActive(true);
        SetFacts(planetProperties.funFacts, planetProperties.funFactAudioClips);
    }

    public void HideFunFactPanel()
    {
        audioSource.Stop();
        funFactsPanel.SetActive(false);
    }

    public void SetFacts(List<string> facts, List<AudioClip> audioClips)
    {

        currentIndex = 0; // Reset index whenever setting new facts

        if (facts.Count > 0)
        {
            UpdateFacts();
        }
        else
        {
            factText.text = "No facts available."; // Placeholder text if no facts
        }

        if (audioClips.Count == 0)
        {
            Debug.LogWarning("No audio clips available. Please check the audio clips list.");
        }
    }

    public void NextFact()
    {
        if (planetProperties.funFacts == null || planetProperties.funFacts.Count == 0) return;
        currentIndex = (currentIndex + 1) % planetProperties.funFacts.Count; // Loop back to the start if at the end
        UpdateFacts();
    }

    public void PreviousFact()
    {
        if (planetProperties.funFacts == null || planetProperties.funFacts.Count == 0) return;

        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = planetProperties.funFacts.Count - 1; // Loop to the end if at the start
        }

        UpdateFacts();
    }

    private void UpdateFacts()
    {
        if (planetProperties.funFacts != null && currentIndex >= 0 && currentIndex < planetProperties.funFacts.Count)
        {
            factText.text = planetProperties.funFacts[currentIndex];

            // Play the corresponding audio clip if available
            if (planetProperties.funFactAudioClips.Count > currentIndex)
            {
                if (audioSource != null)
                {
                    audioSource.clip = planetProperties.funFactAudioClips[currentIndex];
                    audioSource.Play();
                }
                else
                {
                    Debug.LogWarning("AudioSource component not found. Audio clip will not be played.");
                }
            }
        }
        else
        {
            Debug.LogError("Fact index is out of range. Please check the facts list and index.");
            factText.text = "Fact missing."; // Placeholder text if out-of-range error occurs
        }
    }
}

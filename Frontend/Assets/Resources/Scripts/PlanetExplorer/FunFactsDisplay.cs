using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FunFactsDisplay : MonoBehaviour
{
    public TMP_Text factText;
    private List<string> currentFacts;
    private int currentIndex = 0;

    public void SetFacts(List<string> facts)
    {
        currentFacts = new List<string>(facts); // Clone the list to avoid reference issues
        currentIndex = 0; // Reset index whenever setting new facts
        if (currentFacts.Count > 0)
        {
            UpdateFactText();
        }
        else
        {
            factText.text = "No facts available."; // Placeholder text if no facts
        }
    }

    public void NextFact()
    {
        if (currentFacts == null || currentFacts.Count == 0) return;
        currentIndex = (currentIndex + 1) % currentFacts.Count; // Loop back to the start if at the end
        UpdateFactText();
    }

    public void PreviousFact()
    {
        if (currentFacts == null || currentFacts.Count == 0) return;
        currentIndex--;
        if (currentIndex < 0) currentIndex = currentFacts.Count - 1; // Loop to the end if at the start
        UpdateFactText();
    }

    private void UpdateFactText()
    {
        if (currentFacts != null && currentIndex >= 0 && currentIndex < currentFacts.Count)
        {
            factText.text = currentFacts[currentIndex];
        }
        else
        {
            Debug.LogError("Fact index is out of range. Please check the facts list and index.");
            factText.text = "Fact missing."; // Placeholder text if out-of-range error occurs
        }
    }
}

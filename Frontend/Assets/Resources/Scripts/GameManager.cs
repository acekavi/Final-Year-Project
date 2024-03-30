using System.Collections;
using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class GameManager : MonoBehaviour
{
    public TMP_Text questionText; // Use TMP_Text for TextMeshPro text components
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public Transform planetSpawnPoint;
    public float timeLimit = 30f;

    private GameObject[] planetPrefabs; // Array to store planet prefabs
    private float currentTime;
    private bool isTimerRunning = false;
    private int score = 0;

    private void Start()
    {
        planetPrefabs = FindObjectOfType<ImageTracking>().ArPrefabs; // Get planet prefabs from ImageTracking script
        UpdateQuestion();
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerUI();

            if (currentTime <= 0f)
            {
                EndGame();
            }
        }
    }

    private void UpdateQuestion()
    {
        // Implement your logic to update the question text and correct answer
        questionText.text = "Which planet is known as the Red Planet?";
        StartTimer();
    }

    public void CheckAnswer(string selectedPlanet)
    {
        if (selectedPlanet == "Mars")
        {
            score += CalculateScore();
            SpawnPlanet("Mars"); // Spawn the correct planet
        }
        else
        {
            // Incorrect answer logic
            StartCoroutine(ShowTryAgainMessage());
        }

        scoreText.text = "Score: " + score.ToString();

        // Update question after answering
        UpdateQuestion();
    }

    private int CalculateScore()
    {
        // Calculate score based on time remaining
        float percentageRemaining = currentTime / timeLimit;
        int timeBonus = Mathf.RoundToInt(percentageRemaining * 100);

        // Minimum score is 10
        return Mathf.Max(10, timeBonus);
    }

    private void StartTimer()
    {
        currentTime = timeLimit;
        isTimerRunning = true;
    }

    private void UpdateTimerUI()
    {
        timerText.text = "Time: " + Mathf.CeilToInt(currentTime).ToString();
    }

    private void EndGame()
    {
        isTimerRunning = false;
        // Additional end game logic, if needed
    }

    private void SpawnPlanet(string planetName)
    {
        foreach (var prefab in planetPrefabs)
        {
            if (prefab.name == planetName)
            {
                Instantiate(prefab, planetSpawnPoint.position, planetSpawnPoint.rotation);
                break;
            }
        }
    }

    private IEnumerator ShowTryAgainMessage()
    {
        // Display try again message for 2 seconds
        questionText.text = "Try again!";
        yield return new WaitForSeconds(2f);
        questionText.text = "Which planet is known as the Red Planet?";
    }
}
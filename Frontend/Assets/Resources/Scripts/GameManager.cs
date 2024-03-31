using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Assume this is the parent GameObject of your questionText, which represents the entire question UI prefab
    public GameObject questionUIPrefab;
    public TMP_Text questionText;

    public TMP_Text scoreText;
    public TMP_Text timerText;
    public GameObject feedbackPopup;
    public TMP_Text feedbackText;
    public Button nextQuestionButton;

    private float timeLimit = 30f;
    private float currentTime;
    private int score = 0;
    private int currentQuestionIndex = 0;
    private string currentQuestionPlanet;
    private bool isAnsweringQuestion = false;

    private string[] questions = new string[]
    {
        "Which planet is known as the Red Planet?",
        "Which planet is the largest in our solar system?",
        "Which planet is closest to the Sun?",
        "Which planet is known as the Blue Planet?",
        "Which planet is known as the Ringed Planet?",
    };

    private string[] answers = new string[]
    {
        "Mars",
        "Jupiter",
        "Mercury",
        "Earth",
        "Saturn",
    };

    void Start()
    {
        nextQuestionButton.gameObject.SetActive(false);
        feedbackPopup.SetActive(false);
        GoToNextQuestion();
    }

    void Update()
    {
        if (isAnsweringQuestion)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerUI();

            if (currentTime <= 0f)
            {
                TimeRanOut();
            }
        }
    }
    public void GoToNextQuestion()
    {
        if (currentQuestionIndex >= questions.Length)
        {
            EndGame();
            return;
        }

        questionUIPrefab.SetActive(true); // Ensure the question prefab is visible
        feedbackPopup.SetActive(false);
        nextQuestionButton.gameObject.SetActive(false);
        isAnsweringQuestion = true;

        currentQuestionPlanet = answers[currentQuestionIndex];
        questionText.text = questions[currentQuestionIndex];
        currentTime = timeLimit;
        UpdateTimerUI();
    }

    public void CorrectPlanetScanned()
    {
        if (!isAnsweringQuestion) return;

        score += Mathf.CeilToInt(currentTime);
        UpdateScoreUI();
        ShowFeedback(true);
        isAnsweringQuestion = false;
        currentQuestionIndex++;

        questionUIPrefab.SetActive(false); // Hide the question prefab as the correct answer is shown
    }

    public void WrongPlanetScanned(string scannedPlanet)
    {
        if (!isAnsweringQuestion) return;

        score = Mathf.Max(0, score - 10);
        UpdateScoreUI();
        ShowFeedback(false);
    }

    private void ShowFeedback(bool isCorrect)
    {
        feedbackPopup.SetActive(true);
        feedbackText.text = isCorrect ? "Correct!" : "Try again!";
        StartCoroutine(HideFeedbackAfterDelay(2f)); // Optionally adjust the delay based on your preference
    }

    private IEnumerator HideFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        feedbackPopup.SetActive(false);
        if (currentQuestionIndex < questions.Length)
        {
            nextQuestionButton.gameObject.SetActive(true); // Re-enable the button after hiding the feedback
        }
        else
        {
            // If there are no more questions, you might want to automatically end the game or prompt the user differently
            EndGame();
        }
    }

    private void TimeRanOut()
    {
        ShowFeedback(false);
        isAnsweringQuestion = false;
        currentQuestionIndex++;
    }

    private void UpdateTimerUI()
    {
        timerText.text = "Time: " + Mathf.CeilToInt(currentTime).ToString();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    private void EndGame()
    {
        // Here you could show a game over screen or transition back to a main menu
        Debug.Log("Game Over! Final Score: " + score);
    }

    // Utility method for the GameManager to know the current question's planet
    public string GetCurrentQuestionPlanet()
    {
        return currentQuestionPlanet;
    }
}
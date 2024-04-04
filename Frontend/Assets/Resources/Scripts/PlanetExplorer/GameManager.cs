using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject questionUIPrefab;
    public TMP_Text questionText;
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public GameObject feedbackPopup;
    public Button nextQuestionButton;
    public GameObject InstructionsPanel;
    public GameObject ScoreUIPanel;

    [Header("Game Over Panel")]
    public GameObject GameOverPanel;
    public TMP_Text finalScoreText;
    public TMP_Text finalTotalTimeText;
    public Image[] starImages;

    [Header("Audio Clips")]
    public AudioClip correctAnswerClip;
    public AudioClip wrongAnswerClip;
    public AudioClip timeUpClip;
    public AudioClip gameoverClip;
    public AudioClip[] questionClips;

    private readonly float timeLimit = 60f;
    private float currentTime;
    private int score = -60;
    private string currentQuestion = string.Empty;
    private bool isAnsweringQuestion = false;
    private float totalTime = 0f;
    private bool isShowingFeedback = false;
    private AudioSource currentQuestionClip;
    private Dictionary<string, AudioClip> questionToAudioclip = new Dictionary<string, AudioClip>();

    private readonly Dictionary<string, string> questionToAnswer = new Dictionary<string, string>()
    {
        {"Which planet is known for its beautiful rings?", "Saturn"},
        {"Which planet is closest to the Sun and is also the smallest?", "Mercury"},
        {"On which planet can you find the Great Red Spot, a giant storm?", "Jupiter"},
        {"Which planet is famous for having the highest mountain and volcano in the Solar System?", "Mars"},
        {"Which planet is known as the Evening Star because of its bright appearance?", "Venus"},
        {"This planet is tilted on its side, making it unique. Which planet is it?", "Uranus"},
        {"Which planet is known for its extreme winds and blue color due to methane in its atmosphere?", "Neptune"},
        {"This planet has a moon named Titan, which is larger than the planet Mercury?", "Saturn"},
        {"On which planet would you weigh the least, due to its small size and low gravity?", "Mercury"},
        {"Which planet is known as the Red Planet because of its reddish appearance?", "Mars"},
        {"Which planet is known for its bright blue color and is often called the 'Ice Giant'?", "Neptune"},
        {"This planet has 27 known moons and is the only one that rotates on its side. What's its name?", "Uranus"},
        {"Which planet is known as Earth’s Twin because of their similar size?", "Venus"},
        {"Which planet has seasons, polar ice caps, and could potentially support life with its water sources?", "Mars"},
        {"Known as the 'Gas Giant,' this planet is the largest in our Solar System. What is its name?", "Jupiter"}
    };

    // Updated to use lists for questions and answers
    private readonly List<string> availableQuestions = new();
    private readonly List<string> askedQuestions = new();
    private ImageTracker imageTracker;

    private void Start()
    {
        SetupGame();
        imageTracker = GetComponent<ImageTracker>();
    }

    private void SetupGame()
    {
        InstructionsPanel.SetActive(true);
        ScoreUIPanel.SetActive(false);
        GameOverPanel.SetActive(false);
        feedbackPopup.SetActive(false);
        questionUIPrefab.SetActive(false);

        // Populate available questions from the dictionary
        foreach (var question in questionToAnswer.Keys)
        {
            availableQuestions.Add(question);
        }

        foreach (var question in questionToAnswer.Keys)
        {
            questionToAudioclip.Add(question, questionClips[availableQuestions.IndexOf(question)]);
        }

        ResetGame();
        currentQuestionClip = gameObject.AddComponent<AudioSource>();
    }

    public void StartGame()
    {
        InstructionsPanel.SetActive(false);
        ScoreUIPanel.SetActive(true);
        GoToNextQuestion();
    }

    private void ResetGame()
    {
        score = -60;
        totalTime = 0f;
        askedQuestions.Clear(); // Clear the list of asked questions
        // Ensure availableQuestions is filled if starting a new game after playing
        if (availableQuestions.Count < questionToAnswer.Count)
        {
            foreach (var question in questionToAnswer.Keys)
            {
                if (!askedQuestions.Contains(question))
                {
                    availableQuestions.Add(question);
                }
            }
        }
        currentTime = timeLimit;
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
        // Check if the game has already asked 10 questions
        if (askedQuestions.Count >= 10)
        {
            EndGame(); // Call EndGame function to handle game over logic
            return;
        }

        // Select a random question from the list of available questions
        int questionIndex = Random.Range(0, availableQuestions.Count);
        currentQuestion = availableQuestions[questionIndex]; // Update the current question

        // Move the selected question from available to asked to avoid repeats
        askedQuestions.Add(currentQuestion);
        availableQuestions.RemoveAt(questionIndex);

        // Update UI elements to reflect the new question
        questionText.text = currentQuestion;
        questionUIPrefab.SetActive(true); // Show the question UI
        feedbackPopup.SetActive(false); // Ensure feedback popup is hidden
        nextQuestionButton.gameObject.SetActive(false); // Hide the "Next Question" button

        // Play the sound clip for the current question
        if (questionToAudioclip.TryGetValue(currentQuestion, out AudioClip clip))
        {
            currentQuestionClip.clip = clip;
            currentQuestionClip.Play();
        }

        // Start timing the answer period for the new question
        isAnsweringQuestion = true;
        currentTime = timeLimit; // Reset the timer for the new question
        imageTracker.ClearSpawnedPlanet(); // Remove the current planet if it exists
        UpdateTimerUI(); // Update the UI to show the reset timer
        UpdateScoreUI(); // Update the score UI to show the current score
    }

    public void AnswerSubmitted(string submittedAnswer)
    {
        if (!isAnsweringQuestion) return;

        string correctAnswer = questionToAnswer[questionText.text];
        bool isCorrect = submittedAnswer.Equals(correctAnswer, System.StringComparison.OrdinalIgnoreCase);

        if (isCorrect)
        {
            CorrectAnswerSelected();
        }
    }

    public void CorrectAnswerSelected()
    {
        AudioSource.PlayClipAtPoint(correctAnswerClip, Camera.main.transform.position);
        // Stop the current clip if it's playing
        if (currentQuestionClip.isPlaying)
        {
            currentQuestionClip.Stop();
        }
        score += Mathf.CeilToInt(currentTime);
        questionUIPrefab.SetActive(false);
        isAnsweringQuestion = false;
        totalTime += timeLimit - currentTime;
        UpdateScoreUI();

        if (askedQuestions.Count < 10)
        {
            ShowFeedback();
            isShowingFeedback = true;
        }
        else
        {
            EndGame();
        }
    }


    private void ShowFeedback()
    {
        if (isShowingFeedback) return;

        feedbackPopup.SetActive(true);
        StartCoroutine(HideFeedbackAfterDelay(2f));
    }

    private IEnumerator HideFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        feedbackPopup.SetActive(false);
        isShowingFeedback = false;
        nextQuestionButton.gameObject.SetActive(true);
    }

    private void TimeRanOut()
    {
        isAnsweringQuestion = false;
        questionUIPrefab.SetActive(false);
        if (askedQuestions.Count < 10)
        {
            nextQuestionButton.gameObject.SetActive(true);
        }
        else
        {
            EndGame();
        }
    }

    private void UpdateTimerUI()
    {
        timerText.text = $"{Mathf.CeilToInt(currentTime)}s";
    }

    private void UpdateScoreUI()
    {
        score += isAnsweringQuestion ? Mathf.CeilToInt(currentTime) : 0;
        scoreText.text = $"{score}";
    }

    private void EndGame()
    {
        AudioSource.PlayClipAtPoint(gameoverClip, Camera.main.transform.position);
        GameOverPanel.SetActive(true);
        finalScoreText.text = $"{score}";
        finalTotalTimeText.text = Mathf.RoundToInt(totalTime).ToString() + "s";
        DisplayStarsBasedOnScore();
    }

    private void DisplayStarsBasedOnScore()
    {
        // Correctly pass the necessary parameters to CalculateStars
        int starsToShow = CalculateStars(score, askedQuestions.Count, Mathf.CeilToInt(timeLimit));
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].gameObject.SetActive(i < starsToShow);
        }
    }

    private int CalculateStars(int score, int totalQuestions, int maxScorePerQuestion)
    {
        int totalPossibleScore = totalQuestions * maxScorePerQuestion;
        float scorePercentage = (float)score / totalPossibleScore * 100;

        if (scorePercentage >= 75) return 3;
        if (scorePercentage >= 50) return 2;
        return 1;
    }


    public bool IsAnsweringQuestion()
    {
        return isAnsweringQuestion;
    }

    public string GetCurrentQuestionAnswer()
    {
        // Ensure there's a current question set, and retrieve the answer.
        if (!string.IsNullOrEmpty(currentQuestion) && questionToAnswer.ContainsKey(currentQuestion))
        {
            return questionToAnswer[currentQuestion];
        }
        return string.Empty; // Return an empty string if no question is set or found.
    }
}

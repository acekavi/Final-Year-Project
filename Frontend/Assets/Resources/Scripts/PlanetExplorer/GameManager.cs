using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject questionUIPrefab;
    public TMP_Text questionText;
    public TMP_Text scoreText;
    public GameObject ScoreUIPanel;
    public TMP_Text timerText;
    public Button nextQuestionButton;
    public GameObject InstructionsPanel;
    public GameObject CorrectAnswerPopup;
    public GameObject TryAgainPopup;

    [Header("Game Over Panel")]
    public GameObject GameOverPanel;
    public TMP_Text finalScoreText;
    public TMP_Text finalTotalTimeText;
    public Image[] starImages;
    public GameObject FeedbackPanel;
    public TMP_Text FeedbackText;


    [Header("Audio Clips")]
    public AudioClip correctAnswerClip;
    public AudioClip wrongAnswerClip;
    public AudioClip timeUpClip;
    public AudioClip gameoverClip;
    public AudioClip[] questionClips;

    private readonly float timeLimit = 30f;
    private float currentTime;
    private int score = 0;
    private string currentQuestion = string.Empty;
    private bool isAnsweringQuestion = false;
    private float totalTime = 0f;
    private bool isShowingWelldone = false;
    private bool isShowingTryAgain = false;
    private AudioSource currentQuestionClip;
    private Dictionary<string, AudioClip> questionToAudioclip = new Dictionary<string, AudioClip>();

    private bool isShowingFeedback = false;

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
        {"Known as the 'Gas Giant,' this planet is the largest in our Solar System. What is its name?", "Jupiter"},
        {"What is known as the blue planet?", "Earth"},
        {"Which is home to living species like Cats and Dogs?", "Earth"}
    };

    // Updated to use lists for questions and answers
    private readonly List<string> availableQuestions = new();
    private readonly List<string> askedQuestions = new();
    private ImageTracker imageTracker;


    // Achivement tracking
    private int incorrectAnswerCount = 0;
    private string addAchivementUrl;
    private string addBadgeUrl;
    private Queue<string> feedbackQueue = new Queue<string>(); // Queue to store feedback messages

    void Awake()
    {
        addAchivementUrl = ConfigManager.GetApiUrl("/api/users/add/achievement");
        addBadgeUrl = ConfigManager.GetApiUrl("/api/users/add/badge");
    }

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
        CorrectAnswerPopup.SetActive(false);
        questionUIPrefab.SetActive(false);
        TryAgainPopup.SetActive(false);
        nextQuestionButton.gameObject.SetActive(false);

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
        scoreText.text = "0";
        GoToNextQuestion();
    }

    private void ResetGame()
    {
        score = 0;
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
        imageTracker.ClearAllSpawnedPlanets();
        nextQuestionButton.gameObject.SetActive(false); // Hide the "Next Question" button
        // Select a random question from the list of available questions
        int questionIndex = Random.Range(0, availableQuestions.Count);
        currentQuestion = availableQuestions[questionIndex]; // Update the current question

        // Move the selected question from available to asked to avoid repeats
        askedQuestions.Add(currentQuestion);
        availableQuestions.RemoveAt(questionIndex);

        // Update UI elements to reflect the new question
        questionText.text = currentQuestion;
        questionUIPrefab.SetActive(true); // Show the question UI
        CorrectAnswerPopup.SetActive(false); // Ensure feedback popup is hidden

        // Play the sound clip for the current question
        if (questionToAudioclip.TryGetValue(currentQuestion, out AudioClip clip))
        {
            currentQuestionClip.clip = clip;
            currentQuestionClip.Play();
        }

        // Start timing the answer period for the new question
        isAnsweringQuestion = true;
        currentTime = timeLimit; // Reset the timer for the new question
        UpdateTimerUI(); // Update the UI to show the reset timer
    }

    public void CorrectAnswerSelected()
    {
        AudioSource.PlayClipAtPoint(correctAnswerClip, Camera.main.transform.position);

        // Stop the current clip if it's playing
        if (currentQuestionClip.isPlaying)
        {
            currentQuestionClip.Stop();
        }

        // Hide the question panel
        questionUIPrefab.SetActive(false);
        // Stop the answering process
        isAnsweringQuestion = false;
        // Update total time
        totalTime += timeLimit - currentTime;
        // Update score UI
        UpdateScoreUI();

        // Check if the maximum number of questions has been reached
        if (askedQuestions.Count < 10)
        {
            // Show the "Correct Answer" popup
            ShowCorrectAnswerPopup();
            isShowingWelldone = true;
        }
        else
        {
            // End the game if the maximum number of questions has been reached
            EndGame();
        }
    }

    private void ShowCorrectAnswerPopup()
    {
        if (isShowingWelldone) return;

        // Show the "Correct Answer" popup
        CorrectAnswerPopup.SetActive(true);

        // Hide the popup after a delay
        StartCoroutine(HideWelldonePopup(2f));
    }

    private IEnumerator HideWelldonePopup(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Hide the "Correct Answer" popup
        CorrectAnswerPopup.SetActive(false);

        // Reset the flag indicating the popup is showing
        isShowingWelldone = false;

        // Show the "Next Question" button
        nextQuestionButton.gameObject.SetActive(true);
    }

    public void WrongAnswerSelected()
    {
        // Prevent the "Try Again" popup from running again if it's already active
        if (isShowingTryAgain) return;

        incorrectAnswerCount++;

        // Stop the current question clip if it's playing
        if (currentQuestionClip.isPlaying)
        {
            currentQuestionClip.Stop();
        }

        // Play the wrong answer audio clip
        AudioSource.PlayClipAtPoint(wrongAnswerClip, Camera.main.transform.position);

        // Deduct score if applicable
        if (score >= 20)
        {
            score -= 20;
            scoreText.text = $"{score}";
        }

        // Show the "Try Again" popup
        TryAgainPopup.SetActive(true);
        isShowingTryAgain = true;

        // Hide the question panel for 3 seconds
        StartCoroutine(HideQuestionPanelForSeconds(5f));
    }

    private IEnumerator HideQuestionPanelForSeconds(float seconds)
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(seconds);

        // Hide the "Try Again" popup and show the question panel again
        TryAgainPopup.SetActive(false);

        // Reset the flag indicating the "Try Again" popup is showing
        isShowingTryAgain = false;
    }

    private void TimeRanOut()
    {
        isAnsweringQuestion = false;
        questionUIPrefab.SetActive(false);
        if (askedQuestions.Count < 10)
        {
            AudioSource.PlayClipAtPoint(timeUpClip, Camera.main.transform.position);
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
        int timeRemaining = Mathf.CeilToInt(currentTime);
        float normalizedTime = Mathf.Clamp01(timeRemaining / timeLimit); // Normalize the time remaining between 0 and 1
        int scoreIncrement = Mathf.RoundToInt(Mathf.Lerp(0, 100, normalizedTime)); // Interpolate between 0 and 100 based on normalized time
        score += scoreIncrement;
        scoreText.text = $"{score}";
    }

    private void EndGame()
    {
        AudioSource.PlayClipAtPoint(gameoverClip, Camera.main.transform.position);
        imageTracker.ClearAllSpawnedPlanets();
        GameOverPanel.SetActive(true);
        finalScoreText.text = $"{score}";
        finalTotalTimeText.text = Mathf.RoundToInt(totalTime).ToString() + " s";
        DisplayStarsBasedOnScore();

        AddAchievement("Planet Explorer");
        if (incorrectAnswerCount == 0)
        {
            AddBadge("Perfect");
        }
    }

    private void DisplayStarsBasedOnScore()
    {
        // Correctly pass the necessary parameters to CalculateStars
        int starsToShow = CalculateStars(score, askedQuestions.Count);
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].gameObject.SetActive(i < starsToShow);
        }
    }

    private int CalculateStars(int score, int totalQuestions)
    {
        int maxScore = totalQuestions * 100;
        float scorePercentage = (float)score / maxScore * 100;

        if (scorePercentage >= 75)
        {
            AddBadge("Master");
            return 3;
        }
        if (scorePercentage >= 50)
        {
            AddBadge("Expert");
            return 2;
        }
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

    public void AddAchievement(string achievement)
    {
        Debug.Log($"Achievement unlocked: {achievement}");
        StartCoroutine(AddAchievementToUser(achievement));
    }

    private IEnumerator AddAchievementToUser(string achievement)
    {
        string token = PlayerPrefs.GetString("AuthToken", "");
        // Create a form to send the achievement data to the server
        WWWForm form = new();
        form.AddField("achievement", achievement);

        // Create a POST request to send the achievement data to the server
        UnityWebRequest request = UnityWebRequest.Post(addAchivementUrl, form);
        request.SetRequestHeader("Authorization", "Bearer " + token);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Achievement added successfully!");
            showFeedbackPanel("Achievement Unlocked: " + achievement);
        }
        else
        {
            Debug.LogError("Failed to add achievement: " + request.error);
        }
    }

    public void AddBadge(string badge)
    {
        Debug.Log($"Badge unlocked: {badge}");
        StartCoroutine(AddBadgeToUser(badge));
    }

    private IEnumerator AddBadgeToUser(string badge)
    {
        string token = PlayerPrefs.GetString("AuthToken", "");
        // Create a form to send the badge data to the server
        WWWForm form = new();
        form.AddField("badge", badge);

        // Create a POST request to send the badge data to the server
        UnityWebRequest request = UnityWebRequest.Post(addBadgeUrl, form);
        request.SetRequestHeader("Authorization", "Bearer " + token);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Badge added successfully!");
            showFeedbackPanel("Badge Unlocked: " + badge);
        }
        else
        {
            Debug.LogError("Failed to add badge: " + request.error);
        }
    }

    private void showFeedbackPanel(string feedbackText)
    {
        feedbackQueue.Enqueue(feedbackText); // Enqueue the new feedback message
        if (!isShowingFeedback) // If feedback panel is not already showing, start showing feedback
        {
            StartCoroutine(ShowFeedback());
        }
    }

    private IEnumerator ShowFeedback()
    {
        isShowingFeedback = true;
        while (feedbackQueue.Count > 0)
        {
            string feedbackText = feedbackQueue.Dequeue(); // Dequeue the next feedback message
            FeedbackText.text = feedbackText; // Set the feedback text

            FeedbackPanel.SetActive(true); // Show the feedback panel

            yield return new WaitForSeconds(2f); // Wait for 2 seconds

            FeedbackPanel.SetActive(false); // Hide the feedback panel

            yield return new WaitForSeconds(0.2f); // Add a short delay between feedback messages
        }
        isShowingFeedback = false; // Reset flag when all feedback messages are shown
    }
}

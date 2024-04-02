using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class RegisterManager : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TMP_InputField nameInputField;
    public TMP_InputField ageInputField;
    public TMP_Text feedbackMessageText;
    public GameObject feedbackPopup;

    private string registerUrl; // Replace with your actual register endpoint

    // Method to be called when the register button is pressed
    public void OnRegisterButtonPressed()
    {
        registerUrl = ConfigManager.GetApiUrl("/api/users/register");
        StartCoroutine(RegisterCoroutine(emailInputField.text, passwordInputField.text,
            nameInputField.text, ageInputField.text));
    }

    private IEnumerator RegisterCoroutine(string email, string password, string name, string age)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);
        form.AddField("name", name);
        form.AddField("age", age);

        UnityWebRequest www = UnityWebRequest.Post(registerUrl, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log($"Register Failed: {www.error}");
            feedbackMessageText.text = "Registration failed. Please try again.";
            feedbackPopup.SetActive(true);
            yield return new WaitForSeconds(3); // Show feedback for 3 seconds
            feedbackPopup.SetActive(false);
        }
        else
        {
            Debug.Log("Register Successful");
            feedbackMessageText.text = "Registration successful!";
            feedbackPopup.SetActive(true);
            yield return new WaitForSeconds(3); // Show feedback for 3 seconds
            feedbackPopup.SetActive(false);
            // Optionally, navigate to login scene or automatically log in the user
            UnityEngine.SceneManagement.SceneManager.LoadScene("loginMenu");
        }
    }
}

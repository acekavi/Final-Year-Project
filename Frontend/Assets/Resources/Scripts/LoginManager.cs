using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    [System.Serializable]
    private class AuthResponse
    {
        public string message;
        public string token; // Ensure this matches the JSON response
    }

    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;
    public TMP_Text errorMessageText;
    public GameObject errorPopup;

    private string loginUrl;

    // Method to be called when the login button is pressed
    public void OnLoginButtonPressed()
    {
        loginUrl = ConfigManager.GetApiUrl("/api/users/signin");
        StartCoroutine(LoginCoroutine(usernameInputField.text, passwordInputField.text));
    }

    private IEnumerator LoginCoroutine(string email, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        UnityWebRequest www = UnityWebRequest.Post(loginUrl, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            errorMessageText.text = "Login failed. Please try again.";
            // Show error popup
            errorPopup.SetActive(true);
            // Wait for 1 second
            yield return new WaitForSeconds(1);
            // Hide error popup
            errorPopup.SetActive(false);
        }
        else
        {
            Debug.Log("Login Successful");
            // Extract token from response (assuming JSON response format)
            string jsonResponse = www.downloadHandler.text;
            AuthResponse response = JsonUtility.FromJson<AuthResponse>(jsonResponse);
            SaveAuthToken(response.token);
            // Handle successful login here (e.g., load another scene or display a welcome message)
            errorMessageText.text = "Login successfull!";
            // Show error popup
            errorPopup.SetActive(true);
            // Wait for 1 second
            yield return new WaitForSeconds(1);
            // Hide error popup
            errorPopup.SetActive(false);
            UnityEngine.SceneManagement.SceneManager.LoadScene("StartMenu");
        }
    }

    // Call this method to save the auth token
    private void SaveAuthToken(string token)
    {
        PlayerPrefs.SetString("AuthToken", token);
        PlayerPrefs.Save(); // Make sure to save PlayerPrefs
        Debug.Log("Auth Token Saved");
    }
}
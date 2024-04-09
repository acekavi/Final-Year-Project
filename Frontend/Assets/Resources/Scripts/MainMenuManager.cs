using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


[System.Serializable]
class AuthResponse
{
    public string message;
    public bool auth;
}

[System.Serializable]
class UserDetailsResponse
{
    public string email;
    public string name;
    public int age;
    public string[] achievements;
    public string[] badges;
    public int level;
}

public class MainMenuManager : MonoBehaviour
{
    public Button loginButton;
    public Button registerButton;
    public Button playButton;
    public Button profileButton;
    public TMP_Text usernameText;

    private string authCheckUrl; // Will be loaded from config.json
    private string userDetailUrl; // Will be loaded from config.json

    void Start()
    {
        authCheckUrl = ConfigManager.GetApiUrl("/api/users/check/auth");
        userDetailUrl = ConfigManager.GetApiUrl("/api/users/details");
        StartCoroutine(CheckAuthentication());
    }

    IEnumerator CheckAuthentication()
    {
        string token = PlayerPrefs.GetString("AuthToken", "");
        if (string.IsNullOrEmpty(token))
        {
            ShowLoginRegisterButtons();
            yield break;
        }

        UnityWebRequest request = UnityWebRequest.Get(authCheckUrl);
        request.SetRequestHeader("Authorization", "Bearer " + token);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Assuming the server responds with a JSON object containing an 'auth' boolean.
            AuthResponse response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
            if (response.auth)
            {
                ShowPlayButton();
                StartCoroutine(GetUsername());
            }
            else
            {
                ShowLoginRegisterButtons();
                PlayerPrefs.DeleteKey("AuthToken");
                PlayerPrefs.DeleteKey("Username");
                PlayerPrefs.DeleteKey("Email");
                PlayerPrefs.DeleteKey("Level");
                PlayerPrefs.Save();
            }
        }
        else
        {
            // Handle error (e.g., show an error message)
            ShowLoginRegisterButtons();
        }
    }

    IEnumerator GetUsername()
    {
        string token = PlayerPrefs.GetString("AuthToken", "");
        UnityWebRequest request = UnityWebRequest.Get(userDetailUrl);
        request.SetRequestHeader

        ("Authorization", "Bearer " + token);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Assuming the server responds with a JSON object containing a 'username' field.
            UserDetailsResponse response = JsonUtility.FromJson<UserDetailsResponse>(request.downloadHandler.text);
            usernameText.text = response.name;
            PlayerPrefs.SetString("Username", response.name);
            PlayerPrefs.SetString("Email", response.email);
            PlayerPrefs.SetString("Level", response.level.ToString());
        }
        else
        {
            // Handle error (e.g., show an error message)
            Debug.Log(request.error);
        }
    }

    void ShowLoginRegisterButtons()
    {
        loginButton.gameObject.SetActive(true);
        registerButton.gameObject.SetActive(true);
        playButton.gameObject.SetActive(false);
        profileButton.gameObject.SetActive(false);
    }

    void ShowPlayButton()
    {
        loginButton.gameObject.SetActive(false);
        registerButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(true);
        profileButton.gameObject.SetActive(true);
    }
}

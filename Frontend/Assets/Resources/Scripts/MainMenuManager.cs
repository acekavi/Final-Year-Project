using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


[System.Serializable]
public class AuthResponse
{
    public string message;
    public bool auth;
}

public class MainMenuManager : MonoBehaviour
{
    public Button loginButton;
    public Button registerButton;
    public Button playButton;

    private string authCheckUrl; // Will be loaded from config.json

    void Start()
    {
        authCheckUrl = ConfigManager.GetApiUrl("/api/users/check/auth");
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
            }
            else
            {
                ShowLoginRegisterButtons();
            }
        }
        else
        {
            // Handle error (e.g., show an error message)
            ShowLoginRegisterButtons();
        }
    }

    void ShowLoginRegisterButtons()
    {
        loginButton.gameObject.SetActive(true);
        registerButton.gameObject.SetActive(true);
        playButton.gameObject.SetActive(false);
    }

    void ShowPlayButton()
    {
        loginButton.gameObject.SetActive(false);
        registerButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(true);
    }
}

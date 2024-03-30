using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for interacting with UI elements

public class MainMenuManager : MonoBehaviour
{
    public Button loginButton;
    public Button registerButton;
    public Button playButton;

    // Assuming we have a simple way to check if the player is logged in
    // This could be replaced with your authentication check
    private bool isLoggedIn = false;

    void Start()
    {
        UpdateMenu();
    }

    void UpdateMenu()
    {
        // Check if the player is logged in
        if (isLoggedIn)
        {
            // Player is logged in, show the Play button
            loginButton.gameObject.SetActive(false);
            registerButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(true);
        }
        else
        {
            // Player is not logged in, show the Login and Register buttons
            loginButton.gameObject.SetActive(true);
            registerButton.gameObject.SetActive(true);
            playButton.gameObject.SetActive(false);
        }
    }

    // Example method to simulate player logging in
    public void LogIn()
    {
        isLoggedIn = true;
        UpdateMenu();
    }

    // Example method to simulate player logging out
    public void LogOut()
    {
        isLoggedIn = false;
        UpdateMenu();
    }
}

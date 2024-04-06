using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ProfilePageManager : MonoBehaviour
{
    public TMP_Text emailText;
    public TMP_Text usernameText;
    public TMP_Text levelText;
    public TMP_Text achievementsText;
    public TMP_Text badgesText;
    public GameObject badgesPanel;

    private string userDetailUrl; // Will be loaded from config.json

    // Start is called before the first frame update
    void Start()
    {
        userDetailUrl = ConfigManager.GetApiUrl("/api/users/details");
        StartCoroutine(GetUserDetails());
    }

    IEnumerator GetUserDetails()
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
            emailText.text = response.email;
            usernameText.text = response.name;
            levelText.text = response.level.ToString();
            if (response.achievements != null && response.achievements.Length > 0)
            {
                achievementsText.text = string.Join(", ", response.achievements);
            }
            else
            {
                achievementsText.text = "No achievements yet!";

            }
            if (response.badges != null && response.badges.Length > 0)
            {
                foreach (string badge in response.badges)
                {
                    GameObject badgeGo = Instantiate(Resources.Load<GameObject>("Prefabs/Badge"));
                    badgeGo.transform.SetParent(badgesPanel.transform, false);
                    badgeGo.GetComponentInChildren<TMP_Text>().text = badge;
                }
            }
            else
            {
                badgesText.text = "No badges yet!";
            }
        }
        else
        {
            // Handle error (e.g., show an error message)
            Debug.Log(request.error);
        }
    }

    public void LogOut()
    {
        PlayerPrefs.DeleteKey("AuthToken");
        PlayerPrefs.DeleteKey("Username");
        PlayerPrefs.DeleteKey("Email");
        PlayerPrefs.DeleteKey("Level");
        PlayerPrefs.Save();
        // Assuming there is a scene named "LoginScene"
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartMenu");

    }
}

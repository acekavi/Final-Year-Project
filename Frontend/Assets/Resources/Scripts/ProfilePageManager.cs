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
    public GameObject masterBadgePrefab;
    public GameObject expertBadgePrefab;
    public GameObject perfectBadgePrefab;


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
                    if (badge == "Master")
                    {
                        AddBadge(masterBadgePrefab, badge);
                    }
                    else if (badge == "Expert")
                    {
                        AddBadge(expertBadgePrefab, badge);
                    }
                    else if (badge == "Perfect")
                    {
                        AddBadge(perfectBadgePrefab, badge);
                    }
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

    public void AddBadge(GameObject badgePrefab, string badgeName)
    {
        // Instantiate badge prefab
        Instantiate(badgePrefab, badgesPanel.transform);

        // Position badges dynamically
        RepositionBadges();
    }

    private void RepositionBadges()
    {
        for (int i = 0; i < badgesPanel.transform.childCount; i++)
        {
            Transform badgeTransform = badgesPanel.transform.GetChild(i);
            badgeTransform.localPosition = new Vector3(i * 250f, 0f, 0f); // Example positioning (adjust as needed)
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConfigData
{
    public string apiUrl;
}

public class ConfigManager : MonoBehaviour
{
    public static ConfigData config;

    void Awake()
    {
        TextAsset configFile = Resources.Load<TextAsset>("env");
        if (configFile == null)
        {
            Debug.LogError("Failed to load config file.");
            return;
        }
        config = JsonUtility.FromJson<ConfigData>(configFile.text);
        Debug.Log($"Config loaded: {config.apiUrl}");
    }

    public static string GetApiUrl()
    {
        return config.apiUrl;
    }

    public static string GetApiUrl(string endpoint)
    {
        return config.apiUrl + endpoint;
    }

    public static string GetApiUrl(string endpoint, string query)
    {
        return config.apiUrl + endpoint + "?" + query;
    }

    public static string GetApiUrl(string endpoint, Dictionary<string, string> query)
    {
        string queryString = "";
        foreach (KeyValuePair<string, string> entry in query)
        {
            queryString += entry.Key + "=" + entry.Value + "&";
        }
        return config.apiUrl + endpoint + "?" + queryString.TrimEnd('&');
    }

    public static string GetApiUrl(string endpoint, string[] query)
    {
        string queryString = "";
        foreach (string entry in query)
        {
            queryString += entry + "&";
        }
        return config.apiUrl + endpoint + "?" + queryString.TrimEnd('&');
    }

    public static string GetApiUrl(string endpoint, string[] keys, string[] values)
    {
        string queryString = "";
        for (int i = 0; i < keys.Length; i++)
        {
            queryString += keys[i] + "=" + values[i] + "&";
        }
        return config.apiUrl + endpoint + "?" + queryString.TrimEnd('&');
    }

    public static string GetApiUrl(string endpoint, string key, string value)
    {
        return config.apiUrl + endpoint + "?" + key + "=" + value;
    }

}
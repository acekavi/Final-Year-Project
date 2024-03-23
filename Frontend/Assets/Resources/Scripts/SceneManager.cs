using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public void MovetoScene(int SceneID)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneID);
    }
}

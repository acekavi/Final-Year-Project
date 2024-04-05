using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public void ToggleElement()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void ToggleGameObject(GameObject go)
    {
        go.SetActive(!go.activeSelf);
    }
}

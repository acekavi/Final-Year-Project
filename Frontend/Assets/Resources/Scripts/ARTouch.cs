using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ARTouch : MonoBehaviour
{
    // public Canvas earthPopup;

    void Update()
    {
        // var touchScreen = Touchscreen.current;
        // if (touchScreen != null && touchScreen.touches[0].press.isPressed)
        // {
        //     var touch = touchScreen.touches[0];
        //     if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
        //     {
        //         Ray ray = Camera.main.ScreenPointToRay(touch.position.ReadValue());
        //         RaycastHit hit;
        //         if (Physics.Raycast(ray, out hit))
        //         {
        //             if (hit.collider != null)
        //             {
        //                 if (hit.collider.tag == "Earth")
        //                 {
        //                     earthPopup.SetActive(true);
        //                 }
        //             }
        //         }
        //     }
        // }

        // var mouse = Mouse.current;
        // if (mouse != null && mouse.leftButton.isPressed)
        // {
        //     Ray ray = Camera.main.ScreenPointToRay(mouse.position.ReadValue());
        //     RaycastHit hit;
        //     if (Physics.Raycast(ray, out hit, 100))
        //     {
        //         if (hit.collider != null)
        //         {
        //             if (hit.transform.tag == "TrackingObject")
        //             {
        //                 Vector3 pos = hit.point;
        //                 pos.z += 0.25f;
        //                 pos.y += 0.25f;
        //                 Instantiate(earthPopup, pos, transform.rotation);
        //             }

        //             if (hit.transform.tag == "InfoPanel")
        //             {
        //                 Destroy(hit.transform.gameObject);
        //             }
        //         }
        //     }
        // }
    }
}
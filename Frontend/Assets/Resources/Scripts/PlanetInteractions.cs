using UnityEngine;
using UnityEngine.InputSystem; // Include the new input system namespace

public class PlanetInteraction : MonoBehaviour
{
    public GameObject infoPanelPrefab;
    private GameObject instantiatedInfoPanel;

    private TouchControls touchControls; // The new input system touch controls
    private float rotationSpeed = 10f;
    private bool isRotating = false;
    private float initialPinchDistance;
    private float initialFieldOfView;

    private void Awake()
    {
        touchControls = new TouchControls();

        touchControls.Touch.TouchPress.started += _ => StartTouch();
        touchControls.Touch.TouchPress.canceled += _ => EndTouch();
        touchControls.Touch.Pinch.started += ctx => StartPinch(ctx);
    }

    private void OnEnable()
    {
        touchControls.Enable();
    }

    private void OnDisable()
    {
        touchControls.Disable();
    }

    void Update()
    {
        HandleRotation();
        CheckForTouch();
    }

    void StartTouch()
    {
        isRotating = true;
    }

    void EndTouch()
    {
        isRotating = false;
    }

    void StartPinch(InputAction.CallbackContext context)
    {
        // This method initializes the pinch; you'll need to track two fingers' movement for zoom
        initialPinchDistance = Vector2.Distance(Touchscreen.current.touches[0].position.ReadValue(), Touchscreen.current.touches[1].position.ReadValue());
        initialFieldOfView = Camera.main.fieldOfView;
    }

    void HandleRotation()
    {
        if (isRotating && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 delta = Touchscreen.current.primaryTouch.delta.ReadValue();
            float rotationFactor = delta.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, -rotationFactor, Space.World);
        }
    }

    void HandleZoom()
    {
        // Implemented inside Update or a separate method that's called when two fingers are on the screen
        if (Touchscreen.current.touches[1].isInProgress)
        {
            float currentPinchDistance = Vector2.Distance(Touchscreen.current.touches[0].position.ReadValue(), Touchscreen.current.touches[1].position.ReadValue());
            float pinchRatio = currentPinchDistance / initialPinchDistance;
            Camera.main.fieldOfView = Mathf.Clamp(initialFieldOfView * pinchRatio, 30f, 100f);
        }
    }

    void CheckForTouch()
    {
        // Assuming touch to show info panel is a single touch; adjust as needed
        if (touchControls.Touch.TouchPress.WasPerformedThisFrame())
        {
            Vector2 touchPosition = touchControls.Touch.TouchPosition.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                ShowPlanetInfo();
            }
        }
    }

    void ShowPlanetInfo()
    {
        // Your existing logic to show the information panel
    }
}

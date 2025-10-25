using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine; // für Cinemachine v3

public class CMCameraRMBGate : MonoBehaviour
{
    [Header("Input Actions")]
    [Tooltip("Look-Action (Vector2, z. B. Mouse Delta).")]
    public InputActionReference lookAction;

    [Tooltip("RotateHeld-Action (Button, z. B. RMB).")]
    public InputActionReference rotateHeld;

    [Tooltip("Zoom-Action (Vector2, <Mouse>/scroll).")]
    public InputActionReference zoomAction;

    [Header("Cinemachine Components")]
    [Tooltip("Cinemachine Input Axis Controller auf der Kamera.")]
    public CinemachineInputAxisController axisController;

    [Tooltip("Cinemachine Orbit Follow auf der Kamera.")]
    public CinemachineOrbitalFollow orbitFollow;

    [Header("Zoom Settings")]
    public float zoomSpeed = 2f;
    public float minRadius = 3f;
    public float maxRadius = 12f;

    void Start()
    {
        if (axisController == null)
            axisController = GetComponent<CinemachineInputAxisController>();

        if (orbitFollow == null)
            orbitFollow = GetComponent<CinemachineOrbitalFollow>();
    }

    void OnEnable()
    {
        if (lookAction != null) lookAction.action.Enable();
        if (rotateHeld != null) rotateHeld.action.Enable();
        if (zoomAction != null) zoomAction.action.Enable();
    }

    void OnDisable()
    {
        if (lookAction != null) lookAction.action.Disable();
        if (rotateHeld != null) rotateHeld.action.Disable();
        if (zoomAction != null) zoomAction.action.Disable();
    }

    void Update()
    {
        // --- RMB Gate ---
        bool held = rotateHeld != null && rotateHeld.action.IsPressed();
        if (axisController != null)
            axisController.enabled = held;

        // --- Zoom ---
        if (zoomAction != null && orbitFollow != null)
        {
            Vector2 scrollVec = zoomAction.action.ReadValue<Vector2>();
            float scroll = scrollVec.y; // y = hoch/runter scrollen

            if (Mathf.Abs(scroll) > 0.01f)
            {
                orbitFollow.Radius -= scroll * zoomSpeed * Time.deltaTime * 100f;
                orbitFollow.Radius = Mathf.Clamp(orbitFollow.Radius, minRadius, maxRadius);
            }
        }
    }
}

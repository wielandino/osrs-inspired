using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CMCameraRMBGate : MonoBehaviour
{
    [Header("Input Actions")]
    [Tooltip("Look-Action (Vector2, e.g. Mouse Delta).")]
    public InputActionReference lookAction;

    [Tooltip("RotateHeld-Action (Button, e.g. RMB).")]
    public InputActionReference rotateHeld;

    [Tooltip("Zoom-Action (Vector2, <Mouse>/scroll).")]
    public InputActionReference zoomAction;

    [Header("Cinemachine Components")]
    [Tooltip("Cinemachine Input Axis Controller.")]
    public CinemachineInputAxisController axisController;

    [Tooltip("Cinemachine Orbit Follow")]
    public CinemachineOrbitalFollow orbitFollow;

    [Header("Zoom Settings")]
    public float zoomSpeed = 2f;
    public float minRadius = 3f;
    public float maxRadius = 12f;

    private void Start()
    {
        if (axisController == null)
            axisController = gameObject.GetComponent<CinemachineInputAxisController>();

        if (orbitFollow == null)
            orbitFollow = gameObject.GetComponent<CinemachineOrbitalFollow>();
    }

    private void OnEnable()
    {
        if (lookAction != null) lookAction.action.Enable();
        if (rotateHeld != null) rotateHeld.action.Enable();
        if (zoomAction != null) zoomAction.action.Enable();
    }

    private void OnDisable()
    {
        if (lookAction != null) lookAction.action.Disable();
        if (rotateHeld != null) rotateHeld.action.Disable();
        if (zoomAction != null) zoomAction.action.Disable();
    }

    private void Update()
    {
        // --- RMB Gate ---
        bool held = rotateHeld != null && rotateHeld.action.IsPressed();
        if (axisController != null)
            axisController.enabled = held;

        // --- Zoom ---
        if (zoomAction != null && orbitFollow != null)
        {
            Vector2 scrollVec = zoomAction.action.ReadValue<Vector2>();
            float scroll = scrollVec.y;

            if (Mathf.Abs(scroll) > 0.01f)
            {
                orbitFollow.Radius -= scroll * zoomSpeed * Time.deltaTime * 100f;
                orbitFollow.Radius = Mathf.Clamp(orbitFollow.Radius, minRadius, maxRadius);
            }
        }
    }
}

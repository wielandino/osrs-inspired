using UnityEngine;

public class TreeController : MonoBehaviour
{
    public static TreeController Instance;

    private void OnEnable()
    {
        Instance = this;
    }

    private void OnDisable()
    {
        if (Instance != null)
            Instance = null;
    }
}
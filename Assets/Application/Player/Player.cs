using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            NeedsUIController.Instance.DecreaseValueFromEnergyBar(0.2f);

        if (Input.GetKeyDown(KeyCode.G))
            NeedsUIController.Instance.DecreaseValueFromHungerBar(0.2f);
    }

    private void OnEnable()
    {
        Instance = this;
    }

    private void OnDisable()
    {
        Instance = null;
    }

    private void Start()
    {
        Instance = this;
    }

    public SimpleGroundSnap GetSimpleGroundSnap()
        => gameObject.GetComponent<SimpleGroundSnap>();

    public PlayerStateManager GetPlayerStateManager()
        => gameObject.GetComponent<PlayerStateManager>();
}
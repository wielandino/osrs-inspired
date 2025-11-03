using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;


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
    

}
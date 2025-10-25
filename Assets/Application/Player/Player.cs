using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;


    void OnEnable()
    {
        Instance = this;
    }

    void OnDisable()
    {
        Instance = null;
    }

    void Start()
    {
        Instance = this;
    }

    public SimpleGroundSnap GetSimpleGroundSnap()
    {
        return gameObject.GetComponent<SimpleGroundSnap>();
    }

}
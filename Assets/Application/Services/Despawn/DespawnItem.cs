using UnityEngine;

[System.Serializable]
public class DespawnItem
{
    public GameObject GameObjectToDespawn;
    public float DespawnTime;
    public float TimeRemaining;

    public DespawnItem(GameObject obj, float time)
    {
        GameObjectToDespawn = obj;
        DespawnTime = time;
        TimeRemaining = time;
    }
}

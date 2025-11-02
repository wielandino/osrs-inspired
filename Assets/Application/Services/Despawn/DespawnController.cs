using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Progress;

public class DespawnController : MonoBehaviour
{
    public static DespawnController Instance;


    public List<DespawnItem> DespawnItems = new();

    private void OnEnable()
    {
        Instance = this;
    }

    private void OnDisable()
    {
        if (Instance != null)
            Instance = null;
    }

    private void Update()
    {
        for (int i = DespawnItems.Count - 1; i >= 0; i--)
        {
            DespawnItem item = DespawnItems[i];

            if (item.GameObjectToDespawn == null)
            {
                DespawnItems.RemoveAt(i);
                continue;
            }

            item.TimeRemaining -= Time.deltaTime;

            if (item.TimeRemaining < 0)
                DespawnObject(item);

        }
    }

    private void DespawnObject(DespawnItem item)
    {
        if (item.GameObjectToDespawn != null)
            Destroy(item.GameObjectToDespawn);

        RemoveFromDespawn(item.GameObjectToDespawn);
    }

    public void RemoveFromDespawn(GameObject objectToRemove)
    {
        for (int i = DespawnItems.Count - 1; i >= 0; i--)
        {
            if (DespawnItems[i].GameObjectToDespawn == objectToRemove)
            {
                DespawnItems.RemoveAt(i);
                Debug.Log($"DespawnController: {objectToRemove.name} aus Despawn-Liste entfernt");
                return;
            }
        }
    }

    public void RegisterForDespawn(GameObject objectToDespawn, float despawnTime)
    {
        if (objectToDespawn == null)
        {
            return;
        }

        // Pr�fe ob Objekt bereits registriert ist
        DespawnItem existingItem = DespawnItems.Find(item => item.GameObjectToDespawn == objectToDespawn);
        if (existingItem != null)
        {
            return;
        }

        DespawnItem newItem = new(objectToDespawn, despawnTime);
        DespawnItems.Add(newItem);
    }
}
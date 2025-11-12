using UnityEngine;

public class TreeLogBurning : MonoBehaviour
{
    [SerializeField]
    private TreeLog _treeLog;

    private void OnEnable()
    {
        _treeLog.CanBeStacked = false;
    }

    private void OnDisable()
    {
        _treeLog.CanBeStacked = true;
    }
}

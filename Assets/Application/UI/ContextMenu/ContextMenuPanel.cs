using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContextMenuPanel : MonoBehaviour, IPointerExitHandler
{
    public static ContextMenuPanel Instance { get; private set; }

    public GameObject MenuPanel;
    public GameObject GridContainer;
    public GameObject GridElementPrefab;


    private void Start()
    {
        MenuPanel.SetActive(false);
        Instance = this;
    }

    public void ShowContextMenuForObject(List<ContextMenuOption> options, Vector2 screenPosition)
    {
        ClearOptions();

        MenuPanel.transform.position = screenPosition;

        foreach (var option in options)
        {
            var createdGridElement = Instantiate(GridElementPrefab, GridContainer.transform);

            if (!createdGridElement.TryGetComponent<ContextMenuGridElement>(out var createdContextMenuGridElement))
                continue;

            createdContextMenuGridElement.ContextMenuText.text = option.DisplayText;
            createdContextMenuGridElement.ContextMenuButton.onClick.AddListener(() =>
            {
                option.OnExecute();
                HideContextMenu();
            });
        }

        MenuPanel.SetActive(true);
    }

    public void HideContextMenu()
    {
        MenuPanel.SetActive(false);
        ClearOptions();
    }
    
    private void ClearOptions()
    {
        foreach(Transform gridElement in GridContainer.transform)
        {
            Destroy(gridElement.gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideContextMenu();
    }
}
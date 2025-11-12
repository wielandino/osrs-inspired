using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContextMenuPanel : MonoBehaviour, IPointerExitHandler
{
    public static ContextMenuPanel Instance { get; private set; }

    public GameObject MenuPanel;
    public GameObject GridContainer;
    public GameObject GridElementPrefab;

    public GameObject TitleElement;

    [Header("Resize Settings")]

    [SerializeField]
    private float _minPanelWidth = 120f;

    [SerializeField]
    private float _paddingVertical = 10f;

    [SerializeField]
    private float _paddingHorizontal = 15f;

    [SerializeField]
    private float _buttonHeight = 15f;

    private void Start()
    {
        MenuPanel.SetActive(false);
        Instance = this;
    }

    public void ShowContextMenuForObject(List<ContextMenuOption> options, Vector2 screenPosition)
    {
        ClearOptions();
        MenuPanel.transform.position = screenPosition;

        float maxTextWidth = _minPanelWidth;

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

            createdContextMenuGridElement.ContextMenuText.ForceMeshUpdate();
            var textWidth = createdContextMenuGridElement.ContextMenuText.preferredWidth;

            if (textWidth > maxTextWidth)
            {
                maxTextWidth = textWidth;
            }
        }

        TitleElement.GetComponent<ContextMenuGridElement>().ContextMenuText.text = options[0].Label;

        Canvas.ForceUpdateCanvases();
        ResizeMenu(maxTextWidth, options.Count);

        MenuPanel.SetActive(true);
    }

    private void ResizeMenu(float maxTextWidth, int buttonCount)
    {
        float finalPanelHeight = (buttonCount * (_buttonHeight + _buttonHeight)) + _paddingVertical;
        float finalPanelWidth = maxTextWidth + _paddingHorizontal;

        GridContainer.GetComponent<GridLayoutGroup>().cellSize = new(finalPanelWidth, _buttonHeight);
        TitleElement.GetComponent<RectTransform>().sizeDelta = new(finalPanelWidth, 15f);
        MenuPanel.GetComponent<RectTransform>().sizeDelta = new(finalPanelWidth, finalPanelHeight);
        GridContainer.GetComponent<RectTransform>().sizeDelta = new(finalPanelWidth, finalPanelHeight);

        foreach (Transform gridElement in GridContainer.transform)
        {
            if (gridElement.gameObject == TitleElement)
                continue;

            var rectTransformOfbutton =
                gridElement.GetComponent<ContextMenuGridElement>().ContextMenuButton.GetComponent<RectTransform>();

            rectTransformOfbutton.sizeDelta = new(maxTextWidth, _buttonHeight);
        }
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
            if(gridElement.gameObject != TitleElement)
                Destroy(gridElement.gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideContextMenu();
    }
}
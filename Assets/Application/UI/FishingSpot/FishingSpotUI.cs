using UnityEngine;

public class FishingSpotUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FishingSpot fishingSpot;
    [SerializeField] private FloatingIcon floatingIcon;
    
    [Header("Icon Settings")]
    [SerializeField] private Sprite activeIcon;
    [SerializeField] private Sprite depletedIcon;
    
    [Header("Visual Feedback")]
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color depletedColor = new(0.5f, 0.5f, 0.5f, 0.5f);
    [SerializeField] private bool hideWhenDepleted;
    
    private void Awake()
    {
        if (fishingSpot == null)
            fishingSpot = gameObject.GetComponent<FishingSpot>();
        
        if (floatingIcon == null)
            floatingIcon = gameObject.GetComponent<FloatingIcon>();
        
        if (floatingIcon == null)
            floatingIcon = gameObject.AddComponent<FloatingIcon>();
    }
    
    private void Start()
    {
        floatingIcon.Initialize(activeIcon);
        
        fishingSpot.OnCapacityChanged += HandleCapacityChanged;
        fishingSpot.OnSpotDepleted += HandleSpotDepleted;
        fishingSpot.OnSpotReplenished += HandleSpotReplenished;
        
        UpdateVisualState(fishingSpot.GetFishingCapacity() > 0);
    }
    
    private void OnDestroy()
    {
        if (fishingSpot != null)
        {
            fishingSpot.OnCapacityChanged -= HandleCapacityChanged;
            fishingSpot.OnSpotDepleted -= HandleSpotDepleted;
            fishingSpot.OnSpotReplenished -= HandleSpotReplenished;
        }
    }
    
    private void HandleCapacityChanged(float newCapacity)
    {
    }
    
    private void HandleSpotDepleted()
    {
        UpdateVisualState(false);
    }
    
    private void HandleSpotReplenished()
    {
        UpdateVisualState(true);
    }
    
    private void UpdateVisualState(bool isActive)
    {
        if (isActive)
        {
            floatingIcon.SetIcon(activeIcon);
            floatingIcon.SetColor(activeColor);
            floatingIcon.SetVisibility(true);
        }
        else
        {
            if (hideWhenDepleted)
            {
                floatingIcon.SetVisibility(false);
            }
            else
            {
                floatingIcon.SetIcon(depletedIcon);
                floatingIcon.SetColor(depletedColor);
                floatingIcon.SetVisibility(true);
            }
        }
    }
}
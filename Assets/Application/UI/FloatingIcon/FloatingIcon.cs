using UnityEngine;

public class FloatingIcon : MonoBehaviour
{
    [Header("Icon Settings")]
    [SerializeField] private float heightAboveObject = 2f;
    [SerializeField] private float iconSize = 0.5f;
    
    [Header("Animation Settings")]
    [SerializeField] private bool enableBobbing = true;
    [SerializeField] private float bobbingSpeed = 2f;
    [SerializeField] private float bobbingAmount = 0.2f;
    
    [Header("Rotation Settings")]
    [SerializeField] private bool rotateTowardsCamera = true;
    
    private GameObject iconObject;
    private SpriteRenderer spriteRenderer;
    private Vector3 startPosition;
    private Camera mainCamera;
    
    private void Start()
    {
        mainCamera = Camera.main;
    }
    
    public void Initialize(Sprite iconSprite)
    {
        CreateIcon(iconSprite);
    }
    
    private void CreateIcon(Sprite iconSprite)
    {
        iconObject = new GameObject("Icon");
        iconObject.transform.SetParent(transform);
        
        iconObject.transform.localPosition = Vector3.up * heightAboveObject;
        startPosition = iconObject.transform.localPosition;
        
        spriteRenderer = iconObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = iconSprite;
        
        iconObject.transform.localScale = Vector3.one * iconSize;
    
        spriteRenderer.sortingOrder = 100;
    }
    
    private void Update()
    {
        if (iconObject == null) return;
        
        if (rotateTowardsCamera && mainCamera != null)
        {
            iconObject.transform.rotation = Quaternion.LookRotation(
                iconObject.transform.position - mainCamera.transform.position
            );
        }
        
        if (enableBobbing)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
            iconObject.transform.localPosition = new Vector3(
                startPosition.x, 
                newY, 
                startPosition.z
            );
        }
    }
    
    public void SetIcon(Sprite newSprite)
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = newSprite;
    }
    
    public void SetVisibility(bool visible)
    {
        if (iconObject != null)
            iconObject.SetActive(visible);
    }
    
    public void SetColor(Color color)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = color;
    }
    
    private void OnDestroy()
    {
        if (iconObject != null)
            Destroy(iconObject);
    }
}
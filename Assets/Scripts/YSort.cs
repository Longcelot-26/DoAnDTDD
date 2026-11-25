using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YSort : MonoBehaviour
{
    [SerializeField] private float yOffset = 0f;
    [SerializeField] private bool isStatic = true;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = "Default";
        }
        UpdateSorting();
    }

    void LateUpdate()
    {
        if (!isStatic)
        {
            UpdateSorting();
        }
    }

    public void UpdateSorting()
    {
        if (sr != null)
        {
            sr.sortingOrder = -(int)((transform.position.y + yOffset) * 100);
        }
    }
}

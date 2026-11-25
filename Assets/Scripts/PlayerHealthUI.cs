using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Image healthFill;
    [SerializeField] private Image easeFill;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private RectTransform containerRect;
    [SerializeField] private float easeDelay = 0.4f;
    [SerializeField] private float easeSpeed = 3f;
    [SerializeField] private Color fullHealthColor = new Color(0f, 0.9f, 0.95f, 1f);
    [SerializeField] private Color midHealthColor = new Color(0.95f, 0.75f, 0.1f, 1f);
    [SerializeField] private Color lowHealthColor = new Color(0.95f, 0.2f, 0.2f, 1f);

    private Player player;
    private float targetFill = 1f;
    private float easeTimer = 0f;
    private Vector3 originalScale = Vector3.one;

    private void Awake()
    {
        Canvas rootCanvas = GetComponentInParent<Canvas>();
        if (rootCanvas != null)
        {
            rootCanvas.sortingLayerName = "UI";
            if (rootCanvas.sortingOrder < 10)
            {
                rootCanvas.sortingOrder = 10;
            }
            if (rootCanvas.renderMode == RenderMode.ScreenSpaceCamera && rootCanvas.planeDistance > 10f)
            {
                rootCanvas.planeDistance = 5f;
            }
        }
        if (containerRect == null)
        {
            containerRect = GetComponent<RectTransform>();
        }
        if (containerRect != null)
        {
            originalScale = containerRect.localScale;
        }
    }

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        if (player != null)
        {
            SetHealthInstant(player.CurrentHp, player.MaxHp);
        }
    }

    private void Update()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
            if (player != null)
            {
                SetHealthInstant(player.CurrentHp, player.MaxHp);
            }
        }

        if (easeTimer > 0f)
        {
            easeTimer -= Time.deltaTime;
        }
        else if (easeFill != null && easeFill.fillAmount > targetFill)
        {
            easeFill.fillAmount = Mathf.Lerp(easeFill.fillAmount, targetFill, Time.deltaTime * easeSpeed);
            if (Mathf.Abs(easeFill.fillAmount - targetFill) < 0.002f)
            {
                easeFill.fillAmount = targetFill;
            }
        }

        if (healthFill != null)
        {
            if (healthFill.fillAmount < targetFill)
            {
                healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, targetFill, Time.deltaTime * easeSpeed * 2f);
            }
            healthFill.color = EvaluateColor(healthFill.fillAmount);
        }

        if (containerRect != null)
        {
            if (targetFill < 0.25f && targetFill > 0f)
            {
                float pulse = 1f + Mathf.Sin(Time.time * 8f) * 0.035f;
                containerRect.localScale = originalScale * pulse;
            }
            else
            {
                containerRect.localScale = Vector3.Lerp(containerRect.localScale, originalScale, Time.deltaTime * 10f);
            }
        }
    }

    public void UpdateHealth(float current, float max)
    {
        if (max <= 0f) return;
        float prevFill = targetFill;
        targetFill = Mathf.Clamp01(current / max);

        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
        }

        if (targetFill < prevFill)
        {
            if (healthFill != null)
            {
                healthFill.fillAmount = targetFill;
            }
            easeTimer = easeDelay;
            if (containerRect != null)
            {
                containerRect.localScale = originalScale * 1.06f;
            }
        }
        else
        {
            if (easeFill != null)
            {
                easeFill.fillAmount = targetFill;
            }
        }
    }

    public void SetHealthInstant(float current, float max)
    {
        if (max <= 0f) return;
        targetFill = Mathf.Clamp01(current / max);
        if (healthFill != null)
        {
            healthFill.fillAmount = targetFill;
            healthFill.color = EvaluateColor(targetFill);
        }
        if (easeFill != null)
        {
            easeFill.fillAmount = targetFill;
        }
        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
        }
    }

    private Color EvaluateColor(float fill)
    {
        if (fill > 0.5f)
        {
            return Color.Lerp(midHealthColor, fullHealthColor, (fill - 0.5f) * 2f);
        }
        return Color.Lerp(lowHealthColor, midHealthColor, fill * 2f);
    }
}

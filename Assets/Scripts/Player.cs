using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField] private float maxHp = 100f;
    private float currentHp;
    [SerializeField] private Image hpBar;
    [SerializeField] private PlayerHealthUI playerHealthUI;
    [SerializeField] private GameManager GameManager;
    [SerializeField] private AudioManager audioManager;
    public Joystick joystick;

    public float CurrentHp => currentHp;
    public float MaxHp => maxHp;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = "Default";
        }
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        currentHp = maxHp;
        if (playerHealthUI == null)
        {
            playerHealthUI = FindAnyObjectByType<PlayerHealthUI>();
        }
        UpdateHpBar();
    }

    void Update()
    {
        MovePlayer();
    }

    void LateUpdate()
    {
        if (sr != null)
        {
            sr.sortingOrder = -(int)(transform.position.y * 100);
        }
    }

    void MovePlayer()
    {
        Vector2 playerInput = Vector2.zero;
        if (joystick != null && (Mathf.Abs(joystick.Horizontal) > 0.05f || Mathf.Abs(joystick.Vertical) > 0.05f))
        {
            playerInput = new Vector2(joystick.Horizontal, joystick.Vertical);
        }
        else
        {
            playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        rb.linearVelocity = playerInput.normalized * moveSpeed;
        if (playerInput.x < 0)
        {
            sr.flipX = true;
        }
        else if (playerInput.x > 0)
        {
            sr.flipX = false;
        }
        if (playerInput != Vector2.zero)
        {
            anim.SetBool("isRun", true);
        }
        else
        {
            anim.SetBool("isRun", false);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);
        UpdateHpBar();
        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void UpdateHpBar()
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = currentHp / maxHp;
        }
        if (playerHealthUI != null)
        {
            playerHealthUI.UpdateHealth(currentHp, maxHp);
        }
    }

    private void Die()
    {
        GameManager.GameOverMenu();
        audioManager.PlayGameOverSound();
    }
}

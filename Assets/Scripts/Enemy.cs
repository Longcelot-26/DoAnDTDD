using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy: MonoBehaviour
{
    [SerializeField] protected float enemyMoveSpeed=1f;
    protected Player player;
    [SerializeField] protected float maxHp=50f;
    protected float currentHp;
    [SerializeField] private Image hpBar;
    [SerializeField] protected float damageToPlayer=10f;
    [SerializeField] protected float staydamge=1f;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;

    protected virtual void Start()
    {
        player = FindAnyObjectByType<Player>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = "Default";
        }
        currentHp = maxHp;
        UpdateHpBar();
    }
    protected virtual void FixedUpdate()
    {
        MoveToPlayer();
    }
    protected virtual void LateUpdate()
    {
        if (sr != null)
        {
            sr.sortingOrder = -(int)(transform.position.y * 100);
        }
        if (player != null && Vector2.Distance(transform.position, player.transform.position) > 50f)
        {
            Destroy(gameObject);
        }
    }
    protected void MoveToPlayer()
    {
        if (player != null)
        {
            if (rb != null)
            {
                Vector2 dir = ((Vector2)player.transform.position - (Vector2)transform.position).normalized;
                rb.linearVelocity = dir * enemyMoveSpeed;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemyMoveSpeed * Time.fixedDeltaTime);
            }
            FlipEnemy();
        }
        else if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    protected void FlipEnemy()
    {
        if (player != null)
        {
            transform .localScale = new Vector3(player.transform.position.x < transform.position.x ? -1 : 1, 1, 1);
        }
    }
    public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);
        UpdateHpBar();
        if (currentHp <= 0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        Destroy(gameObject);
    }

protected void UpdateHpBar()
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = currentHp / maxHp;
        }
    }
}
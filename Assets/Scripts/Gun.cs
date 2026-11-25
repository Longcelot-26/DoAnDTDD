using UnityEngine;

public class Gun : MonoBehaviour
{
    private float rotateOffset = 180f;
    [SerializeField] private Transform firePos;
    [SerializeField] private GameObject bulletPrefabs;
    [SerializeField] private float shotDelay = 0.15f;
    private float nextShot;
    [SerializeField] private int maxAmmo = 24;
    [SerializeField] private AudioManager audioManager;
    public int currentAmmo;
    public Joystick joystick;

    void Start()
    {
     currentAmmo=maxAmmo;

    }


    void Update()
    {
        RotateGun();
        shoot();
    }
    void RotateGun()
    {
        Vector2 aimInput = joystick != null ? new Vector2(joystick.Horizontal, joystick.Vertical) : Vector2.zero;

        if (aimInput.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(-aimInput.y, -aimInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + rotateOffset);

            if (angle < -90 || angle > 90)
            {
                transform.localScale = new Vector3(2, 2, 2);
            }
            else
            {
                transform.localScale = new Vector3(2, -2, 2);
            }
        }
        else if (Camera.main != null)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldMouse = Camera.main.ScreenToWorldPoint(mousePos);
            Vector3 diff = transform.position - worldMouse;
            float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + rotateOffset);

            if (angle < -90 || angle > 90)
            {
                transform.localScale = new Vector3(2, 2, 2);
            }
            else
            {
                transform.localScale = new Vector3(2, -2, 2);
            }
        }
    }

    void shoot()
    {
        Vector2 aimDirection = joystick != null ? new Vector2(joystick.Horizontal, joystick.Vertical) : Vector2.zero;
        bool joystickActive = aimDirection.magnitude >= 0.1f;
        bool mouseShoot = Input.GetMouseButton(0);

        if ((joystickActive || mouseShoot) && currentAmmo > 0 && Time.time > nextShot)
        {
            nextShot = Time.time + shotDelay;
            Instantiate(bulletPrefabs, firePos.position, firePos.rotation);
            audioManager.PlayShootSound();
        }
    }
}
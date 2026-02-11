using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;

public class PlayerController : SpaceshipController
{
    [Header("Camera Settings")]
    [SerializeField] private float maxcamFOV = 100f;
    [SerializeField] private float avrageVelocityDivisor = 160f;
    [SerializeField] private float adjustSpeed = 10f;
    [SerializeField] private Vector3 cameraOffset;

    private float initialCameraFOV;
    private Quaternion initialCameraRotation;
    private Quaternion smoothedRotation;

    [Header("Aim Settings")]
    [SerializeField] private Image crosshair;
    [SerializeField] private LayerMask aimLayer;

    //[Header("Rendering Settings")]
    private Volume volume;
    private Vignette vignette;

    [Header("Bullet Settings")]
    [SerializeField] private float maxContinousShootingTime = 10f;
    [SerializeField] private AudioSource shootingSound;

    private bool isOverheated = false;
    private float accumilatedShootingTime = 0f;

    public float CurrentOverheat => accumilatedShootingTime / maxContinousShootingTime;

    private AudioSource audioSource;

    [Header("Shop Settings")]
    [SerializeField] private float shopInteractionDistance = 15f;
    [SerializeField] private GameObject interactText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private LayerMask interactableLayer;

    private RocketShop shop;
    private Vector3 shopPosition;

    private int money = 0;
    public int CurrentMoney => money;

    public float Throttle => throttle;
    private float throttle = 0;

    public enum PlayerState
    {
        Flying,
        Shopping
    }

    private PlayerState state = PlayerState.Flying;
    public PlayerState State => state;

    private static PlayerController instance;
    public static PlayerController Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<PlayerController>();
            return instance;
        }
    }

    protected override void Start()
    {
        // Call the base Start method
        base.Start();

        // Load the difficulty data
        ReadDifficultyData(LoadDifficultyData(GameManager.GameDifficulty));

        // Set the initial camera field of view and rotation
        initialCameraFOV = Camera.main.fieldOfView;
        initialCameraRotation = Camera.main.transform.rotation;

        // Get the vignette from the volume
        volume = FindFirstObjectByType<Volume>();
        volume.profile.TryGet<Vignette>(out vignette);

        audioSource = GetComponent<AudioSource>();

        bullet = Resources.Load<GameObject>("playerBullet");

        shop = FindObjectOfType<RocketShop>();
        shopPosition = shop.transform.position;
    }

    protected override void Update()
    {
        CheckShopInteraction();
        Move();
        OverheatCheck();
        TestMouseOverlap();
        base.Update();
    }

    private void FixedUpdate()
    {
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        Camera.main.fieldOfView = Mathf.SmoothStep(initialCameraFOV, maxcamFOV, AvrageVelocity / avrageVelocityDivisor);
        smoothedRotation = Quaternion.Slerp(smoothedRotation, transform.rotation, Time.fixedDeltaTime * adjustSpeed);
        Camera.main.transform.SetPositionAndRotation(transform.position + smoothedRotation * cameraOffset, smoothedRotation * initialCameraRotation);
    }

    private void Move()
    {
        throttle = Mathf.Clamp01(Input.GetKey(KeyCode.LeftShift) ? throttle + Time.deltaTime : Input.GetKey(KeyCode.LeftControl) ? throttle - Time.deltaTime : throttle);
        ApplyForwardForce(throttle);
        ApplyHorizontalTorqe(Input.GetAxis("Horizontal"));
        ApplyVerticalTorqe(Input.GetAxis("Vertical"));
        ApplyRollTorqe(Input.GetAxis("Roll"));
        audioSource.pitch = Mathf.Lerp(1f, 2f, AvrageVelocity / avrageVelocityDivisor);
    }

    private void OverheatCheck()
    {
        if (isShooting && !isOverheated)
        {
            accumilatedShootingTime += Time.deltaTime;
            if (accumilatedShootingTime >= maxContinousShootingTime) isOverheated = true;
        }
        else if (accumilatedShootingTime > 0)
        {
            accumilatedShootingTime -= Time.deltaTime * 2;
            if (accumilatedShootingTime <= 0) isOverheated = false;
        }
    }

    protected override bool Shoot(Vector3 target)
    {
        bool result = base.Shoot(target);
        if (result) shootingSound.Play();
        return result;
    }

    private void TestMouseOverlap()
    {
        crosshair.transform.position = Input.mousePosition;
        Ray aim = Camera.main.ScreenPointToRay(Input.mousePosition);
        crosshair.color = Physics.Raycast(aim, out RaycastHit hit, maxProjectileDistance, aimLayer.value) ? crosshair.color = Color.red : Color.white;
        if (Input.GetMouseButton(0)) Shoot(hit.collider != null ? hit.collider.gameObject.transform.position : aim.GetPoint(maxProjectileDistance));
        else isShooting = false;
    }

    private void CheckShopInteraction()
    {
        float dist = Vector3.Distance(transform.position, shopPosition);
        bool isCloseEnough = dist <= shopInteractionDistance;
        interactText.SetActive(isCloseEnough);
        distanceText.text = (int)dist + "m";
        if (!isCloseEnough) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            state = PlayerState.Shopping;
            shop.Interact();
        }

        //Dynamic shop interaction - if there are multiple interactable objects
        //RaycastHit[] interactable = new RaycastHit[1];
        //int hits = Physics.SphereCastNonAlloc(transform.position, shopInteractionDistance, Vector3.zero, interactable, 0f, interactableLayer.value);
        //if (hits == 0)
        //{
        //    interactText.SetActive(false);
        //    return;
        //}
        //interactText.SetActive(true);
        //distanceText.text = Vector3.Distance(transform.position, interactable[0].transform.position) + "m";
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    state = PlayerState.Shopping;
        //    interactable[0].collider.GetComponent<IInteractable>().Interact();
        //}
    }

    public void ApplyUpgrade(RocketShop.UpgradeType type, float value)
    {
        switch (type)
        {
            case RocketShop.UpgradeType.Damage:
                damage = value;
                break;
            case RocketShop.UpgradeType.Cooldown:
                shootCooldown = value;
                break;
            case RocketShop.UpgradeType.Overheat:
                maxContinousShootingTime = value;
                break;
            case RocketShop.UpgradeType.MaxHealth:
                maxHealth = (int)value;
                break;
        }
        Debug.Log($"upg - dmg: {damage}, cld: {shootCooldown}, hlt: {maxHealth}");
    }

    public void AddMoney(int value) => money += value;

    public override void Heal(int amount)
    {
        base.Heal(amount);
        vignette.intensity.value = Mathf.Lerp(0, 1, 1 - (float)health / maxHealth);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        vignette.intensity.value = Mathf.Lerp(0, 1, 1 - (float)health / maxHealth);
    }

    public override void Die()
    {
        GameManager.Instance.GameOver();
    }

    protected override bool AllowShooting()
    {
        if (isOverheated) return false;
        return base.AllowShooting();
    }
}

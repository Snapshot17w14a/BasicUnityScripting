using System.Collections.Generic;
using UnityEngine;

public abstract class SpaceshipController : MonoBehaviour, IDamagable, IPausable
{
    [Header("Movement Settings")]
    [SerializeField] protected float forwardSpeed = 10f;
    [SerializeField] protected float backwardMultiplier = 0.5f;

    [Header("Torque Settings")]
    [SerializeField] protected float horizontalTorqe = 10f;
    [SerializeField] protected float verticalTorqe = 10f;
    [SerializeField] protected float rollTorqe = 10f;

    [Header("Velocity Settings")]
    [SerializeField] protected int avgVelocityBufferSize = 10;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem exhaustParticles;
    private readonly ParticleSystem[] gunParticles = new ParticleSystem[2];
    private static GameObject explosionParticles;

    [Header("Gun Settings")]
    [SerializeField] protected float shootCooldown = 0.1f;
    [SerializeField] protected float bulletSpeed = 100f;
    [SerializeField] protected float maxProjectileDistance = 1000f;

    private readonly GameObject[] bulletOrigins = new GameObject[2];
    protected float lastShootTime = -2f;
    protected bool isShooting = false;
    protected float damage = 0f;

    private readonly List<float> velocityBuffer = new();

    protected Rigidbody rb;
    protected GameObject bullet;

    protected int health;
    protected int maxHealth;
    public int Health => health;
    public int MaxHealth => maxHealth;

    private bool isPaused= false;
    public bool IsPaused => isPaused;

    protected float VelocityBufferSum
    {
        get
        {
            float sum = 0;
            foreach (var velocity in velocityBuffer) sum += velocity;
            return sum;
        }
    }

    protected float AvrageVelocity
    {
        get
        {
            if (velocityBuffer.Count == 0) return 0f;
            else return VelocityBufferSum / velocityBuffer.Count;
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (explosionParticles == null) explosionParticles = Resources.Load<GameObject>("Explosion");
        bulletOrigins[0] = transform.Find("Bullet Origins").GetChild(0).gameObject;
        bulletOrigins[1] = transform.Find("Bullet Origins").GetChild(1).gameObject;
        gunParticles[0] = transform.Find("Gun particles").GetChild(0).gameObject.GetComponent<ParticleSystem>();
        gunParticles[1] = transform.Find("Gun particles").GetChild(1).gameObject.GetComponent<ParticleSystem>();
        PauseManager.OnPauseStateChanged += SetState;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateVelocityValues();
        UpdateExhaustValues();
    }

    private void UpdateExhaustValues()
    {
        var emmission = exhaustParticles.emission;
        emmission.rateOverTime = Mathf.Clamp(AvrageVelocity * 2f, 10, 50);
    }

    protected void ApplyForwardForce(float muliplier)
    {
        rb.AddForce((muliplier * forwardSpeed * transform.forward) * Time.deltaTime);
    }

    protected void ApplyHorizontalTorqe(float multiplier)
    {
        rb.AddTorque((multiplier * horizontalTorqe * transform.up) * Time.deltaTime);
    }

    protected void ApplyVerticalTorqe(float multiplier)
    {
        rb.AddTorque((multiplier * verticalTorqe * transform.right) * Time.deltaTime);
    }

    protected void ApplyRollTorqe(float multiplier)
    {
        rb.AddTorque((multiplier * rollTorqe * transform.forward) * Time.deltaTime);
    }

    private void UpdateVelocityValues()
    {
        velocityBuffer.Add(rb.velocity.magnitude);
        if (velocityBuffer.Count > avgVelocityBufferSize) velocityBuffer.RemoveAt(0);
    }

    protected void PlayShotParticles(Vector3 targetPos)
    {
        foreach (var gunParticle in gunParticles)
        {
            gunParticle.transform.LookAt(targetPos);
            gunParticle.Play();
        }
    }

    protected virtual bool AllowShooting()
    {
        bool isCooldownOver = lastShootTime + shootCooldown <= Time.time;
        return isCooldownOver;
    }

    protected virtual bool Shoot(Vector3 target)
    {
        if (!AllowShooting()) return false;
        isShooting = true;
        lastShootTime = Time.time;
        foreach (var bulletOrigin in bulletOrigins)
        {
            var instantiatedBullet = Instantiate(bullet, bulletOrigin.transform.position, Quaternion.identity, GameObject.Find("Bullets").transform).GetComponent<Bullet>();
            instantiatedBullet.damage = damage;
            instantiatedBullet.GetComponent<Rigidbody>().AddForce((target - transform.position).normalized * bulletSpeed, ForceMode.Impulse);
            instantiatedBullet.expirationTime = maxProjectileDistance / bulletSpeed;
        }
        PlayShotParticles(target);
        return true;
    }

    public virtual void TakeDamage(float damage)
    {
        Debug.Log(this.GetType() + " took " + damage + " damage.");
        health -= (int)Mathf.Round(damage);
        if (health <= 0) Die();
    }

    public virtual void Heal(int amount)
    {
        health = Mathf.Clamp(health + System.Math.Abs(amount), 0, maxHealth);
    }

    public virtual void Die()
    {
        Debug.Log(this.GetType() + " died.");
        Destroy(gameObject);
        var tempGameObject = Instantiate(explosionParticles, transform.position, Quaternion.identity, GameObject.Find("Explosions").transform);
        var particleSystem = tempGameObject.GetComponent<ParticleSystem>();
        particleSystem.Play();
        Destroy(tempGameObject, particleSystem.main.duration);
    }

    protected DifficultyData LoadDifficultyData(DifficultyData.Difficulty difficulty)
    {
        return Resources.Load<DifficultyData>(DifficultyData.GetDifficultyPath(difficulty));
    }

    protected virtual void ReadDifficultyData(DifficultyData difficultyData)
    {
        health = difficultyData.startingHealth;
        maxHealth = difficultyData.maxHealth;
        damage = difficultyData.initialDamage;
        shootCooldown = difficultyData.initialCooldown;
    }

    public void SetState(bool state)
    {
        isPaused = state;
        if (isPaused) exhaustParticles.Pause();
        else exhaustParticles.Play();
    }

    private void OnDestroy()
    {
        PauseManager.OnPauseStateChanged -= SetState;
    }
}

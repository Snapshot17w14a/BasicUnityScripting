using UnityEngine;

public class Bullet : MonoBehaviour, IPausable
{
    public float damage = 1f;
    public float expirationTime = 5f;
    public float timeAlive = 0f;

    public bool IsPaused => isPaused;
    private bool isPaused = false;

    public void SetState(bool state) => isPaused = state;

    private void Start()
    {
        PauseManager.OnPauseStateChanged += SetState;
    }

    private void Update()
    {
        if ((timeAlive += Time.deltaTime) >= expirationTime) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamagable damagable)) damagable.TakeDamage(damage);
        Destroy(gameObject);
    }
}

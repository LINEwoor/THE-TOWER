using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    
    public Transform Transform => transform;
    public Vector3 Position => transform.position;
    public bool IsAlive => currentHealth > 0;
    public float CurrentHealth => currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        tag = "Dead";
        //Destroy(gameObject);
    }
}
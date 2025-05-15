

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int damage = 25;                    
    public float attackRange = 2.5f;           
    public float attackCooldown = 0.5f;      
    [Header("References")]
    //public Animator animator;                   
    public Transform attackOrigin;
    public ShopSystem shop;

    private float lastAttackTime;
    private PlayerInputActions inputActions;

    void Awake()
    {
        // Instantiate the generated InputActions
        inputActions = new PlayerInputActions();
        // Subscribe to the Attack action
        inputActions.Player.Attack.performed += ctx => HandleAttack();
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void HandleAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;

        // Play attack animation
        //animator?.SetTrigger("Attack");

        // Raycast for hit detection
        RaycastHit hit;
        Vector3 origin = attackOrigin != null ? attackOrigin.position : Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;

        if (Physics.Raycast(origin, direction, out hit, attackRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                    enemyHealth.TakeDamage(damage);

            }
            if (hit.collider.CompareTag("Shop"))
            {
                shop.OpenShop();
            }
        }
    }
}




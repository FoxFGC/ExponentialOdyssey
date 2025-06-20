

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
  
    public double damage = 25;                    
    public float attackRange = 2.5f;           
    public double attackCooldown = 2;      
                     
    public Transform attackOrigin;

    
    public Image crosshairImage;


    private float lastAttackTime;
    private PlayerInputActions inputActions;
    public ShopSystem shop1;
    public ShopSystem shop2;

    void Awake()
    {
   
        inputActions = new PlayerInputActions();

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


    void Update()
    {
        if (crosshairImage != null)
        {
            int upgradeAttackSpeed = shop1.upgrades[3].purchaseCount;
            float t = (Time.time - lastAttackTime) / ((float) attackCooldown / Mathf.Pow(2, upgradeAttackSpeed));
            
            crosshairImage.fillAmount = Mathf.Clamp01(t);
        }
    }


    void HandleAttack()
    {
        int upgradeAttackSpeed = shop1.upgrades[3].purchaseCount;
        if (Time.time < lastAttackTime + (attackCooldown / Mathf.Pow(2, upgradeAttackSpeed)))
            return;

        lastAttackTime = Time.time;
        
        
        RaycastHit hit;
        Vector3 origin = attackOrigin != null ? attackOrigin.position : Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;

        if (Physics.Raycast(origin, direction, out hit, attackRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                int attackUpgradesBought = shop1.upgrades[2].purchaseCount;
                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                    enemyHealth.TakeDamage(damage + (5 * attackUpgradesBought));

            }
            if (hit.collider.CompareTag("Shop1"))
            {
               shop1.OpenShop();
            }
            if (hit.collider.CompareTag("Shop2"))
            { 
                shop2.OpenShop();
            }

        }
    }
}




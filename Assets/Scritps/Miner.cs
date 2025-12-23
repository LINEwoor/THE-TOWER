using UnityEngine;

public class Miner : MonoBehaviour
{
    [SerializeField] private GameObject[] res;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private GameObject tower;
    [SerializeField] private bool isTower;
    [SerializeField] private ResoursesUppers ru;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private int radios = 5;
    [SerializeField] private Collider[] hitColliders;
    [SerializeField] private GameObject target;
    [SerializeField] private int damage = 20;
    [SerializeField] private float timeAttack = 1f;
    [SerializeField] private float resSearchInterval = 1f;
    [SerializeField] private float enemySearchInterval = 1f;
    private float resSearchTimer = 0f;
    private float enemySearchTimer = 0f;

    void Start()
    {
        ru = GetComponent<ResoursesUppers>();
        if (tower == null)
        {
            tower = GameObject.FindGameObjectWithTag("Player");
        }
    }

    void FoundRes()
    {
        res = GameObject.FindGameObjectsWithTag("Resourses");
    }
    
    void Move(Transform target)
    {
        if (target == null) return;
        
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
    
    public GameObject GetClosestObject()
    {
        if (res == null || res.Length == 0) return null;
        
        GameObject closest = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject obj in res)
        {
            if (obj == null || !obj.activeSelf) continue;
            
            Vector3 diff = obj.transform.position - currentPos;
            float curDistSqr = diff.sqrMagnitude;

            if (curDistSqr < minDist)
            {
                closest = obj;
                minDist = curDistSqr;
            }
        }
        return closest;
    }
    
    void MoveSystem()
    {
        if (ru == null) return;
        
        if (ru.IsCarryingResource())
        {
            if (tower != null)
                Move(tower.transform);
        }
        else
        {
            GameObject g = GetClosestObject();
            if (g != null)
                Move(g.transform);
        }
    }
    
    void Damage()
    {
        if (target != null)
        {
            Health health = target.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }
    
    void Attack()
    {
        if (target == null)
        {
            isAttacking = false;
            return;
        }

        float d = Vector3.Distance(this.transform.position, target.transform.position);
        if (d > 1)
        {
            Move(target.transform);
        }
        else
        {
            if (timeAttack > 0)
            {
                timeAttack -= Time.deltaTime;
            }
            else
            {
                Damage();
                timeAttack = 1f;
            }
        }
        
        if (target.tag == "Dead")
        {
            isAttacking = false;
            target = null;
        }
    }

    void FoundEnemy()
    {
        hitColliders = Physics.OverlapSphere(this.transform.position, radios);
        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.tag == "Enemy" && !isAttacking)
            {
                isAttacking = true;
                target = collider.gameObject;
                break;
            }
        }
    }
    
    void Update()
    {
        resSearchTimer -= Time.deltaTime;
        if (resSearchTimer <= 0)
        {
            FoundRes();
            resSearchTimer = resSearchInterval;
        }
        
        if (!isAttacking)
        {
            enemySearchTimer -= Time.deltaTime;
            if (enemySearchTimer <= 0)
            {
                FoundEnemy();
                enemySearchTimer = enemySearchInterval;
            }
            
            MoveSystem();
        }
        else
        {
            Attack();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Tower")
        {
            if (ru != null)
            {
                ru.GetCarriedResourceType();
            }
        }
    }
}
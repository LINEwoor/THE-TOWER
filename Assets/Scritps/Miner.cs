using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Miner : MonoBehaviour
{
    [SerializeField] private GameObject[] res;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [SerializeField] private GameObject tower;

    [SerializeField] private bool isTower;
    [SerializeField] private ResoursesUppers ru;

    [SerializeField] private bool isattsck = false;
    [SerializeField] private int radios = 5;

    [SerializeField] private Collider[] hitColliders;

    [SerializeField] private GameObject? target;
    [SerializeField] private int damage = 20;

    [SerializeField] private float TimeAttack = 1;
    void Start()
    {
        ru = GetComponent<ResoursesUppers>();
    }

    void FoundRes()
    {
        res = GameObject.FindGameObjectsWithTag("Resourses");
    }
    void Move(Transform target)
    {
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
        GameObject closest = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject obj in res)
        {
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
        if (ru.thisRes!= ResoursesUppers.Resourses.None)
        {
            Move(tower.transform);
        }else if (ru.thisRes == ResoursesUppers.Resourses.None)
        {
            GameObject g = GetClosestObject();
            if(g != null)
                Move(g.transform);
        }

    }
    void Damage()
    {
        target.GetComponent<Health>().TakeDamage(damage);
    }
    void Attack()
    {

        float d = Vector3.Distance(this.transform.position,target.transform.position);
        if (d > 1)
        {
            Move(target.transform);
        }
        else
        {
            if(TimeAttack > 0)
            {
                TimeAttack -= Time.deltaTime;
            }
            else
            {
                Damage();
                TimeAttack = 1f;
            }
        }
        
        if (target.tag == "Dead")
        {

            isattsck = false;
            target = null;
        }
    }

    void FoundEnemy()
    {
        hitColliders = Physics.OverlapSphere(this.transform.position, radios);
        foreach (Collider collider in hitColliders)
        {
            if(collider.gameObject.tag == "Enemy")
            {
                isattsck = true;
                target = collider.gameObject;
                break;
            }
        }
    }
    void Update()
    {
        InvokeRepeating(nameof(FoundRes), 0f, 1f);
        if (isattsck == false)
        {
            InvokeRepeating(nameof(FoundEnemy), 0f, 1f);
            MoveSystem();
        }
        else Attack();
        //Move(t.transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Tower")
        {
            ru.thisRes = ResoursesUppers.Resourses.None;
        }
        
        
    }

    
}

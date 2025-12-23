using Enemy.EnemyVariants;
using UnityEngine;

public class RangedAttackState : AttackState
{
    private GameObject projectilePrefab;
    private Transform firePoint;
    private float projectileSpeed;

    public RangedAttackState(IEnemy enemy, Transform target) : base(enemy, target)
    {
        RangedEnemy rangedEnemy = enemy.GameObject.GetComponent<RangedEnemy>();
        if (rangedEnemy != null)
        {
            projectilePrefab = rangedEnemy.ProjectilePrefab;
            firePoint = rangedEnemy.FirePoint;
            projectileSpeed = rangedEnemy.ProjectileSpeed;
            
            if (firePoint == null)
            {
                firePoint = enemy.Transform;
            }
        }
    }

    public override void Enter()
    {
        base.Enter();
    }

    protected override void Attack()
    {
        if (projectilePrefab == null || firePoint == null || target == null)
        {
            enemy.DetectTargets();
            return;
        }

        GameObject projectile = GameObject.Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(target.position - firePoint.position)
        );
        
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null && target != null)
        {
            projectileScript.Initialize(target, enemy.Stats.Damage, projectileSpeed);
        }
    }
}

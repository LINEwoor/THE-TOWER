using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Miner : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float stoppingDistance = 1.5f;

    [Header("Combat")]
    [SerializeField] private int damage = 20;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackRange = 2f;

    [Header("Behavior")]
    [SerializeField] private float searchInterval = 1f;
    [SerializeField] private float wanderRadius = 8f;
    [SerializeField] private float wanderInterval = 4f;
    [SerializeField] private float restTime = 2f;
    [SerializeField] private float resourceApproachDistance = 1.8f;

    [Header("Navigation")]
    [SerializeField] private float stuckCheckInterval = 0.5f;
    [SerializeField] private float stuckDistanceThreshold = 0.5f;
    [SerializeField] private float maxPathRecalculateTime = 2f;
    [SerializeField] private float repathDelay = 0.5f;

    [Header("References")]
    [SerializeField] private ResoursesUppers resourceCarrier;

    // Core components
    private Animator animator;
    private NavMeshAgent agent;
    private GameObject towerBase;

    // State
    private enum MinerState { Idle, MovingToResource, CarryingToBase, Attacking, Resting, Waiting }
    private MinerState currentState = MinerState.Idle;

    // Targets
    private GameObject targetResource;
    private GameObject targetEnemy;

    // Timers
    private float searchTimer;
    private float attackTimer;
    private float wanderTimer;
    private float restTimer;
    private float waitTimer;
    private float stuckTimer;
    private float pathTimer;

    // Navigation
    private Vector3 lastPosition;
    private float lastPositionChangeTime;
    private bool isPathInvalid;
    private Vector3 currentDestination;

    // Resource reservation system
    private static Dictionary<GameObject, GameObject> reservedResources = new Dictionary<GameObject, GameObject>();

    // Cached transforms
    private Transform cachedTransform;
    private Rigidbody rb;

    // Resources array
    private GameObject[] allResources;

    void Awake()
    {
        cachedTransform = transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        resourceCarrier = GetComponent<ResoursesUppers>();
        rb = GetComponent<Rigidbody>();

        towerBase = GameObject.FindGameObjectWithTag("Player");

        // Настраиваем коллайдер как триггер
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<CapsuleCollider>();
            ((CapsuleCollider)col).height = 2f;
            ((CapsuleCollider)col).radius = 0.5f;
        }
        col.isTrigger = true;
    }

    void Start()
    {
        ConfigureNavMeshAgent();
        InitializeTimers();
        currentState = MinerState.Idle;
        lastPosition = cachedTransform.position;
        lastPositionChangeTime = Time.time;
    }

    void ConfigureNavMeshAgent()
    {
        if (agent == null) return;

        agent.speed = moveSpeed * Random.Range(0.9f, 1.1f);
        agent.angularSpeed = rotationSpeed * 100f;
        agent.acceleration = 12f;
        agent.stoppingDistance = stoppingDistance;
        agent.autoBraking = true;
        agent.autoRepath = true;
        agent.radius = 0.4f;
        agent.height = 1.8f;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.avoidancePriority = Random.Range(30, 100);
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.updateUpAxis = true;
        agent.enabled = true;
    }

    void InitializeTimers()
    {
        wanderTimer = Random.Range(wanderInterval * 0.5f, wanderInterval * 1.5f);
        searchTimer = Random.Range(0f, searchInterval);
        attackTimer = attackCooldown;
        restTimer = restTime;
        stuckTimer = stuckCheckInterval;
        pathTimer = 0f;
    }

    void FindResources()
    {
        allResources = GameObject.FindGameObjectsWithTag("Resources");
    }

    void Update()
    {
        if (!IsAgentValid()) return;

        UpdateTimers();
        CheckIfStuck();

        if (searchTimer <= 0)
        {
            FindResources();
            searchTimer = searchInterval * Random.Range(0.8f, 1.2f);
        }

        switch (currentState)
        {
            case MinerState.Idle:
                UpdateIdle();
                break;
            case MinerState.MovingToResource:
                UpdateMovingToResource();
                break;
            case MinerState.CarryingToBase:
                UpdateCarryingToBase();
                break;
            case MinerState.Resting:
                UpdateResting();
                break;
            case MinerState.Waiting:
                UpdateWaiting();
                break;
        }

        UpdateAnimations();
        CleanupReservations();
    }

    bool IsAgentValid()
    {
        if (agent == null) return false;

        if (!agent.enabled)
        {
            agent.enabled = true;
        }

        if (!agent.isOnNavMesh)
        {
            return RestoreAgentToNavMesh();
        }

        return true;
    }

    bool RestoreAgentToNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(cachedTransform.position, out hit, 10f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            return true;
        }
        return false;
    }

    void UpdateTimers()
    {
        searchTimer -= Time.deltaTime;
        attackTimer -= Time.deltaTime;
        wanderTimer -= Time.deltaTime;
        restTimer -= Time.deltaTime;
        waitTimer -= Time.deltaTime;
        stuckTimer -= Time.deltaTime;
        pathTimer -= Time.deltaTime;
    }

    void CheckIfStuck()
    {
        if (stuckTimer <= 0 && agent.hasPath && agent.velocity.magnitude < 0.1f)
        {
            float distanceMoved = Vector3.Distance(lastPosition, cachedTransform.position);

            if (distanceMoved < stuckDistanceThreshold)
            {
                HandleStuck();
            }

            lastPosition = cachedTransform.position;
            lastPositionChangeTime = Time.time;
            stuckTimer = stuckCheckInterval;
        }
    }

    void HandleStuck()
    {
        if (currentState == MinerState.MovingToResource || currentState == MinerState.CarryingToBase)
        {
            if (rb != null)
            {
                rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            }

            if (agent.hasPath)
            {
                agent.ResetPath();
            }

            pathTimer = repathDelay;

            if (currentState == MinerState.MovingToResource && targetResource != null)
            {
                if (!IsPathToTargetValid(targetResource.transform.position))
                {
                    ReleaseReservation();
                    targetResource = null;
                    currentState = MinerState.Idle;
                }
            }
        }
    }

    bool IsPathToTargetValid(Vector3 target)
    {
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(target, path))
        {
            return path.status == NavMeshPathStatus.PathComplete;
        }
        return false;
    }

    #region States

    void UpdateIdle()
    {
        targetResource = null;

        if (resourceCarrier != null && resourceCarrier.IsCarryingResource())
        {
            currentState = MinerState.CarryingToBase;
            return;
        }

        GameObject resource = GetAvailableResource();
        if (resource != null)
        {
            if (IsPathToTargetValid(resource.transform.position))
            {
                targetResource = resource;
                ReserveResource(resource);
                currentState = MinerState.MovingToResource;
                return;
            }
        }

        if (wanderTimer <= 0 && !agent.hasPath)
        {
            Wander();
            wanderTimer = wanderInterval * Random.Range(0.7f, 1.3f);
        }
    }

    void UpdateMovingToResource()
    {
        if (resourceCarrier != null && resourceCarrier.IsCarryingResource())
        {
            ReleaseReservation();
            targetResource = null;
            currentState = MinerState.CarryingToBase;
            return;
        }

        if (targetResource == null || !targetResource.activeInHierarchy)
        {
            ReleaseReservation();
            targetResource = null;
            currentState = MinerState.Idle;
            return;
        }

        float distanceToResource = Vector3.Distance(cachedTransform.position, targetResource.transform.position);

        if (distanceToResource <= resourceApproachDistance + 0.3f)
        {
            StopMoving();
            CollectResourceDirectly();
            return;
        }

        if (HasReachedDestination())
        {
            if (distanceToResource <= resourceApproachDistance)
            {
                StopMoving();
                CollectResourceDirectly();
            }
            else
            {
                MoveToResource();
            }
        }
        else if (pathTimer <= 0 || isPathInvalid)
        {
            MoveToResource();
        }
    }

    void CollectResourceDirectly()
    {
        if (targetResource == null || !targetResource.activeInHierarchy) return;

        Res resource = targetResource.GetComponent<Res>();
        if (resource != null && resourceCarrier != null)
        {
            resourceCarrier.CollectResource(targetResource);

            ReleaseReservation();
            targetResource = null;
            currentState = MinerState.CarryingToBase;
        }
        else
        {
            currentState = MinerState.Idle;
        }
    }

    void MoveToResource()
    {
        if (targetResource != null)
        {
            MoveToPosition(targetResource.transform.position);
            pathTimer = maxPathRecalculateTime;
            isPathInvalid = false;
        }
    }

    void UpdateCarryingToBase()
    {
        if (resourceCarrier == null || !resourceCarrier.IsCarryingResource())
        {
            currentState = MinerState.Idle;
            return;
        }

        if (towerBase == null)
        {
            towerBase = GameObject.FindGameObjectWithTag("Player");
            if (towerBase == null) return;
        }

        float distanceToTower = Vector3.Distance(cachedTransform.position, towerBase.transform.position);

        if (distanceToTower <= resourceApproachDistance + 1f)
        {
            StopMoving();

            if (resourceCarrier != null)
            {
                resourceCarrier.DeliverResources();
                resourceCarrier.ResetResource();

                currentState = MinerState.Resting;
                restTimer = restTime * Random.Range(0.8f, 1.2f);
            }
        }
        else if (pathTimer <= 0 || !agent.hasPath || (agent.hasPath && agent.velocity.magnitude < 0.1f))
        {
            MoveToTower();
        }
    }

    void MoveToTower()
    {
        if (towerBase != null)
        {
            MoveToPosition(towerBase.transform.position);
            pathTimer = maxPathRecalculateTime;
        }
    }

    void UpdateResting()
    {
        StopMoving();

        if (restTimer <= 0)
        {
            ReleaseReservation();
            targetResource = null;

            if (agent.hasPath)
            {
                agent.ResetPath();
            }

            FindResources();
            searchTimer = 0;

            currentState = MinerState.Idle;
        }
    }

    void UpdateWaiting()
    {
        StopMoving();

        if (waitTimer <= 0)
        {
            if (targetResource != null && targetResource.activeInHierarchy)
            {
                CollectResourceDirectly();
            }
            else
            {
                ReleaseReservation();
                targetResource = null;
                currentState = MinerState.Idle;
            }
        }
    }

    bool HasReachedDestination()
    {
        if (!agent.hasPath || agent.pathStatus == NavMeshPathStatus.PathInvalid)
            return false;

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance + 0.1f)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    #endregion

    #region Movement

    void MoveToPosition(Vector3 targetPosition)
    {
        if (!agent.isOnNavMesh)
        {
            RestoreAgentToNavMesh();
            return;
        }

        if (currentDestination != targetPosition)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPosition, out hit, 5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                currentDestination = targetPosition;
            }
        }
    }

    void StopMoving()
    {
        if (agent.isOnNavMesh && agent.hasPath)
        {
            agent.ResetPath();
        }
        currentDestination = Vector3.zero;
    }

    void Wander()
    {
        if (!agent.isOnNavMesh) return;

        for (int i = 0; i < 5; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
            Vector3 randomPoint = cachedTransform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                currentDestination = hit.position;
                break;
            }
        }
    }

    #endregion

    #region Resource Reservation

    GameObject GetAvailableResource()
    {
        if (allResources == null || allResources.Length == 0) return null;

        GameObject bestResource = null;
        float bestScore = float.MaxValue;
        Vector3 currentPos = cachedTransform.position;

        foreach (GameObject resource in allResources)
        {
            if (resource == null || !resource.activeInHierarchy) continue;
            if (IsResourceReserved(resource)) continue;
            if (!IsPathToTargetValid(resource.transform.position)) continue;

            float distance = Vector3.Distance(currentPos, resource.transform.position);
            float score = distance * Random.Range(0.9f, 1.1f);

            if (score < bestScore)
            {
                bestScore = score;
                bestResource = resource;
            }
        }

        return bestResource;
    }

    bool IsResourceReserved(GameObject resource)
    {
        if (reservedResources.ContainsKey(resource))
        {
            GameObject reservingMiner = reservedResources[resource];
            if (reservingMiner == null || !reservingMiner.activeInHierarchy)
            {
                reservedResources.Remove(resource);
                return false;
            }
            return reservingMiner != gameObject;
        }
        return false;
    }

    void ReserveResource(GameObject resource)
    {
        if (resource == null) return;
        ReleaseReservation();
        reservedResources[resource] = gameObject;
    }

    void ReleaseReservation()
    {
        List<GameObject> toRemove = new List<GameObject>();
        foreach (var pair in reservedResources)
        {
            if (pair.Value == gameObject)
            {
                toRemove.Add(pair.Key);
            }
        }
        foreach (GameObject key in toRemove)
        {
            reservedResources.Remove(key);
        }
    }

    void CleanupReservations()
    {
        List<GameObject> toRemove = new List<GameObject>();
        foreach (var pair in reservedResources)
        {
            if (pair.Key == null || !pair.Key.activeSelf ||
                pair.Value == null || !pair.Value.activeInHierarchy)
            {
                toRemove.Add(pair.Key);
            }
        }
        foreach (GameObject key in toRemove)
        {
            reservedResources.Remove(key);
        }
    }

    #endregion

    #region Animation

    void UpdateAnimations()
    {
        if (animator == null) return;

        bool isMoving = agent.velocity.magnitude > 0.1f && agent.hasPath;
        animator.SetBool("isRunning", isMoving);

        if (isMoving)
        {
            animator.SetFloat("SpeedMultiplier", Mathf.Lerp(0.8f, 1.2f, agent.velocity.magnitude / moveSpeed));
        }
    }

    #endregion

    #region Collision

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (currentState == MinerState.CarryingToBase && resourceCarrier != null)
            {
                resourceCarrier.DeliverResources();
                resourceCarrier.ResetResource();
                currentState = MinerState.Resting;
                restTimer = restTime * Random.Range(0.8f, 1.2f);
            }
        }

        if (collision.gameObject.GetComponent<NavMeshAgent>() != null)
        {
            Vector3 pushDirection = (cachedTransform.position - collision.transform.position).normalized;
            pushDirection.y = 0;

            if (rb != null)
            {
                rb.AddForce(pushDirection * 2f, ForceMode.Impulse);
            }
        }
    }

    #endregion

    #region Cleanup

    void OnDestroy()
    {
        ReleaseReservation();
    }

    void OnDisable()
    {
        ReleaseReservation();
    }

    #endregion
}
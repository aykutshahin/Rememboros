using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rememboros;


[RequireComponent(typeof(Collider2D))]
public class GY_Crow : EnemyRenderer2D
{

    public Collider2D AgentCollider { get { return enemyCollider; } }
    private Vector2 moveVector;
    public void Init()
    {
        //Timer
        m_Timer = GetComponent<Timer>();
        m_Timer.addTimer("Attack", 1f, m_Attack.enemyAttackCooldown);

        m_General.currentHealth = m_General.Health;

        m_General.enemyAnimator = GetComponent<Animator>();

        enemyState = EnemyState.Patroling;

        enemyIsFacingRight = true;

    }

    private void Awake()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<BoxCollider2D>();
        m_Motor = GetComponent<CharacterMotor2D>();
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        UpdateStates();
        SetTargetType();
    }
    private void FixedUpdate()
    {
        GetComponent<FlockManager>().FlockMove();
        UpdateTimers();
        SetEnemyState();
        ChangeTargetLocations();
    }

    public override Vector3 Move()
    {
        m_Pathfinding.path = GetComponent<Pathfinder>().path;
        Vector2 start = transform.position;

        if (m_Pathfinding.path.Count <= 0 || m_Pathfinding.path == null)
        {
            return Vector3.zero;
        }
        if (m_Pathfinding.path.Count > 0 || m_Pathfinding.path != null)
        {
            Vector3 targetPos = new Vector3(m_Pathfinding.path[0].Position.x + 0.5f, m_Pathfinding.path[0].Position.y + 0.5f);
            start = (targetPos - transform.position).normalized;
            start = start * new Vector3(m_Movement.Speed, m_Movement.Speed);
        }
        if (!IsChasing() && !IsPatrolling())
        {
            start = Vector3.zero;
        }
        return start;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, GetComponent<FlockManager>().neighborRadius);
    }
}
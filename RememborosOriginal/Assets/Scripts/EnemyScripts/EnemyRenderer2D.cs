using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rememboros;

public class EnemyRenderer2D : MonoBehaviour
{
    #region Enums

    public enum EnemyState
    { 
        Chasing,
        Idling,
        Patroling,
        inAttacking,
        onHitted,
        Dead
    }

    public enum EnemyType
    {
        Landed,
        Flying
    }

    #endregion

    #region SUB CLASSES
    [System.Serializable]
    public class General
    {
        public Player playerTarget = null;
        public float Health = 100f;
        public float currentHealth = 0f;
        public float gravity = 40f;
        public bool canFlip = true;
        public Animator enemyAnimator;
        public LayerMask enemyLayer;
        public LayerMask groundLayer;
        public LayerMask playerLayer;
    }

    [System.Serializable]
    public class MovementSettings
    {
        public float Speed = 2.5f;

    }

    [System.Serializable]
    public class PathfindingSettings
    {
        public List<Node> path = null;
    }

    [System.Serializable]
    public class WaypointSettings
    {
        [SerializeField] public Transform[] _wayPoints = null;
        public int _randomPoint = 0;
    }

    [System.Serializable]
    public class Attack
    {
       public float enemyAttackRange = 0;
       public int enemyAttackDamage = 0;
       public float enemyAttackCooldown = 0;
       public int noOfClicks = 0;
       public Transform enemyAttackPoint = null;
    }

    #region Classes and Enums Instances

    [SerializeField]
    public Attack m_Attack = new Attack();

    [SerializeField]
    public PathfindingSettings m_Pathfinding = new PathfindingSettings();

    [SerializeField]
    public WaypointSettings m_Waypoints = new WaypointSettings();

    [SerializeField]
    public MovementSettings m_Movement = new MovementSettings();

    [SerializeField]
    public General m_General = new General();

    protected CharacterMotor2D m_Motor;
    protected BaseInputDriver m_InputDriver;

    public Timer m_Timer;

    protected EnemyState enemyState;
    protected EnemyType enemyType;
    #endregion

    #endregion

    #region Action Delegates
    public event System.Action<int> OnFacingFlip = delegate { };

    public event System.Action OnAttack = delegate { };
    public event System.Action OnAttackEnd = delegate { };

    public event System.Action OnPatrol = delegate { };
    public event System.Action OnPatrolEnd = delegate { };

    public event System.Action OnChase = delegate { };
    public event System.Action OnChaseEnd = delegate { };

    public event System.Action OnIdle = delegate { };
    public event System.Action OnIdleEnd = delegate { };

    public event System.Action<EnemyState, EnemyState> OnMotorStateChanged = delegate { };
    #endregion

    #region Protected Variables
    protected int m_FacingDirection = 1;

    protected bool enemyIsFacingRight;

    ///Enemy Components
    protected SpriteRenderer enemySprite;
    protected Rigidbody2D enemyRigidbody;
    protected Collider2D enemyCollider;

    #endregion

    #region Public Variables
    public int FacingDirection
    {
        get { return m_FacingDirection; }
        private set
        {
            int oldFacing = m_FacingDirection;
            m_FacingDirection = value;

            if (m_FacingDirection != oldFacing)
            {
                OnFacingFlip(m_FacingDirection);
            }
        }
    }

    public float m_distanceFromPlayer
    {
        get
        {
            return Vector2.Distance(transform.position, m_General.playerTarget.transform.position);
        }
    }


    public bool IsFacingRight
    {
        get
        {
            return enemyIsFacingRight;
        }
    }
    #endregion

    #region Functions

    #region Public Functions

    ///<summary>
    /// enemyacter Damage Function 
    ///</summary>
    public virtual void GiveDamage()
    {
        Collider2D[] hitPlayer;
        // create a circle in enemyAttackPoint position which has a radius size is equal to enemyAttackRange and last parameter represents what kind of layer is touched
        hitPlayer = Physics2D.OverlapCircleAll(m_Attack.enemyAttackPoint.position, m_Attack.enemyAttackRange, m_General.playerLayer);
        foreach (Collider2D player in hitPlayer)
        {
            player.GetComponent<Player>().TakeDamage(m_Attack.enemyAttackDamage);
        }
    }

    ///<summary>
    ///enemyacter Taking Damage Function
    ///<param name="damage">Damage Value </param>
    ///</summary>
    public virtual void TakeDamage(float damage)
    {
        m_General.enemyAnimator.SetTrigger("Hit");
        ChangeState(EnemyState.onHitted);
        m_General.currentHealth -= damage;
        if (m_General.currentHealth <= 0)
        {
            ChangeState(EnemyState.Dead);
            m_General.enemyAnimator.SetTrigger("Die");
        }
    }

    ///<summary>
    /// enemyacter Die Function 
    ///</summary>
    public virtual void Die()
    {
        Destroy(gameObject);
    }


    public virtual void ResetVelocity()
    {
        enemyRigidbody.velocity = Vector2.zero;
    }
    #endregion

    #region Protected Functions
    ///<summary>
    /// enemyacter Move Function 
    ///</summary>
    public virtual Vector3 Move()
    {
        m_Pathfinding.path = GetComponent<Pathfinder>().path;
        Vector2 start = transform.position;

        if(m_Pathfinding.path.Count <= 0 || m_Pathfinding.path == null)
        {
            return Vector3.zero;
        }
        if (m_Pathfinding.path.Count > 0 || m_Pathfinding.path != null)
        {
           Vector3 targetPos = new Vector3(m_Pathfinding.path[0].Position.x + 1f * FacingDirection, m_Pathfinding.path[0].Position.y);
           start = (targetPos - transform.position).normalized;
           start = start * new Vector3(m_Movement.Speed, m_Movement.Speed);
        }
        if(!IsChasing() && !IsPatrolling())
        {
            start = Vector3.zero;
        }
        return start;
    }

    protected virtual void ChangeTargetLocations()
    {
        if (Vector2.Distance(transform.position, GetComponent<Pathfinder>().target) <= 1f)
        {
            m_Waypoints._randomPoint = Random.Range(0, m_Waypoints._wayPoints.Length);
        }
    }

    protected virtual void UpdateTimers()
    {
        m_Timer.DecreaseCurrentFrame();
    }

    protected virtual void UpdateStates()
    {
        if (m_General.canFlip)
        {
            FacingDirection = (int)Mathf.Sign((GetComponent<Pathfinder>().target - (Vector2)transform.position).x);
        }
    }

    ///<summary>
    /// Changing enemyacter state by detecting what kind of layer it touches 
    ///</summary>
    protected virtual void ChangeState(EnemyState state)
    {
        if (enemyState == state || IsDead())
        {
            return;
        }

        if (IsAttacking())
        {
            OnAttackEnd.Invoke();
        }

        if (IsPatrolling())
        {
            OnPatrolEnd.Invoke();
        }

        if (IsChasing())
        {
            OnChaseEnd.Invoke();
        }

        if (IsIdling())
        {
            OnIdleEnd.Invoke();
        }
        // set new state
        var prevState = enemyState;
        enemyState = state;

        if (!IsAttacking())
        {
            m_General.canFlip = true;
        }

        if (IsAttacking())
        {
            OnAttack.Invoke();
        }

        if (IsPatrolling())
        {
            OnPatrol.Invoke();
        }

        if (IsChasing())
        {
            OnChase.Invoke();
        }

        if (IsIdling())
        {
            OnIdle.Invoke();
        }

        OnMotorStateChanged.Invoke(prevState, enemyState);
    }

    ///<summary>
    /// Setting enemyacter state by checking which states are true 
    ///</summary>
    protected virtual void SetEnemyState()
    {
        if (m_General.playerTarget != null)
        {
            if (m_distanceFromPlayer <= 5f && m_distanceFromPlayer > 0.7f && !IsAttacking() && !IsHitted())
            {
                m_Timer.SetCooldownFrame("Attack");
                ChangeState(EnemyState.Chasing);
            }
            if (m_distanceFromPlayer <= 0.7f && !m_Timer.isOnCooldown("Attack") && !IsHitted())
            {
                m_General.canFlip = false;
                ChangeState(EnemyState.inAttacking);
            }
            if (m_distanceFromPlayer > 5f)
            {
                ChangeState(EnemyState.Patroling);
            }

        }
    }

    protected virtual void SetTargetType()
    {
        if (IsAttacking() || IsChasing())
        {
            GetComponent<Pathfinder>().target = m_General.playerTarget.transform.position;
        }
        else if(IsPatrolling())
        {
            GetComponent<Pathfinder>().target = new Vector2(m_InputDriver.Horizontal, m_InputDriver.Vertical);
        }
    }


    protected virtual void EndAttack()
    {
        m_Timer.ResetCooldownFrame("Attack");
        m_General.canFlip = true;
        ChangeState(m_distanceFromPlayer <= 5f ? EnemyState.Idling : EnemyState.Chasing);   
    }

    protected virtual void EndHit()
    {
        ChangeState(m_distanceFromPlayer <= 0.7f ? EnemyState.inAttacking : m_distanceFromPlayer <= 5f ? EnemyState.Chasing : EnemyState.Patroling);
    }
    protected virtual void KnockBack()
    {
        m_Motor.Move(new Vector2(-0.25f * FacingDirection, m_Motor.Velocity.y));
    }
    protected virtual void OnDrawGizmos()
    {
        if (m_Attack.enemyAttackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(m_Attack.enemyAttackPoint.position, m_Attack.enemyAttackRange);
    }

    #region Enemy States Checking Functions 
    public bool IsIdling()
    {
        return enemyState == EnemyState.Idling;
    }

    public bool IsAttacking()
    {
        return enemyState == EnemyState.inAttacking;
    }

    public bool IsHitted()
    {
        return enemyState == EnemyState.onHitted;
    }

    public bool IsDead()
    {
        return enemyState == EnemyState.Dead;
    }

    public bool IsPatrolling()
    {
        return enemyState == EnemyState.Patroling;
    }
    public bool IsChasing()
    {
        return enemyState == EnemyState.Chasing;
    }
    #endregion

    #endregion

    #endregion
}

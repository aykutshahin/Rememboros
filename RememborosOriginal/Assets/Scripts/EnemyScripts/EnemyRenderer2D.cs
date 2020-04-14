using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyRenderer2D : MonoBehaviour
{
    public enum EnemyState
    { 
        Chasing,
        Patroling,
        inAttacking,
        onHitted,
        onWaitingToAttack,
        Dead
    }

    public enum EnemyType
    {
        GY_Crow,
        Enemy
    }

    #region Protected Variables
    [SerializeField] protected Player player = null;
    protected bool isOnSight;
    public int noOfClicks;
    public bool canFlip;
    public List<Node> path;

    [SerializeField] float LineOfSightX = 0f;
    [SerializeField] protected Transform[] _wayPoints = null;
    protected int _randomPoint;
    protected Vector2 target;
    protected int directionValue;
    protected int counter;
    protected int intersectionValue;
    protected Vector3[] _enemyGridCoords;
    protected bool canAttack;

    [SerializeField] protected Transform LineOfSight = null;

    ///Enemy Components
    protected SpriteRenderer enemySprite;
    protected Rigidbody2D enemyRigidbody;
    protected Collider2D enemyCollider;

    ///Members of other classes
    protected Animator enemyAnimationController;
    protected EnemyState enemyState;
    protected EnemyType enemyType;
    public EnemyType _enemyType{get{ return enemyType; }}
    protected Timer enemyTimer;

    ///GENERAL
    [Header("General")]
    [Range(0, 7)]
    [SerializeField] [Tooltip("enemyacter Movement Speed")] protected float enemyMoveSpeed = 0;
    [SerializeField] [Tooltip("enemyacter Maximum Health")] protected float enemyMaxHealth = 0;
    [SerializeField] protected int _enemyChallengeWeight = 8;
    protected Vector3 coor_N, coor_NW, coor_NE, coor_S, coor_SW, coor_SE, coor_E, coor_W;
    protected Vector3 getGridCoordinates;

    [Space(5)]
    ///STATE CONTROLS
    [Header("State Control Variables")]
    [SerializeField] [Tooltip("enemyacter Ground Check Radius")] protected float enemyCheckRadius = 0;
    [SerializeField] [Tooltip("enemyacter Ground Check Point")] protected Transform enemyGroundCheckPoint = null;
    protected float enemyCurrentHealth;
    [SerializeField] protected GameObject gridTile;

    [Space(5)]
    ///ATTACK
    [Header("Attack")]
    [SerializeField] [Tooltip("enemyacter Attack Range")] protected float enemyAttackRange = 0;
    [SerializeField] [Tooltip("enemyacter Attack Rate")] protected float enemyAttackRate = 0;
    [SerializeField] [Tooltip("enemyacter Attack Damage")] protected int enemyAttackDamage = 0;
    [SerializeField] [Tooltip("enemyacter Attack Point")] protected Transform enemyAttackPoint = null;

    ///STATES
    protected bool enemyIsGrounded;
    protected bool enemyIsAttacking;
    protected bool enemyIsFacingRight;

    [Space(5)]
    //Layers
    [Header("Layers")]
    [SerializeField] [Tooltip("Enemy Layer")] public LayerMask enemyLayers = 0;
    [SerializeField] [Tooltip("Player Layer")] protected LayerMask playerLayer = 0;
    [SerializeField] [Tooltip("Ground Layer")] protected LayerMask groundLayer = 0;
    [SerializeField] [Tooltip("Wall Layer")] protected LayerMask wallLayer = 0;
    #endregion

    #region Public Variables
    [SerializeField] public Transform headPos;
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
        hitPlayer = Physics2D.OverlapCircleAll(enemyAttackPoint.position, enemyAttackRange, playerLayer);
        foreach (Collider2D player in hitPlayer)
        {
            player.GetComponent<Player>().TakeDamage(enemyAttackDamage);
        }
    }

    ///<summary>
    ///enemyacter Taking Damage Function
    ///<param name="damage">Damage Value </param>
    ///</summary>
    public virtual void TakeDamage(int damage)
    {
        enemyAnimationController.SetTrigger("Hit");
        ChangeState(EnemyState.onHitted);
        enemyCurrentHealth -= damage;
        if (enemyCurrentHealth <= 0)
        {
            ChangeState(EnemyState.Dead);
            enemyAnimationController.SetTrigger("Die");
            GetComponent<Collider2D>().enabled = false;
        }
        if (!IsDead() && IsHitted())
        {
            enemyRigidbody.velocity = new Vector2(directionValue * 2f, enemyRigidbody.velocity.y);
        }
    }

    ///<summary>
    /// enemyacter Die Function 
    ///</summary>
    public virtual void Die()
    {
        if (counter == 0)
        {
            player.PlayerChallengeWeight += _enemyChallengeWeight;
            player.PlayerChallengeWeight = Mathf.Clamp(player.PlayerChallengeWeight, 0, 8);
        }
        Destroy(gameObject);
    }

    ///<summary>
    /// enemyacter Attack Function 
    ///</summary>
    public virtual void EnemyAttack()
    {
        CheckMovementDirection();
    }

    ///<summary>
    /// Enemy Patrol Function 
    ///</summary>
    public virtual void EnemyPatrol()
    {
        Move();
        CheckMovementDirection();
    }
    ///<summary>
    /// Enemy Chase Function 
    ///</summary>
    public virtual void EnemyChase()
    {
        Move();
        CheckMovementDirection();
    }

    public virtual void ResetVelocity()
    {
        enemyRigidbody.velocity = Vector2.zero;
    }
    ///If is necessary to use
    /*///<summary>
    /// enemyacter Reset Variables Function 
    ///</summary>
    protected virtual void ResetValues()
    {
        return;
    }*/
    #endregion

    #region Protected Functions

    ///<summary>
    /// enemyacter Flip Function 
    ///</summary>
    protected virtual void Flip()
    {
        enemyIsFacingRight = !enemyIsFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    ///<summary>
    /// enemyacter Move Function 
    ///</summary>
    public virtual Vector3 Move()
    {
        GetComponent<Pathfinder>().StartNodePosition = new Vector2(transform.position.x, transform.position.y);
        path = GetComponent<Pathfinder>().path;
        Vector2 start = transform.position;
            if (path.Count > 0 || path != null)
            {
                Vector2 targetRaw = new Vector3(path[0].Position.x, path[0].Position.y);
                Vector3 targetPos = new Vector3(path[0].Position.x + 0.5f, path[0].Position.y + 0.5f);
                //start = Vector3.Lerp(start, targetPos, Time.smoothDeltaTime * 10f);
                start = (targetPos - transform.position).normalized;
                start = start * new Vector3(enemyMoveSpeed, enemyMoveSpeed);
                if (Vector2.Distance(transform.position, targetPos) <= 0.01f && targetRaw != GetComponent<Pathfinder>().GoalNodePosition)
                {
                    GetComponent<Pathfinder>().StartNodePosition = new Vector2(transform.position.x, transform.position.y);
                }
            }
        if (Vector2.Distance(transform.position, target) < 0.2f)
        {
            _randomPoint = Random.Range(0, _wayPoints.Length);
        }
        //TO DO transform.position += (Vector3)start * Time.deltaTime; Working with enemy, with this assignment it can move also patrol problem probably here check here.
        return start;
    }

    ///<summary>
    /// enemyacter Checking Movement Direction Function 
    ///</summary>
    protected virtual void CheckMovementDirection()
    {
        Vector2 targetPos = new Vector2(target.x, enemyRigidbody.position.y);
        float dir = (targetPos - enemyRigidbody.position).normalized.x;
        if (dir < 0 && enemyIsFacingRight && canFlip)
        {
            //TO DO according to player direction because these values when enemy hitted from back its not working
            directionValue = 1;
            Flip();
        }
        else if (dir > 0 && !enemyIsFacingRight && canFlip)
        {
            directionValue = -1;
            Flip();
        }
    }

    ///<summary>
    /// Changing enemyacter state by detecting what kind of layer it touches 
    ///</summary>
    protected virtual void ChangeState(EnemyState enemyState)
    {
        if(this.enemyState != enemyState)
        this.enemyState = enemyState;
    }

    ///<summary>
    /// Setting enemyacter state by checking which states are true 
    ///</summary>
    protected virtual void SetEnemyState()
    {
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position,player.transform.position);
            if (distance < 5f )//&& canAttack) // && (isOnSight || IsHitted())
            {
                ChangeState(EnemyState.Chasing);
            }
            if (distance > 5f) //&& !isOnSight)
            {
                ChangeState(EnemyState.Patroling);
            }
            if (distance < 0.6f && isOnSight && canAttack)
            {
                if (noOfClicks == 1)
                {
                    enemyAnimationController.SetTrigger("Attack");
                }
                else if (noOfClicks == 2)
                {
                    enemyAnimationController.SetTrigger("Attack2");
                }
                ChangeState(EnemyState.inAttacking);
            }
            if (IsAttacking() && !isOnSight )//&& canAttack)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
    }

    protected void UpdateAnimations()
    {
        enemyAnimationController.SetBool("isChasing", IsChasing());
        enemyAnimationController.SetBool("isAttacking", IsAttacking());
        enemyAnimationController.SetBool("isOnSight", isOnSight);
        //enemyAnimationController.SetBool("isReady", IsOnWaitingToAttack());
        enemyAnimationController.SetBool("isPatrolling", IsPatrolling());
    }

    protected virtual void SetGridsAroundPlayer()
    {
        coor_N = new Vector3(getGridCoordinates.x, getGridCoordinates.y + 1f);
        coor_NE = new Vector3(getGridCoordinates.x + 1f, getGridCoordinates.y + 1f);
        coor_NW = new Vector3(getGridCoordinates.x - 1f, getGridCoordinates.y + 1f);
        coor_S = new Vector3(getGridCoordinates.x, getGridCoordinates.y - 1f);
        coor_SE = new Vector3(getGridCoordinates.x + 1f, getGridCoordinates.y - 1f);
        coor_SW = new Vector3(getGridCoordinates.x - 1f, getGridCoordinates.y - 1f);
        coor_W = new Vector3(getGridCoordinates.x - 1f, getGridCoordinates.y);
        coor_E = new Vector3(getGridCoordinates.x + 1f, getGridCoordinates.y);
    }

    protected virtual void OnDrawGizmos()
    {

        Vector3 tester = new Vector3(getGridCoordinates.x + 0.5f, getGridCoordinates.y + 0.5f, 0.0f);
        Vector3[] tester2 = { new Vector3(coor_W.x + 0.5f, coor_W.y + 0.5f, 0.0f), new Vector3(coor_E.x + 0.5f, coor_E.y + 0.5f, 0.0f) };
        if (enemyAttackPoint == null || LineOfSight == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(enemyAttackPoint.position, enemyAttackRange);
        if (enemyIsFacingRight)
        {
            Gizmos.DrawLine(LineOfSight.position, new Vector3(LineOfSight.position.x + LineOfSightX, LineOfSight.position.y + (LineOfSightX * 0.58f), LineOfSight.position.z));
            Gizmos.DrawLine(LineOfSight.position, new Vector3(LineOfSight.position.x + LineOfSightX, LineOfSight.position.y - (LineOfSightX * 0.58f), LineOfSight.position.z));
        }
        else
        {
            Gizmos.DrawLine(LineOfSight.position, new Vector3(LineOfSight.position.x - LineOfSightX, LineOfSight.position.y + (LineOfSightX * 0.58f), LineOfSight.position.z));
            Gizmos.DrawLine(LineOfSight.position, new Vector3(LineOfSight.position.x - LineOfSightX, LineOfSight.position.y - (LineOfSightX * 0.58f), LineOfSight.position.z));
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(tester, new Vector3(gridTile.GetComponent<Tilemap>().layoutGrid.cellSize.x, gridTile.GetComponent<Tilemap>().layoutGrid.cellSize.y));
        for (int i = 0; i < 2; i++)
        {
            Gizmos.DrawWireCube(tester2[i], new Vector3(gridTile.GetComponent<Tilemap>().layoutGrid.cellSize.x, gridTile.GetComponent<Tilemap>().layoutGrid.cellSize.y));
        }


    }

    protected void SetTargetType()
    {
        if (IsAttacking() || IsChasing())
        {
            target = player.transform.position;
            //enemyMoveSpeed = 3f;
        }
        else
        {
            target = _wayPoints[_randomPoint].position;
            //enemyMoveSpeed = 1.5f;
        }
    }

    protected void CheckGrids()
    {
        if (player.PlayerLastGrids != null)
        {
            for (int i = 0; i < 9; i++)
            {
                if ((player.PlayerLastGrids[i] == new Vector3(getGridCoordinates.x + (-directionValue), getGridCoordinates.y)) && counter == 1)
                {

                    if ((player.PlayerChallengeWeight - _enemyChallengeWeight) >= 0)
                    {
                        if (counter == 1)
                        {
                            canAttack = true;
                            player.PlayerChallengeWeight = (player.PlayerChallengeWeight - _enemyChallengeWeight);
                            counter--;
                            ChangeState(EnemyState.Chasing);
                        }

                    }
                    else
                    {
                        canAttack = false;
                        ChangeState(EnemyState.onWaitingToAttack);
                    }
                }
            }
        }
    }
    protected void GetGridsCoordinates()
    {
        getGridCoordinates = gridTile.GetComponent<Tilemap>().layoutGrid.WorldToCell(transform.position);
        Vector3[] tempCoords = {new Vector3(getGridCoordinates.x + 1f,getGridCoordinates.y), new Vector3(getGridCoordinates.x, getGridCoordinates.y),
        new Vector3(getGridCoordinates.x + -1f,getGridCoordinates.y)};
        _enemyGridCoords = tempCoords;
    }

    protected void CheckIfEnemyInside()
    {
        if(player != null)
        {
            if (!((player.PlayerLastGrids[0].y >= getGridCoordinates.y) && (player.PlayerLastGrids[2].y <= getGridCoordinates.y) &&
       (player.PlayerLastGrids[4].x <= getGridCoordinates.x) && (player.PlayerLastGrids[6].x >= getGridCoordinates.x)))
            {
                if (counter == 0)
                {
                    player.PlayerChallengeWeight += _enemyChallengeWeight;
                    player.PlayerChallengeWeight = Mathf.Clamp(player.PlayerChallengeWeight, 0, 20); // TO DO add variable to max challenge value
                }
                counter = 1;
                canAttack = true;
            }
        }
       
    }
    protected void CheckLineOfSight()
    {
        if (player != null)
        {
            if (enemyIsFacingRight)
            {
                isOnSight = (player.transform.position.x < LineOfSight.position.x + LineOfSightX) && (player.transform.position.x > LineOfSight.position.x) && (player.transform.position.y < LineOfSight.position.y + (LineOfSightX * 0.58f)) && (player.transform.position.y > LineOfSight.position.y - (LineOfSightX * 0.58f));
            }
            else
            {
                isOnSight = (player.transform.position.x > LineOfSight.position.x - LineOfSightX) && (player.transform.position.x < LineOfSight.position.x) && (player.transform.position.y < LineOfSight.position.y + (LineOfSightX * 0.58f)) && (player.transform.position.y > LineOfSight.position.y - (LineOfSightX * 0.58f));
            }
        }
    }
    #region Functions of Returning Enemy States
    protected bool IsOnWaitingToAttack()
    {
        return enemyState == EnemyState.onWaitingToAttack;
    }

    protected bool IsAttacking()
    {
        return enemyState == EnemyState.inAttacking;
    }

    protected bool IsHitted()
    {
        return enemyState == EnemyState.onHitted;
    }

    protected bool IsDead()
    {
        return enemyState == EnemyState.Dead;
    }

    protected bool IsPatrolling()
    {
        return enemyState == EnemyState.Patroling;
    }
    protected bool IsChasing()
    {
        return enemyState == EnemyState.Chasing;
    }
    #endregion

    #endregion

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterRenderer2D : MonoBehaviour
{
    #region Enums
    public enum CharacterState
    {
        #region Player States
        inIdling,
        onRunning,
        inAirAttacking,
        onJumping,
        onFalling,
        onWallSliding,
        onDashing,
        onEnemy,
        #endregion
        #region Enemy States
        Chasing,
        Patroling,
        #endregion
        #region Common States
        inAttacking,
        onHitted,
        Dead
        #endregion

    }
    public enum CollidedAreas
    {
       None,
       Ground,
       LeftWall,
       RightWall,
       Ladder,
       Platform
    }
    #endregion

    #region Protected Variables
    ///Character Components
    protected SpriteRenderer charSprite;
    protected Rigidbody2D charRigidbody;

    ///Members of other classes
    protected AnimationController charAnimationController;
    protected CharacterState charState;
    protected CollidedAreas collidingAgainst;
    protected Timer charTimer;

    ///GENERAL
    [Header("General")]
    [Range(0,7)]
    [SerializeField][Tooltip("Character Movement Speed")] protected float charMoveSpeed = 0;
    [SerializeField][Tooltip("Character Maximum Health")] protected float charMaxHealth = 0;
    [SerializeField] protected int _charChallengeWeight = 8;
    protected Vector3 coor_N, coor_NW, coor_NE, coor_S, coor_SW, coor_SE, coor_E, coor_W;
    protected Vector3 getGridCoordinates;

    [Space(5)]
    ///STATE CONTROLS
    [Header("State Control Variables")]
    [SerializeField] [Tooltip("Character Ground Check Radius")] protected float charCheckRadius = 0;
    [SerializeField] [Tooltip("Character Ground Check Point")] protected Transform charGroundCheckPoint = null;
    protected float charCurrentHealth;
    [SerializeField] protected GameObject gridTile;

    [Space(5)]
    ///ATTACK
    [Header("Attack")]
    [SerializeField] [Tooltip("Character Attack Range")] protected float charAttackRange = 0;
    [SerializeField] [Tooltip("Character Attack Rate")] protected float charAttackRate = 0;
    [SerializeField] [Tooltip("Character Attack Damage")] protected int charAttackDamage = 0;
    [SerializeField] [Tooltip("Character Attack Point")] protected Transform charAttackPoint = null;

    ///STATES
    protected bool charIsGrounded;
    protected bool charIsAttacking;
    protected bool charIsFacingRight;

    [Space(5)]
    //Layers
    [Header("Layers")]
    [SerializeField] [Tooltip("Enemy Layer")] protected LayerMask enemyLayers = 0;
    [SerializeField] [Tooltip("Player Layer")] protected LayerMask playerLayer = 0;
    [SerializeField] [Tooltip("Ground Layer")] protected LayerMask groundLayer = 0;
    [SerializeField] [Tooltip("Wall Layer")] protected LayerMask wallLayer = 0;
    #endregion

    #region Functions

    #region Public Functions

    ///<summary>
    /// Character Damage Function 
    ///</summary>
    public virtual void GiveDamage()
    {
        return;
    }

    ///<summary>
    ///Character Taking Damage Function
    ///<param name="damage">Damage Value </param>
    ///</summary>
    public virtual void TakeDamage(int damage)
    {
        return;
    }

    ///<summary>
    /// Character Die Function 
    ///</summary>
    public virtual void Die()
    {
        return;
    }

    ///<summary>
    /// Character Attack Function 
    ///</summary>
    public virtual void Attack()
    {
        return;
    }

    #region Enemy Specific Functions
    ///<summary>
    /// Enemy Patrol Function 
    ///</summary>
    public virtual void EnemyPatrol()
    {
        return;
    }
    ///<summary>
    /// Enemy Chase Function 
    ///</summary>
    public virtual void EnemyChase()
    {
        return;
    }
    #endregion

    ///If is necessary to use
    /*///<summary>
    /// Character Reset Variables Function 
    ///</summary>
    protected virtual void ResetValues()
    {
        return;
    }*/
    #endregion

    #region Protected Functions

    ///<summary>
    /// Character Flip Function 
    ///</summary>
    protected virtual void Flip()
    {
        charIsFacingRight = !charIsFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    ///<summary>
    /// Character Move Function 
    ///</summary>
    protected virtual void Move()
    {
        return;
    }

    ///<summary>
    /// Character Checking Movement Direction Function 
    ///</summary>
    protected virtual void CheckMovementDirection()
    {
        return;
    }

    ///<summary>
    /// Character Checking Surroundings Function.
    ///</summary>
    protected virtual void CheckSurroundings()
    {
        return;
    }

    ///<summary>
    /// Changing character state by detecting what kind of layer it touches 
    ///</summary>
    protected virtual void ChangeState(CharacterState charState)
    {
        return;
    }

    ///<summary>
    /// Setting character state by checking which states are true 
    ///</summary>
    protected virtual void SetCharacterState()
    {
        return;
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
    #region Functions of Returning Character States
    protected bool IsIdling()
    {
        return charState == CharacterState.inIdling;
    }

    protected bool IsAttacking()
    {
        return charState == CharacterState.inAttacking;
    }

    protected bool IsAirAttacking()
    {
        return charState == CharacterState.inAirAttacking;
    }

    protected bool IsRunning()
    {
        return charState == CharacterState.onRunning;
    }

    protected bool IsJumping()
    {
        return charState == CharacterState.onJumping;
    }

    protected bool IsFalling()
    {
        return charState == CharacterState.onFalling;
    }

    protected bool IsWallSliding()
    {
        return charState == CharacterState.onWallSliding;
    }

    protected bool IsDashing()
    {
        return charState == CharacterState.onDashing;
    }

    protected bool IsHitted()
    {
        return charState == CharacterState.onHitted;
    }

    protected bool IsDead()
    {
        return charState == CharacterState.Dead;
    }

    protected bool IsOnEnemy()
    {
        return charState == CharacterState.onEnemy;
    }

    #region Enemy Functions
    protected bool IsPatrolling()
    {
        return charState == CharacterState.Patroling;
    }
    protected bool IsChasing()
    {
        return charState == CharacterState.Chasing;
    }
    #endregion

    //COLLIDED AREAS
    protected bool IsGrounded()
    {
        return collidingAgainst == CollidedAreas.Ground;
    }
    #endregion

    #endregion

    #endregion
}
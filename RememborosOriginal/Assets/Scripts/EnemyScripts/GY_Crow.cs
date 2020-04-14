using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TO DO LIST
//Integrate belgian ai with flock algorithm
//Fix flipping bug 
//Fix when target not player, not moving bug

[RequireComponent(typeof(Collider2D))]
public class GY_Crow : EnemyRenderer2D
{

    public Collider2D AgentCollider { get { return enemyCollider; } }
    private Vector2 moveVector;
    private void Start()
    {
        path = new List<Node>();
        moveVector = Vector2.zero;
        enemyType = EnemyType.GY_Crow;
        enemyCollider = GetComponent<Collider2D>();
        canAttack = true;
        counter = 1;
        canFlip = true;
        noOfClicks = 1;
        directionValue = 1;
        enemyState = EnemyState.Patroling;
        target = new Vector2(0, 0);
        _randomPoint = Random.Range(0, _wayPoints.Length);
        isOnSight = false;
        enemyIsAttacking = false;
        enemyIsFacingRight = true;
        enemyAnimationController = GetComponent<Animator>();
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyCurrentHealth = enemyMaxHealth;
    }
    private void Update()
    {
        GetComponent<FlockManager>().FlockMove();
        CheckLineOfSight();
        SetEnemyState();
        SetTargetType();
    }
    private void FixedUpdate()
    {
        // TO DO assign target but at the moment it doesnt work because of ai animation selector and patrolling doesnt work cant change waypoints.
        GetComponent<Pathfinder>().GoalNodePosition = player.transform.position; 
        CheckIfEnemyInside();
        GetGridsCoordinates();
        CheckGrids();
        SetGridsAroundPlayer();
    }

    public override Vector3 Move()
    {
        GetComponent<Pathfinder>().StartNodePosition = new Vector2(transform.position.x, transform.position.y);
        path = GetComponent<Pathfinder>().path;
        Vector2 start = transform.position;
        if (path.Count > 0 || path != null)
        {
            Vector2 targetRaw = new Vector3(path[0].Position.x, path[0].Position.y);
            Vector3 targetPos = new Vector3(path[0].Position.x + 0.5f, path[0].Position.y + 0.5f);
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
        return start;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, GetComponent<FlockManager>().neighborRadius);
    }



}
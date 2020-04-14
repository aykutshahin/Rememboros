using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : EnemyRenderer2D
{
    // Start is called before the first frame update
    public bool FacingRight
    {
        get
        {
            return enemyIsFacingRight;
        }
    }
    void Start()
    {
        enemyType = EnemyType.Enemy;
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

    // Update is called once per frame
    void Update()
    {
        // TO DO assign target but at the moment it doesnt work because of ai animation selector and patrolling doesnt work cant change waypoints.
        GetComponent<Pathfinder>().GoalNodePosition = player.transform.position;
        CheckLineOfSight();
        SetEnemyState();
        SetTargetType();
    }
    private void FixedUpdate()
    {
        CheckIfEnemyInside();
        GetGridsCoordinates();
        CheckGrids();
        SetGridsAroundPlayer();
        UpdateAnimations();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
}

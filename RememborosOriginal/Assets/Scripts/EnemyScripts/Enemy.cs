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
        canAttack = true;
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
        CheckLineOfSight();
        SetEnemyState();
        SetTargetType();
    }
    private void FixedUpdate()
    {
        UpdateAnimations();
        ChangeTargetLocations();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
}

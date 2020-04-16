using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : CharacterRenderer2D
{
    [Space(10)]
    [Header("Player Variables")]
    [SerializeField] private float jumpHeight = 4f;
    [SerializeField] private float timeToJumpApex = .4f;
    Animator myAnimator; // animator component
    private float move; // Movement input variable range in [-1,1]
    private bool isTouchingWall;
    public Vector3 dashDistance;
    private Vector3 dashDistanceVector;
    public float dashSpeed = 0;
    public float dashCooldown = 0;
    public float attackCooldown = 0;
    CharacterState prevState;

    //CharacterController2D
    CharacterController2D charController;
    Vector3 velocity;
    float gravity;
    float jumpVelocity;
    float velocityXSmoothing;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;
    float facingDirection;

    private int extraJumps; // Amount of jump
    [SerializeField] private int extraJumpsValue = 0;
    [SerializeField] private float fallMultiplier = 2.5f; // to make an advanced jump
    [SerializeField] private float wallCheckDistance = 0; // wall check raycast size(horizontal)
    [SerializeField] private float wallSlideSpeed = 0;
    [SerializeField] SendingRay testPoint;


    float tempcharMoveSpeed; // created for changing charMoveSpeed when player attacks
    [SerializeField] private Transform wallCheck = null;
    [SerializeField] private Transform wallCheck2 = null;


    //TIMER
    public int noOfClicks = 1; // amount of clicks ranged in [0,3] since player has 3 attack animations
    public int noOfClicksAir = 1;
    float lastClickedTime = 0;
    public float maxComboDelay = 0.9f; // checking step by step click not spawning
    public float maxComboDelayAnimation = 0.53f; // animation delay
    Vector3 start;


    public CharacterState PlayerState
    {
        get
        {
            return charState;
        }
        set
        {
            charState = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        attackCooldown = 1f / charAttackRate;
        facingDirection = 1;
        charTimer = GetComponent<Timer>();
        charController = GetComponent<CharacterController2D>();
        collidingAgainst = CollidedAreas.Ground;
        ChangeState(CharacterState.inIdling);
        charCurrentHealth = charMaxHealth;
        charIsFacingRight = true;
        tempcharMoveSpeed = charMoveSpeed;
        extraJumps = extraJumpsValue;
        myAnimator = GetComponent<Animator>();
        charSprite = GetComponent<SpriteRenderer>();
        charTimer.addTimer("Attack", 1f, attackCooldown);
        charTimer.addTimer("Dash", 1f, dashCooldown);
    }

    private void Update()
    {
        gravity = -(2 * jumpHeight) / (Mathf.Pow(timeToJumpApex, 2));
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        if (charController.collisions.above || charController.collisions.below)
        {
            velocity.y = 0;
        }
        velocity.y += gravity * Time.deltaTime;

        Move();
        SetCharacterState();
        CheckMovementDirection();
        Jump();
        Attack();
        RunAnimations();
        Dash();
    }
    private void FixedUpdate()
    {
        IsOnGround();
        CheckSurroundings();
        AdvancedJump();
        charTimer.DecreaseCurrentFrame();
    }
    protected override void Move()
    {
        move = Input.GetAxisRaw("Horizontal");
        float targetVelocityX = move * charMoveSpeed;
        // get movement input value
        /* crossplatfrominputmanager dan çekmek daha iyi olabilir her türlü platformda çalışması için*/
        // check if attacking, if so dont get input for movement because of stopping character
        if (IsRunning() || IsJumping() || IsFalling())// && canMove)
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (charController.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        }
        if (IsDashing())
        {
            velocity.y = 0;
        }
        charController.Move(velocity * Time.deltaTime);
        // check if sliding, if so make slide movement
        if (IsWallSliding())
        {
            if (velocity.y < -wallSlideSpeed)
            {
                velocity = new Vector2(velocity.x, -wallSlideSpeed);
            }
        }

    }
    void Jump()
    {
        // check if is grounded or wall sliding, if so reset extrajump value
        if (charIsGrounded || IsWallSliding())
        {
            extraJumps = extraJumpsValue;
        }
        if (Input.GetKeyDown(KeyCode.Space) && extraJumps > 0 && !IsAirAttacking())
        {
            velocity = Vector2.up * jumpVelocity;
            charController.Move(velocity * Time.deltaTime);
            myAnimator.SetTrigger("Jump");
            extraJumps--;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && extraJumps == 0 && charIsGrounded == true && !IsAirAttacking())
        {
            velocity = Vector2.up * jumpVelocity;
            charController.Move(velocity * Time.deltaTime);
            myAnimator.SetTrigger("Jump");
        }
    }
    void RunAnimations()
    {
        myAnimator.SetBool("isWallSliding", IsWallSliding());
        myAnimator.SetBool("isAttacking", IsAttacking());
        myAnimator.SetBool("isAirAttacking", IsAirAttacking());
        myAnimator.SetBool("isFalling", IsFalling());
        myAnimator.SetBool("isIdling", IsIdling());
        myAnimator.SetBool("isRunning", IsRunning());
        myAnimator.SetBool("isOnEnemy", IsOnEnemy());
    }
    protected override void Flip()
    {
        // check if wall sliding, if not player can flip
        if (!IsWallSliding() && !IsDashing())
        {
            // flip player
            facingDirection *= -1;
            charIsFacingRight = !charIsFacingRight;
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }

    }
    public override void Attack()
    {
        if (!charTimer.isOnCooldown("Attack"))
        {
            // when click time pass, attack animation resets for example when you click the mouse after that you will see Attack animation 1 and so on
            if (Time.time - lastClickedTime > maxComboDelay && (IsAttacking() || IsAirAttacking()))
            {
                if (charIsGrounded)
                {
                    ChangeState(CharacterState.inIdling);
                }
                else
                {
                    ChangeState(CharacterState.onFalling);
                }

            }
            if (Time.time - lastClickedTime > maxComboDelay * 6)
            {
                noOfClicks = 1;
                noOfClicksAir = 1;
            }
            // check if left mouse button is clicked (0) for left (1) for right

            if (IsAttacking() || IsAirAttacking())
            {
                // get current time
                // increase amount of clicks           
                // check if player touchs ground, if so rigidbody velocity is made zero for stopping 
                if (IsAttacking())
                {
                    //Normal Attack
                    if (IsAttacking())
                    {
                        if (noOfClicks == 1)
                        {
                            myAnimator.SetTrigger("Attack1");
                        }
                        else if (noOfClicks == 2)
                        {
                            myAnimator.SetTrigger("Attack2");
                        }
                        else if (noOfClicks == 3)
                        {
                            myAnimator.SetTrigger("Attack3");
                        }
                    }
                }
                //air attack
                if (IsAirAttacking())
                {
                    // gravity = -10f;
                    //Air attack
                    if (noOfClicksAir == 1)
                    {
                        myAnimator.SetTrigger("AirAttack1");
                    }
                    else if (noOfClicksAir == 2)
                    {
                        myAnimator.SetTrigger("AirAttack2");
                    }
                }

                charTimer.ResetCooldownFrame("Attack");

            }
        }
    }

    public override void TakeDamage(int damage)
    {
        myAnimator.SetTrigger("Hit");
        charCurrentHealth -= damage;
        if (charCurrentHealth <= 0)
        {
            myAnimator.SetTrigger("Die");
            GetComponent<Collider2D>().enabled = false;
            this.enabled = false;
        }
    }

    // Attack animation event to check whether enemy take damage or not.
    public override void GiveDamage()
    {
        Collider2D[] hitEnemies;
        // create a circle in charAttackPoint position which has a radius size is equal to charAttackRange and last parameter represents what kind of layer is touched
        hitEnemies = Physics2D.OverlapCircleAll(charAttackPoint.position, charAttackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(charAttackDamage);
        }
    }

    public override void Die()
    {
        Destroy(gameObject);
    }

    // check movement direction , move range in [-1,1]
    protected override void CheckMovementDirection()
    {
        if (charIsFacingRight && move < 0)
        {
            Flip();
        }
        else if (!charIsFacingRight && move > 0)
        {
            Flip();
        }
    }

    private void IsOnGround()
    {
        if (charIsGrounded && !IsGrounded())
        {
            collidingAgainst = CollidedAreas.Ground;
        }
    }
    protected override void CheckSurroundings()
    {
        charIsGrounded = Physics2D.OverlapCircle(charGroundCheckPoint.position, charCheckRadius, groundLayer);
        // creating a raycast form wallcheck position to right in wallcheckdistance size and last parameter represents what kind of layer is touched
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, wallLayer);
        myAnimator.SetBool("isGrounded", charIsGrounded);
    }

    // for drawing gizmos, unity library
    private void OnDrawGizmos()
    {
        if (charAttackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(charAttackPoint.position, charAttackRange);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
        Gizmos.color = Color.red;
        /*Vector3 a1Test = (testPoint.foundedTarget - transform.position);
        Gizmos.DrawRay(transform.position, a1Test);*/ // TO DO Avoiding obstacles
    }

    void AdvancedJump()
    {
        //if going down then speed up the falling
        if (velocity.y < 0 && !IsWallSliding() && !charIsGrounded && !isTouchingWall && !IsAirAttacking())
        {
            velocity += (Vector3)(Vector2.up * gravity * (fallMultiplier - 1) * Time.deltaTime);
        }
    }

    protected override void Dash()
    {
        if (IsDashing())
        {
            transform.position = Vector3.Lerp(transform.position,dashDistanceVector, dashSpeed * Time.deltaTime);
            if(Mathf.Abs(transform.position.x - dashDistanceVector.x) <= 0.2f || (charController.collisions.left || charController.collisions.right))
            {
                EndDash();
            }
        }

    }

    void EndDash()
    {
        ChangeState(prevState);
        charTimer.ResetCooldownFrame("Dash");
    }

    protected override void ChangeState(CharacterState charState)
    {
        if(this.charState != charState)
        {
            prevState = this.charState;
            this.charState = charState;
        }
        
    }

    protected override void SetCharacterState()
    {
        //Setting whether character is running or is idling
        if (!IsAttacking() && !IsDashing())
        {
            if (move != 0 && charIsGrounded && !IsJumping())
            {
                ChangeState(CharacterState.onRunning);
            }
            else if (charIsGrounded && !IsJumping() & !IsIdling())
            {
                ChangeState(CharacterState.inIdling);
                velocity = Vector2.zero;
            }
        }
        //
        if (!charIsGrounded && velocity.y > 0 && !IsAirAttacking() && !IsDashing())
        {
            ChangeState(CharacterState.onJumping);
        }
        //
        if (isTouchingWall && !charIsGrounded && velocity.y < 0)
        {
            ChangeState(CharacterState.onWallSliding);
        }
        //
        if (velocity.y < 0 && !isTouchingWall && !charIsGrounded && !IsAirAttacking() && !IsDashing())
        {
            ChangeState(CharacterState.onFalling);
        }
        if(Input.GetKeyDown(KeyCode.LeftShift) && !IsWallSliding() && !charTimer.isOnCooldown("Dash") && !IsDashing())
        {
            float distance = dashDistance.x * facingDirection;
            ChangeState(CharacterState.onDashing);
            distance = transform.position.x + distance;
            dashDistanceVector = new Vector3(distance, transform.position.y);
        }
        //
        if (Input.GetMouseButtonDown(0) && !IsWallSliding() && !IsFalling() && !IsDashing())
        {
            velocity = Vector2.zero;
            lastClickedTime = Time.time;
            noOfClicks = Mathf.Clamp(noOfClicks, 1, 3);
            noOfClicksAir = Mathf.Clamp(noOfClicksAir, 1, 3);
            if (charIsGrounded)
            {
                ChangeState(CharacterState.inAttacking);
            }
            else
            {
                ChangeState(CharacterState.inAirAttacking);
            }
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(CharacterController2D))]
public class Player : CharacterRenderer2D
{
    [Space(10)]
    [Header("Player Variables")]
    [SerializeField] private float jumpForce = 0;

    [SerializeField] private float jumpHeight = 4f;
    [SerializeField] private float timeToJumpApex = .4f;

    Animator myAnimator; // animator component
    private float move; // Movement input variable range in [-1,1]
    private bool isTouchingWall;
    private bool isTouchingEnemyHead;
    private bool isTouchingEnemyHeadWithHead;

    //CharacterController2D
    CharacterController2D charController;
    Vector3 velocity;
    float gravity;
    float jumpVelocity;
    float velocityXSmoothing;
   public float accelerationTimeAirborne = .2f;
   public float accelerationTimeGrounded = .1f;

    private int extraJumps; // Amount of jump
    [SerializeField] private int extraJumpsValue = 0;
    [SerializeField] private float fallMultiplier = 2.5f; // to make an advanced jump
    [SerializeField] private float wallCheckDistance = 0; // wall check raycast size(horizontal)
    [SerializeField] private float wallSlideSpeed = 0;
    [SerializeField] SendingRay testPoint;

    private Vector3[] _aroundGrids;

    float tempcharMoveSpeed; // created for changing charMoveSpeed when player attacks
    [SerializeField] private Transform wallCheck = null;
    [SerializeField] private Transform wallCheck2 = null;

    //THROWING 
    [SerializeField] private GameObject Knife = null;
    [SerializeField] private Transform throwPoint = null;

    //TIMER
    float nextAttackTime = 0f;
    public int noOfClicks = 1; // amount of clicks ranged in [0,3] since player has 3 attack animations
    public int noOfClicksAir = 1;
    float lastClickedTime = 0;
    public float maxComboDelay = 0.9f; // checking step by step click not spawning
    public float maxComboDelayAnimation = 0.53f; // animation delay
    private Vector3 enemyPos;
    [SerializeField] Transform feetPos;
    Vector3 start;

    public int PlayerChallengeWeight
    {
        get
        {
            return _charChallengeWeight;
        }

        set
        {
            _charChallengeWeight = value;
        }
    }

    public Vector3[] PlayerLastGrids
    {
        get
        {
            return _aroundGrids;
        }
    }

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
        charController = GetComponent<CharacterController2D>();
        isTouchingEnemyHeadWithHead = false;
        collidingAgainst = CollidedAreas.Ground;
        charTimer = GetComponent<Timer>();
        ChangeState(CharacterState.inIdling);
        charCurrentHealth = charMaxHealth;
        charIsFacingRight = true;
        tempcharMoveSpeed = charMoveSpeed;
        extraJumps = extraJumpsValue;
        myAnimator = GetComponent<Animator>();
        charSprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        gravity = -(2 * jumpHeight) / (Mathf.Pow(timeToJumpApex, 2));
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        if (charController.collisions.above || charController.collisions.below)
        {
            velocity.y = 0;
        }
        Move();
        SetCharacterState();
        CheckMovementDirection();
        Jump();
        Attack();
        RunAnimations();
    }
    private void FixedUpdate()
    {
        IsOnGround();
        AdvancedJump();
        Move();
        //SettingGridsCoordinates();
        SetGridsAroundPlayer();
        //ThrowKnife();
        CheckSurroundings();
    }
    protected override void Move()
    {
        velocity.y += gravity * Time.deltaTime;
        //getGridCoordinates = gridTile.GetComponent<Tilemap>().layoutGrid.WorldToCell(transform.position);
        move = Input.GetAxisRaw("Horizontal");
        float targetVelocityX = move * charMoveSpeed;
        // get movement input value
        /* crossplatfrominputmanager dan çekmek daha iyi olabilir her türlü platformda çalışması için*/
        // check if attacking, if so dont get input for movement because of stopping character
        if (IsRunning() || IsJumping() || IsFalling())
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (charController.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
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
       // myAnimator.SetFloat("FallingVelocity", velocity.y);  // play animation falling according to velocity of players rigidbody's y axis 
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
        if (!IsWallSliding())
        {
            // flip player
            charIsFacingRight = !charIsFacingRight;
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }

    }
    public override void Attack()
    {
        if (Time.time >= nextAttackTime)
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
                   else if(noOfClicksAir == 2)
                   {
                        myAnimator.SetTrigger("AirAttack2");
                   }
                }
                // next click time should be that attack rate which is specified by ourselves.
                nextAttackTime = Time.time + 1f / charAttackRate;

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
        if(charIsFacingRight && move < 0)
        {
            Flip();
        }
        else if(!charIsFacingRight && move > 0)
        {
            Flip();
        }
    }

    private void IsOnGround()
    {
        if(charIsGrounded && !IsGrounded())
        {
            collidingAgainst = CollidedAreas.Ground;
        }
    }
    protected override void CheckSurroundings()
    {
        charIsGrounded = Physics2D.OverlapCircle(charGroundCheckPoint.position, charCheckRadius, groundLayer);
        // creating a raycast form wallcheck position to right in wallcheckdistance size and last parameter represents what kind of layer is touched
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, wallLayer);
        //isTouchingEnemyHead = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, enemyLayers);
        myAnimator.SetBool("isGrounded", charIsGrounded);
    }

    // for drawing gizmos, unity library
   /* private void OnDrawGizmos()
    {
        Vector3 tester = new Vector3(getGridCoordinates.x + 0.5f, getGridCoordinates.y + 0.5f, 0.0f);
        Vector3[] aroundCoordinates = { new Vector3(coor_N.x + 0.5f, coor_N.y + 0.5f, 0.0f), new Vector3(coor_NW.x + 0.5f, coor_NW.y + 0.5f, 0.0f), new Vector3(coor_NE.x + 0.5f, coor_NE.y + 0.5f, 0.0f),
        new Vector3(coor_S.x + 0.5f, coor_S.y + 0.5f, 0.0f),new Vector3(coor_SE.x + 0.5f, coor_SE.y + 0.5f, 0.0f),new Vector3(coor_SW.x + 0.5f, coor_SW.y + 0.5f, 0.0f),new Vector3(coor_W.x + 0.5f, coor_W.y + 0.5f, 0.0f),
        new Vector3(coor_E.x + 0.5f, coor_E.y + 0.5f, 0.0f)};
        if (charAttackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(charAttackPoint.position, charAttackRange);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(tester, new Vector3(gridTile.GetComponent<Tilemap>().layoutGrid.cellSize.x, gridTile.GetComponent<Tilemap>().layoutGrid.cellSize.y));
        for(int i = 0; i < 8; i++)
        {
            Gizmos.DrawWireCube(aroundCoordinates[i], new Vector3(gridTile.GetComponent<Tilemap>().layoutGrid.cellSize.x, gridTile.GetComponent<Tilemap>().layoutGrid.cellSize.y));
        }
        Vector3 a1Test = (testPoint.foundedTarget - transform.position);
        Gizmos.DrawRay(transform.position, a1Test);
    }

    void ThrowKnife()
    {
        if(Input.GetKeyDown(KeyCode.Q) && !IsWallSliding())
        {
            GameObject knife = (GameObject)Instantiate(Knife, throwPoint.position, Quaternion.identity);
            if (charIsFacingRight)
            {
                knife.transform.localScale = new Vector3(1.0f, knife.transform.localScale.y, transform.localScale.z);
            }
            else
            {
                knife.transform.localScale = new Vector3(-1.0f, knife.transform.localScale.y, transform.localScale.z);
            }
            
        }
    }*/

    void AdvancedJump()
    {
        //if going down then speed up the falling
        if(velocity.y < 0 && !IsWallSliding() && !charIsGrounded && !isTouchingWall && !IsAirAttacking())
        {
            velocity += (Vector3)(Vector2.up * gravity * (fallMultiplier - 1) * Time.deltaTime);
        }
    }

    protected override void ChangeState(CharacterState charState)
    {
         this.charState = charState;
    }

    protected override void SetCharacterState()
    {
        //Setting whether character is running or is idling
        if (!IsAttacking())
        {
            if (move != 0 && charIsGrounded && !IsJumping())
            {
                ChangeState(CharacterState.onRunning);
            }
            else if (charIsGrounded && !IsJumping() & !IsIdling()) // TO DO dynamic to kinematic rigidbody &&
            {
                ChangeState(CharacterState.inIdling);
                velocity = Vector2.zero;
            }
        }
        //
        if (!charIsGrounded && velocity.y > 0 && !IsAirAttacking())
        {
            ChangeState(CharacterState.onJumping);
        }
        //
        if (isTouchingWall && !charIsGrounded && velocity.y < 0)
        {
            ChangeState(CharacterState.onWallSliding);
        }
        //
        if(velocity.y < 0 && !isTouchingWall && !charIsGrounded && !IsAirAttacking())
        {
            ChangeState(CharacterState.onFalling);
        }

        //
        if (Input.GetMouseButtonDown(0) && !IsWallSliding() && !IsFalling())
        {
            velocity = Vector2.zero;
            //noOfClicks++;
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

        //TO DO jumping on enemys head (needed animations)
     /* if (isTouchingEnemyHead && isTouchingEnemyHeadWithHead)
        {
            start = transform.position;
            charRigidbody.bodyType = RigidbodyType2D.Kinematic;
            RaycastHit2D enemyHit = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, enemyLayers);
            Vector3 target = enemyHit.rigidbody.gameObject.GetComponent<Enemy>().headPos.position;
            start = Vector3.Lerp(start, target, (Time.smoothDeltaTime) * 10f);
            transform.position = start;
            if(Vector2.Distance(transform.position,enemyHit.rigidbody.gameObject.GetComponent<Enemy>().headPos.position) < 0.08f)
            {
                charRigidbody.bodyType = RigidbodyType2D.Static;
                ChangeState(CharacterState.onEnemy);
                isTouchingEnemyHeadWithHead = false;
            }
            //Vector2 onEnemy = new Vector2(enemyHit.rigidbody.gameObject.GetComponent<Collider2D>().bounds.center.x, enemyHit.rigidbody.gameObject.GetComponent<Collider2D>().bounds.center.y + 0.6f);
            //enemyHit.rigidbody.MovePosition(Vector2.MoveTowards(enemyPos, enemyPos - new Vector3(0.5f, 0f), Time.deltaTime));
        }
        if (IsOnEnemy())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                charRigidbody.bodyType = RigidbodyType2D.Dynamic;
                charRigidbody.velocity = new Vector2(1 * jumpVelocity, jumpVelocity * 2);
                myAnimator.SetTrigger("Jump");
            }
        }
        */

    }

   /* private void SettingGridsCoordinates()
    {
        Vector3[] aroundCoordinates = { coor_N, coor_NE, coor_S, coor_NW, coor_W, coor_SW, coor_E, coor_SE, getGridCoordinates};
        _aroundGrids = aroundCoordinates;
    }*/

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider == Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, enemyLayers).collider && !isTouchingEnemyHeadWithHead)
        {
            enemyPos = collision.rigidbody.gameObject.transform.position;
        }
    }

}

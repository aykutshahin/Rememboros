using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeObstacle : MonoBehaviour
{
    //Prefabs
    [SerializeField] Player playerPrefab;

    //Layers
    [SerializeField] LayerMask enemyLayer;

    //Floats
    [SerializeField] float throwDistance = 0f;
    [SerializeField] float throwCircleRadius = 0f;
    [SerializeField] float throwSpeed = 0f;
     

    //Ints
    [SerializeField] int damage = 40;
    //Components
    Animator myAnimator;
    Rigidbody2D rb;

    //Booleans

    void Start()
    {
        myAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Collider2D[] hitEnemies;
        hitEnemies = Physics2D.OverlapCircleAll(transform.position, throwCircleRadius, enemyLayer);
        if (hitEnemies != null)
        {
            foreach (Collider2D enemy in hitEnemies)
            {             
                if(enemy.tag == "Player")
                {
                    Destroy(gameObject);
                }
                if(enemy.tag == "Enemy")
                {
                    enemy.GetComponent<Enemy>().TakeDamage(damage);
                    transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y,transform.localScale.z);
                }
            }
        }
    }
    void FixedUpdate()
    {
        Move();
    }

     void Move()
    {
        rb.velocity = new Vector2(transform.localScale.x * throwSpeed, rb.velocity.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, throwCircleRadius);
    }
}
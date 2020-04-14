using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendingRay : MonoBehaviour
{
    [SerializeField] Player player;
    public float dangerCircleRadius;
    public float angle;
    Vector3 target;
    Vector3 distanceVecFromPlayer;
    public Vector3 foundedTarget;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Arc usage Mathf.Atan(1f) * Mathf.Rad2Deg
        // Normal usage Mathf.Sin(30f * Mathf.Deg2Rad);
        target = new Vector3((dangerCircleRadius * Mathf.Cos(angle * Mathf.Deg2Rad)), (dangerCircleRadius * Mathf.Sin(angle * Mathf.Deg2Rad)));
        distanceVecFromPlayer = (player.transform.position - transform.position);
        float b = Vector3.Angle(distanceVecFromPlayer, target);
        float w = Mathf.Asin(dangerCircleRadius / Vector3.Distance(player.transform.position, transform.position)) * Mathf.Rad2Deg;
        float a = 90 - (b + w);
        foundedTarget = new Vector3(dangerCircleRadius * Mathf.Cos(a * Mathf.Deg2Rad) + transform.position.x, dangerCircleRadius * Mathf.Sin(a * Mathf.Deg2Rad) + transform.position.y);

    }

  
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, dangerCircleRadius);
        Gizmos.color = Color.red;
        //Debug.Log(target);
        Gizmos.DrawRay(transform.position, target);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, distanceVecFromPlayer.normalized);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, foundedTarget.normalized);
    }
}

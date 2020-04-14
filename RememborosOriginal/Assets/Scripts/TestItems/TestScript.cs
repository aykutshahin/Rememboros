using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    List<Node> path;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Pathfinder>().GoalNodePosition = new Vector2((int)Random.Range(CameraScript.GetCameraLowerBounds().x, GetComponent<Pathfinder>().GraphWidth), (int)Random.Range(CameraScript.GetCameraLowerBounds().y, GetComponent<Pathfinder>().GraphHeight));
        path = new List<Node>();
        transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<Pathfinder>().map.IsNodeInside(GetComponent<Pathfinder>().GoalNodePosition) || GetComponent<Pathfinder>().map.IsObstacle(GetComponent<Pathfinder>().GoalNodePosition))
        {
            GetComponent<Pathfinder>().GoalNodePosition = GetComponent<Pathfinder>().GoalNodePosition = new Vector2((int)Random.Range(CameraScript.GetCameraLowerBounds().x, GetComponent<Pathfinder>().GraphWidth), (int)Random.Range(CameraScript.GetCameraLowerBounds().y, GetComponent<Pathfinder>().GraphHeight));
        }
        path = GetComponent<Pathfinder>().path;
        Move();
    }

    private void Move()
    {

        if (path.Count > 0 || path != null)
        {
            Vector2 targetRaw = new Vector3(path[0].Position.x, path[0].Position.y);
            Vector3 target = new Vector3(path[0].Position.x + 0.5f, path[0].Position.y + 0.5f);
            transform.position = Vector3.Lerp(transform.position, target, Time.smoothDeltaTime * 10f);
            if(Vector2.Distance(transform.position, target) <= 0.01f && targetRaw != GetComponent<Pathfinder>().GoalNodePosition)
            {
                GetComponent<Pathfinder>().StartNodePosition = new Vector2((int)transform.position.x, (int)transform.position.y);
            }
            if (Vector2.Distance(transform.position, GetComponent<Pathfinder>().GoalNodePosition) <= 1f)
            {
                GetComponent<Pathfinder>().GoalNodePosition = new Vector2((int)Random.Range(CameraScript.GetCameraLowerBounds().x, GetComponent<Pathfinder>().GraphWidth), (int)Random.Range(CameraScript.GetCameraLowerBounds().y, GetComponent<Pathfinder>().GraphHeight));
                if (GetComponent<Pathfinder>().StartNodePosition == GetComponent<Pathfinder>().GoalNodePosition)
                {
                    GetComponent<Pathfinder>().GoalNodePosition = new Vector2((int)Random.Range(CameraScript.GetCameraLowerBounds().x, GetComponent<Pathfinder>().GraphWidth), (int)Random.Range(CameraScript.GetCameraLowerBounds().y, GetComponent<Pathfinder>().GraphHeight));
                }
            }
        }
            
     }
 }     




using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    #region Variables

    [Range(0f, 10f)]
    [Tooltip("Assign neighbour range of each creature")] [SerializeField] public float neighborRadius = 2f;

    #endregion

    #region Functions

    #region Public Functions
    /// <summary>
    /// Sums alignment,cohesion,separation vectors .
    /// </summary>
    public void FlockMove()
    {
        Vector3 acceleration = Alignment() + Cohesion() + Separation() * 2;
        transform.position += (Vector3)acceleration * Time.deltaTime;
    }
    #endregion

    #region Private Functions
    /// <summary>
    ///  Aligment function specify which direction flock will go. 
    /// </summary>
    /// <returns>Alignment Vector</returns>
    private Vector2 Alignment()
    {
        Vector2 steering = Vector2.zero;
        List<Transform> flock = GetNearbyObjects(this.GetComponent<GY_Crow>());
        foreach (Transform agent in flock)
        {
            steering += (Vector2)agent.gameObject.GetComponent<GY_Crow>().Move();
        }
        if (flock.Count > 0)
        {
            steering /= flock.Count;
        }
        if (flock.Count <= 0)
        {
            steering = (Vector2)GetComponent<GY_Crow>().Move();
        }
        return steering;
    }

    /// <summary>
    /// Takes average positions of creatures in common neighbour circle in order to add another one.
    /// </summary>
    private Vector2 Cohesion()
    {
        Vector2 steering = Vector2.zero;
        List<Transform> flock = GetNearbyObjects(this.GetComponent<GY_Crow>());
        foreach (Transform agent in flock)
        {
            steering += (Vector2)agent.position;
        }
        if (flock.Count > 0)
        {
            steering /= flock.Count;
            steering -= (Vector2)transform.position;
        }

        return steering;
    }

    /// <summary>
    /// Separating creatures which are in common neighbour circle from each other.
    /// </summary>
    private Vector2 Separation()
    {
        Vector2 steering = Vector2.zero;
        List<Transform> flock = GetNearbyObjects(this.GetComponent<GY_Crow>());
        foreach (Transform agent in flock)
        {
            Vector2 diff = this.transform.position - agent.position;
            diff /= Vector2.Distance(this.transform.position, agent.position);
            steering += diff;
        }
        if (flock.Count > 0)
        {
            steering /= flock.Count;
        }
        return steering;
    }
    /// <summary>
    /// Getting creatures that agent's neighbour circle covers.
    /// </summary>
    private List<Transform> GetNearbyObjects(GY_Crow agent)
    {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighborRadius, GetComponent<GY_Crow>().enemyLayers);
        foreach (Collider2D c in contextColliders)
        {
            if (c != this.GetComponent<GY_Crow>().AgentCollider)
            {
                context.Add(c.transform);
            }
        }
        return context;
    }
}
#endregion

#endregion





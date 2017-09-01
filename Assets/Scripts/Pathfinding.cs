using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Pathfinding : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform destination;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.avoidancePriority = Random.Range(1, 100);
    }

    public void MoveTo()
    {
        agent.SetDestination(destination.position);
    }

    public bool AtDestination()
    {
        if (destination)
        {
            if ((transform.position - destination.position).magnitude < 0.2f)
                return true;
            else return false;
        }
        return true;
    }
}

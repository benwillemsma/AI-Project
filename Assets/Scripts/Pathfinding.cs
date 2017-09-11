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

    public void MoveTo(Transform destination)
    {
        this.destination = destination;
        agent.avoidancePriority = Random.Range(1, 99);
        agent.SetDestination(destination.position);
    }

    public bool AtDestination(Transform newDestination)
    {
        if (newDestination)
        {
            if ((transform.position - newDestination.position).magnitude < 0.2f)
            {
                agent.avoidancePriority = 100;
                return true;
            }
            else return false;
        }
        return false;
    }
    public bool AtDestination()
    {
        return AtDestination(destination);
    }
}

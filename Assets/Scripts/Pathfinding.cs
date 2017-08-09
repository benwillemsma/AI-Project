using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform destination;

    public void MoveTo(Transform destination = null)
    {
        if(destination != null)
            this.destination = destination;
        transform.position = Vector3.MoveTowards(transform.position, this.destination.position, 0.05f);
    }

    public bool AtDestination()
    {
        if (destination)
        {
            if ((transform.position - destination.position).magnitude < 0.2f)
                return true;
        }
        return false;
    }

    public bool FindPath()
    {
        //if path found
        return true;
        //else
        return false;
    }
}

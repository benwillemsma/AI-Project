using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum studentStats
{
    Energy,
    Happyness,
    Stamina,
    Count
}
public enum studentResources
{
    Money,
    CourseWork,
    Count
}

[RequireComponent(typeof(Pathfinding))]
public class Student : MonoBehaviour
{
    #region Student Initilization
    public List<Course> courses;
    private Pathfinding pathing;

    private Interactable currentObject;
    private Queue<Activity> currentActivity = new Queue<Activity>();
    private Activity walking;

    private void Start()
    {
        GameController.instance.students.Add(this);

        pathing = GetComponent<Pathfinding>();
        walking = new Activity("Walk",new float[] { -0.05f, 0, -0.05f });

        for (int i = 0; i < (int)studentStats.Count; i++)
            stats[i] = 10;
        for (int i = 0; i < (int)studentResources.Count; i++)
            resources.Add(0);
    }
    #endregion

    #region Student Stats and Resources
    public List<float> stats;
    private float Energy
    {
        get { return stats[(int)studentStats.Energy]; }
        set { stats[(int)studentStats.Energy] = value; }
    }
    private float Happyness
    {
        get { return stats[(int)studentStats.Happyness]; }
        set { stats[(int)studentStats.Happyness] = value; }
    }
    private float Stamina
    {
        get { return stats[(int)studentStats.Stamina]; }
        set { stats[(int)studentStats.Stamina] = value; }
    }

    public List<float> resources;
    private float CourseWork
    {
        get { return resources[(int)studentResources.CourseWork]; }
        set { resources[(int)studentResources.CourseWork] = value; }
    }
    private float Money
    {
        get { return resources[(int)studentResources.Money]; }
        set { resources[(int)studentResources.Money] = value; }
    }
    
    public List<float> setStats(params float[] newStats)
    {
        if (stats.Count == newStats.Length)
            for (int i = 0; i < stats.Count; i++)
                stats[i] = newStats[i];
        return stats;
    }
    public List<float> changeStats(params float[] statDeltas)
    {
        if (stats.Count == statDeltas.Length)
            for (int i = 0; i < stats.Count; i++)
                stats[i] += statDeltas[i] * Time.deltaTime;
        return stats;
    }
    public List<float> changeStatsDirect(params float[] statDeltas)
    {
        if (stats.Count == statDeltas.Length)
            for (int i = 0; i < stats.Count; i++)
                stats[i] += statDeltas[i];
        return stats;
    }

    public List<float> setResources(params float[] newResources)
    {
        if (resources.Count == newResources.Length)
            for (int i = 0; i < resources.Count; i++)
                resources[i] = newResources[i];
        return resources;
    }
    public List<float> changeResources(params float[] resourceDeltas)
    {
        if (resources.Count == resourceDeltas.Length)
            for (int i = 0; i < resources.Count; i++)
                resources[i] += resourceDeltas[i] * Time.deltaTime;
        return resources;
    }
    public List<float> changeResourcesDirect(params float[] resourceDeltas)
    {
        if (resources.Count == resourceDeltas.Length)
            for (int i = 0; i < resources.Count; i++)
                resources[i] += resourceDeltas[i];
        return resources;
    }
    #endregion

    #region Student Logic
    private void Update()
    {
        Happyness = (Energy + Stamina + Money) / 3;

        // Base Logic
        if (Energy <= 0)
            Die();

        // If Student does not have a current Activity Assign one.
        else if (currentActivity.Count == 0)
        {
            if (Money == 0)
                FindInteractable(InteractableType.Job);
            else if (CourseWork == 0)
                FindInteractable(InteractableType.Book);
            else if (Energy < Stamina && Energy < 5)
                FindInteractable(InteractableType.FoodSource);
            else if(Stamina < 5)
                FindInteractable(InteractableType.Bed);
            else
                FindInteractable(InteractableType.Desk);
        }

        // If Student has an Activity, do Activity.
        if (currentActivity.Count > 0)
        {
            if (currentActivity.Peek().isDone(this) == true || pathing.destination == null)
            {
                currentObject.InUse = null;
                currentObject = null;
                currentActivity.Dequeue();
            }
        }
    }
    private void OnDestroy()
    {
        GameController.instance.students.Remove(this);
        while (courses.Count > 0)
            courses[0].KickStudent(this);
    }

    // Find Functions
    void FindInteractable(InteractableType type)
    {
        float closestDistance = Mathf.Infinity;

        Interactable[] objects = GameController.instance.FindOfType(type);
        if (objects.Length == 0)
        {
            Debug.LogWarning("There are not enough " + type);
            //Build one...
        }
        else
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i])
                {
                    float distance = (objects[i].transform.position - transform.position).magnitude;
                    if (distance <= closestDistance)
                    {
                        closestDistance = distance;
                        currentObject = objects[i];
                    }
                }
            }

            if (closestDistance > 0.2f)
                StartCoroutine(Travel(currentObject));
            else currentActivity.Enqueue(currentObject.activity);
        }
    }

    //Activities
    private IEnumerator Travel(Interactable interactableObject)
    {
        if (interactableObject.InUse)
        {
            if (GameController.FindCloser(transform, interactableObject.InUse.transform, interactableObject.activityPoint.position) == transform)
                interactableObject.InUse = this;
            else yield break;
        }
        else interactableObject.InUse = this;

        pathing.destination = interactableObject.activityPoint;
        transform.LookAt(transform.position + (pathing.destination.position - transform.position).normalized);

        currentActivity.Enqueue(walking);
        pathing.MoveTo();

        bool thereYet = false;
        while (!thereYet)
        {
            if (pathing.AtDestination() || this != interactableObject.InUse)
                thereYet = true;
            yield return null;
        }
        if(currentActivity.Count > 0)
            currentActivity.Dequeue();
        if (pathing.AtDestination())
        {
            transform.rotation = interactableObject.activityPoint.rotation;
            currentActivity.Enqueue(interactableObject.activity);
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
    #endregion
}
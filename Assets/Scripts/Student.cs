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

    [SerializeField]
    private Queue<Activity> currentState = new Queue<Activity>();
    private Activity walking;

    private void Start()
    {
        pathing = GetComponent<Pathfinding>();
        walking = new Activity("Walk",new float[] { -0.01f, 0, -0.05f });

        for (int i = 0; i < (int)studentStats.Count; i++)
            stats[i] = 10;
        for (int i = 0; i < (int)studentResources.Count; i++)
            resources.Add(0);
        Money = 10;


        GameController.instance.students.Add(this);
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

    //For Settig multiple stats
    private List<float> setStats(params float[] newStats)
    {
        if (stats.Count == newStats.Length)
            for (int i = 0; i < stats.Count; i++)
                stats[i] = newStats[i];
        return stats;
    }
    //For Changing mutiple stats by a delta per second
    public List<float> changeStats(params float[] statDeltas)
    {
        if (stats.Count == statDeltas.Length)
            for (int i = 0; i < stats.Count; i++)
                stats[i] += statDeltas[i] * Time.deltaTime;
        return stats;
    }

    //For Settig multiple resources 
    private List<float> setResources(params float[] newResources)
    {
        if (resources.Count == newResources.Length)
            for (int i = 0; i < resources.Count; i++)
                resources[i] = newResources[i];
        return resources;
    }
    //For Changing mutiple resources by a delta per second
    public List<float> changeResource(params float[] resourceDeltas)
    {
        if (resources.Count == resourceDeltas.Length)
            for (int i = 0; i < resources.Count; i++)
                resources[i] += resourceDeltas[i] * Time.deltaTime;
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

        // If Student does not have a current state Assign one.
        else if (currentState.Count == 0)
        {
            if (Energy < Stamina)
                FindInteractable(InteractableType.FoodSource);
            else
                FindInteractable(InteractableType.Bed);
        }
        else if (Energy < 5) //<-running low on Energy
            FindInteractable(InteractableType.FoodSource);

        else if (Stamina < 5) //<-running low on Stamina
            FindInteractable(InteractableType.Bed);

        if (currentState.Count > 0)
        {
            Debug.Log(currentState.Peek().activityName);
            currentState.Peek().DoActivity(this);
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
        Debug.Log("here");
        float closestDistance = Mathf.Infinity;
        Interactable closestObject = null;

        Interactable[] objects = GameController.instance.FindOfType(type);
        if (objects.Length == 0)
            Debug.LogWarning("There are not enough " + type);
        else
        {
            for (int i = 0; i < objects.Length; i++)
            {
                float distance = (objects[i].transform.position - transform.position).magnitude;
                if (distance <= closestDistance && !objects[i].InUse)
                {
                    closestDistance = distance;
                    closestObject = objects[i];
                }
            }

            if (closestDistance > 0.2f)
                StartCoroutine(Travel(closestObject));
            else currentState.Enqueue(closestObject.activity);
        }
    }

    //Activities
    private IEnumerator Travel(Interactable interactableObject)
    {
        Debug.Log("here");
        currentState.Enqueue(walking);

        bool thereYet = false;
        while (!thereYet)
        {
            if (pathing.AtDestination() || interactableObject.InUse)
                break;
            else
                pathing.MoveTo(interactableObject.activityPoint);
            yield return null;
        }
        currentState.Dequeue();
        if (pathing.AtDestination())
        {
            transform.rotation = interactableObject.activityPoint.rotation;
            interactableObject.InUse = true;
            currentState.Enqueue(interactableObject.activity);
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
    #endregion
}
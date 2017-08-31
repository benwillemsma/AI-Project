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
    private Pathfinding pathing;

    private Interactable currentObject;
    private Queue<Activity> currentActivity = new Queue<Activity>();
    private Activity walking;
    private Activity pendingActivity = null;
    
    public Course currentCourse;
    private Dictionary<Course, bool> passedCourses = new Dictionary<Course, bool>();

    private void Start()
    {
        GameController.instance.students.Add(this);

        pathing = GetComponent<Pathfinding>();
        walking = new Activity("Walk",new float[] { -0.05f, 0, -0.1f });

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
        else if (currentActivity.Count == 0)
        {
            Debug.Log(name + ":" + currentActivity.Count);
            if (Money == 0)
                FindInteractable(InteractableType.Job);
            else if (Energy < 5 && Energy < Stamina)
                FindInteractable(InteractableType.FoodSource);
            else if (Stamina < 5)
                FindInteractable(InteractableType.Bed);
            else if (CourseWork == 0)
                FindInteractable(InteractableType.Book);
            else
                FindInteractable(InteractableType.Desk);
        }
        else if (currentActivity.Count > 0)
        {
            Debug.Log(name + ":" + currentActivity.Peek().activityName);
            if (currentActivity.Peek().isDone(this) == true || !currentObject)
            {
                if (currentObject)
                {
                    currentObject.InUse = false;
                    currentObject = null;
                }
                currentActivity.Dequeue();
            }
        }
    }
    private void OnDestroy()
    {
        GameController.instance.students.Remove(this);
        if(currentCourse != null)
        currentCourse.KickStudent(this);
    }

    public void AddActivity(Activity newActivity)
    {
        pendingActivity = newActivity;
    }

    // Course Functions
    public void FindNextCourse()
    {
        currentCourse = GetDependency(GameController.goalCourse);
    }

    public Course GetDependency(Course checkCourse)
    {
        for (int i = 0; i < checkCourse.PreReq.Count; i++)
        {
            Course temp = GetDependency(checkCourse.PreReq[i]);
            if (GetDependency(checkCourse.PreReq[i]) != null)
                return temp;
        }
        if (passedCourses[GameController.goalCourse])
            return null;
        else return checkCourse;
    }

    // Find Functions
    void FindInteractable(InteractableType type)
    {
        Interactable[] objects = GameController.instance.FindInteractable(type);
        if (objects.Length == 0 && type != InteractableType.Build)
        {
            if (!FindConstruction(type))
            {
                FindInteractable(InteractableType.Build);

                if (currentObject)
                {
                    (currentObject as Job).progress.Invoke();
                    GameController.instance.BuildRoom(type, currentObject.transform);
                }
            }
        }
        else
        {
            currentObject = GameController.FindClosest(objects, transform);
            StartCoroutine(Travel(currentObject));
        }
    }

    public bool FindConstruction(InteractableType type)
    {
        Construction[] objects = GameController.instance.FindConstruction(type);
        if (objects.Length == 0 && type != InteractableType.Build)
        {
            return false;
        }
        else
        {
            Construction temp = GameController.FindClosest(objects, transform);
            Interactable[] jobs = temp.GetComponentsInChildren<Interactable>();
            StartCoroutine(Travel(GameController.FindClosest(jobs, transform)));
        }
        return true;
    }

    //Activities
    private IEnumerator Travel(Interactable interactableObject)
    {
        if (interactableObject)
        {

            pathing.destination = interactableObject.activityPoint;
            transform.LookAt(transform.position + (pathing.destination.position - transform.position).normalized);

            currentActivity.Enqueue(walking);
            pathing.MoveTo();

            bool thereYet = false;
            while (!thereYet)
            {
                if (pathing.AtDestination() || interactableObject.InUse)
                    thereYet = true;
                yield return null;
            }
            if (currentActivity.Count > 0)
                currentActivity.Dequeue();
            if (pathing.AtDestination())
            {
                interactableObject.InUse = true;
                currentActivity.Enqueue(interactableObject.activity);
            }
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
    #endregion
}
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

    private Interactable currentInteractable;
    private Stack<IEnumerator> currentState = new Stack<IEnumerator>();

    public Course currentCourse;
    private Dictionary<Course, bool> passedCourses = new Dictionary<Course, bool>();

    private static bool initActivities = true;
    private static Activity Wait;
    private static Activity Walk;
    private static Activity Eat;
    private static Activity Sleep;
    private static Activity Read;
    private static Activity Study;
    private static Activity Work;

    private void Start()
    {
        Manager.instance.students.Add(this);
        pathing = GetComponent<Pathfinding>();

        if (initActivities)
            InitActivities();
        FindNextCourse();

        currentState.Push(AddWait());
    }

    private void InitActivities()
    {
        Wait = new Activity("Waiting", new float[] { -0.1f, 0, -0.2f });
        Walk = new Activity("Walking", new float[] { -0.1f, 0, -0.2f });

        Sleep = new Activity("Sleeping", new float[] { 0, 0, 1f });
        Eat = new Activity("Eating", new float[] { 1f, 0, 0 }, new float[] { -0.5f, 0 });
        Work = new Activity("Working", new float[] { -0.1f, 0, -0.2f }, new float[] { 1, 0 });

        Read = new Activity("Reading", new float[] { -0.1f, 0, -0.2f }, new float[] { 0, 1 });
        Study = new Activity("Studying", new float[] { -0.1f, 0, -0.2f }, new float[] { 0, -1 });

        initActivities = false;
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
        if (Energy <= 0)
            Die();

        if (currentState.Count < 2)
        {
            if (Money <= 0)
                AddState(Work, InteractableType.Job);
            else if (Energy < 5 && Energy < Stamina)
                AddState(Eat, InteractableType.FoodSource);
            else if (Stamina < 5)
                AddState(Sleep, InteractableType.Bed);
            else if (CourseWork <= 0)
                AddState(Read, InteractableType.Book);
            else
                AddState(Study, InteractableType.Desk);
        }

        if (currentState.Count > 0)
        {
            //Debug.Log(gameObject.name + ":  :" + currentState.Count + ":  :" + currentState.Peek() + ":  :" + currentInteractable);
            if (!currentState.Peek().MoveNext() || !currentInteractable)
            {
                currentState.Pop();
                if (currentInteractable)
                {
                    currentInteractable.InUse = null;
                    currentInteractable = null;
                }
            }
        }
    }
    private void OnDestroy()
    {
        Manager.instance.students.Remove(this);
        if(currentCourse != null)
            currentCourse.KickStudent(this);
    }

    // Course Functions
    private void FindNextCourse()
    {
        currentCourse = GetDependency(Manager.goalCourse);
    }

    private Course GetDependency(Course checkCourse)
    {
        if (checkCourse == null)
            return null;
        if (checkCourse.PreReq.Count > 0)
        {
            for (int i = 0; i < checkCourse.PreReq.Count; i++)
            {
                Course temp = GetDependency(checkCourse.PreReq[i]);
                if (GetDependency(checkCourse.PreReq[i]) != null)
                    return temp;
            }
            if (passedCourses[Manager.goalCourse])
                return null;
            else return checkCourse;
        }
        else return checkCourse;
    }

    //Activities
    private void Die()
    {
        Destroy(gameObject);
    }

    private IEnumerator AddWait()
    {
        while (true)
            yield return DoActivity(Wait).MoveNext();
    }

    private void AddState(Activity newActivity, InteractableType type)
    {
        // Try to Find Interactable of Type.
        if (currentInteractable = FindInteractable(type))
        {
            currentInteractable.InUse = this;
            if (pathing.AtDestination(currentInteractable.activityPoint.transform))
                currentState.Push(DoActivity(newActivity));
            else currentState.Push(Travel(currentInteractable));
        }
        // Failed to Find Interactable, Try to Find Construction of Type.
        else if ((currentInteractable = FindConstruction(type)))
        {
            currentInteractable.InUse = this;
            if (pathing.AtDestination(currentInteractable.activityPoint.transform))
                currentState.Push(DoActivity(Work, currentInteractable));
            else currentState.Push(Travel(currentInteractable));
        }
        // Failed to Find Construction, Start a new Construction Project.
        else
        {
            if (currentInteractable = FindInteractable(InteractableType.Build))
            {
                (currentInteractable).progress.Invoke();
                if (currentInteractable = Manager.instance.BuildRoom(type, currentInteractable.activityPoint))
                {
                    currentInteractable.InUse = this;
                    if (pathing.AtDestination(currentInteractable.activityPoint.transform))
                        currentState.Push(DoActivity(Work, currentInteractable));
                    else currentState.Push(Travel(currentInteractable));
                }
            }
        }
    }

    private IEnumerator DoActivity(Activity activity, Interactable jobObject = null)
    {
        if (jobObject)
            jobObject.progress.Invoke();
        if (activity.oneUse)
        {
            changeStatsDirect(activity.statsDelta.ToArray());
            changeResourcesDirect(activity.resourcesDelta.ToArray());
        }
        else
        {
            changeStats(activity.statsDelta.ToArray());
            changeResources(activity.resourcesDelta.ToArray());
            while (!activity.isDone(this))
            {
                yield return null;
                changeStats(activity.statsDelta.ToArray());
                changeResources(activity.resourcesDelta.ToArray());
            }
        }
    }

    private IEnumerator Travel(Interactable Object)
    {
        if (Object)
        {
            pathing.destination = Object.activityPoint;
            if (!pathing.AtDestination() && Object.InUse == this)
            {
                pathing.MoveTo();

                bool thereYet = false;
                while (!thereYet)
                {
                    if (pathing.AtDestination() || !Object.InUse == this)
                        thereYet = true;
                    DoActivity(Walk).MoveNext();
                    yield return null;
                }
            }
            currentInteractable = null;
        }
    }

    // Find Functions
    private Interactable FindInteractable(InteractableType type)
    {
        Interactable[] objects = Manager.instance.FindInteractable(type, this);
        if (objects.Length <= 0)
            return null;
        else
            return Manager.FindClosest(objects, transform);
    }

    private Interactable FindConstruction(InteractableType type)
    {
        Construction[] objects = Manager.instance.FindConstruction(type);
        if (objects.Length == 0 && type != InteractableType.Build)
            return null;
        else
        {
            Construction temp = Manager.FindClosest(objects, transform);
            Interactable[] jobs = temp.GetComponentsInChildren<Interactable>();
            return Manager.FindClosest(jobs, transform);
        }
    }
    #endregion
}
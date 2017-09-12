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

    [HideInInspector]
    public ComputerLab currentLab = null;
    [HideInInspector,Course]
    public Course currentCourse = null;

    private Dictionary<Course, bool> passedCourses = new Dictionary<Course, bool>();

    private static bool initActivities = true;
    private static Activity Wait;
    private static Activity Walk;
    private static Activity Eat;
    private static Activity Sleep;
    private static Activity Read;
    private static Activity Study;
    private static Activity Work;
    private static Activity OpenLab;

    private void Start()
    {
        Manager.instance.students.Add(this);

        foreach (KeyValuePair<string, Course> course in Manager.instance.courses)
            passedCourses.Add(course.Value, false);

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
        OpenLab = new Activity("PayForClass", new float[] { -0.1f, 0, -0.2f }, new float[] { -0.5f, 0 });

        initActivities = false;
    }
    #endregion

    #region Student Stats and Resources
    [HideInInspector]
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

    [HideInInspector]
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
    public void Graduate(Course course)
    {
        passedCourses[course] = true;
        currentLab = null;
        FindNextCourse();
    }

    private void FindNextCourse()
    {
        currentCourse = GetDependency(Manager.goalCourse);

        if (currentCourse == null)
        {
            //Dance and recoice bacuse you Graduated!
            Destroy(gameObject);
        }
        else currentCourse.EnrollStudent(this);
    }

    private Course GetDependency(Course checkCourse)
    {
        if (checkCourse == null)
            return null; //return if checkCourse doesn't exist

        if (passedCourses[checkCourse])
            return null; // Return null if checkCourse is already passed
        else // else check Course PreRec
        {
            for (int i = 0; i < checkCourse.PreReq.Count; i++)
            {

                if (GetDependency(checkCourse.PreReq.GetPreReq(i)) != null)
                    return GetDependency(checkCourse.PreReq.GetPreReq(i)); // Return dependency recursivly
            }
            return checkCourse; // return checkCourse if all dependecies passed
        }
    }

    private void AddState(Activity newActivity, InteractableType type)
    {
        if (type == InteractableType.Desk && (currentInteractable = FindLab()))
        {
            if (currentInteractable.type == type)
            {
                currentInteractable.InUse = this;
                if (pathing.AtDestination(currentInteractable.activityPoint.transform))
                    currentState.Push(DoActivity(newActivity, currentInteractable));
                else currentState.Push(Travel(currentInteractable));
            }
            else
            {
                currentInteractable.InUse = this;
                if (pathing.AtDestination(currentInteractable.activityPoint.transform))
                    currentState.Push(DoActivity(OpenLab, currentInteractable));
                else currentState.Push(Travel(currentInteractable));
            }
        }
        // Try to Find Interactable of Type.
        else if ((currentInteractable = FindInteractable(type)) && type != InteractableType.Desk)
        {
            currentInteractable.InUse = this;
            if (pathing.AtDestination(currentInteractable.activityPoint.transform))
                currentState.Push(DoActivity(newActivity, currentInteractable));
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
        else if (currentInteractable = FindInteractable(InteractableType.Build))
        {
    
            (currentInteractable).progress.Invoke();
            if (currentInteractable = Manager.instance.BuildRoom(type, currentInteractable.activityPoint))
            {
                currentInteractable.findActivityPoint();
                currentInteractable.InUse = this;
            }
        }
    }

    private void Die()
    {
        Debug.Log("Student Died");
        Destroy(gameObject);
    }

    //Activities
    private IEnumerator AddWait()
    {
        while (true)
            yield return DoActivity(Wait, null).MoveNext();
    }

    private IEnumerator DoActivity(Activity activity, Interactable jobObject)
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

                if (jobObject)
                    jobObject.progress.Invoke();
            }
        }
    }

    private IEnumerator Travel(Interactable Object)
    {
        if (Object)
        {
            pathing.MoveTo(Object.activityPoint);
            if (!pathing.AtDestination() && Object.InUse == this)
            {

                bool thereYet = false;
                while (!thereYet)
                {
                    if (pathing.AtDestination() || !Object.InUse == this)
                        thereYet = true;
                    DoActivity(Walk, null).MoveNext();
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
        if (objects.Length > 0)
            return Manager.FindClosest(objects, transform);
        return null;
    }

    private Interactable[] FindAvailable(Interactable[] array, Student reference)
    {
        List<Interactable> list = new List<Interactable>();
        list.AddRange(array);
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].InUse == null || list[i].InUse == reference)
                continue;
            list.RemoveAt(i);
        }
        return list.ToArray();
    }

    private Interactable FindConstruction(InteractableType type)
    {
        Construction[] objects = Manager.instance.FindConstruction(type);
        if (objects.Length > 0 && type != InteractableType.Build)
        {
            Construction temp = Manager.FindClosest(objects, transform);
            Interactable[] jobs = temp.GetComponentsInChildren<Interactable>();
            jobs = FindAvailable(jobs, this);
            return Manager.FindClosest(jobs, transform);
        }
        return null;
    }

    private Interactable FindLab()
    {
        if (currentLab)
        {
            if (currentLab.isOpen)
            {
                Interactable[] desks = currentLab.gameObject.GetComponentsInChildren<Interactable>();
                desks = FindAvailable(desks, this);
                return Manager.FindClosest(desks, transform);
            }
            else return currentLab.Lab;
        }
        else
        {
            ComputerLab[] objects = Manager.instance.FindComputerLab(this);
            if (objects.Length > 0)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i].labCourse == currentCourse)
                    {
                        currentLab = objects[i];
                        if(currentLab.isOpen)
                            return FindLab();
                    }
                }
                return Manager.FindClosest(objects, transform).Lab;
            }
        }
        return null;
    }
    #endregion
}
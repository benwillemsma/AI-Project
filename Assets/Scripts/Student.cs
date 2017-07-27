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

public class Resource
{
    public string name;
    public float value;

    public Resource(string name, float value)
    {
        this.name = name;
        this.value = value;
    }
    public Resource(string name)
    {
        this.name = name;
        this.value = 0;
    }
}
public class StudentStat : Resource
{
    public float maxValue;
    public float minValue;

    public StudentStat(string name, float value, float maxValue, float minValue) : base(name, value)
    {
        this.maxValue = maxValue;
        this.minValue = minValue;
    }
    public StudentStat(string name) : base(name)
    {
        this.maxValue = 10;
        this.minValue = 0;
    }
}

public class Student : MonoBehaviour
{
    // Stats and Resources
    public List<Course> courses = new List<Course>();
    
    public List<StudentStat> stats = new List<StudentStat>();
    public StudentStat Energy
    {
        get { return stats[(int)studentStats.Energy]; }
        set { stats[(int)studentStats.Energy] = value; }
    }
    public StudentStat Happyness
    {
        get { return stats[(int)studentStats.Happyness]; }
        set { stats[(int)studentStats.Happyness] = value; }
    }
    public StudentStat Stamina
    {
        get { return stats[(int)studentStats.Stamina]; }
        set { stats[(int)studentStats.Stamina] = value; }
    }
    
    public List<Resource> resources = new List<Resource>();
    public Resource CourseWork
    {
        get { return resources[(int)studentResources.CourseWork]; }
        set { resources[(int)studentResources.CourseWork] = value; }
    }
    public Resource Money
    {
        get { return resources[(int)studentResources.Money]; }
        set { resources[(int)studentResources.Money] = value; }
    }

    // Variables

    private void Start()
    {
        //Initial Stats
        for (int i = 0; i < (int)studentStats.Count; i++)
            stats.Add(new StudentStat(System.Enum.GetNames(typeof(studentStats))[i]));

        Happyness.value = 10;
        Energy.value = 10;
        Stamina.value = 10;

        //Initial Resources
        for (int i = 0; i < (int)studentResources.Count; i++)
            resources.Add(new StudentStat(System.Enum.GetNames(typeof(studentResources))[i]));

        Money.value = 10;

        //Add student to apropriate lists
        GameController.instance.students.Add(this);
        GameController.instance.courses[0].EnrollStudent(this);
        
    }
    private void Update()
    {
        if(Stamina.value  < 5)
        StartCoroutine(Travel(FindBed()));

        if (Energy.value < 5)
            StartCoroutine(Travel(FindFood()));


        if (stats[(int)studentStats.Energy].value <= 0)
            Die();

        Happyness.value
           = (stats[(int)studentStats.Energy].value
           + stats[(int)studentStats.Stamina].value
           + resources[(int)studentResources.Money].value) / 3;

    }
    private void OnDestroy()
    {
        GameController.instance.students.Remove(this);
        while (courses.Count > 0)
            courses[0].KickStudent(this);
    }

    //For Settig multiple stats
    private List<StudentStat> setStats(params float[] newStats)
    {
        if (stats.Count == newStats.Length)
            for (int i = 0; i < stats.Count; i++)
                stats[i].value = newStats[i];
        return stats;
    }
    //For Changing mutiple stats by a delta
    public List<StudentStat> changeStats(params float[] statDeltas)
    {
        if(stats.Count == statDeltas.Length)
            for (int i = 0; i < stats.Count; i++)
                stats[i].value += statDeltas[i];
        return stats;
    }

    //For Settig multiple resources
    private List<Resource> setResources(params float[] newResources)
    {
        if (resources.Count == newResources.Length)
            for (int i = 0; i < resources.Count; i++)
                resources[i].value = newResources[i];
        return resources;
    }
    //For Changing mutiple resources by a delta
    public List<Resource> changeResource(params float[] resourceDeltas)
    {
        if (resources.Count == resourceDeltas.Length)
            for (int i = 0; i < resources.Count; i++)
                resources[i].value += resourceDeltas[i];
        return resources;
    }

    // Logic Functions
    Interactable FindBed()
    {
        return new Interactable();
    }

    Interactable FindFood()
    {
        return new Interactable();
    }

    //Activities
    private IEnumerator Travel(Interactable activyObject)
    {
        bool thereYet = true;
        while (!thereYet)
        {
            Stamina.value -= Time.deltaTime / 10;
            Energy.value -= Time.deltaTime / 100;
            //check activyObject is in use by other student

            //pathfinding stuff to find activyObject

            yield return null;
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum studentStats
{
    Hunger,
    Happyness,
    Stamina,
    Count
}
public enum studentResources
{
    CourseWork,
    Money,
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
        this.maxValue = 1;
        this.minValue = 0;
    }
}

public class Student : MonoBehaviour
{
    [SerializeField]
    public List<Course> courses = new List<Course>();

    [SerializeField]
    public List<StudentStat> stats = new List<StudentStat>();
    public StudentStat Hunger
    {
        get { return stats[(int)studentStats.Hunger]; }
        set { stats[(int)studentStats.Hunger] = value; }
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

    [SerializeField]
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

    private void Start()
    {
        //Initial Stats
        for (int i = 0; i < (int)studentStats.Count; i++)
            stats.Add(new StudentStat(System.Enum.GetNames(typeof(studentStats))[i]));

        Hunger.value = 1;
        Happyness.value = 1;
        Stamina.value = 15;
        Stamina.maxValue = 15;
        Stamina.minValue = 0;

        //Initial Resources
        for (int i = 0; i < (int)studentResources.Count; i++)
            resources.Add(new StudentStat(System.Enum.GetNames(typeof(studentResources))[i]));

        //Add student to apropriate lists
        GameController.instance.students.Add(this);
        GameController.instance.courses[0].EnrollStudent(this);
        
    }
    private void Update()
    {
        if (Stamina.value > 0)
            Stamina.value -= Time.deltaTime / 10;
        else
            Stamina.value = 0;

        if (stats[(int)studentStats.Hunger].value <= 0)
            Die();
    }
    private void OnDestroy()
    {
        GameController.instance.students.Remove(this);
        while (courses.Count > 0)
            courses[0].KickStudent(this);
    }

    private List<StudentStat> setStats(params float[] newStats)
    {
        if (stats.Count == newStats.Length)
            for (int i = 0; i < stats.Count; i++)
                stats[i].value = newStats[i];
        return stats;
    }
    private List<StudentStat> changeStats(params float[] statDeltas)
    {
        if(stats.Count == statDeltas.Length)
            for (int i = 0; i < stats.Count; i++)
                stats[i].value += statDeltas[i];
        return stats;
    }

    private List<Resource> setResources(params float[] newResources)
    {
        if (resources.Count == newResources.Length)
            for (int i = 0; i < resources.Count; i++)
                resources[i].value = newResources[i];
        return resources;
    }
    private List<Resource> changeResource(params float[] resourceDeltas)
    {
        if (resources.Count == resourceDeltas.Length)
            for (int i = 0; i < resources.Count; i++)
                resources[i].value += resourceDeltas[i];
        return resources;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
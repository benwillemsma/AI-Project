using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Activity
{
    public string activityName;
    public List<float> statsDelta = new List<float>();
    public List<float> resourcesDelta = new List<float>();

    public Activity() { }
    public Activity(string name, float[] newStatsDeltas,params float[] newResourcesDelta)
    {
        activityName = name;
        statsDelta.AddRange(newStatsDeltas);
        resourcesDelta.AddRange(newResourcesDelta);
    }

    public void DoActivity(Student studentReference)
    {
        studentReference.changeStats(statsDelta.ToArray());
        studentReference.changeResource(resourcesDelta.ToArray());
    }
}

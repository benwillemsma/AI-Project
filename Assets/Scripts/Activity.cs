using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Activity
{
    public string activityName;
    public bool oneUse = false;
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
        if (oneUse)
        {
            studentReference.changeStatsDirect(statsDelta.ToArray());
            studentReference.changeResourcesDirect(resourcesDelta.ToArray());
        }
        else
        {   
            studentReference.changeStats(statsDelta.ToArray());
            studentReference.changeResources(resourcesDelta.ToArray());
        }
    }

    public bool isDone(Student studentReference)
    {
        DoActivity(studentReference);
        bool done = false;

        for (int i = 0; i < statsDelta.Count; i++)
        {
            if (statsDelta[i] > 0 && studentReference.stats[i] >= 10)
            {
                studentReference.stats[i] = 10;
                done = true;
            }
            else if (statsDelta[i] < 0 && studentReference.stats[i] <= 0)
            {
                studentReference.stats[i] = 0;
                done = true;
            }
        }
        for (int i = 0; i < resourcesDelta.Count; i++)
        {
            if (resourcesDelta[i] > 0 && studentReference.resources[i] >= 10)
            {
                studentReference.resources[i] = 10;
                done = true;
            }
            else if (resourcesDelta[i] < 0 && studentReference.resources[i] <= 0)
            {
                studentReference.resources[i] = 0;
                done = true;
            }
        }
        if (oneUse)
            done = true;

        return done;
    }
}

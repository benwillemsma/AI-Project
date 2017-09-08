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
    public Activity(Activity copyActivity)
    {
        activityName = copyActivity.activityName;
        statsDelta = copyActivity.statsDelta;
        resourcesDelta = copyActivity.resourcesDelta;
    }
    public Activity(string name, float[] newStatsDeltas,params float[] newResourcesDelta)
    {
        activityName = name;
        statsDelta.AddRange(newStatsDeltas);
        resourcesDelta.AddRange(newResourcesDelta);
    }

    public Activity(string name, bool oneUse, float[] newStatsDeltas, params float[] newResourcesDelta)
    {
        this.oneUse = oneUse;
        activityName = name;
        statsDelta.AddRange(newStatsDeltas);
        resourcesDelta.AddRange(newResourcesDelta);
    }

    public bool isDone(Student studentReference)
    {
        bool done = false;
        if (oneUse)
        {
            done = true;
        }
        else
        {
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
        }
        return done;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job : Activity
{
    public delegate float Progress();
    public Progress progress;

    public Job(string name, float[] newStatsDeltas, params float[] newResourcesDelta):base(name,newStatsDeltas,newResourcesDelta)
    {
        activityName = name;
        statsDelta.AddRange(newStatsDeltas);
        resourcesDelta.AddRange(newResourcesDelta);
    }
}

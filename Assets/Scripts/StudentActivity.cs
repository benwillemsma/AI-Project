using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentActivity
{
    private Student student;
    private float[] deltaValues;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="studentReference"></param>
    /// <param name="name"></param>
    /// <param name="StatDeltas: Energy, Happyness, Stamina, Money,CourseWork"></param>
    public StudentActivity(Student studentReference,string name, params float[] StatDeltas)
    {
        student = studentReference;

        deltaValues = new float[(int)studentStats.Count + (int)studentResources.Count];

        for (int i = 0; i < StatDeltas.Length; i++)
        {
            if (i < deltaValues.Length)
                deltaValues[i] = StatDeltas[i];
        }
    }

    protected virtual void DoActivity()
    {
        List<float> statsDelta = new List<float>();
        List<float> resourcesDelta = new List<float>();

        for (int i = 0; i < (int)studentStats.Count; i++)
            statsDelta.Add(deltaValues[i]);

        for (int i = (int)studentStats.Count; i < (int)studentStats.Count + (int)studentResources.Count; i++)
            resourcesDelta.Add(deltaValues[i]);

        student.changeStats(statsDelta.ToArray());
        student.changeResource(resourcesDelta.ToArray());
    }
}
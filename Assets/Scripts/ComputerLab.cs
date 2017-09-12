using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerLab : MonoBehaviour
{
    public float GradeDelta = 5;
    public bool isOpen = false;

    private float cost;

    Dictionary<Student, float> grades = new Dictionary<Student, float>();

    [Course]
    public Course labCourse = null;

    [SerializeField]
    private Interactable[] Desks;
    public Interactable Lab;

    public void Start()
    {
        Manager.instance.ComputerLabs.Add(this);
        Lab = GetComponent<Interactable>();
    }

    public void OpenLab(Interactable reference)
    {
        Student student = reference.InUse;
        if (labCourse == null)
        {
            labCourse = student.currentCourse;
            cost = labCourse.courseCost;
        }

        cost -= Time.deltaTime;
        if (cost <= 0)
        {
            //Debug.Log("Lab is Open:" + labCourse.name);
            isOpen = true;
            Lab.InUse = student;
        }
    }

    private void CloseLab()
    {
        //Debug.Log("Lab is CLosed");
        grades.Clear();
        Lab.InUse = null;
        isOpen = false;
        labCourse = null;
    }

    public void ImproveGrade(Interactable reference)
    {
        Student student = reference.InUse;
        if (student)
        {
            if (labCourse == null)
                labCourse = student.currentCourse;

            float newStudent;
            if (!grades.TryGetValue(student, out newStudent))
                grades.Add(student, 0);

            Manager.instance.SchoolReputation += 5 * Time.deltaTime;
            grades[student] += GradeDelta * Time.deltaTime;
            for (int i = 0; i < labCourse.Students.Length; i++)
            {
                if (labCourse.Students[i] != student)
                    grades[student] += GradeDelta * 0.1f * Time.deltaTime;
            }
            if (grades[student] >= 100)
            {
                reference.InUse = null;
                labCourse.GraduateStudent(student);
                if (labCourse.Students.Length <= 0)
                    CloseLab();
            }
        }
    }
}

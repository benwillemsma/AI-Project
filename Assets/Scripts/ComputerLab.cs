using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ComputerLab : MonoBehaviour
{
    public float amountPerUse;
    public bool labOpen;

    Dictionary<Student, float> grades = new Dictionary<Student, float>();

    public Course labCourse = null;

    [SerializeField]
    Interactable[] Desks;
    Interactable openLab;

    public void Start()
    {
        openLab = GetComponent<Interactable>();
    }

    public void Update()
    {
        if (labCourse != null)
        {
            if (labCourse.Students.Length * 10 >= labCourse.courseCost)
            {
                for (int i = 0; i < labCourse.Students.Length; i++)
                    labCourse.Students[i].AddActivity(openLab.activity);
            }
        }
    }

    public void OpenLab()
    {
        if (labCourse.courseCost <= 0)
        {
            labOpen = true;
            openLab.InUse = true;
        }
    }

    public void CloseLab()
    {
        labOpen = false;
        labCourse = null;
        openLab.InUse = false;
    }

    public void ImproveGrade(Student student, float increase)
    {
        GameController.instance.SchoolReputation += 5;
        grades[student] += increase;
        for (int i = 0; i < labCourse.Students.Length; i++)
        {
            if (labCourse.Students[i] != student)
                grades[student] += increase * 0.1f;
        }
        if (grades[student] >= 100)
        {
            labCourse.GraduateStudent(student);
            if (labCourse.Students.Length <= 0)
                CloseLab();
        }
    }
}

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
    Interactable door;

    public void Start()
    {
        door = GetComponent<Interactable>();
    }

    public void OpenLab(Course newCourse)
    {
        labCourse = newCourse;
        if (labCourse.courseCost <= 0)
        {
            labOpen = true;
            door.InUse = true;
        }
    }

    public void CloseLab()
    {
        labOpen = false;
        labCourse = null;
        door.InUse = false;
    }

    public void ImproveGrade(Student student, float increase)
    {
        Manager.instance.SchoolReputation += 5;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerLab : MonoBehaviour
{
    public float amountPerUse;
    public float LabTimeLeft;

    public Course labCourse;
    List<Student> students;

    [SerializeField]
    Transform[] Desks;

    public void AddStudent(Student student)
    {
        students.Add(student);
    }
    public void RemoveStudent(Student student)
    {
        students.Remove(student);
    }

    public void UseLab(Student student)
    {
        student.CourseWork.value--;
        student.courses[student.courses.Count].ImproveGrade(student, amountPerUse);

        for (int i = 0; i < students.Count; i++)
        {
            if (students[i] != student)
                students[i].courses[students[i].courses.Count].ImproveGrade(students[i], amountPerUse * 0.1f);
        }
    }

    public void Update()
    {
        LabTimeLeft -= Time.deltaTime;
    }

    public void OpenLab(float duration)
    {
        //do an open
        LabTimeLeft = duration;
    }

    public void CloseLab(float duration)
    {
        //such an closed
        LabTimeLeft = duration;
    }
}

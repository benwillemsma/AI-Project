using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Course
{
    public string name;
    public ComputerLab lab;
    public float courseCost = 10;

    List<Student> students = new List<Student>();
    Dictionary<Student, float> grades = new Dictionary<Student, float>();

    public Course(string name, ComputerLab lab, float cost, params Student[] students)
    {
        this.name = name;
        this.lab = lab;
        this.courseCost = cost;
        for (int i = 0; i < students.Length; i++)
            this.students.Add(students[i]);
    }
    public Course(string name,ComputerLab lab, float cost)
    {
        this.name = name;
        this.lab = lab;
        this.courseCost = cost;
    }

    public void SetLabCost()
    {
        lab.GetComponent<Interactable>().activity.resourcesDelta[0] = courseCost;
    }

    public void EnrollStudent(Student newStudent)
    {
        students.Add(newStudent);
        newStudent.courses.Add(this);
    }
    public void KickStudent(Student student)
    {
        students.Remove(student);
        student.courses.Remove(this);
    }

    public void ImproveGrade(Student student,float increase)
    {
        GameController.instance.SchoolReputation += 5;
        grades[student] += increase;
        for (int i = 0; i < students.Count; i++)
        {
            if (students[i] != student)
                grades[student] += increase * 0.1f;
        }
        if (grades[student] >= 100)
            GraduateStudent(student);
    }
    public void GraduateStudent(Student student)
    {
        GameController.instance.SchoolReputation += 20;
        KickStudent(student);
    }
}
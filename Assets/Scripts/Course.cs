using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Course
{
    public string name;
    public ComputerLab lab;

    Dictionary<Student, float> students = new Dictionary<Student, float>();

    public Course(string name, ComputerLab lab, params Student[] students)
    {
        this.name = name;
        this.lab = lab;
        for (int i = 0; i < students.Length; i++)
        {
            this.students.Add(students[i], 0);
        }
    }
    public Course(string name,ComputerLab lab)
    {
        this.name = name;
        this.lab = lab;
    }

    public void EnrollStudent(Student newStudent)
    {
        students.Add(newStudent, 0);
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
        students[student] += increase;
        if (students[student] >= 100)
            GraduateStudent(student);
    }
    public void GraduateStudent(Student student)
    {
        GameController.instance.SchoolReputation += 20;
        KickStudent(student);
    }
}
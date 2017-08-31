using System.Collections.Generic;
using UnityEngine;

public class Course
{
    private List<Course> preReq = new List<Course>();
    public List<Course> PreReq
    {
        get { return preReq; }
    }

    List<Student> students = new List<Student>();
    public Student[] Students
    {
        get { return students.ToArray(); }
    }

    public string name;
    public float courseCost = 10;

    public Course(string name, float cost, params Course[] preReq)
    {
        this.name = name;
        this.courseCost = cost;
        this.preReq.AddRange(preReq);
    }
    public Course(string name, float cost)
    {
        this.name = name;
        this.courseCost = cost;
    }

    public Course(Course refCourse)
    {
        this.name = refCourse.name;
        this.courseCost = refCourse.courseCost;
        this.preReq = refCourse.preReq;
    }

    public void GraduateStudent(Student student)
    {
        GameController.instance.SchoolReputation += 20;
        KickStudent(student);
    }

    public void EnrollStudent(Student newStudent)
    {
        students.Add(newStudent);
        newStudent.currentCourse = this;
    }
    public void KickStudent(Student student)
    {
        if (student && students != null)
        {
            students.Remove(student);
            student.currentCourse = null;
        }
    }
}
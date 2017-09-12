using System.Collections.Generic;
using UnityEngine;

public class CourseAttribute : PropertyAttribute { }

public class PreRequsites
{
    private List<Course> preReq = new List<Course>();
    public PreRequsites(IEnumerable<Course> collection)
    {
        preReq.AddRange(collection);
    }
    public PreRequsites() { }

    public Course GetPreReq(int index)
    {
        return preReq[index];
    }
    public int Count
    {
        get { return preReq.Count; }
    }

    public void Add(Course course)
    {
        preReq.Add(course);
    }
    public void Remove(Course course)
    {
        preReq.Remove(course);
    }
    public void AddRange(IEnumerable<Course> collection)
    {
        preReq.AddRange(collection);
    }
}

public class Course
{
    #region Course Initilization
    private PreRequsites preReq = new PreRequsites();
    public PreRequsites PreReq
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
    #endregion

    #region Student Functions
    public void GraduateStudent(Student student)
    {
        Debug.Log("Student Graduated: " + student.name + ": " + name);
        student.Graduate(this);
        Manager.instance.SchoolReputation += 50;
        KickStudent(student);
    }

    public void EnrollStudent(Student newStudent)
    {
        Debug.Log("EnrollStudent: " + newStudent.name + ": " + name);
        students.Add(newStudent);
    }

    public void KickStudent(Student student)
    {
        students.Remove(student);
    }
    #endregion
}
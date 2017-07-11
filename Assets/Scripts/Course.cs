using System.Collections.Generic;

[System.Serializable]
public class Course
{
    public string name;

    Dictionary<Student, float> students = new Dictionary<Student, float>();

    public Course(string name)
    {
        this.name = name;
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
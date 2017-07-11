using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public GameObject studentPrefab;

    [SerializeField,Range(0,100)]
    private float schoolRep;
    public float SchoolReputation
    {
        get { return schoolRep; }
        set
        {
            schoolRep = value;
            if (schoolRep >= 100)
            {
                CreateStudent();
                schoolRep -= 100;
            }
        }
    }

    public List<Student> students = new List<Student>();
    public List<Course> courses = new List<Course>();

    private void Start()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);

        AddCourse("Beginners Course");
        SchoolReputation = schoolRep;
    }

    public void CreateStudent()
    {
        Instantiate(studentPrefab, Vector3.zero, Quaternion.identity);
    }
    public void AddCourse(string courseName, params Student[] courseStudents)
    {
        Course newCourse = new Course(courseName);

        for (int i = 0; i < courseStudents.Length; i++)
            newCourse.EnrollStudent(courseStudents[i]);
        
        courses.Add(newCourse);
    }
}

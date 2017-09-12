using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager instance;

    public int ROWS = 21;
    public int COLS = 21;

    public int studentsCount;
    public GameObject StudentSpawn;
    public GameObject studentPrefab;
    private int studentID = 0;

    public static Course goalCourse;

    private bool[,] roomGrid;

    [SerializeField, Range(0, 100)]
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
    
    public List<Interactable> InteractableObjects = new List<Interactable>();
    public List<Construction> ConsructionObjects = new List<Construction>();
    public List<ComputerLab> ComputerLabs = new List<ComputerLab>();

    public List<Student> students = new List<Student>();
    public Dictionary<string, Course> courses = new Dictionary<string, Course>();
    public GameObject[] Rooms;

    private IEnumerator Start()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);

        roomGrid = new bool[ROWS + 1, COLS + 1];
        roomGrid[(ROWS / 2) + 1, (COLS / 2) + 1] = true;

        InteractableObjects.AddRange(FindObjectsOfType<Interactable>());
        yield return StartCoroutine(InitCourses());

        goalCourse = courses["Course_5Z"];

        if (studentsCount >= 0)
        {
            for (int i = 0; i < studentsCount; i++)
            {
                CreateStudent();
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private IEnumerator InitCourses()
    {
        WWW www = new WWW("https://docs.google.com/spreadsheets/u/0/d/1bp8Roa7rC8mpt6PrdweFRunRiCGHqk2JW0Ymx_GGlEA/export?format=csv&id=1bp8Roa7rC8mpt6PrdweFRunRiCGHqk2JW0Ymx_GGlEA&gid=0");
        yield return www;

        string[] coursesFromFile = www.text.Split("\n"[0]);
        List<string[]> CourseLines = new List<string[]>();
        Course temp;

        for (int i = 1; i < coursesFromFile.Length; i++)
        {
            string[] courseData = coursesFromFile[i].Split(","[0]);
            CourseLines.Add(courseData);
            float cost;
            float.TryParse(courseData[1],out cost);
            temp = new Course(courseData[0], cost);
            courses.Add(temp.name, temp);
        }

        for (int i = 0; i < CourseLines.Count; i++)
            for (int d = 2; d < CourseLines[i].Length; d++)
            {
                if (courses.TryGetValue(CourseLines[i][d], out temp))
                    courses[CourseLines[i][0]].PreReq.Add(temp);
            }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Time.timeScale += 1f;
            Debug.Log(Time.timeScale);
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            Time.timeScale -= 1f;
            Debug.Log(Time.timeScale);
        }
    }

    public void CreateStudent()
    {
        GameObject newStudent = Instantiate(studentPrefab, StudentSpawn.transform.position, StudentSpawn.transform.rotation, StudentSpawn.transform);
        newStudent.name = "Student_" + studentID;
        studentID++;
    }

    public Interactable BuildRoom(InteractableType type, Transform point)
    {
        int roomIndex = (int)type;

        Vector3 spawnLocation = point.GetChild(0).position;
        spawnLocation.x = Mathf.Round(spawnLocation.x / 15);
        spawnLocation.z = Mathf.Round(spawnLocation.z / 15);

        if (spawnLocation.x > -ROWS && spawnLocation.x < ROWS &&
           spawnLocation.y > -COLS && spawnLocation.y < COLS)
        {
            if (!roomGrid[(int)spawnLocation.x + 11, (int)spawnLocation.z + 11])
            {
                roomGrid[(int)spawnLocation.x + 11, (int)spawnLocation.z + 11] = true;
                return Instantiate(Rooms[roomIndex], spawnLocation * 15, point.GetChild(0).rotation, point.root).GetComponentInChildren<Interactable>();
            }
        }
        return null;
    }

    #region Find Functions
    public Interactable[] FindInteractable(InteractableType type, Student reference)
    {
        List<Interactable> ObjectsOfType = new List<Interactable>();
        for (int i = 0; i < InteractableObjects.Count; i++)
        {
            if(InteractableObjects[i].type == type && (InteractableObjects[i].InUse == null || InteractableObjects[i].InUse == reference))
                ObjectsOfType.Add(InteractableObjects[i]);
        }

        return ObjectsOfType.ToArray();
    }

    public Construction[] FindConstruction(InteractableType type)
    {
        List<Construction> ObjectsOfType = new List<Construction>();
        for (int i = 0; i < ConsructionObjects.Count; i++)
        {
            if (ConsructionObjects[i].type == type)
                ObjectsOfType.Add(ConsructionObjects[i]);
        }
        return ObjectsOfType.ToArray();
    }

    public ComputerLab[] FindComputerLab(Student reference)
    {
        List<ComputerLab> ObjectsOfType = new List<ComputerLab>();
        for (int i = 0; i < ComputerLabs.Count; i++)
        {
            if (!ComputerLabs[i].isOpen && (ComputerLabs[i].labCourse == null || ComputerLabs[i].labCourse == reference.currentCourse))
                ObjectsOfType.Add(ComputerLabs[i]);
        }
        return ObjectsOfType.ToArray();
    }

    public static Transform FindCloser(Transform ObjectOne,Transform ObjectTwo, Vector3 point)
    {
        if ((point - ObjectOne.position).magnitude < (point - ObjectTwo.position).magnitude)
            return ObjectOne;
        else
            return ObjectTwo;
    }

    public static T FindClosest<T>(T[] objects, Transform reference)
    {
        T temp = default(T);
        float closestDistance = Mathf.Infinity;
        for (int i = 0; i < objects.Length; i++)
        {
            if (!objects[i].Equals(null))
            {
                float distance = ((objects[i] as MonoBehaviour).transform.position - reference.position).magnitude;
                if (distance <= closestDistance)
                {
                    closestDistance = distance;
                    temp = objects[i];
                }
            }
        }
        return temp;
    }
    #endregion  
}

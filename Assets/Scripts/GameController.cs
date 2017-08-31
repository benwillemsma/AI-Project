using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public GameObject studentPrefab;
    public static Course goalCourse;
    private bool[,] roomGrid = new bool[21, 21];

    public List<Interactable> InteractableObjects = new List<Interactable>();
    public List<Construction> ConsructionObjects = new List<Construction>();

    public List<Student> students = new List<Student>();
    public Dictionary<string, Course> courses = new Dictionary<string, Course>();
    public GameObject[] Rooms;

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

    private void Start()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);

        InteractableObjects.AddRange(FindObjectsOfType<Interactable>());
        StartCoroutine(InitCourses());

        SchoolReputation = schoolRep;
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
        Instantiate(studentPrefab, Vector3.zero, Quaternion.identity);
    }

    public void BuildRoom(InteractableType type, Transform point)
    {
        int roomIndex = (int)type;

        Vector3 spawnLocation = point.GetChild(0).position;
        spawnLocation.x = Mathf.Round(spawnLocation.x / 15);
        spawnLocation.z = Mathf.Round(spawnLocation.z / 15);
        
        if (!roomGrid[(int)spawnLocation.x + 11, (int)spawnLocation.z + 11])
        {
            roomGrid[(int)spawnLocation.x + 11, (int)spawnLocation.z + 11] = true;

            if (FindConstruction(type).Length <= 0)
            {
                Instantiate
                    (Rooms[roomIndex], spawnLocation * 15, point.GetChild(0).rotation, point.root);
            }
        }
    }

    public Interactable[] FindInteractable(InteractableType type)
    {
        List<Interactable> ObjectsOfType = new List<Interactable>();
        for (int i = 0; i < InteractableObjects.Count; i++)
        {
            if(InteractableObjects[i].type == type && !InteractableObjects[i].InUse)
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
                    if (objects[i] as Interactable && (objects[i] as Interactable).InUse)
                        continue;
                    temp = objects[i];
                }
            }
        }
        return temp;
    }
}

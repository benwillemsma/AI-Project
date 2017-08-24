using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public GameObject studentPrefab;

    public List<Interactable> InteractableObjects = new List<Interactable>();
    public List<Construction> ConsructionObjects = new List<Construction>();

    public List<Student> students = new List<Student>();
    public Course[] courses;
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
        InteractableObjects.AddRange(FindObjectsOfType<Interactable>());

        if (!instance)
            instance = this;
        else
            Destroy(gameObject);

        SchoolReputation = schoolRep;
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

    public Interactable[] FindOfType(InteractableType type)
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

    public static Transform FindCloser(Transform ObjectOne,Transform Objecttwo, Vector3 point)
    {
        if ((point - ObjectOne.position).magnitude < (point - Objecttwo.position).magnitude)
            return ObjectOne;
        else
            return Objecttwo;
    }

    public static T FindClosest<T>(T[] objects, Transform reference)
    {
        float closestDistance = Mathf.Infinity;
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
            {
                float distance = ((objects[i] as MonoBehaviour).transform.position - reference.position).magnitude;
                if (distance <= closestDistance)
                {
                    closestDistance = distance;
                    return objects[i];
                }
            }
        }
        return default(T);
    }
}

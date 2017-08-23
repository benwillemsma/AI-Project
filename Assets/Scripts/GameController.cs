using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public GameObject studentPrefab;

    public List<Interactable> InteractableObjects = new List<Interactable>();

    public List<Student> students = new List<Student>();
    public Course[] courses;

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

    public static Transform FindCloser(Transform ObjectOne,Transform Objecttwo, Vector3 point)
    {
        if ((point - ObjectOne.position).magnitude < (point - Objecttwo.position).magnitude)
            return ObjectOne;
        else
            return Objecttwo;
    }
}

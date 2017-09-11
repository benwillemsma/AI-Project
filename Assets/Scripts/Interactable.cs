using UnityEngine;
using UnityEngine.Events;

public enum InteractableType
{
    Bed,
    FoodSource,
    Job,
    Desk,
    Book,
    Lab,
    Build,
    Count,
}

[System.Serializable]
public class Interactable : MonoBehaviour
{
    public InteractableType type;
    public Student InUse = null;
    public UnityEvent progress;

    public Transform activityPoint;

    private void Start()
    {
        Manager.instance.InteractableObjects.Add(this);
        findActivityPoint();
    }

    public void findActivityPoint()
    {
        if (!activityPoint)
        {
            if (transform.childCount > 0)
                activityPoint = transform.GetChild(0);
            else
                activityPoint = transform;
        }
    }

    public void Delete()
    {
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        Delete();
    }

    private void OnDestroy()
    {
        Manager.instance.InteractableObjects.Remove(this);
    }
}
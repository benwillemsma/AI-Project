using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class Interactable : MonoBehaviour
{
    public InteractableType type;
    public Student InUse = null;

    public Transform activityPoint;
    public Activity activity;

    private void Start()
    {
        GameController.instance.InteractableObjects.Add(this);
        if (!activityPoint)
            activityPoint = transform;
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
        GameController.instance.InteractableObjects.Remove(this);
    }
}
﻿using UnityEngine;

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
    public bool InUse = false;

    public Transform activityPoint;
    public Activity activity;

    private void Start()
    {
        GameController.instance.InteractableObjects.Add(this);
        if (!activityPoint)
        {
            if (transform.GetChild(0))
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
        GameController.instance.InteractableObjects.Remove(this);
    }
}
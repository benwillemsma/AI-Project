using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableType
{
    Bed,
    FoodSource,
    Desk,
    Job,
    Book,
    Lab,
    Count,
}

public class Interactable : MonoBehaviour
{
    public InteractableType type;
    public bool InUse = false;

    public Transform activityPoint;
    public Activity activity;
}
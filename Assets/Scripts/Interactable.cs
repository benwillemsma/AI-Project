using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableType
{
    Bed,
    FoodSource,
    Desk,
    Count,
}

public class Interactable : MonoBehaviour
{
    public InteractableType type;
    public bool InUse;

    public Transform activityPoint;
    public Activity activity;

    /* If this where a pollished game
     * public Animation startActivity;
     * public Animation activityLoop;
     * public Animation finishActivity;*/
}
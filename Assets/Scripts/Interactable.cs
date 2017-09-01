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

[System.Serializable]
public class Interactable : MonoBehaviour
{
    public InteractableType type;
    public bool InUse = false;

    public Transform activityPoint;
    public Activity activity;

    private float UseTime = 0;

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

    private void Update()
    {
        if (InUse)
            UseTime += Time.deltaTime;
        if (UseTime >= 11)
        {
            InUse = false;
            UseTime = 0;
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
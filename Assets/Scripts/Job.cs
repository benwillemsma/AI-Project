using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Job : Interactable
{
    public UnityEvent progress;

    private void Update()
    {
        if (InUse && (InUse.transform.position - activityPoint.position).magnitude < 0.2f)
            progress.Invoke();
    }
}

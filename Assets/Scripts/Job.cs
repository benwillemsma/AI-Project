using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Interactable))]
public class Job : MonoBehaviour
{
    private Interactable stats;
    public UnityEvent progress;

    private void Awake()
    {
        stats = GetComponent<Interactable>();
    }

    private void Update()
    {
        if (stats.InUse && (stats.InUse.transform.position - stats.activityPoint.position).magnitude < 0.2f)
            progress.Invoke();
    }
}

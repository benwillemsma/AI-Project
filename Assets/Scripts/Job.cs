using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Job : Interactable
{
    public UnityEvent progress;

    private void Update()
    {
        Debug.Log(InUse);
        if (InUse == true)
        {
            progress.Invoke();
        }
    }
}

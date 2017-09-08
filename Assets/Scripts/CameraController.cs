using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 offset;
    private Transform target;

    private int targetIndex = 0;
    private float elapsedTime = 0;

    private void Update()
    {
        if (Manager.instance.students.Count <= 0)
            StopAllCoroutines();
        else if(target == null)
        {
            target = Manager.instance.students[0].transform;
            StartCoroutine(UpdatePosition());
        }
    }

    private IEnumerator UpdatePosition ()
    {
        elapsedTime = 0;
        while (elapsedTime < 5)
        {
            transform.position = target.position + offset;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(Changetarget(Manager.instance.students[FindNextTarget()].transform));
    }

    private int FindNextTarget()
    {
        return (targetIndex++) % Manager.instance.students.Count;
    }

    private IEnumerator Changetarget(Transform newTarget)
    {
        elapsedTime = 0;
        while (elapsedTime < 1)
        {
            transform.position = Vector3.Lerp(target.position, newTarget.position, elapsedTime) + offset;
            elapsedTime += Time.deltaTime / 2;
            yield return null;
        }
        target = newTarget;
        StartCoroutine(UpdatePosition());
    }
}

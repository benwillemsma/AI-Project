﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 offset;
    public Transform target;

    private int targetIndex = 0;
    private float elapsedTime = 0;

    void Start ()
    {
        target = GameController.instance.students[targetIndex].transform;
        StartCoroutine(UpdatePosition());
	}

    private void Update()
    {
        if (GameController.instance.students.Count <= 0)
            StopAllCoroutines();
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
        StartCoroutine(Changetarget(GameController.instance.students[FindNextTarget()].transform));
    }

    private int FindNextTarget()
    {
        return (targetIndex++) % GameController.instance.students.Count;
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

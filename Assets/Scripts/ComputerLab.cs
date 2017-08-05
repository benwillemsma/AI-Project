using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ComputerLab : MonoBehaviour
{
    public float amountPerUse;
    public bool LabOpen;

    [SerializeField]
    Interactable[] Desks;

    private float LabTimeLeft;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Update()
    {
        LabTimeLeft -= Time.deltaTime;

        //if (LabTimeLeft <= 0)
            //CloseLab();
    }

    public void OpenLab(float duration)
    {
        LabOpen = true;
        LabTimeLeft = duration;
        anim.SetBool("Open",true);
    }

    public void CloseLab()
    {
        anim.SetBool("Open", false);
        LabOpen = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : MonoBehaviour
{
    int ConstructionPhase;
    public GameObject[] Phases;
    public GameObject FinishedProduct;

    private float completion = 0;
    private bool hasBeenWorkedOn;
    
    public float Completion
    {
        get { return completion; }

        set
        {
            completion = value;
            if (hasBeenWorkedOn == false)
            UpdatePhase();
        }
    }
	
	IEnumerator UpdatePhase ()
    {
        if (completion == 100)
        {
            Instantiate(FinishedProduct, transform.position, transform.rotation);
            Destroy(this);
        }
        else
        {
            hasBeenWorkedOn = true;
            yield return new WaitForEndOfFrame();

            ConstructionPhase = Mathf.RoundToInt(completion / Phases.Length);
            for (int i = 0; i < Phases.Length; i++)
                Phases[i].SetActive(ConstructionPhase == i || ConstructionPhase == i + 1);
        }
	}
}

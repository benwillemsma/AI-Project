using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : MonoBehaviour
{
    int ConstructionPhase;
    public GameObject[] Phases;
    public GameObject FinishedProduct;

    [SerializeField]
    private float completion = 0;
    private bool hasBeenWorkedOn = false;
    
    public void WorkOnProject(int completionDelta)
    {
        completion += completionDelta * Time.deltaTime; // completionDelta = Percent of Work to be Done in one second
        if (hasBeenWorkedOn == false)
            StartCoroutine(UpdatePhase());
    }
	
	IEnumerator UpdatePhase ()
    {
        if (completion >= 100)
        {
            Instantiate(FinishedProduct, transform.position, transform.rotation, transform.parent);
            Destroy(gameObject);
        }
        else
        {
            hasBeenWorkedOn = true;
            yield return new WaitForEndOfFrame();
            hasBeenWorkedOn = false;
            
            ConstructionPhase = Mathf.RoundToInt(completion / 100 * (Phases.Length-1));
            for (int i = 0; i < Phases.Length; i++)
                Phases[i].SetActive(ConstructionPhase == i || ConstructionPhase == i + 1);
        }
	}
}

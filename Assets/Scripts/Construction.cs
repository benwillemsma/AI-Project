using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : MonoBehaviour
{
    int ConstructionPhase;

    public InteractableType type;
    public GameObject[] Foundation;
    public GameObject[] Phases;
    [Space(20)]
    public GameObject FinishedProduct;

    [SerializeField]
    private float completion = 0;
    private bool hasBeenWorkedOn = false;

    private void Start()
    {
        GameController.instance.ConsructionObjects.Add(this);
    }

    private void OnDestroy()
    {
        GameController.instance.ConsructionObjects.Remove(this);
    }

    public void WorkOnProject(int completionDelta)
    {
        // completionDelta = Percent of Work to be Done in one second
        completion += completionDelta * Time.deltaTime; 
        if (hasBeenWorkedOn == false)
            StartCoroutine(UpdatePhase());
    }
	
	IEnumerator UpdatePhase ()
    {
        hasBeenWorkedOn = true;
        if (completion >= 100)
        {
            GameObject go = Instantiate(FinishedProduct, transform.position, transform.rotation, transform.parent);
            go.transform.Translate(Vector3.forward * 0.0001f);
            Destroy(gameObject);
        }
        else
        {
            yield return new WaitForEndOfFrame();

            ConstructionPhase = Mathf.RoundToInt(completion / 100 * (Foundation.Length + Phases.Length - 1));

            if (ConstructionPhase < Foundation.Length)
            {
                for (int i = 0; i < Foundation.Length; i++)
                    Foundation[i].SetActive(ConstructionPhase == i);
            }
            else
            {
                ConstructionPhase -= Foundation.Length;
                for (int i = 0; i < Phases.Length; i++)
                    Phases[i].SetActive(ConstructionPhase == i || ConstructionPhase == i + 1);
            }
            hasBeenWorkedOn = false;
        }
    }
}

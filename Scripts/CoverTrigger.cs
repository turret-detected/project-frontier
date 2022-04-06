using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverTrigger : MonoBehaviour
{
    public GameObject CoverHitbox;

    // Start is called before the first frame update
    void Start()
    {
        CoverHitbox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        CoverHitbox.SetActive(true);
        other.GetComponentInChildren<Combatant>().SetSelfInCover(true);
    }

    private void OnTriggerExit(Collider other) {
        CoverHitbox.SetActive(false);
        other.GetComponentInChildren<Combatant>().SetSelfInCover(false);
    }
}

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
        Combatant c = other.GetComponentInChildren<Combatant>();
        if (c != null) {
            CoverHitbox.SetActive(true);
            c.SetSelfInCover(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        Combatant c = other.GetComponentInChildren<Combatant>();
        if (c != null) {
            CoverHitbox.SetActive(false);
            c.SetSelfInCover(false);
        }
    }
}

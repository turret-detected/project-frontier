using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var particleEmission = GetComponentInChildren<ParticleSystem>().emission;
        particleEmission.enabled = false; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

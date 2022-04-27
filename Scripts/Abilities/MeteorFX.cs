using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorFX : MonoBehaviour
{
    public GameObject target;
    public GameObject explosionEffect;
    private float speed_mult = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().AddForce((target.transform.position - transform.position) * speed_mult);
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("COLLSION: "+other.name); 
        Instantiate(explosionEffect, target.transform.position, new Quaternion());
        Destroy(transform.parent.gameObject);
        // spawn explosion FX
        // destroy me   
        
    }
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    float scale;
    float rotation_scale;
    //float rotation_temp;
    Camera cam;
    public Transform target;
    Vector3 startingPos;
    Quaternion startingAngle;
    Vector3 targetStartingPos;
    
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        scale = 3.0f;
        rotation_scale = 50.0f; 

        startingPos = transform.position;
        startingAngle = transform.rotation;
        targetStartingPos = target.transform.position;

        // draw a 5-unit white line from the origin for 2.5 seconds
        // Debug.DrawLine(Vector3.zero, new Vector3(5, 0, 0), Color.white, 2.5f);
    }

    // Update is called once per frame
    void Update()
    {
        // https://answers.unity.com/questions/1601260/fastest-way-to-set-z-axis-rotation-to-0-c.html
        // Credit: xxmariofer
        // Makes sure control cube is flat
        Vector3 eulerRotation = target.rotation.eulerAngles;
        target.rotation = Quaternion.Euler(0, eulerRotation.y, 0);

        // Camera follows control cube
        transform.LookAt(target);
        
        // Movement
        if (Input.GetButton("Backward")) {
            target.Translate(Vector3.forward * Time.deltaTime * scale);
            transform.Translate(Vector3.forward * Time.deltaTime * scale, target);
        }
        if (Input.GetButton("Forward"))
        {
            target.Translate(Vector3.back * Time.deltaTime * scale);
            transform.Translate(Vector3.back * Time.deltaTime * scale, target);
        } 
        if (Input.GetButton("Right"))
        {
            target.Translate(Vector3.left * Time.deltaTime * scale);
            transform.Translate(Vector3.left * Time.deltaTime * scale, target);
        }
        if(Input.GetButton("Left")) {
            target.Translate(Vector3.right * Time.deltaTime * scale);
            transform.Translate(Vector3.right * Time.deltaTime * scale, target);
        }
        

        // Rotate
        if(Input.GetMouseButton(2)) // Middle mouse
        {
            // Move camera based on mouse movement
            transform.RotateAround(target.position, Vector3.up, Input.GetAxis("Mouse X") * Time.deltaTime * rotation_scale);

            // Align control cube with camera
            target.LookAt(transform);
        }

        // Zoom
        if(Input.GetAxis("Mouse ScrollWheel") > 0)  
        {
            if(Vector3.Distance(transform.position, target.position) > 1.0f) {
                transform.Translate(Vector3.forward * Time.deltaTime * scale * 5);
            }            
        }
        if(Input.GetAxis("Mouse ScrollWheel") < 0)  
        {
            transform.Translate(Vector3.back * Time.deltaTime * scale* 5);
        }

        // Reset Camera
        if (Input.GetButton("Reset")) {
            transform.position = startingPos;
            transform.rotation = startingAngle;
            target.transform.position = targetStartingPos;
        }           
    }
}

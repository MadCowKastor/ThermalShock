using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceToCamera : MonoBehaviour
{
    Vector3 cameraAngle = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        cameraAngle = Camera.main.transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.rotation = Quaternion.Euler( gameObject.transform.InverseTransformDirection(cameraAngle) );
    }
}

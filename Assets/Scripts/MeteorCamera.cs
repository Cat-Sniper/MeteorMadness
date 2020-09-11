using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorCamera : MonoBehaviour {

     private Camera cam;

     // Start is called before the first frame update
     void Start() {
          cam = gameObject.GetComponent<Camera>();
     }

     // Update is called once per frame
     void Update() {



          if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight) {
               cam.orthographicSize = 4.78f;
          }
          
          else if (Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown) {
               cam.orthographicSize = 6f;
          }

     }
}

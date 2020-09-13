using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchToCamera : MonoBehaviour
{

     // Start is called before the first frame update
     public bool KeepAspectRatio;
     private float aspectRatio;
     private Vector3 topRightCorner;

     void Start() {

          
          StretchImage();
     }

     void Update() {

          if(aspectRatio - Camera.main.aspect < -0.01f || aspectRatio - Camera.main.aspect > 0.01f){
               StretchImage();
          }
     }

     private void StretchImage () {

          topRightCorner = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
          aspectRatio = Camera.main.aspect;

          var worldSpaceWidth = topRightCorner.x * 2;
          var worldSpaceHeight = topRightCorner.y * 2;

          var spriteSize = gameObject.GetComponent<SpriteRenderer>().bounds.size;

          var scaleFactorX = worldSpaceWidth / spriteSize.x;
          var scaleFactorY = worldSpaceHeight / spriteSize.y;

          if (KeepAspectRatio) {
               if (scaleFactorX > scaleFactorY) {
                    scaleFactorY = scaleFactorX;
               } else {
                    scaleFactorX = scaleFactorY;
               }
          }

          gameObject.transform.localScale = new Vector3(scaleFactorX, scaleFactorY, 1);

     }
     
}

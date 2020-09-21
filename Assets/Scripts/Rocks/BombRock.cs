using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombRock : Rock
{
     public SpriteRenderer secondSpr;

     public override void Rotation() {
    
          rotSpeed = moveDistance * 0.75f;

          if (rotateRight) {

               spr.transform.Rotate(Vector3.forward * rotSpeed);
               secondSpr.transform.Rotate(Vector3.forward * rotSpeed);
               spr.flipY = false;
               secondSpr.flipY = false;

          } else {

               spr.transform.Rotate(Vector3.back * rotSpeed);
               secondSpr.transform.Rotate(Vector3.back * rotSpeed);
               spr.flipY = true;
               secondSpr.flipY = true;
          }

     }

     public override void Movement(float dt) {

          transform.Translate(Vector3.up * dt * moveDistance);

     }

}

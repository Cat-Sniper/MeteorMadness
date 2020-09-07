/*   
     Splitee.cs
     ------------------------------
     Author: Michael Ward
     Last Edited: September 4, 2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A rock created when a 'Splitter' rock is destroyed by the player.
/// Similiar to a normal rock but doesn't message the factory to spawn more rocks on death.
/// </summary>
public class Splitee : Rock
{

     protected override void Awake() {
          base.Awake();

          Vector2 pos = transform.position;
          Vector2 bounds = rockFactory.GetBounds();

          if (pos.x < bounds.x) pos.x = bounds.x + 0.5f;
          if (pos.x > bounds.y) pos.x = bounds.y - 0.5f;

          transform.position = pos;
     }

     /// <summary>
     /// Splitee Rocks do not message the rock factory to spawn more rocks.
     /// </summary>
     public override void DisableRock() {

          destructionEffect = (GameObject)Instantiate(destructionPrefab);
          destructionEffect.transform.position = transform.position;

          Destroy(gameObject);

     }

     public override void DeathCheck(float dt) {
         
          if(isDying && !snitched) {

               if (deathTimer >= timeToDeath) {

                    gameManager.MissedRock();
                    snitched = true;

               } else deathTimer += dt;
          }
     }

     public override void OnTriggerEnter2D(Collider2D col) {
          base.OnTriggerEnter2D(col);
          Debug.Log("Entered Trigger in Splitee");
     }

     public override void OnTriggerExit2D(Collider2D col) {

          if (col.gameObject.tag == "Lava") {

               Destroy(gameObject);
          }
     }

}

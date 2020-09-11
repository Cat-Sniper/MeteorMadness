/*   
     Splitter.cs
     ------------------------------
     Author: Michael Ward
     Last Edited: September 4, 2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A splitter Rock will spawn 2 additional rocks when destroyed by the player. 
/// This iteration will spawn the 'Splitee' rocks on either side of this 'Splitter' current position (if the splitter isn't in lava)
/// </summary>
public class Splitter : Rock {

     [SerializeField] private GameObject spliteePrefab;
     private GameObject rightSplitee;
     private GameObject leftSplitee;
     
     /// <summary>
     /// Splitter Rocks spawn 2 Splitee Rocks.
     /// </summary>
     public override void DisableRock() {

          Vector3 leftPosition = transform.position;
          Vector3 rightPosition = transform.position;
          Vector2 bounds = rockFactory.GetBounds();

          leftPosition.x -= 1f;
          rightPosition.x += 1f;

          
          // Ensure Splitee rocks don't go off the screen
          if (leftPosition.x < bounds.x) leftPosition.x = bounds.x;
          if (rightPosition.x > bounds.y) rightPosition.x = bounds.y;


          rockFactory.activeRocks.Remove(gameObject);
          rockFactory.AddRockToQueue();

          SpawnDestructionEffect();
          
          rightSplitee = (GameObject)Instantiate(spliteePrefab);
          rightSplitee.transform.position = rightPosition;
          rightSplitee.GetComponent<Splitee>().SetSpeed(moveDistance);
          
          leftSplitee = (GameObject)Instantiate(spliteePrefab);
          leftSplitee.transform.position = leftPosition;
          leftSplitee.GetComponent<Splitee>().SetSpeed(moveDistance);

          gameObject.SetActive(false);
          
     }


}

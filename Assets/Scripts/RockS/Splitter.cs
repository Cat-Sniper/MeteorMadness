﻿/*   
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


     // Start is called before the first frame update
     void Start() {
        
     }

     public override void DisableRock() {

          Vector3 leftPosition = transform.position;
          Vector3 rightPosition = transform.position;
          Vector2 bounds = rockFactory.GetBounds();

          leftPosition.x -= 1f;
          rightPosition.x += 1f;


          if (leftPosition.x < bounds.x) leftPosition.x = bounds.x;
          if (rightPosition.x > bounds.y) rightPosition.x = bounds.y;


          rockFactory.activeRocks.Remove(gameObject);
          rockFactory.AddRockToQueue();

          destructionEffect = (GameObject)Instantiate(destructionPrefab);
          destructionEffect.transform.position = transform.position;
          
          rightSplitee = (GameObject)Instantiate(spliteePrefab);
          rightSplitee.transform.position = rightPosition;
          rightSplitee.GetComponent<Splitee>().SetSpeed(moveDistance);
          
          leftSplitee = (GameObject)Instantiate(spliteePrefab);
          leftSplitee.transform.position = leftPosition;
          leftSplitee.GetComponent<Splitee>().SetSpeed(moveDistance);

          gameObject.SetActive(false);
          
     }


}
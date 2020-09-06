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


     // Start is called before the first frame update
     void Start() {
        
     }

     public override void DisableRock() {

          Vector3 leftPosition = transform.position;
          Vector3 rightPosition = transform.position;
          leftPosition.x -= 1f;
          rightPosition.x += 1f;
          
          rockFactory.activeRocks.Remove(gameObject);

          destructionEffect = (GameObject)Instantiate(destructionPrefab);
          destructionEffect.transform.position = transform.position;
          
          rightSplitee = (GameObject)Instantiate(spliteePrefab);
          rightSplitee.transform.position = rightPosition;
          
          leftSplitee = (GameObject)Instantiate(spliteePrefab);
          leftSplitee.transform.position = leftPosition;

          gameObject.SetActive(false);
          
     }


}

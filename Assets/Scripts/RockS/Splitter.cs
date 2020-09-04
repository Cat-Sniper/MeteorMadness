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

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    protected override void Update() {
        
    }
}

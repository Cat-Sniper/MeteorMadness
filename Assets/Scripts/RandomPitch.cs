/*   
     RandomPitch.cs
     ------------------------------
     Author: Michael Ward
     Last Edited: September 4, 2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Randomizes the pitch of an attached audio source
/// </summary>
public class RandomPitch : MonoBehaviour {

     private AudioSource audioOut;

    // Start is called before the first frame update
    void Start()
    {
          audioOut = gameObject.GetComponent<AudioSource>();
          audioOut.pitch = Random.Range(0.75f, 1.25f);
    }


}

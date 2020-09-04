/*   
     CameraExtension.cs
     ------------------------------
     Author: Michael Ward
     Last Edited: September 4, 2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the resizing of the camera for when an android device changes orientation.
/// 
/// NOTE: currently placeholder, doesn't do anything at all as of now...
/// </summary>
public static class CameraExtension
{
    public static Vector2 Extents(this Camera camera) {
        if(camera.orthographic) {
            return new Vector2(camera.orthographicSize * Screen.width / Screen.height, camera.orthographicSize);

        } else {
            Debug.LogError("Camera is not orthographic!");
            return new Vector2();
        }
    }

    public static Vector2 BoundsMin(this Camera camera) {
        return (Vector2)camera.transform.position - camera.Extents();
    }

    public static Vector2 BoundsMax(this Camera camera) {
        return (Vector2)camera.transform.position + camera.Extents();
    }


}

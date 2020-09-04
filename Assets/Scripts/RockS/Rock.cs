/*   
     Rock.cs
     ------------------------------
     Author: Michael Ward
     Last Edited: September 4, 2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The basic rock type - a solid foundation for all other rocks (hehe).
/// </summary>
/// <remarks>
/// Right now there are no physics, the rocks simply translate 'down' from where ever they spawn.
/// (I do plan on having physics drive the rocks, specifically to bounce the rocks off the side of the screen and/or have 'Splitee' rocks initially impulse upwards in some direction)
/// </remarks>
public class Rock : MonoBehaviour {

     private bool paused = false;
     public bool canBeClicked = true;
     private bool rotateRight = true;
     private bool canRotate = true;
     [SerializeField] private float moveDistance = 1f;

     // Death Timer
     private bool isDying = false;
     private float timeToDeath = 1f;
     private float deathTimer = 0f;
     private bool snitched = false;                         // flag for when the rock factory & game manager have been told that the player has missed a rock

     public GameObject smokePrefab;
     public GameObject destructionPrefab;

     private GameObject smokeEffect;
     private GameObject destructionEffect;
     private RockSpawner rockFactory;
     private GameManager gameManager;
     private SpriteRenderer spr;
    

     // Start is called before the first frame update
     protected virtual void Awake() {

          rockFactory = GameObject.Find("RockFactory").GetComponent<RockSpawner>();
          gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
          spr = gameObject.GetComponentInChildren<SpriteRenderer>();
 
     }

     protected virtual void OnEnable() {

          smokeEffect = null;
          destructionEffect = null;
          canBeClicked = true;
          canRotate = true;
          isDying = false;
          deathTimer = 0f;
          snitched = false;
          

          float rotation = spr.transform.rotation.eulerAngles.z;

          if (rotation > 180f)
               rotateRight = true;
          else
               rotateRight = false;
     }


     // Update is called once per frame
     protected virtual void Update() {

          if (!paused) {

               #region MOVEMENT 
               Vector2 prevPos = transform.position;
               float dT = Time.deltaTime;

               transform.Translate(Vector3.down * dT * moveDistance);
               #endregion

               #region ROTATION
               if (rotateRight) {
                    spr.transform.Rotate(Vector3.forward * moveDistance * 0.1f);
               } else
                    spr.transform.Rotate(Vector3.back * moveDistance * 0.1f);
               #endregion

               if (isDying && !snitched)
                    if (deathTimer >= timeToDeath) {

                         gameManager.MissedRock();
                         rockFactory.AddRockToQueue();
                         snitched = true;

                    } else 
                         deathTimer += dT;
                    
                         
          }
     }     


    //Particle effects and slowdown of rock on entering lava - can no longer be clicked on
     protected virtual void OnTriggerEnter2D(Collider2D col) {

          if (col.gameObject.tag == "Lava") {

               if (gameObject.tag == "Rock") {

                     canBeClicked = false;
                     spr.color = Color.red;
                     moveDistance = 0.2f;
                     smokeEffect = (GameObject)Instantiate(smokePrefab);
                     smokeEffect.transform.position = transform.position;

                    // Start death timer?
                    isDying = true;

               } else {

               }
          }
     }

     //Destroy the rock once it has left the screen
     protected virtual void OnTriggerExit2D(Collider2D col) {

          if (col.gameObject.tag == "Lava") {

               if (gameObject.tag == "Rock") {

                    spr.color = Color.white;
                    rockFactory.activeRocks.Remove(gameObject);
                    gameObject.SetActive(false);
                
               } else {

                    DisableRock();

               }
          }
     }

     public virtual void DisableRock() {

          rockFactory.activeRocks.Remove(gameObject);
          rockFactory.AddRockToQueue();

          destructionEffect = (GameObject)Instantiate(destructionPrefab);
          destructionEffect.transform.position = transform.position;

          gameObject.SetActive(false);
        
     }

     public void SetSpeed(float spd) { moveDistance = spd; }
     public void SetPaused(bool psd) { paused = psd; }
     public GameObject GetSpriteObj() { return spr.gameObject; }

}

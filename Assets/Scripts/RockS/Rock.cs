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

     public int ID = 0;                      // Identifies the type of rock to the rock spawner.

     protected bool paused = false;
     public bool canBeClicked = true;
     protected bool rotateRight = true;
     protected bool canRotate = true;
     [SerializeField] protected float moveDistance = 1f;
     [SerializeField] protected float rotSpeed;

     // Death Timer
     protected bool isDying = false;
     protected float timeToDeath = 1f;
     protected float deathTimer = 0f;
     protected bool snitched = false;                         // flag for when the rock factory & game manager have been told that the player has missed a rock

     public GameObject smokePrefab;
     public GameObject destructionPrefab;

     protected GameObject smokeEffect;
     protected GameObject destructionEffect;
     protected RockSpawner rockFactory;
     protected GameManager gameManager;
     protected SpriteRenderer spr;

     protected Color spriteTint;
    

     // Start is called before the first frame update
     protected virtual void Awake() {

          rockFactory = GameObject.Find("RockFactory").GetComponent<RockSpawner>();
          gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
          spr = gameObject.GetComponentInChildren<SpriteRenderer>();
 
     }

     /// <summary>
     /// Initialize the rock once it spawns somewhere.
     /// </summary>
     public virtual void OnEnable() {

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

          float dt = Time.deltaTime;            // Note that Timescale == 0 when the game is paused.
          if (!paused) {
          
               

               Movement(dt);
               Rotation();
               DeathCheck(dt);
               
          }
     }     
     
     /// <summary>
     /// Handles movement of the rock - Base rock just moves down from current position at a speed given by RockSpawner
     /// </summary>
     protected virtual void Movement(float dt) {

          Vector2 prevPos = transform.position;
          transform.Translate(Vector3.down * dt * moveDistance);

     }
     
     /// <summary>
     /// Handles the rotation of the sprite - Base rock rotates in the direction it is told by RockSpawner and at a rate relative to movement speed. 
     /// Flips sprite depending on direction.
     /// </summary>
     protected virtual void Rotation() {

          rotSpeed = moveDistance * 0.1f;

          if (rotateRight) {

               spr.transform.Rotate(Vector3.forward * rotSpeed);
               spr.flipY = false;

          } else {

               spr.transform.Rotate(Vector3.back * rotSpeed);
               spr.flipY = true;
          }
     }
     

     /// <summary>
     /// Handles the death of the Rock - Base Rock will tell the game manager to update the score by one, and add another rock to the queue for spawning.
     /// </summary>
     public virtual void DeathCheck(float dt){

          // Snitched is a flag reset on enable so that we only update the game manager and rock factory once per instantiation.
          if (isDying && !snitched){
               if (deathTimer >= timeToDeath) {

                    gameManager.MissedRock();
                    rockFactory.AddRockToQueue();
                    snitched = true;

               } else deathTimer += dt;
               
          }             

     }


    /// <summary> 
    /// The rock will start to 'burn' up and spawn a smoke effect once it hits the 'lava' trigger area at the bottom of the screen.
    /// This lava is the lose condition for the player - once a rock hits it, the rock can no longer be destroyed, the player loses a life, and a timer is started to cleanup the rock offscreen.
    /// </summary>
    /// <param name="col">The Collider2D object handed by Unity</param>
     public virtual void OnTriggerEnter2D(Collider2D col) {

          if (col.gameObject.tag == "Lava") {

               canBeClicked = false;
               spr.color = Color.red;
               moveDistance = 0.2f;
               smokeEffect = (GameObject)Instantiate(smokePrefab);
               smokeEffect.transform.position = transform.position;

               // Start death timer?
               isDying = true;

          }
     }

     /// <summary>
     /// Clean up for when the rock has left the screen/lava trigger area.
     /// </summary>
     /// <param name="col"> Collider2D object handed by Unity</param>
     public virtual void OnTriggerExit2D(Collider2D col) {

          if (col.gameObject.tag == "Lava") {

                    spr.color = Color.white;
                    rockFactory.activeRocks.Remove(gameObject);
                    gameObject.SetActive(false);
          
          }
     }
     
     /// <summary>
     /// Clean up for when the rock has been destroyed by the player. Spawn a destruction effect for positive reinforcement
     /// </summary>
     public virtual void DisableRock() {

          rockFactory.activeRocks.Remove(gameObject);
          rockFactory.AddRockToQueue();

          destructionEffect = (GameObject)Instantiate(destructionPrefab);
          destructionEffect.transform.position = transform.position;

          gameObject.SetActive(false);
        
     }

     public void SetSpeed(float spd)    { moveDistance = spd; }
     public void SetPaused(bool psd)    { paused = psd; }
     
     public SpriteRenderer GetSpriteRenderer()   { return spr; }

}

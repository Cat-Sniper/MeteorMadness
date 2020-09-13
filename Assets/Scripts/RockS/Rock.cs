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

     public int ID = 0;                      // Identifies the type of rock to the rock spawner. Set within the Unity Editor

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
     protected Rigidbody2D rigid;
     protected SpriteRenderer spr;

     protected Color spriteTint;
     protected bool muteSounds;
    

     // Start is called before the first frame update
     protected virtual void Awake() {

          rockFactory = GameObject.Find("RockFactory").GetComponent<RockSpawner>();
          gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
          spr = gameObject.GetComponentInChildren<SpriteRenderer>();
          rigid = gameObject.GetComponent<Rigidbody2D>();
 
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

          muteSounds = gameManager.AreSoundsMuted();

          float rotation = spr.transform.rotation.eulerAngles.z;

          if (rotation > 180f)
               rotateRight = true;
          else
               rotateRight = false;
     }


     // Update is called once per frame
     protected virtual void Update() {

          
     }  
     
     protected virtual void FixedUpdate() {

          float dt = Time.deltaTime;            // Note that Timescale == 0 when the game is paused.
          if (!paused) {

               // Testing
               muteSounds = gameManager.AreSoundsMuted();

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

          rotSpeed = moveDistance * 0.75f;

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

               SpawnSmokeEffect();

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

          SpawnDestructionEffect();
          ReturnToFactory();
        
     }
     
     /// <summary>
     /// Initializes a prefab where a cloud of smoke and a breaking rock sound play out
     /// </summary>
     /// <remarks>
     /// Used when the player clicks on the rock
     /// </remarks>
     protected void SpawnDestructionEffect(){

          destructionEffect = (GameObject)Instantiate(destructionPrefab);
          destructionEffect.GetComponent<AudioSource>().mute = muteSounds;
          destructionEffect.transform.position = transform.position;

     }
     
     /// <summary>
     /// Initializes a prefab where a smoke trail and bubbling sounds play
     /// </summary>
     /// <remarks>
     /// Used when the rock hits the lava
     /// </remarks>
     protected void SpawnSmokeEffect(){

          smokeEffect = (GameObject)Instantiate(smokePrefab);
          smokeEffect.GetComponent<AudioSource>().mute = muteSounds;
          smokeEffect.transform.position = transform.position;
          smokeEffect.transform.parent = transform;

     }
     
     /// <summary>
     /// All-in-one handle for Removing this object from the game and returning it to the pool.
     /// </summary>
     protected void ReturnToFactory() {

          rockFactory.activeRocks.Remove(gameObject);
          rockFactory.AddRockToQueue();
          gameObject.SetActive(false);

     }
     public virtual void SetSpeed(float spd)    { moveDistance = spd; }
     public void SetPaused(bool psd)    { paused = psd; }
     
     public SpriteRenderer GetSpriteRenderer()   { return spr; }

}

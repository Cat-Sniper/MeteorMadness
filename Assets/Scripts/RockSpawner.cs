/*   
     RockSpawner.cs
     ------------------------------
     Author: Michael Ward
     Last Edited: September 4, 2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Factory that spawns rocks when able to (number of active rocks on screen and in queue is determined by the score). 
/// </summary>
/// 
/// <remarks>
/// <list type="bullet">
/// 
/// <item> Different types of rocks will start spawning at certain thresholds. </item>
/// <item> Rocks are given a random speed on an increasing interval as the game progresses. </item>
/// <item> Rock types currently spawned in this iteration: | Normal Rocks | </item>
/// 
/// </list>
/// </remarks>
public class RockSpawner : MonoBehaviour {

     // Constants
     private const float SPEEDINCREMENT = 0.15f;
     private const float UPPERMULTIPLIER = 2f;
     private const float LOWERMULTIPLIER = 0.5f;
     private const float MAXPADDING = SPEEDINCREMENT * 2;
     private const float MINPADDING = 0.6f;
     private const float MINSPEED = 0.5f;
     [SerializeField] private float speed = 1.8f;
     
     private float minPosX = 0;
     private float maxPosX = 0;
     private float paddingX = 0.5f;
     public float rockSpawnTimer = 0.75f;
     private float rockTimer = 0.0f;
     private float multiplierPadding = 0f;

     private int nextRockID = 0;

     public List<GameObject> rockPool;
     public List<GameObject> activeRocks;

     /// <summary>
     /// Holds all the different rock prefabs - Set in unity Editor
     /// <list type="bullet">
     /// <item> 0 : Rock (Base) </item>
     /// <item> 1 : Splitter </item>
     /// </list>
     /// </summary>
     public GameObject[] rockPrefabs;
     
   


     [SerializeField] private int rocksQueued = 1;        // How many rocks are ready to be spawned
     public int rocksToPool;

     public bool shouldExpandPool = true;
     private bool canSpawnRocks = true;

    

     // Start is called before the first frame update
     void Start()
     {
        
          //Object Pool for rocks
          rockPool = new List<GameObject>();
          
          // Pool all the different types of rocks found in rockPrefabs
          for(int i = 0; i < rockPrefabs.Length; i++){

               // Initialize basic rock pool. These are the most numerous so it makes sense to pool a bunch of these.
               if( i == 0 ) {

                    for (int j = 0; j < rocksToPool; j++) {

                         AddRockToPool(i);

                    }

               // Special rocks are less common, will only pool more than one as needed.
               } else {

                    AddRockToPool(i);

               }
          }
         
         
     }

     /// <summary>
     /// Add a new rock of the specified type to the rock pool
     /// </summary>
     /// <param name="id"> type of rock - ID in script and index of rockPrefabs.</param>
     /// <returns></returns>
     private GameObject AddRockToPool(int id) {
          GameObject obj = (GameObject)Instantiate(rockPrefabs[id]);                                      // Spawn only the base rock at the start of the game
          obj.GetComponentInChildren<SpriteRenderer>().sortingOrder = rockPool.Count;                    // give a sorting order to the pooled object to avoid graphic artifacts
          obj.SetActive(false);
          obj.transform.parent = gameObject.transform;
          rockPool.Add(obj);
          return obj;
     }

     /// <summary>
     /// Return an inactive Rock of the specified type from the rock pool, create a new one if there are no available rocks in the pool.
     /// </summary>
     /// <param name="rockID"> order in the rockPrefabs container & ID in rock script </param>
     /// <returns></returns>
     public GameObject GetPooledObject(int rockID) {
          
          
          for (int i = 0; i < rockPool.Count; i++) {
               if (!rockPool[i].activeInHierarchy && rockID == rockPool[i].GetComponent<Rock>().ID) {
                    return rockPool[i];
               }
          }

          if (shouldExpandPool) {
               return AddRockToPool(rockID);

          } else {
               return null;
          }
     }

     // Update is called once per frame
     void Update()
     {
          rockTimer -= Time.deltaTime;

          

          if (rockTimer <= 0)
               canSpawnRocks = true;

          if(rocksQueued > 0) {
               if (canSpawnRocks) {
                    canSpawnRocks = false;
                    CreateRock();
                    rocksQueued--;
                    rockTimer = rockSpawnTimer;
                
               }
          }

     }

  

     /// <summary>
     /// Logic for the Rock Factory: gets a rock from the object pool then randomizes its rotation, speed, and color
     /// </summary>
     /// <param name="rockID"></param>
     public void CreateRock()
     {
          

          GameObject newRock = GetPooledObject(nextRockID);

          if (nextRockID != 0) nextRockID = 0;                                       // Reset back to normal rocks after special rock is spawned.

          if (newRock != null) {

               Rock rockScript = newRock.GetComponent<Rock>();                       // All rocks are derived of the same base class, so we can manipulate their shared properties here with ease.

               // Random rotation direction
               #region RandRot

               float rot = GetRandomRotation();

               newRock.transform.position = GetNewSpawnLocation();
               //newRock.transform.rotation = Quaternion.Euler(0, 0, 0);
               rockScript.GetSpriteRenderer().transform.rotation = Quaternion.Euler(0, 0, rot);

               #endregion
               
               
               // Random speed
               #region RandSpeed

               float variantSpeed = Random.Range(speed - MINPADDING, speed + MAXPADDING);

               // Random multiplier applied as special case: only the slowest rocks get a chance at being really fast for the current stage
               if (variantSpeed <= MINSPEED + multiplierPadding) {

                    variantSpeed = Random.Range(LOWERMULTIPLIER, UPPERMULTIPLIER) * speed;

                    if (variantSpeed <= MINSPEED) variantSpeed = MINSPEED;

               }

               rockScript.SetSpeed(variantSpeed);

               #endregion               


               // Random Color
               #region RandCol
               float v = Random.Range(0.6f, 1f);
               Color variantColor = new Color(v, v, v, 1f);
               rockScript.GetSpriteRenderer().color = variantColor;
               #endregion
               
               // Keep track of the rocks
               activeRocks.Add(newRock);
               newRock.SetActive(true);

          }
     }

     public void IncreaseSpeed() { speed += SPEEDINCREMENT; multiplierPadding += SPEEDINCREMENT; }
     public void IncreaseDifficulty() { rocksQueued++; }

     // Gets the screen boundaries for spawn position limitations
     public void SetBounds(Vector2 Minimum, Vector2 Maximum) {
          minPosX = Minimum.x + paddingX;
          maxPosX = Maximum.x - paddingX;

          foreach (GameObject rock in rockPool) {

               if (rock.activeInHierarchy) {

                    //In case rocks are off screen after rotating
                    if (rock.transform.position.x < minPosX)
                    rock.transform.position = new Vector2(minPosX, rock.transform.position.y);
                    else if (rock.transform.position.x > maxPosX)
                    rock.transform.position = new Vector2(maxPosX, rock.transform.position.y);
               }
          }
     }

     // Returns a random position just above the top of the screen
     Vector2 GetNewSpawnLocation() {
          float randX = Random.Range(minPosX, maxPosX);
          return new Vector2(randX, this.transform.position.y);
     }

     // returns a random orientation for the rock to spawn
     float GetRandomRotation() {
          float rand0 = Random.Range(0f, 1f) * 360;
          return rand0;
     }

     public void AddRockToQueue() { rocksQueued += 1; }
     
     public Vector2 GetBounds() { return new Vector2(minPosX, maxPosX); }
     public void SetNextRockID(int id) { nextRockID = id; }
     
     
}

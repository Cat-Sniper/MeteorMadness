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
    
     public List<GameObject> rockPool;
     public List<GameObject> activeRocks;

     public GameObject rockPrefab;

     [SerializeField] private int rocksQueued = 1;        // How many rocks are ready to be spawned
     public int rocksToPool;

     public bool shouldExpandPool = true;
     private bool canSpawnRocks = true;

    

     // Start is called before the first frame update
     void Start()
     {
        
          //Object Pool for rocks
          rockPool = new List<GameObject>();
          for (int i = 0; i < rocksToPool; i++) {
               GameObject obj = (GameObject)Instantiate(rockPrefab);
               obj.GetComponentInChildren<SpriteRenderer>().sortingOrder = rockPool.Count;                    // give a sorting order to the pooled object to avoid graphic artifacts
               obj.SetActive(false);
               obj.transform.parent = gameObject.transform;
               rockPool.Add(obj);
          }

     }

     //Goes through the current pooled rocks and returns the first inactive one it finds (if any)
     public GameObject GetPooledObject() {
          for (int i = 0; i < rockPool.Count; i++) {
               if (!rockPool[i].activeInHierarchy) {
                    return rockPool[i];
               }
          }

          if (shouldExpandPool) {
               GameObject newRock = (GameObject)Instantiate(rockPrefab);

               newRock.GetComponentInChildren<SpriteRenderer>().sortingOrder = rockPool.Count;

               newRock.SetActive(false);
               newRock.transform.parent = gameObject.transform;
               rockPool.Add(newRock);
               return newRock;

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

  

     //Logic for spawning a rock from the rock object pool
     public void CreateRock()
     {
          
          GameObject newRock = GetPooledObject();
          if (newRock != null) {

               Rock rockScript = newRock.GetComponent<Rock>();

               // Random rotation direction
               #region RandRot
               float rot = GetRandomRotation();
               newRock.transform.position = GetNewSpawnLocation();
               newRock.transform.rotation = Quaternion.Euler(0, 0, 0);
               rockScript.GetSpriteObj().transform.rotation = Quaternion.Euler(0, 0, rot);
               #endregion

               // Random Color
               #region RandCol
               float v = Random.Range(0.6f, 1f);
               Color variantColor = new Color(v, v, v, 1f);
               rockScript.GetSpriteObj().GetComponent<SpriteRenderer>().color = variantColor;
               #endregion

               // Random speed
               #region RandSpeed

               float variantSpeed = Random.Range(speed - MINPADDING, speed + MAXPADDING);

               // Random multiplier applied as special case: only the slowest rocks get a chance at being really fast for the current stage
               if (variantSpeed <= MINSPEED + multiplierPadding) {

                    variantSpeed = Random.Range(LOWERMULTIPLIER, UPPERMULTIPLIER) * speed;

                    if (variantSpeed <= MINSPEED) variantSpeed = MINSPEED;

                    Debug.Log("multiplier activated");
               }

               rockScript.SetSpeed(variantSpeed);

               #endregion               

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

}

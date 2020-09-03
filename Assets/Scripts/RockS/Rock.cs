using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
     void Awake() {

          rockFactory = GameObject.Find("RockFactory").GetComponent<RockSpawner>();
          gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
          spr = gameObject.GetComponentInChildren<SpriteRenderer>();

     }

     void OnEnable() {

     //     currentLerpTime = 0f;
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
     public virtual void Update() {

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
     void OnTriggerEnter2D(Collider2D col) {

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
     void OnTriggerExit2D(Collider2D col) {

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

     public void DisableRock() {

          rockFactory.activeRocks.Remove(gameObject);
          rockFactory.AddRockToQueue();

          destructionEffect = (GameObject)Instantiate(destructionPrefab);
          destructionEffect.transform.position = transform.position;

          gameObject.SetActive(false);
        
     }

     void OnDisable() {

     }

     public void SetSpeed(float spd) { moveDistance = spd; }

     public void SetPaused(bool psd) {
          paused = psd;
     }

     public GameObject GetSpriteObj() { return spr.gameObject; }

}

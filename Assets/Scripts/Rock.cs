using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {
    private bool paused = false;
    public bool canBeClicked = true;
    private bool rotateRight = true;
    
    private float lerpTime = 100f;
    private float currentLerpTime = 0f;
    private float moveDistance = 1f;


    public GameObject smokePrefab;
    private GameObject smokeEffect;
    private RockSpawner rockFactory;
    private GameManager gameManager;
    private SpriteRenderer spr;
    

    // Start is called before the first frame update
    void Awake() {
        rockFactory = GameObject.Find("RockFactory").GetComponent<RockSpawner>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        spr = gameObject.GetComponent<SpriteRenderer>();
    }
    void OnEnable() {
        currentLerpTime = 0f;
        smokeEffect = null;
        canBeClicked = true;

        float rotation = transform.rotation.eulerAngles.z;
        if (rotation > 180f)
            rotateRight = true;
        else
            rotateRight = false;
    }


    // Update is called once per frame
    void Update() {

        if (!paused) {

            #region MOVEMENT 
            currentLerpTime += Time.deltaTime;
            if(currentLerpTime > lerpTime) {
                currentLerpTime = lerpTime;
            }

 //           Vector2 xBounds = rockFactory.GetBounds();           //x is minimum bound along x-axis, y is maximum bound along x-axis
            Vector2 prevPos = transform.position;

 /*           //Check in case the screen was flipped
            if(prevPos.x < xBounds.x) 
                prevPos.x = xBounds.x;
            
            else if(prevPos.x > xBounds.y) 
                prevPos.x = xBounds.y;
 */           
            Vector2 newPos = new Vector2(prevPos.x, prevPos.y - moveDistance);
            float percent = currentLerpTime / lerpTime;

            transform.position = Vector2.Lerp(prevPos, newPos, percent);
            #endregion 

            #region ROTATION
            if (rotateRight) {
                transform.Rotate(Vector3.forward * moveDistance/2);
            } else
                transform.Rotate(Vector3.back * moveDistance/2);

            #endregion
        }
    }


    //Particle effects and slowdown of rock on entering lava - can no longer be clicked on
    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Lava") {
            if (gameObject.tag == "Rock") {
                canBeClicked = false;
                spr.color = Color.red;
                lerpTime = 1000;
                smokeEffect = (GameObject)Instantiate(smokePrefab);
                smokeEffect.transform.position = transform.position;
                
            } else {

            }
        }
    }

    //Destroy the rock once it has left the screen
    void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.tag == "Lava") {
            if (gameObject.tag == "Rock") {
                rockFactory.AddRockToQueue();
                spr.color = Color.white;
                gameObject.SetActive(false);
                gameManager.MissedRock();
                
            } else {
                DisableRock();
            }
        }
    }

    public void DisableRock() {
        rockFactory.activeRocks.Remove(gameObject);
        rockFactory.AddRockToQueue();
        gameObject.SetActive(false);
        
    }

    void OnDisable() {

    }

    public void SetSpeed(float spd) { moveDistance = spd; }
    public void SetFallTime(float fall) { lerpTime = fall; }
    public void SetPaused(bool psd) {
        paused = psd;
    }

}

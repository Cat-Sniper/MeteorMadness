using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {
    private bool paused = false;

    private float RotationSpeed = 0;
    
    private float lerpTime = 100f;
    private float currentLerpTime = 0f;
    private float moveDistance = 1f;


    public GameObject lavaSprayPrefab;
    private RockSpawner rockFactory;
    

    // Start is called before the first frame update
    void Awake() {
        rockFactory = GameObject.Find("RockFactory").GetComponent<RockSpawner>();
    }
    void OnEnable() {
        currentLerpTime = 0f;
    }


    // Update is called once per frame
    void Update() {

        if (!paused) {
            currentLerpTime += Time.deltaTime;
            if(currentLerpTime > lerpTime) {
                currentLerpTime = lerpTime;
            }

            Vector2 prevPos = transform.position;
            Vector2 newPos = new Vector2(prevPos.x, prevPos.y - moveDistance);
            float percent = currentLerpTime / lerpTime;
            transform.position = Vector2.Lerp(prevPos, newPos, percent);
        }
    }


    //Particle effects and slowdown of rock on entering lava
    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Lava") {
            if (gameObject.tag == "Rock") {

            } else {

            }
        }
    }

    //Destroy the rock once it has left the screen
    void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.tag == "Lava") {
            if (gameObject.tag == "Rock") {
                gameObject.SetActive(false);
                rockFactory.CreateRock();
            } else {
                DisableRock();
            }
        }
    }

    public void DisableRock() {
        rockFactory.activeRocks.Remove(gameObject);
        gameObject.SetActive(false);
        rockFactory.CreateRock();
    }

    void OnDisable() {

    }

    public void SetSpeed(float spd) { moveDistance = spd; }
    public void SetFallTime(float fall) { lerpTime = fall; }
    public void SetPaused(bool psd) {
        paused = psd;
        Debug.Log("Paused");
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawner : MonoBehaviour {

    private float speed = 1f;
    [SerializeField] private float fallTime = 100f;
    private float rotationSpeed = 0;
    
    public List<GameObject> rockPool;
    public List<GameObject> activeRocks;
    public GameObject rockPrefab;
    public int rocksToPool;
    private int maxRocks = 10;
    public bool shouldExpandPool = true;

    private bool canSpawnRocks = true;
    private float minPosX = 0;
    private float maxPosX = 0;
    private float paddingX = 0.5f;
    public float rockSpawnTimer = 0.75f;
    private float rockTimer = 0.0f;
    private int rocksOnScreen = 1;      // Starting rocks / current active rocks on screen
    private int rocksQueued = 0;        // How many rocks are ready to be spawned
    

    // Start is called before the first frame update
    void Start()
    {
        
        //Object Pool for rocks
        rockPool = new List<GameObject>();
        for (int i = 0; i < rocksToPool; i++) {
            GameObject obj = (GameObject)Instantiate(rockPrefab);
            obj.SetActive(false);
            obj.transform.parent = gameObject.transform;
            rockPool.Add(obj);
        }

        //Starting Rocks
        for(int i = 0; i < rocksOnScreen; i++) {
            rocksQueued++;
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
            float rot = GetRandomRotation();
            newRock.transform.position = GetNewSpawnLocation();
            newRock.transform.rotation = Quaternion.Euler(0, 0, rot);
        }

        float variantSpeed = Random.Range(0.5f, speed + 0.2f);
        if (variantSpeed <= 0.6f)
            variantSpeed = Random.Range(0.5f, 1.5f) * variantSpeed;


        newRock.GetComponent<Rock>().SetFallTime(fallTime);
        newRock.GetComponent<Rock>().SetSpeed(variantSpeed);
        activeRocks.Add(newRock);
        newRock.SetActive(true);
        
       
    }

    public void IncreaseDifficulty() {
        CreateRock();
        rocksOnScreen++;
    }

    // Gets the screen boundaries for spawn position limitations
    public void SetBounds(Vector2 Minimum, Vector2 Maximum) {
        minPosX = Minimum.x + paddingX;
        maxPosX = Maximum.x - paddingX;
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


}

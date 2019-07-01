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
    

    // Start is called before the first frame update
    void Start()
    {
        
        rockPool = new List<GameObject>();
        for (int i = 0; i < rocksToPool; i++) {
            GameObject obj = (GameObject)Instantiate(rockPrefab);
            obj.SetActive(false);
            obj.transform.parent = gameObject.transform;
            rockPool.Add(obj);
        }

        for(int i = 0; i < 5; i++) {
            CreateRock();
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
        
    }

    public void CreateRock()
    {
        
        if (canSpawnRocks)
        {
            
            GameObject newRock = GetPooledObject();
            if (newRock != null) {
                float rot = GetRandomRotation();
                newRock.transform.position = GetNewSpawnLocation();
                newRock.transform.rotation = Quaternion.Euler(0, 0, rot);
            }

            float variantSpeed = Random.Range(0.5f, speed + 0.2f);
            if(variantSpeed <= 0.6f)
               variantSpeed = Random.Range(0.5f, 1.5f) * variantSpeed;
            

            newRock.GetComponent<Rock>().SetFallTime(fallTime);
            newRock.GetComponent<Rock>().SetSpeed(variantSpeed);
            activeRocks.Add(newRock);
            newRock.SetActive(true);
        }
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

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
     private bool paused = false;

     [SerializeField] GameManager gameManager;

     // Start is called before the first frame update
     void Start() {

     }

     // Update is called once per frame
     void Update() {
          TouchController();
     }

     private void TouchController() {

          // Normal Gameplay
          if (!paused) {


               // Mouse Input
               if (Input.touchCount == 0 && Input.GetMouseButtonUp(0)) {

                    Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(clickPos, Vector2.zero);

                    if (hit.collider != null && hit.collider.tag == "Rock") {

                         Rock rock = hit.collider.gameObject.GetComponent<Rock>();

                         if (rock.canBeClicked) {
                              gameManager.IncrementScore();
                              rock.DisableRock();
                         }
                    }
               }


               // Touch Input
               for (int i = 0; i < Input.touchCount; i++) {

                    if(Input.GetTouch(i).phase == TouchPhase.Began) {

                         Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
                         RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

                         if (hit.collider != null && hit.collider.tag == "Rock") {

                              Rock rock = hit.collider.gameObject.GetComponent<Rock>();

                              if (rock.canBeClicked) {
                                   gameManager.IncrementScore();
                                   rock.DisableRock();
                              }
                         }
                    }
                         
               }

               // Check for back button input to pause the game
               if (Application.platform == RuntimePlatform.Android) 
                    if(Input.GetKeyDown(KeyCode.Escape))
                         gameManager.PauseMenuButton();

          
          // Gameplay is paused
          } else {

               // Ask to exit game if on android
               if (Application.platform == RuntimePlatform.Android) 
                    if (Input.GetKeyDown(KeyCode.Escape)) gameManager.RestartOrQuitButton(1);

          }
     }

     public void SetPaused(bool psd) { paused = psd; }
}

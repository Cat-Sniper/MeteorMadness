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
        if (!paused) {
            #region Testing
            if (Input.touchCount == 0 && Input.GetMouseButton(0)) {
                Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(clickPos, Vector2.zero);

                if (hit.collider != null && hit.collider.tag == "Rock") {
                    gameManager.IncrementScore();
                    Rock rock = hit.collider.gameObject.GetComponent<Rock>();
                    rock.DisableRock();
                }
            }
            #endregion

            for (int i = 0; i < Input.touchCount; i++) {
                Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
                RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

                if (hit.collider != null && hit.collider.tag == "Rock") {

                    gameManager.IncrementScore();
                    Rock rock = hit.collider.gameObject.GetComponent<Rock>();
                    rock.DisableRock();
                    
                }
            }
        }
    }

    public void SetPaused(bool psd) { paused = psd; }
}

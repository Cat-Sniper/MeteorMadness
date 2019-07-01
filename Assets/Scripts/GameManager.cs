using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private int playerHp = 3;
    [SerializeField] private GameObject[] hpUI;
    [SerializeField] private RockSpawner rockPool;
    [SerializeField] private InputManager inputMan;

    public GameObject pauseMenu;
    public Camera cam;
    private int score;

    [SerializeField] private Text scoreUI;

    [SerializeField] private GameObject pauseMenuUI;
    //Buttons
    private int buttonId = -1; //0 - restart, 1 - Quit;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject yesButton;
    [SerializeField] private GameObject noButton;
   

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
        yesButton.SetActive(false);
        noButton.SetActive(false);
        rockPool.SetBounds(cam.BoundsMin(), cam.BoundsMax());
        Time.timeScale = 1.0f;
        score = 0;
        scoreUI.text = score.ToString();
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    public void PauseMenuButton() {
     
        pauseMenuUI.SetActive(true);
        pauseButton.SetActive(false);
        Time.timeScale = 0.0f;

        inputMan.SetPaused(true);
       
        //stop physics for rocks
        foreach(GameObject rock in rockPool.activeRocks) {
            rock.GetComponent<Rock>().SetPaused(true);
        }
        
    }

    public void ContinueButton() {
        pauseMenuUI.SetActive(false);
        pauseButton.SetActive(true);
        Time.timeScale = 1.0f;

        inputMan.SetPaused(false);
        //restart physics for rocks
        foreach (GameObject rock in rockPool.activeRocks) {
            rock.GetComponent<Rock>().SetPaused(false);
        }
    }

    // Pass in 0 for Restart or 1 for Quit: Important for YesButton 
    public void RestartOrQuitButton(int buttonID) {
        restartButton.SetActive(false);
        quitButton.SetActive(false);
        continueButton.SetActive(false);
        yesButton.SetActive(true);
        noButton.SetActive(true);
        buttonId = buttonID;
    }

    public void NoButton() {
        restartButton.SetActive(true);
        quitButton.SetActive(true);
        continueButton.SetActive(true);
        yesButton.SetActive(false);
        noButton.SetActive(false);
    }

    public void YesButton() {
        switch (buttonId) {

            case 0:     //Restart the game
            SceneManager.LoadScene(0, LoadSceneMode.Single);
            break;

            case 1:     //Quit the game normally
            Application.Quit(0);
            break;

            default:    //Quit because error
            Application.Quit(-1);
            break;
        }
    }

    public void IncrementScore() {
        score ++;
        scoreUI.text = score.ToString();
    }
}

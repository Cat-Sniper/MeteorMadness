/*   
     GameManager.cs
     ------------------------------
     Author: Michael Ward
     Last Edited: September 4, 2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The game's brain, holds the game together by tying all the different systems together. It does this by focusing on the following:
/// 
/// <list type="bullet">
/// 
/// <item> logic for ui (buttons, score, lives, and pause menu)</item> 
/// <item> game progression ( current score dictates difficulty and additional lives, lets the 'RockSpawner' know how many rocks to spawn) </item>
/// <item> current game state (game in progress, paused, game over) </item>
/// </list>
/// 
/// </summary>
public class GameManager : MonoBehaviour
{

     private const int SPLITTERSPAWN = 15;
     private const int TANKSPAWN = 35;

     private int playerHp;
     [SerializeField] private GameObject[] hpUI;
     [SerializeField] private RockSpawner rockPool;
     [SerializeField] private GameObject lava;
     [SerializeField] private InputManager inputMan;

     public GameObject pauseMenu;
     public Camera cam;
     public AudioSource song;
     private const int firstInc = 25;
     private int score;
     private int highScore = 0;
     private int prevScore;
     private int difficultyInc;
     private int scoreThreshold;
     private bool gameOver;
     private float screenWidth;
     private bool muteSounds = false;
     private bool muteMusic = false;

     [SerializeField] private Text scoreUI;
     [SerializeField] private Text gameOverScore;

     [SerializeField] private GameObject pauseMenuUI;

     private int buttonId = -1; // 0 - restart, 1 - Quit;
     [SerializeField] private GameObject pauseButton;
     [SerializeField] private GameObject restartButton;
     [SerializeField] private GameObject quitButton;
     [SerializeField] private GameObject continueButton;
     [SerializeField] private GameObject yesButton;
     [SerializeField] private GameObject noButton;
     [SerializeField] private GameObject gameOverPanel;
     [SerializeField] private GameObject soundsDisableImage;
     [SerializeField] private GameObject musicDisableImage;
     [SerializeField] private Text highScoreDisplay;


     private GameObject hearts;
     private GameObject scorePanel;
     
     

     // Start is called before the first frame update
     void Start()
     {
          pauseMenu.SetActive(false);
          yesButton.SetActive(false);
          noButton.SetActive(false);
          gameOverPanel.SetActive(false);

          highScore = PlayerPrefs.GetInt("HighScore");
          highScoreDisplay.text = highScore.ToString();
          muteMusic = PlayerPrefs.GetInt("MuteMusic") != 0;
          muteSounds = PlayerPrefs.GetInt("MuteSounds") != 0;
          
#if UNITY_EDITOR

          if(UnityEditor.EditorUtility.audioMasterMute){

               muteMusic = true;
               muteSounds = true;

          }
#endif      

          if (muteMusic) musicDisableImage.SetActive(true);
          else musicDisableImage.SetActive(false);

          if (muteSounds) soundsDisableImage.SetActive(true);
          else soundsDisableImage.SetActive(false);

          hearts = GameObject.Find("Hearts");
          scorePanel = GameObject.Find("ScorePanel");

          // Initialize camera settings.
          if (cam.aspect < 1) {
               cam.orthographicSize = 6.0f;
               
          } else {
               cam.orthographicSize = 4f;
               rockPool.speed -= 0.4f;
          }

          lava.transform.position = new Vector3((cam.BoundsMin().x + cam.BoundsMax().x) / 2, cam.BoundsMin().y, 0);
          rockPool.transform.position = new Vector3((cam.BoundsMin().x + cam.BoundsMax().x) / 2, cam.BoundsMax().y + 1, 0);
          rockPool.SetBounds(cam.BoundsMin(), cam.BoundsMax());

          screenWidth = cam.orthographicSize * 2f * cam.aspect;
         
          Time.timeScale = 1.0f;
          scoreUI.text = score.ToString();
          gameOverScore.text = score.ToString();

          

          scoreThreshold = 0;
          difficultyInc = 4;

          score = 0;
          prevScore = 0;
          playerHp = 3;
          gameOver = false;

          AudioListener.pause = false;
          song.ignoreListenerPause = true;
          song.mute = muteMusic;
     }

     // Update is called once per frame
     void Update() {

          float height = 2f * cam.orthographicSize;
          float width = cam.aspect * height;
          if (screenWidth != width) {

               if (cam.aspect < 1) {

                    cam.orthographicSize = 6.0f;
                    rockPool.speed += 0.4f;

               } else {

                    cam.orthographicSize = 4f;
                    rockPool.speed -= 0.4f;
               }

               screenWidth = width;

               lava.transform.position = new Vector3((cam.BoundsMin().x + cam.BoundsMax().x) / 2, cam.BoundsMin().y, 0);
               rockPool.transform.position = new Vector3((cam.BoundsMin().x + cam.BoundsMax().x) / 2, cam.BoundsMax().y + 1, 0);
               rockPool.SetBounds(cam.BoundsMin(), cam.BoundsMax());

          }


               
     }
     
     /// <summary>
     /// Opens the pause menu and freezes all updates on active rocks.
     /// </summary>
     public void PauseMenuButton() {
     
          pauseMenuUI.SetActive(true);
          pauseButton.SetActive(false);
          Time.timeScale = 0.0f;
          AudioListener.pause = true;

          inputMan.SetPaused(true);
       
          // Stop physics for rocks
          foreach(GameObject rock in rockPool.activeRocks) {
               rock.GetComponent<Rock>().SetPaused(true);
          }
        
     }

     /// <summary>
     /// Resumes the game in progress.
     /// </summary>
     public void ContinueButton() {
          pauseMenuUI.SetActive(false);
          pauseButton.SetActive(true);
          Time.timeScale = 1.0f;
          AudioListener.pause = false;

          inputMan.SetPaused(false);

          // restart physics for rocks
          foreach (GameObject rock in rockPool.activeRocks) {
               rock.GetComponent<Rock>().SetPaused(false);
          }
     }

     /// <summary>
     /// Logic for both the quit and restart buttons.
     /// </summary>
     /// <remarks>
     /// Since I make the player confirm their choice when they wish to quit or restart, I use the same panel and yes/no buttons for both those options.
     /// I identify which option was pressed and pass that along when the yes button is pressed.
     /// </remarks>
     /// <param name="buttonID"> Pass in 0 for Restart or 1 for Quit</param>
     
     public void RestartOrQuitButton(int buttonID) {
          gameOverPanel.SetActive(false);
          restartButton.SetActive(false);
          quitButton.SetActive(false);
          continueButton.SetActive(false);
          yesButton.SetActive(true);
          noButton.SetActive(true);
          buttonId = buttonID;
     }

     /// <summary>
     /// Logic for the no button: behaviour changes based on the current state of the game.
     /// </summary>
     public void NoButton() {
          if (!gameOver)
               continueButton.SetActive(true);
          else
               gameOverPanel.SetActive(true);

          restartButton.SetActive(true);
          quitButton.SetActive(true);
          yesButton.SetActive(false);
          noButton.SetActive(false);
     }

     /// <summary>
     /// Logic for the yes button: logic changes whether the player chooses to quit or restart.
     /// </summary>
     public void YesButton() {

          // buttonId set when the player clicks on either the restart or quit button.
          switch (buttonId) {

               case 0:     //Restart the game
               SceneManager.LoadScene(0, LoadSceneMode.Single);
               break;

               case 1:     //Quit the game normally

               // Editor option for quitting the application, allows for quick and natural testing in the editor.
#if UNITY_EDITOR

               UnityEditor.EditorApplication.isPlaying = false;

#else
               // Quit the web or Android application.
               Application.Quit(0);

#endif
               break;
              
               default:    //Quit because error
               Application.Quit(-1);
               break;
          }
     }
     
     /// <summary>
     /// Most important logic for the core game loop - this controls the difficulty curve of the game.
     /// </summary>
     /// 
     /// <remarks>
     /// Although the details on how the difficulty changes is an on going process, the core principle hasn't changed: The higher the score, the higher the difficulty.
     /// The core factors in determining difficulty are :
     /// 
     /// <list type="bullet">
     /// 
     /// <item> Speed - In this iteration, the base speed is increased at a base 2 log scale: more speed increases at relatively lower scores. </item>
     /// <item> Number of rocks on screen - In this iteration, </item>
     /// </list>
     /// 
     /// </remarks>
     public void IncrementScore() {

          score ++;
          scoreUI.text = score.ToString();
          gameOverScore.text = score.ToString();

          
          // Check for special rock spawn points
          if (score % SPLITTERSPAWN == 0) rockPool.SetNextRockID(1);
          if (score % TANKSPAWN == 0) rockPool.SetNextRockID(2);

          // Speed Up!
          if (Mathf.Log(score, 2) % 1 == 0) {
               rockPool.IncreaseSpeed();
               Debug.Log("Increasing Speed");
          }

          // More Rocks!
          // Current iteration rock count increases at:
          // 25 60 72 129 204 240 350 486 558
          
          if ( score % (firstInc + scoreThreshold) == 0 
               && scoreThreshold + prevScore  < score )        {

               rockPool.IncreaseDifficulty();
               Debug.Log("Increasing Difficulty: " + score + " % " + (firstInc + scoreThreshold) + " | " + (scoreThreshold + prevScore)+ " < " + score );
;
               scoreThreshold += ++difficultyInc;

               prevScore = score;
          }

          // Gain 1 hp up to three every 100 points
          if(score % 100 == 0 && playerHp < 3) {
               GainHP();
          }

          if( score > highScore){
               highScore = score;
               highScoreDisplay.text = highScore.ToString();
               PlayerPrefs.SetInt("HighScore", highScore);
               PlayerPrefs.Save();
          }
     }
     

     /// <summary>
     /// Remove a life when a rock hits the lava, if there are no lives remaining it's game over
     /// </summary>
     public void MissedRock() {

          playerHp -= 1;
          if (playerHp >= 0)
               hpUI[playerHp].SetActive(false);

          if (playerHp <= 0)
               GameOver();
        
     }
     
     /// <summary>
     /// Gain a life (max of 3)
     /// </summary>
     /// <remarks>
     /// Need to add some sort of animation or graphic to signal the player. Right now they will only see a life return if they have less than 3.
     /// </remarks>
     public void GainHP() {
          hpUI[playerHp].SetActive(true);
          playerHp++;
     }
     
     /// <summary>
     /// Display game over menu with the final score and ask if the player wants to play again.
     /// </summary>
     private void GameOver() {

          gameOver = true;
          PauseMenuButton();
          scorePanel.SetActive(false);
          hearts.gameObject.SetActive(false);
          continueButton.SetActive(false);
          gameOverPanel.SetActive(true);

          if (highScore < score)
               PlayerPrefs.SetInt("HighScore", score);

     }

     public void ToggleMuteSound(){
          muteSounds = !muteSounds;

          if (muteSounds) 
               soundsDisableImage.SetActive(true);

           else
               soundsDisableImage.SetActive(false);
          
          PlayerPrefs.SetInt("MuteSounds", muteSounds ? 1 : 0);
          PlayerPrefs.Save();
     }

     public void ToggleMuteMusic(){
          muteMusic = !muteMusic;
          song.mute = muteMusic;

          if (muteMusic)
               musicDisableImage.SetActive(true);

           else
               musicDisableImage.SetActive(false);

          PlayerPrefs.SetInt("MuteMusic", muteMusic ? 1 : 0);
          PlayerPrefs.Save();
     }
     
     public bool AreSoundsMuted(){ return muteSounds; }
     public bool IsMusicMuted() { return muteMusic; }
}

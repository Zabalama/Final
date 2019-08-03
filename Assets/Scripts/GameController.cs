using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
     public float timeLeft = 30.0f;
    public Text startText; 
    public static GameController instance;
    public Text gameOverText; 
    public Text winText;
    private bool gameOver;
    private bool win;

    [SerializeField]
    private AudioClip deathSound;

    [SerializeField]
    public float deathSoundVolume = 0.75f;
    [SerializeField]
    private AudioClip winSound;

    [SerializeField]
    public float winSoundVolume = 0.75f;

    // Start is called before the first frame update
    void Awake ()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        gameOverText.text = "";
        winText.text = "";
        gameOver = false;
        win = false;
    }


    void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        timeLeft -= Time.deltaTime;
        startText.text = (timeLeft).ToString("0");
        if (timeLeft < 0)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }


    public void GameOver ()
    {
        gameOverText.text = "Game Over";
        gameOver = true;
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, deathSoundVolume);
    }
    public void Win()
    {
        
        winText.text = "Press 'R' to Continue!";
        win = true;
        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene("Scene2", LoadSceneMode.Single);
        }
        AudioSource.PlayClipAtPoint(winSound, Camera.main.transform.position, winSoundVolume);
        

    }
}

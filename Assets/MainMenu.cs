using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    string newGameScene = "SampleScene";

    public TMP_Text highScoreUI;   
    void Start()
    {
        //set highscore text
        int highScore = SaveLoadManager.Instance.LoadHighScore();
        highScoreUI.text = $"Top Wave Survived: {highScore}";
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene(newGameScene); 
    }

    public void ExitApplication()
    {
        //for testing button in editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#else
    Application.Quit();//will only work after building not on editor

#endif        
    }

    }


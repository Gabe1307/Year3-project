using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public int Game;
    public void PlayGame()
    {
        SceneManager.LoadScene(Game);
    }
}

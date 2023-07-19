using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * These scene index enums must align exactly with those found in Unity's File > BuildSettings.
 */
enum SceneIndex_e
{
    SCENE_INDEX_MAIN_MENU   = 0,
    SCENE_INDEX_GAME        = 1,
    SCENE_INDEX_GAME_OVER   = 2
};

public class LevelManager : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene((int)SceneIndex_e.SCENE_INDEX_GAME);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene((int)SceneIndex_e.SCENE_INDEX_MAIN_MENU);
    }

    public void LoadGameOver()
    {
        SceneManager.LoadScene((int)SceneIndex_e.SCENE_INDEX_GAME_OVER);
    }

    public void QuitGame()
    {
        Debug.Log("Quit game (will not actually work in debug mode)");
        Application.Quit(); // NOTE - Only works for standalone exe, but it will not work on embedded applications.
    }
}

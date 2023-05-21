/*  
    Hellish Battle - 2.5D Retro FPS
    Added in Version: 1.0.0a
    Updated in Version: 1.0.0a
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    // The script is temporary or test, if it is not used for the game, better not to modify it
    // because in the next versions it will be modified, removed or rebuilt
    // Thanks, Tiny Slime Studio

    public void OpenScene(int id)
    {
        //SceneManager.LoadScene(id);
        LoadingScreen.instance.loadingScreen(id);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneNum
{
    LogIn = 0,
    Lobby = 1,
    Room = 2,
    Game1 = 3,
}
public class Utils : MonoBehaviour
{
    public static void SceneChange(SceneNum num)
    {
        SceneManager.LoadScene((int)num);

    }
    public static void SceneChange(int num)
    {
        SceneManager.LoadScene(num);

    }

}

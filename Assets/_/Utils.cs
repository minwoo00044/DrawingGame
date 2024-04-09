using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SceneNum
{
    LogIn = 0,
    Lobby = 1,
    Room = 2,
    Game1 = 3,
    Game2 = 4,
}
public static class Utils
{
    public static void SceneChange(SceneNum num)
    {
        SceneManager.LoadScene((int)num);

    }
    public static void SceneChange(int num)
    {
        SceneManager.LoadScene(num);

    }

    public static void FadeOut(Image target, float time)
    {
        CoroutineStarter.Start(FadeOutCo(target, time));
    }
    public static void FadeOut(TMP_Text target, float time)
    {
        CoroutineStarter.Start(FadeOutCo(target, time));
    }

    static IEnumerator FadeOutCo(Image target, float time)
    {
        Color originalColor = target.color;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Clamp01(1.0f - (elapsedTime / time));
            target.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
            yield return null;
        }
        target.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        target.gameObject.SetActive(false);
    }
    static IEnumerator FadeOutCo(TMP_Text target, float time)
    {
        Color originalColor = target.color;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Clamp01(1.0f - (elapsedTime / time));
            target.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
            yield return null;
        }
        target.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        target.gameObject.SetActive(false);
    }
}

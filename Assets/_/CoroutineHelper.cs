using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineHelper
{
    private static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
    private static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();
    private static Dictionary<float, WaitForSeconds> _WaitForSeconds = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        if (!_WaitForSeconds.TryGetValue(seconds, out var waitForSeconds))
        {
            waitForSeconds = new WaitForSeconds(seconds);
            _WaitForSeconds.Add(seconds, waitForSeconds);
        }
        return waitForSeconds;
    }
}

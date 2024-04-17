using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class CloudScriptHelper
{
    public static IEnumerator GetPhotonAppIdFromCloudScript(Action<string> onCompleted)
    {
        bool isRequestCompleted = false;
        string photonAppId = "";

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "getPhotonAppId",
            GeneratePlayStreamEvent = true,
        };

        PlayFabClientAPI.ExecuteCloudScript(request, result =>
        {
            if (result.FunctionResult is Dictionary<string, object> functionResult)
            {
                photonAppId = functionResult["PhotonAppId"].ToString();
                Debug.Log("Photon AppID: " + photonAppId);
                isRequestCompleted = true;
            }
        },
        error =>
        {
            Debug.LogError("Cloud Script Error: " + error.GenerateErrorReport());
            isRequestCompleted = true;
        });

        // 요청이 완료될 때까지 대기
        yield return new WaitUntil(() => isRequestCompleted);

        // 완료 콜백 호출
        onCompleted?.Invoke(photonAppId);
    }
}

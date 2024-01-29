using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMDrawing : UVDrawing
{
    public bool isSync;
    protected override void DrawLineOnTexture(Vector2 fromUv, Vector2 toUv, Color color, int brushSize)
    {
        base.DrawLineOnTexture(fromUv, toUv, color, brushSize);
        float[] colorArray = new float[4] { color.r, color.g, color.b, color.a };
        if (isSync)
            // 그림 정보를 다른 클라이언트에게 전송합니다.
            view.RPC("SyncDrawing", RpcTarget.Others, fromUv, toUv, colorArray, brushSize);
    }
}

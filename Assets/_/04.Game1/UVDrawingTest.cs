using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UVDrawingTest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image drawingImage;
    [SerializeField] int texSize;
    [SerializeField] int brushSize;
    [SerializeField]private Texture2D drawingTexture;
    [SerializeField]private Texture2D savedTexture;
    [SerializeField]private bool isDrawing = false;
    [SerializeField]private float applyInterval = 0.1f; // 0.1�ʸ��� Apply() ȣ��
    private float timeSinceLastApply = 0f;

    Vector2 previousUv = Vector2.zero; // ���� �������� UV ��ġ�� ������ ����

    void Start()
    {
        // �⺻ �ؽ�ó ���� �� ����
        drawingTexture = new Texture2D(texSize, texSize, TextureFormat.RGBA32, false, true);
        drawingTexture.filterMode = FilterMode.Point;
        drawingTexture.wrapMode = TextureWrapMode.Clamp;
        FillTextureWithColor(drawingTexture, Color.white);
        drawingImage.sprite = Sprite.Create(drawingTexture, new Rect(0, 0, drawingTexture.width, drawingTexture.height), new Vector2(0.5f, 0.5f));
    }

    void Update()
    {
        //if (Input.GetMouseButton(0))
        //{
        //    Vector2 pixelUV = GetMouseUVPosition();
        //    Debug.LogWarning(pixelUV);
        //    DrawOnTexture(pixelUV, Color.black,brushSize);
        //}
        if (isDrawing && Input.GetMouseButton(0))
        {
            Vector2 localCursor;
            Camera cam = null;
            if (drawingImage.canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                cam = drawingImage.canvas.worldCamera;
            }

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingImage.rectTransform, Input.mousePosition, cam, out localCursor))
            {
                // RectTransform�� �ǹ��� ����Ͽ� ���� ��ǥ�� ����
                localCursor.x += drawingImage.rectTransform.rect.width * drawingImage.rectTransform.pivot.x;
                localCursor.y += drawingImage.rectTransform.rect.height * drawingImage.rectTransform.pivot.y;

                // ���� ��ǥ�� ����ȭ�� UV ��ǥ�� ��ȯ
                Vector2 uv = new Vector2(localCursor.x / drawingImage.rectTransform.rect.width, localCursor.y / drawingImage.rectTransform.rect.height);

                if (previousUv != Vector2.zero)
                {
                    // ���� ��ġ�� ���� ��ġ ������ ���� �׸��ϴ�.
                    DrawLineOnTexture(previousUv, uv, Color.black, brushSize);
                }

                previousUv = uv; // ���� UV ��ġ�� ���� UV ��ġ�� ������Ʈ

                timeSinceLastApply += Time.deltaTime;

                // ������ ������ �������� Ȯ���ϰ�, �����ٸ� Apply() ȣ��
                if (timeSinceLastApply >= applyInterval)
                {
                    drawingTexture.Apply();
                    timeSinceLastApply = 0f; // Ÿ�̸� ����
                }

            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            previousUv = Vector2.zero; // ���콺�� �������� �� ���� ��ġ�� ����
            drawingTexture.Apply(false);
            timeSinceLastApply = 0f; // Ÿ�̸� ����

        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SaveTextureInMemory();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ApplySavedTextureToImage();
        }
    }

    void DrawLineOnTexture(Vector2 fromUv, Vector2 toUv, Color color, int brushSize)
    {
        float distance = Vector2.Distance(fromUv, toUv);
        int steps = Mathf.CeilToInt(distance * drawingTexture.width); // ���� ���̿� ���� ���� ���� ����

        for (int i = 0; i <= steps; i++)
        {
            float lerpFactor = (float)i / steps;
            Vector2 interpolatedUv = Vector2.Lerp(fromUv, toUv, lerpFactor);
            // ������ ��ġ�� �귯�ø� �����մϴ�.
            DrawOnTexture(interpolatedUv, color, brushSize);
        }

        // ��������� �� ���� ����
        //drawingTexture.Apply();
    }

    void DrawOnTexture(Vector2 uv, Color color, int brushSize)
    {
        // UV ��ǥ�� �ؽ�ó ��ǥ�� ��ȯ
        int x = (int)(uv.x * drawingTexture.width);
        int y = (int)(uv.y * drawingTexture.height);

        // ���� ũ�⿡ ���� ���� �ȼ��� ���� ����
        for (int i = -brushSize; i < brushSize; i++)
        {
            for (int j = -brushSize; j < brushSize; j++)
            {
                int drawX = x + i;
                int drawY = y + j;
                // �ؽ�ó ��踦 ����� �ʵ��� �˻�
                if (drawX >= 0 && drawX < drawingTexture.width && drawY >= 0 && drawY < drawingTexture.height)
                {
                    drawingTexture.SetPixel(drawX, drawY, color);
                }
            }
        }
        // ������� ����
        //drawingTexture.Apply();
    }

    public void SaveTextureInMemory()
    {
        // ���� �ؽ�ó�� ���纻�� ����� ����
        Color[] pixels = drawingTexture.GetPixels();
        savedTexture = new Texture2D(drawingTexture.width, drawingTexture.height);
        savedTexture.SetPixels(pixels);
        savedTexture.Apply();

        // Image UI�� ����ϴ� �ؽ�ó�� �� ���̷� �ʱ�ȭ
        ResetDrawingTexture();
    }

    void ResetDrawingTexture()
    {
        // drawingTexture�� �Ͼ������ ä��
        FillTextureWithColor(drawingTexture, Color.white);

        // drawingImage�� ��������Ʈ�� ���� ����
        drawingImage.sprite = Sprite.Create(drawingTexture, new Rect(0, 0, drawingTexture.width, drawingTexture.height), new Vector2(0.5f, 0.5f));
    }

    public void ApplySavedTextureToImage()
    {
        if (savedTexture != null)
        {
            Debug.LogWarning("Load");

            // savedTexture���� ��������Ʈ ����
            Sprite savedSprite = Sprite.Create(savedTexture, new Rect(0.0f, 0.0f, savedTexture.width, savedTexture.height), new Vector2(0.5f, 0.5f));
            // Image ������Ʈ�� ������ ��������Ʈ�� �Ҵ�
            drawingImage.sprite = savedSprite;
        }
    }



    void FillTextureWithColor(Texture2D texture, Color color)
    {
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isDrawing = true;
    }

    // ���콺�� Image ������Ʈ�� ���� �� ȣ��˴ϴ�.
    public void OnPointerExit(PointerEventData eventData)
    {
        isDrawing = false;
    }
}

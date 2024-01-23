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
    private Vector2 lastPosition = Vector2.negativeInfinity;
    void Start()
    {
        // 기본 텍스처 생성 및 설정
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
                // RectTransform의 피벗을 고려하여 로컬 좌표를 조정
                localCursor.x += drawingImage.rectTransform.rect.width * drawingImage.rectTransform.pivot.x;
                localCursor.y += drawingImage.rectTransform.rect.height * drawingImage.rectTransform.pivot.y;

                // 로컬 좌표를 정규화된 UV 좌표로 변환
                Vector2 uv = new Vector2(localCursor.x / drawingImage.rectTransform.rect.width, localCursor.y / drawingImage.rectTransform.rect.height);

                // UV 좌표가 0과 1 사이인지 확인
                if (uv.x >= 0f && uv.x <= 1f && uv.y >= 0f && uv.y <= 1f)
                {
                    DrawOnTexture(uv, Color.black, brushSize);
                }
            }
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



    void DrawOnTexture(Vector2 uv, Color color, int brushSize)
    {
        // UV 좌표를 텍스처 좌표로 변환
        int x = (int)(uv.x * drawingTexture.width);
        int y = (int)(uv.y * drawingTexture.height);

        // 붓의 크기에 따라 여러 픽셀에 색상 적용
        for (int i = -brushSize; i < brushSize; i++)
        {
            for (int j = -brushSize; j < brushSize; j++)
            {
                int drawX = x + i;
                int drawY = y + j;
                // 텍스처 경계를 벗어나지 않도록 검사
                if (drawX >= 0 && drawX < drawingTexture.width && drawY >= 0 && drawY < drawingTexture.height)
                {
                    drawingTexture.SetPixel(drawX, drawY, color);
                }
            }
        }
        // 변경사항 적용
        drawingTexture.Apply();
    }

    public void SaveTextureInMemory()
    {
        // 원본 텍스처의 복사본을 만들어 저장
        Color[] pixels = drawingTexture.GetPixels();
        savedTexture = new Texture2D(drawingTexture.width, drawingTexture.height);
        savedTexture.SetPixels(pixels);
        savedTexture.Apply();

        // Image UI가 사용하는 텍스처를 빈 종이로 초기화
        ResetDrawingTexture();
    }

    void ResetDrawingTexture()
    {
        // drawingTexture를 하얀색으로 채움
        FillTextureWithColor(drawingTexture, Color.white);

        // drawingImage의 스프라이트를 새로 갱신
        drawingImage.sprite = Sprite.Create(drawingTexture, new Rect(0, 0, drawingTexture.width, drawingTexture.height), new Vector2(0.5f, 0.5f));
    }

    public void ApplySavedTextureToImage()
    {
        if (savedTexture != null)
        {
            Debug.LogWarning("Load");

            // savedTexture에서 스프라이트 생성
            Sprite savedSprite = Sprite.Create(savedTexture, new Rect(0.0f, 0.0f, savedTexture.width, savedTexture.height), new Vector2(0.5f, 0.5f));
            // Image 컴포넌트에 생성된 스프라이트를 할당
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

    // 마우스가 Image 오브젝트를 떠날 때 호출됩니다.
    public void OnPointerExit(PointerEventData eventData)
    {
        isDrawing = false;
    }
}

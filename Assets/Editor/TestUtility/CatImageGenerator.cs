using UnityEngine;
using UnityEditor;
using System.IO;

public class CatImageGenerator : MonoBehaviour
{
    [MenuItem("Tools/Generate Cat Images")]
    public static void GenerateCatImages()
    {
        string folderPath = "Assets/Sprites/Cats";
        
        // 폴더가 없으면 생성
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        
        // 10개의 고양이 이미지 생성
        for (int i = 1; i <= 10; i++)
        {
            CreateCatImage(i, folderPath);
        }
        
        // 에셋 데이터베이스 새로고침
        AssetDatabase.Refresh();
        
        Debug.Log("고양이 이미지 10개가 생성되었습니다!");
    }
    
    private static void CreateCatImage(int catNumber, string folderPath)
    {
        // 128x128 픽셀 텍스처 생성
        int width = 128;
        int height = 128;
        Texture2D texture = new Texture2D(width, height);
        
        // 고양이 색상 팔레트 (다양한 색상)
        Color[] catColors = {
            new Color(0.8f, 0.6f, 0.4f), // 갈색
            new Color(0.9f, 0.8f, 0.7f), // 베이지
            new Color(0.7f, 0.5f, 0.3f), // 어두운 갈색
            new Color(0.9f, 0.9f, 0.9f), // 흰색
            new Color(0.3f, 0.3f, 0.3f), // 검은색
            new Color(0.8f, 0.7f, 0.5f), // 크림색
            new Color(0.6f, 0.4f, 0.2f), // 오렌지
            new Color(0.5f, 0.5f, 0.7f), // 회색
            new Color(0.9f, 0.6f, 0.8f), // 핑크
            new Color(0.4f, 0.6f, 0.8f)  // 파란색
        };
        
        Color catColor = catColors[(catNumber - 1) % catColors.Length];
        Color eyeColor = Color.green;
        Color noseColor = Color.pink;
        
        // 배경색 (투명)
        Color backgroundColor = new Color(0, 0, 0, 0);
        
        // 픽셀 데이터 생성
        Color[] pixels = new Color[width * height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                
                // 고양이 얼굴 모양 그리기
                if (IsInCatFace(x, y, width, height))
                {
                    pixels[index] = catColor;
                }
                // 눈 그리기
                else if (IsInEye(x, y, width, height, true) || IsInEye(x, y, width, height, false))
                {
                    pixels[index] = eyeColor;
                }
                // 코 그리기
                else if (IsInNose(x, y, width, height))
                {
                    pixels[index] = noseColor;
                }
                // 귀 그리기
                else if (IsInEar(x, y, width, height, true) || IsInEar(x, y, width, height, false))
                {
                    pixels[index] = catColor;
                }
                else
                {
                    pixels[index] = backgroundColor;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        // PNG 파일로 저장
        byte[] pngData = texture.EncodeToPNG();
        string filePath = Path.Combine(folderPath, $"cat{catNumber}.png");
        File.WriteAllBytes(filePath, pngData);
        
        // 텍스처 설정
        TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }
        
        Object.DestroyImmediate(texture);
    }
    
    private static bool IsInCatFace(int x, int y, int width, int height)
    {
        // 원형 고양이 얼굴
        float centerX = width * 0.5f;
        float centerY = height * 0.6f;
        float radius = Mathf.Min(width, height) * 0.4f;
        
        float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
        return distance <= radius;
    }
    
    private static bool IsInEye(int x, int y, int width, int height, bool isLeft)
    {
        float centerX = width * (isLeft ? 0.35f : 0.65f);
        float centerY = height * 0.55f;
        float radius = Mathf.Min(width, height) * 0.08f;
        
        float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
        return distance <= radius;
    }
    
    private static bool IsInNose(int x, int y, int width, int height)
    {
        float centerX = width * 0.5f;
        float centerY = height * 0.5f;
        float radius = Mathf.Min(width, height) * 0.05f;
        
        float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
        return distance <= radius;
    }
    
    private static bool IsInEar(int x, int y, int width, int height, bool isLeft)
    {
        float centerX = width * (isLeft ? 0.25f : 0.75f);
        float centerY = height * 0.8f;
        float radius = Mathf.Min(width, height) * 0.12f;
        
        float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
        return distance <= radius;
    }
} 
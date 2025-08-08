using UnityEngine;
using UnityEditor;
using System.IO;

public class CatImageGenerator : MonoBehaviour
{
    private const int TEXTURE_SIZE = 128;
    private const int CAT_COUNT = 10;
    
    [MenuItem(EditorConstants.Menus.TOOLS_BASE + "Generate Cat Images")]
    public static void GenerateCatImages()
    {
        try
        {
            string folderPath = EditorConstants.Folders.SPRITES_CATS;
            
            // 폴더가 없으면 생성
            EditorCommonUtility.EnsureDirectoryExists(folderPath);
            
            int successCount = 0;
            
            // 10개의 고양이 이미지 생성
            for (int i = 1; i <= CAT_COUNT; i++)
            {
                if (CreateCatImage(i, folderPath))
                {
                    successCount++;
                }
            }
            
            // 에셋 데이터베이스 새로고침
            AssetDatabase.Refresh();
            
            if (successCount == CAT_COUNT)
            {
                Debug.Log($"고양이 이미지 {successCount}개가 성공적으로 생성되었습니다!");
            }
            else
            {
                Debug.LogWarning($"고양이 이미지 생성 완료: {successCount}/{CAT_COUNT}개 성공");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"고양이 이미지 생성 중 오류가 발생했습니다: {e.Message}");
        }
    }
    
    private static bool CreateCatImage(int catNumber, string folderPath)
    {
        try
        {
            // 128x128 픽셀 텍스처 생성
            Texture2D texture = new Texture2D(EditorConstants.Texture.TEXTURE_SIZE, EditorConstants.Texture.TEXTURE_SIZE, TextureFormat.RGBA32, false);
            
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
            Color[] pixels = new Color[TEXTURE_SIZE * TEXTURE_SIZE];
            
            for (int y = 0; y < TEXTURE_SIZE; y++)
            {
                for (int x = 0; x < TEXTURE_SIZE; x++)
                {
                    int index = y * TEXTURE_SIZE + x;
                    
                    // 고양이 얼굴 모양 그리기
                    if (IsInCatFace(x, y))
                    {
                        pixels[index] = catColor;
                    }
                    // 눈 그리기
                    else if (IsInEye(x, y, true) || IsInEye(x, y, false))
                    {
                        pixels[index] = eyeColor;
                    }
                    // 코 그리기
                    else if (IsInNose(x, y))
                    {
                        pixels[index] = noseColor;
                    }
                    // 귀 그리기
                    else if (IsInEar(x, y, true) || IsInEar(x, y, false))
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
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"고양이 이미지 {catNumber} 생성 실패: {e.Message}");
            return false;
        }
    }
    
    private static bool IsInCatFace(int x, int y)
    {
        // 원형 고양이 얼굴
        float centerX = TEXTURE_SIZE * 0.5f;
        float centerY = TEXTURE_SIZE * 0.6f;
        float radius = TEXTURE_SIZE * 0.4f;
        
        float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
        return distance <= radius;
    }
    
    private static bool IsInEye(int x, int y, bool isLeft)
    {
        float centerX = TEXTURE_SIZE * (isLeft ? 0.35f : 0.65f);
        float centerY = TEXTURE_SIZE * 0.55f;
        float radius = TEXTURE_SIZE * 0.08f;
        
        float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
        return distance <= radius;
    }
    
    private static bool IsInNose(int x, int y)
    {
        float centerX = TEXTURE_SIZE * 0.5f;
        float centerY = TEXTURE_SIZE * 0.5f;
        float radius = TEXTURE_SIZE * 0.05f;
        
        float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
        return distance <= radius;
    }
    
    private static bool IsInEar(int x, int y, bool isLeft)
    {
        float centerX = TEXTURE_SIZE * (isLeft ? 0.25f : 0.75f);
        float centerY = TEXTURE_SIZE * 0.8f;
        float radius = TEXTURE_SIZE * 0.12f;
        
        float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
        return distance <= radius;
    }
} 
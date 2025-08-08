using UnityEngine;
using UnityEditor;

public class CreateDiscPrefab : MonoBehaviour
{
    [MenuItem(EditorConstants.Menus.PREFAB_MENU + "Create Disc Prefab")]
    public static void CreateDisc()
    {
        try
        {
            // 1. Disc GameObject 생성
            GameObject disc = new GameObject("Disc");
            SpriteRenderer mainRenderer = disc.AddComponent<SpriteRenderer>();

            // 2. MiniImage 자식 오브젝트 생성
            GameObject miniImage = new GameObject("MiniImage");
            miniImage.transform.SetParent(disc.transform, false);
            SpriteRenderer miniRenderer = miniImage.AddComponent<SpriteRenderer>();
            miniRenderer.sortingOrder = 1;

            // 3. Disc.cs 스크립트 연결 (스크립트가 존재해야 함)
            var discScript = disc.AddComponent<Disc>();
            if (discScript == null)
            {
                Debug.LogError("Disc 스크립트를 찾을 수 없습니다. Disc.cs 파일이 있는지 확인하세요.");
                Object.DestroyImmediate(disc);
                return;
            }

            // 3-1. mainRenderer, miniRenderer 필드 Inspector 연결 자동화
            var so = new SerializedObject(discScript);
            var mainRendererProp = so.FindProperty("mainRenderer");
            var miniRendererProp = so.FindProperty("miniRenderer");
            
            if (mainRendererProp != null)
                mainRendererProp.objectReferenceValue = mainRenderer;
            if (miniRendererProp != null)
                miniRendererProp.objectReferenceValue = miniRenderer;
            
            so.ApplyModifiedProperties();

            // 4. 프리팹 저장
            string prefabPath = EditorConstants.Files.DISC_PREFAB;
            
            // 폴더가 없으면 생성
            string folderPath = System.IO.Path.GetDirectoryName(prefabPath);
            EditorCommonUtility.EnsureDirectoryExists(folderPath);
            
            GameObject prefabAsset = PrefabUtility.SaveAsPrefabAsset(disc, prefabPath);
            if (prefabAsset != null)
            {
                Debug.Log($"Disc 프리팹이 성공적으로 생성되었습니다: {prefabPath}");
                
                // 생성된 프리팹을 선택
                Selection.activeObject = prefabAsset;
            }
            else
            {
                Debug.LogError($"Disc 프리팹 생성에 실패했습니다: {prefabPath}");
            }

            // 5. 임시 오브젝트 삭제
            Object.DestroyImmediate(disc);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Disc 프리팹 생성 중 오류가 발생했습니다: {e.Message}");
        }
    }
} 
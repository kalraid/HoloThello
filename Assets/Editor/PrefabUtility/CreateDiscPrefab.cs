using UnityEngine;
using UnityEditor;

public class CreateDiscPrefab : MonoBehaviour
{
    [MenuItem("Tools/Prefab/Create Disc Prefab")]
    public static void CreateDisc()
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

        // 3-1. mainRenderer, miniRenderer 필드 Inspector 연결 자동화
        var so = new SerializedObject(discScript as UnityEngine.Object);
        so.FindProperty("mainRenderer").objectReferenceValue = mainRenderer;
        so.FindProperty("miniRenderer").objectReferenceValue = miniRenderer;
        so.ApplyModifiedProperties();

        // 4. 프리팹 저장
        string prefabPath = "Assets/Prefabs/Disc.prefab";
        PrefabUtility.SaveAsPrefabAsset(disc, prefabPath);
        Debug.Log($"Disc 프리팹이 생성되었습니다: {prefabPath}");

        // 5. 임시 오브젝트 삭제
        GameObject.DestroyImmediate(disc);
    }
} 
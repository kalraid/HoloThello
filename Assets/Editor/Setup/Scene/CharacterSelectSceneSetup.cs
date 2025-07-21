using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class CharacterSelectSceneSetup : MonoBehaviour
{
    [MenuItem("Tools/Setup/Scene/CharacterSelectScene")] // 메뉴 경로 정리
    public static void SetupCharacterSelectScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/CharacterSelectScene.unity");
        
        // --- 공통 오브젝트 생성 ---
        // 카메라, Canvas, EventSystem, GameData, CharacterDataManager 등
        // (필요하다면 별도의 공통 메서드로 분리 가능)
        if (FindFirstObjectByType<Camera>() == null) new GameObject("Main Camera", typeof(Camera));
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }
        if (FindFirstObjectByType<GameData>() == null) new GameObject("GameData", typeof(GameData));
        if (FindFirstObjectByType<CharacterDataManager>() == null) new GameObject("CharacterDataManager", typeof(CharacterDataManager));

        // CharacterSelectManager 찾기/생성
        CharacterSelectManager manager = FindFirstObjectByType<CharacterSelectManager>() ?? new GameObject("CharacterSelectManager").AddComponent<CharacterSelectManager>();
        
        // --- UI 생성 로직을 CompleteUnitySetup의 공통 메서드 호출로 대체 ---
        CompleteUnitySetup.CreateCharacterSelectUI(canvas.transform, manager);
        
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("CharacterSelectScene 설정 완료!");
    }

    // --- 아래의 중복된 UI 및 오브젝트 생성 메서드들은 모두 삭제 ---
} 
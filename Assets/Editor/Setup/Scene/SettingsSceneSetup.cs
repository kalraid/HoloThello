using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class SettingsSceneSetup : MonoBehaviour
{
    [MenuItem("Tools/Setup/Scene/SettingsScene")] // 메뉴 경로 정리
    public static void SetupSettingsScene()
    {
        Scene settingsScene = EditorSceneManager.OpenScene("Assets/Scenes/SettingsScene.unity");
        
        // Canvas 생성
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        // EventSystem 생성
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        // SettingsManager는 UI 생성 시 자동으로 찾아지므로 별도 생성 로직 불필요

        // --- UI 생성 로직을 CompleteUnitySetup의 공통 메서드 호출로 대체 ---
        CompleteUnitySetup.CreateSettingsUI(canvas.transform);
        
        // 씬 저장
        EditorSceneManager.MarkSceneDirty(settingsScene);
        EditorSceneManager.SaveScene(settingsScene);
        
        Debug.Log("SettingsScene UI 설정이 완료되었습니다!");
    }

    // --- 아래의 중복된 UI 생성 메서드들은 모두 삭제 ---
} 
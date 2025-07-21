using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class GameSceneSetup : MonoBehaviour
{
    [MenuItem("Tools/Setup/Scene/GameScene")] // 메뉴 경로 정리
    public static void SetupGameScene()
    {
        Scene gameScene = EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");
        
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

        // GameManager 찾기/생성
        GameManager gameManager = FindFirstObjectByType<GameManager>() ?? new GameObject("GameManager").AddComponent<GameManager>();

        // BoardManager 찾기/생성
        BoardManager boardManager = FindFirstObjectByType<BoardManager>() ?? new GameObject("BoardManager").AddComponent<BoardManager>();

        // --- UI 생성 로직을 CompleteUnitySetup의 공통 메서드 호출로 대체 ---
        CompleteUnitySetup.CreateGameUI(canvas.transform, gameManager, boardManager);
        
        // 씬 저장
        EditorSceneManager.MarkSceneDirty(gameScene);
        EditorSceneManager.SaveScene(gameScene);
        
        Debug.Log("GameScene UI 설정이 완료되었습니다!");
    }

    // --- 아래의 중복된 UI 생성 메서드들은 모두 삭제 ---
    // private static void CreateGameUI(...) { ... }
    // private static void CreateFightingArea(...) { ... }
    // private static void CreateHealthBars(...) { ... }
    // ... 등등
} 
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CharacterSelectSceneSetup : MonoBehaviour
{
    [MenuItem("Tools/Setup CharacterSelectScene")]
    public static void SetupCharacterSelectScene()
    {
        // Play 모드 체크
        if (Application.isPlaying)
        {
            Debug.LogError("Play 모드에서는 실행할 수 없습니다. Play 모드를 종료하고 다시 시도하세요.");
            return;
        }
        
        Debug.Log("CharacterSelectScene 설정 시작...");
        
        // CharacterSelectScene 로드
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/CharacterSelectScene.unity");
        
        // 카메라 생성
        CreateMainCamera();
        
        // Canvas 생성
        Canvas canvas = CreateCanvas();
        
        // EventSystem 생성
        CreateEventSystem();
        
        // GameData 오브젝트 생성
        CreateGameDataObject();
        
        // CharacterDataManager 생성
        CreateCharacterDataManager();
        
        // UI 요소들 생성
        CreateCharacterSelectUI(canvas.transform);
        
        // 씬 저장
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        
        Debug.Log("CharacterSelectScene 설정 완료!");
    }
    
    private static void CreateMainCamera()
    {
        // 기존 카메라가 있는지 확인
        Camera existingCamera = Object.FindFirstObjectByType<Camera>();
        if (existingCamera != null)
        {
            Debug.Log("기존 카메라가 이미 존재합니다.");
            return;
        }
        
        // 메인 카메라 생성
        GameObject cameraObj = new GameObject("Main Camera");
        Camera camera = cameraObj.AddComponent<Camera>();
        camera.tag = "MainCamera";
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.depth = -1;
        
        Debug.Log("메인 카메라 생성 완료");
    }
    
    private static Canvas CreateCanvas()
    {
        // 기존 Canvas 찾기
        Canvas existingCanvas = Object.FindFirstObjectByType<Canvas>();
        if (existingCanvas != null)
        {
            return existingCanvas;
        }
        
        // 새 Canvas 생성
        GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        return canvas;
    }
    
    private static void CreateEventSystem()
    {
        // 기존 EventSystem 찾기
        EventSystem existingEventSystem = Object.FindFirstObjectByType<EventSystem>();
        if (existingEventSystem != null)
        {
            Debug.Log("EventSystem이 이미 존재합니다.");
            return;
        }
        
        // 새 EventSystem 생성
        GameObject eventSystemGO = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        Debug.Log("EventSystem 생성 완료");
    }
    
    private static void CreateGameDataObject()
    {
        // 기존 GameData 찾기
        GameData existingGameData = Object.FindFirstObjectByType<GameData>();
        if (existingGameData != null)
        {
            Debug.Log("GameData가 이미 존재합니다.");
            return;
        }
        
        // 새 GameData 오브젝트 생성
        GameObject gameDataGO = new GameObject("GameData", typeof(GameData));
        Debug.Log("GameData 오브젝트 생성 완료");
    }
    
    private static void CreateCharacterDataManager()
    {
        // 기존 CharacterDataManager 찾기
        CharacterDataManager existingManager = Object.FindFirstObjectByType<CharacterDataManager>();
        if (existingManager != null)
        {
            Debug.Log("CharacterDataManager가 이미 존재합니다.");
            return;
        }
        
        // 새 CharacterDataManager 오브젝트 생성
        GameObject managerGO = new GameObject("CharacterDataManager", typeof(CharacterDataManager));
        Debug.Log("CharacterDataManager 생성 완료");
    }
    
    private static void CreateCharacterSelectUI(Transform canvasTransform)
    {
        // 메인 UI 영역
        GameObject mainUI = new GameObject("CharacterSelectUI", typeof(RectTransform));
        mainUI.transform.SetParent(canvasTransform, false);
        RectTransform mainUIRect = mainUI.GetComponent<RectTransform>();
        mainUIRect.anchorMin = Vector2.zero;
        mainUIRect.anchorMax = Vector2.one;
        mainUIRect.sizeDelta = Vector2.zero;
        
        // 배경
        GameObject background = new GameObject("Background", typeof(Image));
        background.transform.SetParent(mainUI.transform, false);
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        Image bgImage = background.GetComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.2f, 1f);
        
        // 제목
        GameObject titleGO = new GameObject("Title", typeof(Text));
        titleGO.transform.SetParent(mainUI.transform, false);
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.1f, 0.8f);
        titleRect.anchorMax = new Vector2(0.9f, 0.95f);
        titleRect.sizeDelta = Vector2.zero;
        Text titleText = titleGO.GetComponent<Text>();
        titleText.text = "캐릭터 선택";
        titleText.fontSize = 48;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        
        // CharacterSelectManager 추가
        GameObject managerGO = new GameObject("CharacterSelectManager", typeof(CharacterSelectManager));
        CharacterSelectManager manager = managerGO.GetComponent<CharacterSelectManager>();
        
        Debug.Log("캐릭터 선택 UI 생성 완료");
    }
} 
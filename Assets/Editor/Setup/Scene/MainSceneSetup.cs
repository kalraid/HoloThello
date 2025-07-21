using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainSceneSetup : MonoBehaviour
{
    [MenuItem("Tools/Setup MainScene")]
    public static void SetupMainScene()
    {
        // Play 모드 체크
        if (Application.isPlaying)
        {
            Debug.LogError("Play 모드에서는 실행할 수 없습니다. Play 모드를 종료하고 다시 시도하세요.");
            return;
        }
        
        Debug.Log("MainScene 설정 시작...");
        
        // MainScene 로드
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
        
        // Canvas 생성
        Canvas canvas = CreateCanvas();
        
        // EventSystem 생성
        CreateEventSystem();
        
        // 카메라 생성
        CreateMainCamera();
        
        // GameData 오브젝트 생성
        CreateGameDataObject();
        
        // UI 요소들 생성
        CreateMainMenuUI(canvas.transform);
        
        // 씬 저장
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        
        Debug.Log("MainScene 설정 완료!");
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
        camera.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f); // 어두운 배경
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.depth = -1;
        
        Debug.Log("메인 카메라 생성 완료");
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
    
    private static void CreateMainMenuUI(Transform canvasTransform)
    {
        // 중복 생성 방지
        if (canvasTransform.Find("MainUI") != null)
        {
            Debug.Log("MainUI가 이미 존재하여 생성을 건너뜁니다.");
            return;
        }
        // MainMenuManager 중복 방지
        MainMenuManager manager = Object.FindFirstObjectByType<MainMenuManager>();
        if (manager == null)
        {
            GameObject managerGO = new GameObject("MainMenuManager", typeof(MainMenuManager));
            manager = managerGO.GetComponent<MainMenuManager>();
            Debug.Log("MainMenuManager 생성 완료");
        }
        else
        {
            Debug.Log("MainMenuManager가 이미 존재합니다.");
        }
        // 메인 UI 영역
        GameObject mainUI = new GameObject("MainUI", typeof(RectTransform));
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
        titleRect.anchorMin = new Vector2(0.1f, 0.7f);
        titleRect.anchorMax = new Vector2(0.9f, 0.9f);
        titleRect.sizeDelta = Vector2.zero;
        Text titleText = titleGO.GetComponent<Text>();
        titleText.text = "HoloThello";
        titleText.fontSize = 72;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        
        // 버튼들 생성
        CreateMainMenuButtons(mainUI, manager);
    }
    
    private static void CreateMainMenuButtons(GameObject parent, MainMenuManager manager)
    {
        // 게임 시작 버튼
        GameObject startButton = new GameObject("StartButton", typeof(Button), typeof(Image));
        startButton.transform.SetParent(parent.transform, false);
        RectTransform startRect = startButton.GetComponent<RectTransform>();
        startRect.anchorMin = new Vector2(0.3f, 0.4f);
        startRect.anchorMax = new Vector2(0.7f, 0.5f);
        startRect.sizeDelta = Vector2.zero;
        
        GameObject startTextGO = new GameObject("StartText", typeof(Text));
        startTextGO.transform.SetParent(startButton.transform, false);
        RectTransform startTextRect = startTextGO.GetComponent<RectTransform>();
        startTextRect.anchorMin = Vector2.zero;
        startTextRect.anchorMax = Vector2.one;
        startTextRect.sizeDelta = Vector2.zero;
        Text startText = startTextGO.GetComponent<Text>();
        startText.text = "게임 시작";
        startText.fontSize = 32;
        startText.alignment = TextAnchor.MiddleCenter;
        startText.color = Color.black;
        
        // 설정 버튼
        GameObject settingsButton = new GameObject("SettingsButton", typeof(Button), typeof(Image));
        settingsButton.transform.SetParent(parent.transform, false);
        RectTransform settingsRect = settingsButton.GetComponent<RectTransform>();
        settingsRect.anchorMin = new Vector2(0.3f, 0.3f);
        settingsRect.anchorMax = new Vector2(0.7f, 0.4f);
        settingsRect.sizeDelta = Vector2.zero;
        
        GameObject settingsTextGO = new GameObject("SettingsText", typeof(Text));
        settingsTextGO.transform.SetParent(settingsButton.transform, false);
        RectTransform settingsTextRect = settingsTextGO.GetComponent<RectTransform>();
        settingsTextRect.anchorMin = Vector2.zero;
        settingsTextRect.anchorMax = Vector2.one;
        settingsTextRect.sizeDelta = Vector2.zero;
        Text settingsText = settingsTextGO.GetComponent<Text>();
        settingsText.text = "설정";
        settingsText.fontSize = 32;
        settingsText.alignment = TextAnchor.MiddleCenter;
        settingsText.color = Color.black;
        
        // 종료 버튼
        GameObject quitButton = new GameObject("QuitButton", typeof(Button), typeof(Image));
        quitButton.transform.SetParent(parent.transform, false);
        RectTransform quitRect = quitButton.GetComponent<RectTransform>();
        quitRect.anchorMin = new Vector2(0.3f, 0.2f);
        quitRect.anchorMax = new Vector2(0.7f, 0.3f);
        quitRect.sizeDelta = Vector2.zero;
        
        GameObject quitTextGO = new GameObject("QuitText", typeof(Text));
        quitTextGO.transform.SetParent(quitButton.transform, false);
        RectTransform quitTextRect = quitTextGO.GetComponent<RectTransform>();
        quitTextRect.anchorMin = Vector2.zero;
        quitTextRect.anchorMax = Vector2.one;
        quitTextRect.sizeDelta = Vector2.zero;
        Text quitText = quitTextGO.GetComponent<Text>();
        quitText.text = "종료";
        quitText.fontSize = 32;
        quitText.alignment = TextAnchor.MiddleCenter;
        quitText.color = Color.black;
        
        // 버튼 이벤트 연결
        Button startBtn = startButton.GetComponent<Button>();
        Button settingsBtn = settingsButton.GetComponent<Button>();
        Button quitBtn = quitButton.GetComponent<Button>();
        
        startBtn.onClick.AddListener(() => manager.OnClickStart());
        settingsBtn.onClick.AddListener(() => manager.OnClickSettings());
        quitBtn.onClick.AddListener(() => manager.OnClickExit());
        
        Debug.Log("메인 메뉴 버튼들 생성 완료");
    }
} 
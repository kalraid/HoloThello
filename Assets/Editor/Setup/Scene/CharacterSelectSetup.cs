using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class CharacterSelectSetup : MonoBehaviour
{
    [MenuItem("Tools/Setup CharacterSelectScene (Legacy)")]
    public static void SetupCharacterSelectScene()
    {
        Debug.Log("CharacterSelectScene 설정 시작...");
        
        // CharacterSelectScene 로드
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/CharacterSelectScene.unity");
        
        // Canvas 생성
        Canvas canvas = CreateCanvas();
        
        // GameData 오브젝트 생성 (DontDestroyOnLoad용)
        CreateGameDataObject();
        
        // CharacterDataManager 오브젝트 생성
        CreateCharacterDataManager();
        
        // UI 요소들 생성
        CreateCharacterSelectUI(canvas.transform);
        
        // 씬 저장
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        
        Debug.Log("CharacterSelectScene 설정 완료!");
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
        Debug.Log("CharacterDataManager 오브젝트 생성 완료");
    }
    
    private static void CreateCharacterSelectUI(Transform canvasTransform)
    {
        // 중복 생성 방지
        if (canvasTransform.Find("MainUI") != null)
        {
            Debug.Log("MainUI가 이미 존재하여 생성을 건너뜁니다.");
            return;
        }
        // CharacterSelectManager 중복 방지
        CharacterSelectManager manager = Object.FindFirstObjectByType<CharacterSelectManager>();
        if (manager == null)
        {
            GameObject managerGO = new GameObject("CharacterSelectManager", typeof(CharacterSelectManager));
            manager = managerGO.GetComponent<CharacterSelectManager>();
            Debug.Log("CharacterSelectManager 생성 완료");
        }
        else
        {
            Debug.Log("CharacterSelectManager가 이미 존재합니다.");
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
        titleRect.anchorMin = new Vector2(0.1f, 0.8f);
        titleRect.anchorMax = new Vector2(0.9f, 0.95f);
        titleRect.sizeDelta = Vector2.zero;
        Text titleText = titleGO.GetComponent<Text>();
        titleText.text = "캐릭터 선택";
        titleText.fontSize = 48;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        
        // 캐릭터 타입 선택 버튼들
        CreateTypeSelectionButtons(mainUI);
        
        // 캐릭터 선택 영역
        CreateCharacterSelectionArea(mainUI);
        
        // 게임 모드 선택 영역
        CreateGameModeSelectionArea(mainUI);
        
        // 시작 버튼
        CreateStartButton(mainUI);
    }
    
    private static void CreateTypeSelectionButtons(GameObject parent)
    {
        // 타입A 버튼
        GameObject typeAButton = new GameObject("TypeAButton", typeof(Button), typeof(Image));
        typeAButton.transform.SetParent(parent.transform, false);
        RectTransform typeARect = typeAButton.GetComponent<RectTransform>();
        typeARect.anchorMin = new Vector2(0.1f, 0.6f);
        typeARect.anchorMax = new Vector2(0.45f, 0.75f);
        typeARect.sizeDelta = Vector2.zero;
        
        // 타입A 버튼 텍스트
        GameObject typeATextGO = new GameObject("TypeAText", typeof(Text));
        typeATextGO.transform.SetParent(typeAButton.transform, false);
        RectTransform typeATextRect = typeATextGO.GetComponent<RectTransform>();
        typeATextRect.anchorMin = Vector2.zero;
        typeATextRect.anchorMax = Vector2.one;
        typeATextRect.sizeDelta = Vector2.zero;
        Text typeAText = typeATextGO.GetComponent<Text>();
        typeAText.text = "Hololive 타입";
        typeAText.fontSize = 24;
        typeAText.alignment = TextAnchor.MiddleCenter;
        typeAText.color = Color.black;
        
        // 타입B 버튼
        GameObject typeBButton = new GameObject("TypeBButton", typeof(Button), typeof(Image));
        typeBButton.transform.SetParent(parent.transform, false);
        RectTransform typeBRect = typeBButton.GetComponent<RectTransform>();
        typeBRect.anchorMin = new Vector2(0.55f, 0.6f);
        typeBRect.anchorMax = new Vector2(0.9f, 0.75f);
        typeBRect.sizeDelta = Vector2.zero;
        
        // 타입B 버튼 텍스트
        GameObject typeBTextGO = new GameObject("TypeBText", typeof(Text));
        typeBTextGO.transform.SetParent(typeBButton.transform, false);
        RectTransform typeBTextRect = typeBTextGO.GetComponent<RectTransform>();
        typeBTextRect.anchorMin = Vector2.zero;
        typeBTextRect.anchorMax = Vector2.one;
        typeBTextRect.sizeDelta = Vector2.zero;
        Text typeBText = typeBTextGO.GetComponent<Text>();
        typeBText.text = "고양이 타입";
        typeBText.fontSize = 24;
        typeBText.alignment = TextAnchor.MiddleCenter;
        typeBText.color = Color.black;
    }
    
    private static void CreateCharacterSelectionArea(GameObject parent)
    {
        // 캐릭터 선택 영역
        GameObject charSelectArea = new GameObject("CharacterSelectionArea", typeof(RectTransform));
        charSelectArea.transform.SetParent(parent.transform, false);
        RectTransform charSelectRect = charSelectArea.GetComponent<RectTransform>();
        charSelectRect.anchorMin = new Vector2(0.1f, 0.2f);
        charSelectRect.anchorMax = new Vector2(0.9f, 0.55f);
        charSelectRect.sizeDelta = Vector2.zero;
        
        // 캐릭터 바 (10개)
        for (int i = 0; i < 10; i++)
        {
            GameObject charBar = new GameObject($"CharacterBar_{i}", typeof(Image), typeof(Button));
            charBar.transform.SetParent(charSelectArea.transform, false);
            RectTransform charBarRect = charBar.GetComponent<RectTransform>();
            charBarRect.anchorMin = new Vector2(i * 0.1f, 0.5f);
            charBarRect.anchorMax = new Vector2((i + 1) * 0.1f, 0.9f);
            charBarRect.sizeDelta = Vector2.zero;
            
            Image charBarImage = charBar.GetComponent<Image>();
            charBarImage.color = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), 1f);
        }
        
        // 풀사진 영역
        GameObject fullImageArea = new GameObject("FullImageArea", typeof(Image));
        fullImageArea.transform.SetParent(charSelectArea.transform, false);
        RectTransform fullImageRect = fullImageArea.GetComponent<RectTransform>();
        fullImageRect.anchorMin = new Vector2(0.1f, 0.1f);
        fullImageRect.anchorMax = new Vector2(0.9f, 0.45f);
        fullImageRect.sizeDelta = Vector2.zero;
        
        Image fullImage = fullImageArea.GetComponent<Image>();
        fullImage.color = new Color(0.8f, 0.8f, 0.8f, 1f);
    }
    
    private static void CreateGameModeSelectionArea(GameObject parent)
    {
        // 게임 모드 선택 영역
        GameObject gameModeArea = new GameObject("GameModeArea", typeof(RectTransform));
        gameModeArea.transform.SetParent(parent.transform, false);
        RectTransform gameModeRect = gameModeArea.GetComponent<RectTransform>();
        gameModeRect.anchorMin = new Vector2(0.1f, 0.05f);
        gameModeRect.anchorMax = new Vector2(0.9f, 0.15f);
        gameModeRect.sizeDelta = Vector2.zero;
        
        // 1P vs CPU 버튼
        GameObject pvcButton = new GameObject("PlayerVsCPUButton", typeof(Button), typeof(Image));
        pvcButton.transform.SetParent(gameModeArea.transform, false);
        RectTransform pvcRect = pvcButton.GetComponent<RectTransform>();
        pvcRect.anchorMin = new Vector2(0, 0);
        pvcRect.anchorMax = new Vector2(0.33f, 1);
        pvcRect.sizeDelta = Vector2.zero;
        
        GameObject pvcTextGO = new GameObject("PVCText", typeof(Text));
        pvcTextGO.transform.SetParent(pvcButton.transform, false);
        RectTransform pvcTextRect = pvcTextGO.GetComponent<RectTransform>();
        pvcTextRect.anchorMin = Vector2.zero;
        pvcTextRect.anchorMax = Vector2.one;
        pvcTextRect.sizeDelta = Vector2.zero;
        Text pvcText = pvcTextGO.GetComponent<Text>();
        pvcText.text = "1P vs CPU";
        pvcText.fontSize = 16;
        pvcText.alignment = TextAnchor.MiddleCenter;
        pvcText.color = Color.black;
        
        // 1P vs 2P 버튼
        GameObject pvpButton = new GameObject("PlayerVsPlayerButton", typeof(Button), typeof(Image));
        pvpButton.transform.SetParent(gameModeArea.transform, false);
        RectTransform pvpRect = pvpButton.GetComponent<RectTransform>();
        pvpRect.anchorMin = new Vector2(0.34f, 0);
        pvpRect.anchorMax = new Vector2(0.67f, 1);
        pvpRect.sizeDelta = Vector2.zero;
        
        GameObject pvpTextGO = new GameObject("PVPText", typeof(Text));
        pvpTextGO.transform.SetParent(pvpButton.transform, false);
        RectTransform pvpTextRect = pvpTextGO.GetComponent<RectTransform>();
        pvpTextRect.anchorMin = Vector2.zero;
        pvpTextRect.anchorMax = Vector2.one;
        pvpTextRect.sizeDelta = Vector2.zero;
        Text pvpText = pvpTextGO.GetComponent<Text>();
        pvpText.text = "1P vs 2P";
        pvpText.fontSize = 16;
        pvpText.alignment = TextAnchor.MiddleCenter;
        pvpText.color = Color.black;
        
        // CPU vs CPU 버튼
        GameObject cvcButton = new GameObject("CPUVsCPUButton", typeof(Button), typeof(Image));
        cvcButton.transform.SetParent(gameModeArea.transform, false);
        RectTransform cvcRect = cvcButton.GetComponent<RectTransform>();
        cvcRect.anchorMin = new Vector2(0.68f, 0);
        cvcRect.anchorMax = new Vector2(1, 1);
        cvcRect.sizeDelta = Vector2.zero;
        
        GameObject cvcTextGO = new GameObject("CVCText", typeof(Text));
        cvcTextGO.transform.SetParent(cvcButton.transform, false);
        RectTransform cvcTextRect = cvcTextGO.GetComponent<RectTransform>();
        cvcTextRect.anchorMin = Vector2.zero;
        cvcTextRect.anchorMax = Vector2.one;
        cvcTextRect.sizeDelta = Vector2.zero;
        Text cvcText = cvcTextGO.GetComponent<Text>();
        cvcText.text = "CPU vs CPU";
        cvcText.fontSize = 16;
        cvcText.alignment = TextAnchor.MiddleCenter;
        cvcText.color = Color.black;
    }
    
    private static void CreateStartButton(GameObject parent)
    {
        // 시작 버튼
        GameObject startButton = new GameObject("StartButton", typeof(Button), typeof(Image));
        startButton.transform.SetParent(parent.transform, false);
        RectTransform startRect = startButton.GetComponent<RectTransform>();
        startRect.anchorMin = new Vector2(0.4f, 0.05f);
        startRect.anchorMax = new Vector2(0.6f, 0.12f);
        startRect.sizeDelta = Vector2.zero;
        
        GameObject startTextGO = new GameObject("StartText", typeof(Text));
        startTextGO.transform.SetParent(startButton.transform, false);
        RectTransform startTextRect = startTextGO.GetComponent<RectTransform>();
        startTextRect.anchorMin = Vector2.zero;
        startTextRect.anchorMax = Vector2.one;
        startTextRect.sizeDelta = Vector2.zero;
        Text startText = startTextGO.GetComponent<Text>();
        startText.text = "게임 시작";
        startText.fontSize = 20;
        startText.alignment = TextAnchor.MiddleCenter;
        startText.color = Color.black;
    }
} 
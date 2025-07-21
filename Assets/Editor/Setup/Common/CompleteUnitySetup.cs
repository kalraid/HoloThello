using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
#if UNITY_EDITOR
using System.IO;
#endif
using System.Linq;

public class CompleteUnitySetup : MonoBehaviour
{
    [MenuItem("Tools/Complete Unity Setup - All Scenes")]
    public static void CompleteUnitySetupAll()
    {
        Debug.Log("=== 홀로 격투 오셀로 Unity 설정 시작 ===");
        
        // 0. 고양이 이미지 생성
        GenerateCatImages();
        
        // 1. MainScene 설정
        SetupMainScene();
        
        // 2. CharacterSelectScene 설정
        SetupCharacterSelectScene();
        
        // 3. SettingsScene 설정
        SetupSettingsScene();
        
        // 4. GameScene 설정
        SetupGameScene();
        
        // 5. 빌드 설정에 씬들 추가
        SetupBuildSettings();
        
        // 6. MainScene으로 돌아가기
        EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
        
        Debug.Log("=== 모든 Unity 설정이 완료되었습니다! ===");
        Debug.Log("이제 Unity Editor에서 게임을 테스트할 수 있습니다.");
    }

    private static void GenerateCatImages()
    {
        Debug.Log("고양이 이미지 생성 중...");
        
        // CatImageGenerator의 GenerateCatImages 메서드 호출
        CatImageGenerator.GenerateCatImages();
        
        Debug.Log("고양이 이미지 생성 완료!");
    }

    private static void SetupMainScene()
    {
        Debug.Log("MainScene 설정 중...");
        
        // MainScene 로드
        Scene mainScene = EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
        
        // Canvas 찾기
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("MainScene에 Canvas가 없습니다!");
            return;
        }

        // MainMenuManager 찾기/생성
        MainMenuManager mainManager = FindFirstObjectByType<MainMenuManager>();
        if (mainManager == null)
        {
            GameObject managerGO = new GameObject("MainMenuManager");
            mainManager = managerGO.AddComponent<MainMenuManager>();
        }
        else
        {
            Debug.Log("MainMenuManager가 이미 존재합니다.");
        }

        // 버튼들 찾기 및 이벤트 연결
        Button[] buttons = canvas.GetComponentsInChildren<Button>();
        
        foreach (Button button in buttons)
        {
            string buttonName = button.gameObject.name;
            
            // 기존 이벤트 제거
            button.onClick.RemoveAllListeners();
            
            // 버튼 이름에 따라 이벤트 연결
            if (buttonName.Contains("시작하기"))
            {
                button.onClick.AddListener(mainManager.OnClickStart);
                Debug.Log("시작하기 버튼 이벤트 연결됨");
            }
            else if (buttonName.Contains("설정하기"))
            {
                button.onClick.AddListener(mainManager.OnClickSettings);
                Debug.Log("설정하기 버튼 이벤트 연결됨");
            }
            else if (buttonName.Contains("종료하기"))
            {
                button.onClick.AddListener(mainManager.OnClickExit);
                Debug.Log("종료하기 버튼 이벤트 연결됨");
            }
        }

        // 씬 저장
        EditorSceneManager.MarkSceneDirty(mainScene);
        EditorSceneManager.SaveScene(mainScene);
        
        Debug.Log("MainScene 설정 완료!");
    }

    private static void SetupCharacterSelectScene()
    {
        Debug.Log("CharacterSelectScene 설정 중...");
        
        // CharacterSelectScene 로드
        Scene characterScene = EditorSceneManager.OpenScene("Assets/Scenes/CharacterSelectScene.unity");
        
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

        // CharacterSelectManager 찾기/생성
        CharacterSelectManager manager = FindFirstObjectByType<CharacterSelectManager>();
        if (manager == null)
        {
            GameObject managerGO = new GameObject("CharacterSelectManager");
            manager = managerGO.AddComponent<CharacterSelectManager>();
        }
        else
        {
            Debug.Log("CharacterSelectManager가 이미 존재합니다.");
        }

        // UI 요소들 생성
        CreateCharacterSelectUI(canvas.transform, manager);
        
        // 씬 저장
        EditorSceneManager.MarkSceneDirty(characterScene);
        EditorSceneManager.SaveScene(characterScene);
        
        Debug.Log("CharacterSelectScene 설정 완료!");
    }

    // 다른 스크립트에서 재사용할 수 있도록 public으로 변경
    public static void CreateCharacterSelectUI(Transform canvasTransform, CharacterSelectManager manager)
    {
        // 중복 생성 방지
        if (canvasTransform.Find("CharacterBarArea") != null)
        {
            Debug.Log("CharacterSelectScene UI가 이미 존재하여 생성을 건너뜁니다.");
            return;
        }

        // 캐릭터 바 영역 (상단)
        GameObject characterBarArea = new GameObject("CharacterBarArea", typeof(RectTransform));
        characterBarArea.transform.SetParent(canvasTransform, false);
        RectTransform charBarRect = characterBarArea.GetComponent<RectTransform>();
        charBarRect.anchorMin = new Vector2(0, 0.6f);
        charBarRect.anchorMax = new Vector2(1, 0.9f);
        charBarRect.sizeDelta = Vector2.zero;

        // 캐릭터 바 이미지들 생성 (10개)
        manager.characterBarImages = new Image[10];
        for (int i = 0; i < 10; i++)
        {
            GameObject charBarGO = new GameObject($"CharacterBar_{i}", typeof(Image), typeof(Button));
            charBarGO.transform.SetParent(characterBarArea.transform, false);
            RectTransform charRect = charBarGO.GetComponent<RectTransform>();
            charRect.anchorMin = new Vector2(i * 0.1f, 0);
            charRect.anchorMax = new Vector2((i + 1) * 0.1f, 1);
            charRect.sizeDelta = Vector2.zero;
            
            manager.characterBarImages[i] = charBarGO.GetComponent<Image>();
            Button charButton = charBarGO.GetComponent<Button>();
            int index = i; // 클로저를 위한 변수
            charButton.onClick.AddListener(() => manager.OnClickCharacter(index));
            
            // 임시 색상 설정 (나중에 실제 스프라이트로 교체)
            charBarGO.GetComponent<Image>().color = new Color(
                Random.Range(0.5f, 1f),
                Random.Range(0.5f, 1f),
                Random.Range(0.5f, 1f),
                1f
            );
        }

        // 풀사진 영역 (좌측)
        GameObject fullCharArea = new GameObject("FullCharacterArea", typeof(RectTransform));
        fullCharArea.transform.SetParent(canvasTransform, false);
        RectTransform fullCharRect = fullCharArea.GetComponent<RectTransform>();
        fullCharRect.anchorMin = new Vector2(0, 0.1f);
        fullCharRect.anchorMax = new Vector2(0.4f, 0.6f);
        fullCharRect.sizeDelta = Vector2.zero;

        // 풀사진 이미지
        GameObject fullCharImageGO = new GameObject("FullCharacterImage", typeof(Image));
        fullCharImageGO.transform.SetParent(fullCharArea.transform, false);
        RectTransform fullCharImgRect = fullCharImageGO.GetComponent<RectTransform>();
        fullCharImgRect.anchorMin = Vector2.zero;
        fullCharImgRect.anchorMax = Vector2.one;
        fullCharImgRect.sizeDelta = Vector2.zero;
        manager.fullCharacterImage = fullCharImageGO.GetComponent<Image>();
        manager.fullCharacterImage.color = Color.gray; // 임시 색상

        // 안내 텍스트
        GameObject selectLabelGO = new GameObject("SelectLabel", typeof(Text));
        selectLabelGO.transform.SetParent(canvasTransform, false);
        RectTransform labelRect = selectLabelGO.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.4f, 0.7f);
        labelRect.anchorMax = new Vector2(1, 0.8f);
        labelRect.sizeDelta = Vector2.zero;
        manager.selectLabel = selectLabelGO.GetComponent<Text>();
        manager.selectLabel.text = "1P 캐릭터를 선택하세요";
        manager.selectLabel.fontSize = 24;
        manager.selectLabel.alignment = TextAnchor.MiddleCenter;

        // 배경 선택 영역 (하단)
        GameObject bgSelectArea = new GameObject("BackgroundSelectArea", typeof(RectTransform));
        bgSelectArea.transform.SetParent(canvasTransform, false);
        RectTransform bgSelectRect = bgSelectArea.GetComponent<RectTransform>();
        bgSelectRect.anchorMin = new Vector2(0.4f, 0.1f);
        bgSelectRect.anchorMax = new Vector2(1, 0.6f);
        bgSelectRect.sizeDelta = Vector2.zero;

        // 배경 선택 바들 생성 (임시로 3개)
        manager.backgroundBarImages = new Image[3];
        for (int i = 0; i < 3; i++)
        {
            GameObject bgBarGO = new GameObject($"BackgroundBar_{i}", typeof(Image), typeof(Button));
            bgBarGO.transform.SetParent(bgSelectArea.transform, false);
            RectTransform bgRect = bgBarGO.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(i * 0.33f, 0.5f);
            bgRect.anchorMax = new Vector2((i + 1) * 0.33f, 1);
            bgRect.sizeDelta = Vector2.zero;
            
            manager.backgroundBarImages[i] = bgBarGO.GetComponent<Image>();
            Button bgButton = bgBarGO.GetComponent<Button>();
            int index = i;
            bgButton.onClick.AddListener(() => manager.OnClickBackground(index));
            
            // 임시 색상 설정
            bgBarGO.GetComponent<Image>().color = new Color(
                Random.Range(0.3f, 0.7f),
                Random.Range(0.3f, 0.7f),
                Random.Range(0.3f, 0.7f),
                1f
            );
        }

        // 선택된 배경 미리보기
        GameObject selectedBgGO = new GameObject("SelectedBackgroundImage", typeof(Image));
        selectedBgGO.transform.SetParent(bgSelectArea.transform, false);
        RectTransform selectedBgRect = selectedBgGO.GetComponent<RectTransform>();
        selectedBgRect.anchorMin = new Vector2(0, 0);
        selectedBgRect.anchorMax = new Vector2(1, 0.5f);
        selectedBgRect.sizeDelta = Vector2.zero;
        manager.selectedBackgroundImage = selectedBgGO.GetComponent<Image>();
        manager.selectedBackgroundImage.color = Color.gray; // 임시 색상

        // 확인 버튼
        Button confirmButton = CreateButton(canvasTransform, "확인", new Vector2(0, -200));
        confirmButton.onClick.AddListener(manager.OnClickConfirm);

        // CPU 블록 오버레이 (초기에는 비활성화)
        GameObject cpuBlockGO = new GameObject("CPUBlockOverlay", typeof(Image));
        cpuBlockGO.transform.SetParent(canvasTransform, false);
        RectTransform cpuBlockRect = cpuBlockGO.GetComponent<RectTransform>();
        cpuBlockRect.anchorMin = Vector2.zero;
        cpuBlockRect.anchorMax = Vector2.one;
        cpuBlockRect.sizeDelta = Vector2.zero;
        manager.cpuBlockOverlay = cpuBlockGO;
        cpuBlockGO.SetActive(false);

        // 임시 스프라이트 배열 초기화
        manager.characterSprites = new Sprite[10];
        manager.backgroundSprites = new Sprite[3];
    }

    private static void SetupSettingsScene()
    {
        Debug.Log("SettingsScene 설정 중...");
        
        // SettingsScene 로드
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

        // SettingsManager 찾기/생성
        SettingsManager settingsManager = FindFirstObjectByType<SettingsManager>();
        if (settingsManager == null)
        {
            GameObject managerGO = new GameObject("SettingsManager");
            settingsManager = managerGO.AddComponent<SettingsManager>();
        }
        else
        {
            Debug.Log("SettingsManager가 이미 존재합니다.");
        }

        // UI 요소들 생성
        CreateSettingsUI(canvas.transform);
        
        // 씬 저장
        EditorSceneManager.MarkSceneDirty(settingsScene);
        EditorSceneManager.SaveScene(settingsScene);
        
        Debug.Log("SettingsScene 설정 완료!");
    }

    // 다른 스크립트에서 재사용할 수 있도록 public으로 변경
    public static void CreateSettingsUI(Transform canvasTransform)
    {
        // 중복 생성 방지
        if (canvasTransform.Find("Title") != null)
        {
            Debug.Log("SettingsScene UI가 이미 존재하여 생성을 건너뜁니다.");
            return;
        }

        // 제목 텍스트
        GameObject titleGO = new GameObject("Title", typeof(Text));
        titleGO.transform.SetParent(canvasTransform, false);
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.8f);
        titleRect.anchorMax = new Vector2(1, 0.9f);
        titleRect.sizeDelta = Vector2.zero;
        Text titleText = titleGO.GetComponent<Text>();
        titleText.text = "설정";
        titleText.fontSize = 36;
        titleText.alignment = TextAnchor.MiddleCenter;

        // 볼륨 설정 영역
        CreateVolumeSettings(canvasTransform);

        // 화면 크기 설정 영역
        CreateScreenSizeSettings(canvasTransform);

        // 난이도 설정 영역
        CreateDifficultySettings(canvasTransform);

        // 돌아가기 버튼
        Button backButton = CreateButton(canvasTransform, "돌아가기", new Vector2(0, -300));

        // 적용 버튼
        Button applyButton = CreateButton(canvasTransform, "적용", new Vector2(200, -300));

        // 취소 버튼
        Button cancelButton = CreateButton(canvasTransform, "취소", new Vector2(-200, -300));
    }

    private static void CreateVolumeSettings(Transform canvasTransform)
    {
        // 볼륨 설정 영역
        GameObject volumeArea = new GameObject("VolumeArea", typeof(RectTransform));
        volumeArea.transform.SetParent(canvasTransform, false);
        RectTransform volumeRect = volumeArea.GetComponent<RectTransform>();
        volumeRect.anchorMin = new Vector2(0.1f, 0.6f);
        volumeRect.anchorMax = new Vector2(0.9f, 0.75f);
        volumeRect.sizeDelta = Vector2.zero;

        // 배경음 볼륨
        GameObject bgmLabel = new GameObject("BGMLabel", typeof(Text));
        bgmLabel.transform.SetParent(volumeArea.transform, false);
        RectTransform bgmLabelRect = bgmLabel.GetComponent<RectTransform>();
        bgmLabelRect.anchorMin = new Vector2(0, 0.5f);
        bgmLabelRect.anchorMax = new Vector2(0.3f, 1);
        bgmLabelRect.sizeDelta = Vector2.zero;
        Text bgmText = bgmLabel.GetComponent<Text>();
        bgmText.text = "배경음";
        bgmText.fontSize = 20;
        bgmText.alignment = TextAnchor.MiddleLeft;

        // 배경음 슬라이더
        GameObject bgmSliderGO = new GameObject("BGMSlider", typeof(Slider));
        bgmSliderGO.transform.SetParent(volumeArea.transform, false);
        RectTransform bgmSliderRect = bgmSliderGO.GetComponent<RectTransform>();
        bgmSliderRect.anchorMin = new Vector2(0.3f, 0.5f);
        bgmSliderRect.anchorMax = new Vector2(1, 1);
        bgmSliderRect.sizeDelta = Vector2.zero;
        Slider bgmSlider = bgmSliderGO.GetComponent<Slider>();
        bgmSlider.minValue = 0f;
        bgmSlider.maxValue = 1f;
        bgmSlider.value = 0.5f;

        // 효과음 볼륨
        GameObject sfxLabel = new GameObject("SFXLabel", typeof(Text));
        sfxLabel.transform.SetParent(volumeArea.transform, false);
        RectTransform sfxLabelRect = sfxLabel.GetComponent<RectTransform>();
        sfxLabelRect.anchorMin = new Vector2(0, 0);
        sfxLabelRect.anchorMax = new Vector2(0.3f, 0.5f);
        sfxLabelRect.sizeDelta = Vector2.zero;
        Text sfxText = sfxLabel.GetComponent<Text>();
        sfxText.text = "효과음";
        sfxText.fontSize = 20;
        sfxText.alignment = TextAnchor.MiddleLeft;

        // 효과음 슬라이더
        GameObject sfxSliderGO = new GameObject("SFXSlider", typeof(Slider));
        sfxSliderGO.transform.SetParent(volumeArea.transform, false);
        RectTransform sfxSliderRect = sfxSliderGO.GetComponent<RectTransform>();
        sfxSliderRect.anchorMin = new Vector2(0.3f, 0);
        sfxSliderRect.anchorMax = new Vector2(1, 0.5f);
        sfxSliderRect.sizeDelta = Vector2.zero;
        Slider sfxSlider = sfxSliderGO.GetComponent<Slider>();
        sfxSlider.minValue = 0f;
        sfxSlider.maxValue = 1f;
        sfxSlider.value = 0.5f;
    }

    private static void CreateScreenSizeSettings(Transform canvasTransform)
    {
        // 화면 크기 설정 영역
        GameObject screenArea = new GameObject("ScreenSizeArea", typeof(RectTransform));
        screenArea.transform.SetParent(canvasTransform, false);
        RectTransform screenRect = screenArea.GetComponent<RectTransform>();
        screenRect.anchorMin = new Vector2(0.1f, 0.4f);
        screenRect.anchorMax = new Vector2(0.9f, 0.55f);
        screenRect.sizeDelta = Vector2.zero;

        // 화면 크기 라벨
        GameObject screenLabel = new GameObject("ScreenSizeLabel", typeof(Text));
        screenLabel.transform.SetParent(screenArea.transform, false);
        RectTransform screenLabelRect = screenLabel.GetComponent<RectTransform>();
        screenLabelRect.anchorMin = new Vector2(0, 0);
        screenLabelRect.anchorMax = new Vector2(0.3f, 1);
        screenLabelRect.sizeDelta = Vector2.zero;
        Text screenText = screenLabel.GetComponent<Text>();
        screenText.text = "화면 크기";
        screenText.fontSize = 20;
        screenText.alignment = TextAnchor.MiddleLeft;

        // 화면 크기 드롭다운
        GameObject screenDropdownGO = new GameObject("ScreenSizeDropdown", typeof(Dropdown));
        screenDropdownGO.transform.SetParent(screenArea.transform, false);
        RectTransform screenDropdownRect = screenDropdownGO.GetComponent<RectTransform>();
        screenDropdownRect.anchorMin = new Vector2(0.3f, 0);
        screenDropdownRect.anchorMax = new Vector2(1, 1);
        screenDropdownRect.sizeDelta = Vector2.zero;
        Dropdown screenDropdown = screenDropdownGO.GetComponent<Dropdown>();
        screenDropdown.options.Clear();
        screenDropdown.options.Add(new Dropdown.OptionData("1920x1080"));
        screenDropdown.options.Add(new Dropdown.OptionData("1280x720"));
        screenDropdown.options.Add(new Dropdown.OptionData("1024x768"));
        screenDropdown.value = 0;
    }

    private static void CreateDifficultySettings(Transform canvasTransform)
    {
        // 난이도 설정 영역
        GameObject difficultyArea = new GameObject("DifficultyArea", typeof(RectTransform));
        difficultyArea.transform.SetParent(canvasTransform, false);
        RectTransform difficultyRect = difficultyArea.GetComponent<RectTransform>();
        difficultyRect.anchorMin = new Vector2(0.1f, 0.2f);
        difficultyRect.anchorMax = new Vector2(0.9f, 0.35f);
        difficultyRect.sizeDelta = Vector2.zero;

        // 난이도 라벨
        GameObject difficultyLabel = new GameObject("DifficultyLabel", typeof(Text));
        difficultyLabel.transform.SetParent(difficultyArea.transform, false);
        RectTransform difficultyLabelRect = difficultyLabel.GetComponent<RectTransform>();
        difficultyLabelRect.anchorMin = new Vector2(0, 0);
        difficultyLabelRect.anchorMax = new Vector2(0.3f, 1);
        difficultyLabelRect.sizeDelta = Vector2.zero;
        Text difficultyText = difficultyLabel.GetComponent<Text>();
        difficultyText.text = "난이도";
        difficultyText.fontSize = 20;
        difficultyText.alignment = TextAnchor.MiddleLeft;

        // 난이도 드롭다운
        GameObject difficultyDropdownGO = new GameObject("DifficultyDropdown", typeof(Dropdown));
        difficultyDropdownGO.transform.SetParent(difficultyArea.transform, false);
        RectTransform difficultyDropdownRect = difficultyDropdownGO.GetComponent<RectTransform>();
        difficultyDropdownRect.anchorMin = new Vector2(0.3f, 0);
        difficultyDropdownRect.anchorMax = new Vector2(1, 1);
        difficultyDropdownRect.sizeDelta = Vector2.zero;
        Dropdown difficultyDropdown = difficultyDropdownGO.GetComponent<Dropdown>();
        difficultyDropdown.options.Clear();
        difficultyDropdown.options.Add(new Dropdown.OptionData("쉬움"));
        difficultyDropdown.options.Add(new Dropdown.OptionData("보통"));
        difficultyDropdown.options.Add(new Dropdown.OptionData("어려움"));
        difficultyDropdown.value = 1; // 기본값: 보통
    }

    private static void SetupGameScene()
    {
        Debug.Log("GameScene 설정 중...");
        
        // GameScene 로드
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
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            GameObject gameManagerGO = new GameObject("GameManager");
            gameManager = gameManagerGO.AddComponent<GameManager>();
        }
        else
        {
            Debug.Log("GameManager가 이미 존재합니다.");
        }

        // BoardManager 찾기/생성
        BoardManager boardManager = FindFirstObjectByType<BoardManager>();
        if (boardManager == null)
        {
            GameObject boardManagerGO = new GameObject("BoardManager");
            boardManager = boardManagerGO.AddComponent<BoardManager>();
        }
        else
        {
            Debug.Log("BoardManager가 이미 존재합니다.");
        }

        // UI 요소들 생성
        CreateGameUI(canvas.transform, gameManager, boardManager);
        
        // 씬 저장
        EditorSceneManager.MarkSceneDirty(gameScene);
        EditorSceneManager.SaveScene(gameScene);
        
        Debug.Log("GameScene 설정 완료!");
    }

    // 다른 스크립트에서 재사용할 수 있도록 public으로 변경
    public static void CreateGameUI(Transform canvasTransform, GameManager gameManager, BoardManager boardManager)
    {
        // 중복 생성 방지
        if (canvasTransform.Find("TopBar") != null)
        {
            Debug.Log("GameScene UI가 이미 존재하여 생성을 건너뜁니다.");
            return;
        }

        // 상단 검정 바 (1/3 영역)
        GameObject topBar = new GameObject("TopBar", typeof(Image));
        topBar.transform.SetParent(canvasTransform, false);
        RectTransform topBarRect = topBar.GetComponent<RectTransform>();
        topBarRect.anchorMin = new Vector2(0, 0.67f);
        topBarRect.anchorMax = new Vector2(1, 1);
        topBarRect.sizeDelta = Vector2.zero;
        Image topBarImage = topBar.GetComponent<Image>();
        topBarImage.color = Color.black;

        // 격투 게임 영역 (상단 1/3 내부)
        CreateFightingArea(canvasTransform, gameManager);

        // 오셀로 영역 (하단 2/3)
        CreateOthelloArea(canvasTransform, gameManager, boardManager);

        // --- 추가된 UI 요소 생성 ---

        // 1. 보드 커서(Selector) 생성
        GameObject selectorGO = new GameObject("BoardSelector", typeof(Image));
        selectorGO.transform.SetParent(canvasTransform, false); // 씬의 루트에 생성 후 GameManager가 위치 제어
        Image selectorImage = selectorGO.GetComponent<Image>();
        selectorImage.color = new Color(1f, 1f, 0f, 0.5f); // 반투명 노란색
        selectorGO.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100); // 보드 셀 크기에 맞게 조절 필요
        gameManager.boardSelector = selectorGO;

        // 2. 조작 안내 텍스트 생성
        GameObject controlsTextGO = new GameObject("ControlsInfoText", typeof(Text));
        controlsTextGO.transform.SetParent(canvasTransform, false);
        RectTransform controlsTextRect = controlsTextGO.GetComponent<RectTransform>();
        controlsTextRect.anchorMin = new Vector2(0f, 0f);
        controlsTextRect.anchorMax = new Vector2(1f, 0.1f);
        controlsTextRect.anchoredPosition = new Vector2(0, 10);
        controlsTextRect.sizeDelta = new Vector2(-20, 0);

        Text controlsText = controlsTextGO.GetComponent<Text>();
        controlsText.text = "이동: ←→↑↓ | 결정: Z | 스킬: 1, 2, 3";
        controlsText.fontStyle = FontStyle.Bold;
        controlsText.alignment = TextAnchor.MiddleCenter;
        gameManager.controlsInfoText = controlsText;
    }

    private static void CreateFightingArea(Transform canvasTransform, GameManager gameManager)
    {
        // 격투 게임 영역
        GameObject fightingArea = new GameObject("FightingArea", typeof(RectTransform));
        fightingArea.transform.SetParent(canvasTransform, false);
        RectTransform fightingRect = fightingArea.GetComponent<RectTransform>();
        fightingRect.anchorMin = new Vector2(0, 0.67f);
        fightingRect.anchorMax = new Vector2(1, 1);
        fightingRect.sizeDelta = Vector2.zero;

        // 체력바 영역 (2/5, 4/5 위치)
        CreateHealthBars(fightingArea.transform, gameManager);

        // 캐릭터 영역
        CreateCharacterArea(fightingArea.transform, gameManager);

        // 배경 영역
        CreateBackgroundArea(fightingArea.transform, gameManager);
    }

    private static void CreateHealthBars(Transform fightingArea, GameManager gameManager)
    {
        // 1P 체력바 (2/5 위치)
        GameObject playerHpBarGO = new GameObject("PlayerHPBar", typeof(Slider));
        playerHpBarGO.transform.SetParent(fightingArea, false);
        RectTransform playerHpRect = playerHpBarGO.GetComponent<RectTransform>();
        playerHpRect.anchorMin = new Vector2(0.1f, 0.8f);
        playerHpRect.anchorMax = new Vector2(0.4f, 0.9f);
        playerHpRect.sizeDelta = Vector2.zero;
        Slider playerHpSlider = playerHpBarGO.GetComponent<Slider>();
        playerHpSlider.minValue = 0f;
        playerHpSlider.maxValue = 10000f;
        playerHpSlider.value = 10000f;
        gameManager.playerHpBar = playerHpSlider;

        // 1P 체력바 라벨
        GameObject playerHpLabel = new GameObject("PlayerHPLabel", typeof(Text));
        playerHpLabel.transform.SetParent(playerHpBarGO.transform, false);
        RectTransform playerHpLabelRect = playerHpLabel.GetComponent<RectTransform>();
        playerHpLabelRect.anchorMin = new Vector2(0, 0);
        playerHpLabelRect.anchorMax = new Vector2(1, 1);
        playerHpLabelRect.sizeDelta = Vector2.zero;
        Text playerHpText = playerHpLabel.GetComponent<Text>();
        playerHpText.text = "1P HP";
        playerHpText.fontSize = 16;
        playerHpText.alignment = TextAnchor.MiddleCenter;

        // CPU 체력바 (4/5 위치)
        GameObject cpuHpBarGO = new GameObject("CPUHPBar", typeof(Slider));
        cpuHpBarGO.transform.SetParent(fightingArea, false);
        RectTransform cpuHpRect = cpuHpBarGO.GetComponent<RectTransform>();
        cpuHpRect.anchorMin = new Vector2(0.6f, 0.8f);
        cpuHpRect.anchorMax = new Vector2(0.9f, 0.9f);
        cpuHpRect.sizeDelta = Vector2.zero;
        Slider cpuHpSlider = cpuHpBarGO.GetComponent<Slider>();
        cpuHpSlider.minValue = 0f;
        cpuHpSlider.maxValue = 10000f;
        cpuHpSlider.value = 10000f;
        gameManager.cpuHpBar = cpuHpSlider;

        // CPU 체력바 라벨
        GameObject cpuHpLabel = new GameObject("CPUHPLabel", typeof(Text));
        cpuHpLabel.transform.SetParent(cpuHpBarGO.transform, false);
        RectTransform cpuHpLabelRect = cpuHpLabel.GetComponent<RectTransform>();
        cpuHpLabelRect.anchorMin = new Vector2(0, 0);
        cpuHpLabelRect.anchorMax = new Vector2(1, 1);
        cpuHpLabelRect.sizeDelta = Vector2.zero;
        Text cpuHpText = cpuHpLabel.GetComponent<Text>();
        cpuHpText.text = "CPU HP";
        cpuHpText.fontSize = 16;
        cpuHpText.alignment = TextAnchor.MiddleCenter;
    }

    private static void CreateCharacterArea(Transform fightingArea, GameManager gameManager)
    {
        // 캐릭터 영역 (중앙)
        GameObject characterArea = new GameObject("CharacterArea", typeof(RectTransform));
        characterArea.transform.SetParent(fightingArea, false);
        RectTransform charAreaRect = characterArea.GetComponent<RectTransform>();
        charAreaRect.anchorMin = new Vector2(0.1f, 0.3f);
        charAreaRect.anchorMax = new Vector2(0.9f, 0.7f);
        charAreaRect.sizeDelta = Vector2.zero;

        // 1P 캐릭터 이미지
        GameObject playerCharGO = new GameObject("PlayerCharacter", typeof(Image));
        playerCharGO.transform.SetParent(characterArea.transform, false);
        RectTransform playerCharRect = playerCharGO.GetComponent<RectTransform>();
        playerCharRect.anchorMin = new Vector2(0, 0);
        playerCharRect.anchorMax = new Vector2(0.5f, 1);
        playerCharRect.sizeDelta = Vector2.zero;
        gameManager.playerCharacterImage = playerCharGO.GetComponent<Image>();

        // CPU 캐릭터 이미지
        GameObject cpuCharGO = new GameObject("CPUCharacter", typeof(Image));
        cpuCharGO.transform.SetParent(characterArea.transform, false);
        RectTransform cpuCharRect = cpuCharGO.GetComponent<RectTransform>();
        cpuCharRect.anchorMin = new Vector2(0.5f, 0);
        cpuCharRect.anchorMax = new Vector2(1, 1);
        cpuCharRect.sizeDelta = Vector2.zero;
        gameManager.cpuCharacterImage = cpuCharGO.GetComponent<Image>();
    }

    private static void CreateBackgroundArea(Transform fightingArea, GameManager gameManager)
    {
        // 배경 이미지
        GameObject backgroundGO = new GameObject("BackgroundImage", typeof(Image));
        backgroundGO.transform.SetParent(fightingArea, false);
        RectTransform bgRect = backgroundGO.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0);
        bgRect.anchorMax = new Vector2(1, 0.3f);
        bgRect.sizeDelta = Vector2.zero;
        gameManager.backgroundImage = backgroundGO.GetComponent<Image>();
    }

    private static void CreateOthelloArea(Transform canvasTransform, GameManager gameManager, BoardManager boardManager)
    {
        // 오셀로 영역
        GameObject othelloArea = new GameObject("OthelloArea", typeof(RectTransform));
        othelloArea.transform.SetParent(canvasTransform, false);
        RectTransform othelloRect = othelloArea.GetComponent<RectTransform>();
        othelloRect.anchorMin = new Vector2(0, 0);
        othelloRect.anchorMax = new Vector2(1, 0.67f);
        othelloRect.sizeDelta = Vector2.zero;

        // 스킬 버튼 영역
        CreateSkillButtons(othelloArea.transform, gameManager);

        // 오셀로 보드 영역
        CreateOthelloBoard(othelloArea.transform, gameManager, boardManager);

        // 턴 텍스트
        GameObject turnTextGO = new GameObject("TurnText", typeof(Text));
        turnTextGO.transform.SetParent(othelloArea.transform, false);
        RectTransform turnTextRect = turnTextGO.GetComponent<RectTransform>();
        turnTextRect.anchorMin = new Vector2(0.3f, 0.9f);
        turnTextRect.anchorMax = new Vector2(0.7f, 0.95f);
        turnTextRect.sizeDelta = Vector2.zero;
        gameManager.turnText = turnTextGO.GetComponent<Text>();
        gameManager.turnText.text = "흑 차례";
        gameManager.turnText.fontSize = 24;
        gameManager.turnText.alignment = TextAnchor.MiddleCenter;

        // 결과 텍스트
        GameObject resultTextGO = new GameObject("ResultText", typeof(Text));
        resultTextGO.transform.SetParent(othelloArea.transform, false);
        RectTransform resultTextRect = resultTextGO.GetComponent<RectTransform>();
        resultTextRect.anchorMin = new Vector2(0.3f, 0.85f);
        resultTextRect.anchorMax = new Vector2(0.7f, 0.9f);
        resultTextRect.sizeDelta = Vector2.zero;
        gameManager.resultText = resultTextGO.GetComponent<Text>();
        gameManager.resultText.text = "";
        gameManager.resultText.fontSize = 20;
        gameManager.resultText.alignment = TextAnchor.MiddleCenter;
    }

    private static void CreateSkillButtons(Transform othelloArea, GameManager gameManager)
    {
        // 1P 스킬 버튼들 (좌측)
        gameManager.playerSkillButtons = new Button[3];
        for (int i = 0; i < 3; i++)
        {
            GameObject skillButtonGO = new GameObject($"PlayerSkillButton_{i}", typeof(Button), typeof(Image));
            skillButtonGO.transform.SetParent(othelloArea.transform, false);
            RectTransform skillRect = skillButtonGO.GetComponent<RectTransform>();
            skillRect.anchorMin = new Vector2(0.05f, 0.7f + i * 0.1f);
            skillRect.anchorMax = new Vector2(0.15f, 0.8f + i * 0.1f);
            skillRect.sizeDelta = Vector2.zero;
            gameManager.playerSkillButtons[i] = skillButtonGO.GetComponent<Button>();

            // 스킬 버튼 텍스트
            GameObject skillTextGO = new GameObject("SkillText", typeof(Text));
            skillTextGO.transform.SetParent(skillButtonGO.transform, false);
            RectTransform skillTextRect = skillTextGO.GetComponent<RectTransform>();
            skillTextRect.anchorMin = Vector2.zero;
            skillTextRect.anchorMax = Vector2.one;
            skillTextRect.sizeDelta = Vector2.zero;
            Text skillText = skillTextGO.GetComponent<Text>();
            
            // 스킬 이름 설정
            switch (i)
            {
                case 0: skillText.text = "스킬A"; break;
                case 1: skillText.text = "스킬B"; break;
                case 2: skillText.text = "궁극기A"; break;
            }
            skillText.fontSize = 12;
            skillText.alignment = TextAnchor.MiddleCenter;
            skillText.color = Color.black;
        }

        // CPU 스킬 버튼들 (우측)
        gameManager.cpuSkillButtons = new Button[3];
        for (int i = 0; i < 3; i++)
        {
            GameObject skillButtonGO = new GameObject($"CPUSkillButton_{i}", typeof(Button), typeof(Image));
            skillButtonGO.transform.SetParent(othelloArea.transform, false);
            RectTransform skillRect = skillButtonGO.GetComponent<RectTransform>();
            skillRect.anchorMin = new Vector2(0.85f, 0.7f + i * 0.1f);
            skillRect.anchorMax = new Vector2(0.95f, 0.8f + i * 0.1f);
            skillRect.sizeDelta = Vector2.zero;
            gameManager.cpuSkillButtons[i] = skillButtonGO.GetComponent<Button>();

            // 스킬 버튼 텍스트
            GameObject skillTextGO = new GameObject("SkillText", typeof(Text));
            skillTextGO.transform.SetParent(skillButtonGO.transform, false);
            RectTransform skillTextRect = skillTextGO.GetComponent<RectTransform>();
            skillTextRect.anchorMin = Vector2.zero;
            skillTextRect.anchorMax = Vector2.one;
            skillTextRect.sizeDelta = Vector2.zero;
            Text skillText = skillTextGO.GetComponent<Text>();
            
            // 스킬 이름 설정
            switch (i)
            {
                case 0: skillText.text = "스킬A"; break;
                case 1: skillText.text = "스킬B"; break;
                case 2: skillText.text = "궁극기A"; break;
            }
            skillText.fontSize = 12;
            skillText.alignment = TextAnchor.MiddleCenter;
            skillText.color = Color.black;
        }
    }

    private static void CreateOthelloBoard(Transform othelloArea, GameManager gameManager, BoardManager boardManager)
    {
        // 오셀로 보드 영역 (중앙)
        GameObject boardArea = new GameObject("BoardArea", typeof(RectTransform));
        boardArea.transform.SetParent(othelloArea.transform, false);
        RectTransform boardAreaRect = boardArea.GetComponent<RectTransform>();
        boardAreaRect.anchorMin = new Vector2(0.2f, 0.1f);
        boardAreaRect.anchorMax = new Vector2(0.8f, 0.7f);
        boardAreaRect.sizeDelta = Vector2.zero;

        // 보드 배경
        GameObject boardBackground = new GameObject("BoardBackground", typeof(Image));
        boardBackground.transform.SetParent(boardArea.transform, false);
        RectTransform boardBgRect = boardBackground.GetComponent<RectTransform>();
        boardBgRect.anchorMin = Vector2.zero;
        boardBgRect.anchorMax = Vector2.one;
        boardBgRect.sizeDelta = Vector2.zero;
        Image boardBgImage = boardBackground.GetComponent<Image>();
        boardBgImage.color = new Color(0.2f, 0.6f, 0.2f); // 녹색 배경

        // 8x8 그리드 생성 (임시로 빈 오브젝트들)
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                GameObject cellGO = new GameObject($"Cell_{x}_{y}", typeof(RectTransform));
                cellGO.transform.SetParent(boardArea.transform, false);
                RectTransform cellRect = cellGO.GetComponent<RectTransform>();
                cellRect.anchorMin = new Vector2(x * 0.125f, y * 0.125f);
                cellRect.anchorMax = new Vector2((x + 1) * 0.125f, (y + 1) * 0.125f);
                cellRect.sizeDelta = Vector2.zero;
            }
        }
    }

    private static void SetupBuildSettings()
    {
        Debug.Log("빌드 설정에 씬들 추가 중...");
        
        // 빌드 설정에 씬들 추가
        string[] scenes = {
            "Assets/Scenes/MainScene.unity",
            "Assets/Scenes/CharacterSelectScene.unity", 
            "Assets/Scenes/SettingsScene.unity",
            "Assets/Scenes/GameScene.unity"
        };

        EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            buildScenes[i] = new EditorBuildSettingsScene(scenes[i], true);
        }
        EditorBuildSettings.scenes = buildScenes;
        
        Debug.Log("빌드 설정에 씬들이 추가되었습니다!");
    }

    private static Button CreateButton(Transform parent, string text, Vector2 anchoredPosition)
    {
        GameObject buttonGO = new GameObject($"Button_{text}", typeof(Button), typeof(Image));
        buttonGO.transform.SetParent(parent, false);
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchoredPosition = anchoredPosition;
        buttonRect.sizeDelta = new Vector2(150, 50);
        
        // 텍스트 추가
        GameObject textGO = new GameObject("Text", typeof(Text));
        textGO.transform.SetParent(buttonGO.transform, false);
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        Text buttonText = textGO.GetComponent<Text>();
        buttonText.text = text;
        buttonText.fontSize = 20;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        return buttonGO.GetComponent<Button>();
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Setup/Setup AudioManager & Sound")]
    public static void SetupAudioManagerAndSound()
    {
        // 1. AudioManager 오브젝트 찾기/생성
        var audioManager = GameObject.FindFirstObjectByType<AudioManager>();
        if (audioManager == null)
        {
            var go = new GameObject("AudioManager");
            audioManager = go.AddComponent<AudioManager>();
            Debug.Log("AudioManager 오브젝트가 자동 생성되었습니다.");
        }

        // 2. Assets/Audio 폴더 내 mp3/wav 파일 임포트
        string audioFolder = "Assets/Audio";
        if (!Directory.Exists(audioFolder))
        {
            Debug.LogWarning("Assets/Audio 폴더가 없습니다. 사운드 파일을 직접 넣어주세요.");
            return;
        }
        var audioFiles = Directory.GetFiles(audioFolder, "*.*").Where(f => f.EndsWith(".mp3") || f.EndsWith(".wav")).ToArray();
        if (audioFiles.Length == 0)
        {
            Debug.LogWarning("Assets/Audio 폴더에 mp3/wav 파일이 없습니다. 사운드 파일을 직접 넣어주세요.");
            return;
        }
        // 3. AudioClip으로 임포트 및 Inspector 연결
        var so = new SerializedObject(audioManager);
        var bgmList = new System.Collections.Generic.List<AudioClip>();
        foreach (var file in audioFiles)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(file.Replace("\\", "/"));
            if (clip == null) continue;
            string lower = Path.GetFileName(file).ToLower();
            if (lower.Contains("bgm")) bgmList.Add(clip);
            if (lower.Contains("click")) so.FindProperty("buttonClickSfx").objectReferenceValue = clip;
            if (lower.Contains("flip")) so.FindProperty("flipSfx").objectReferenceValue = clip;
            if (lower.Contains("skill")) so.FindProperty("skillUseSfx").objectReferenceValue = clip;
            if (lower.Contains("damage")) so.FindProperty("damageSfx").objectReferenceValue = clip;
            if (lower.Contains("ko")) so.FindProperty("koSfx").objectReferenceValue = clip;
            if (lower.Contains("finish")) so.FindProperty("finishSfx").objectReferenceValue = clip;
            if (lower.Contains("victory")) so.FindProperty("victorySfx").objectReferenceValue = clip;
            if (lower.Contains("defeat")) so.FindProperty("defeatSfx").objectReferenceValue = clip;
        }
        // bgmClips 배열 연결
        var bgmClipsProp = so.FindProperty("bgmClips");
        bgmClipsProp.arraySize = bgmList.Count;
        for (int i = 0; i < bgmList.Count; i++)
            bgmClipsProp.GetArrayElementAtIndex(i).objectReferenceValue = bgmList[i];
        so.ApplyModifiedProperties();
        Debug.Log("AudioManager Inspector 사운드 자동 연결 완료");
    }
#endif
} 
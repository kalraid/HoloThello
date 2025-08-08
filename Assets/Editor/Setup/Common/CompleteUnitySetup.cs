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
    // 로깅 설정
    private static bool enableVerboseLogging = false;
    
    [MenuItem("Tools/Complete Unity Setup - All Scenes")]
    public static void CompleteUnitySetupAll()
    {
        LogInfo("=== 홀로 격투 오셀로 Unity 설정 시작 ===");
        
        try
        {
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
            
            LogInfo("=== 모든 Unity 설정이 완료되었습니다! ===");
            LogInfo("이제 Unity Editor에서 게임을 테스트할 수 있습니다.");
        }
        catch (System.Exception e)
        {
            LogError($"Unity 설정 중 오류 발생: {e.Message}");
        }
    }

    private static void GenerateCatImages()
    {
        LogInfo("고양이 이미지 생성 중...");
        
        // CatImageGenerator의 GenerateCatImages 메서드 호출
        CatImageGenerator.GenerateCatImages();
        
        LogInfo("고양이 이미지 생성 완료!");
    }

    private static void SetupMainScene()
    {
        LogInfo("MainScene 설정 중...");
        
        try
        {
            // MainScene 로드
            Scene mainScene = EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
            
            // Canvas 찾기
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                LogError("MainScene에 Canvas가 없습니다!");
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
                LogVerbose("MainMenuManager가 이미 존재합니다.");
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
                    LogVerbose("시작하기 버튼 이벤트 연결됨");
                }
                else if (buttonName.Contains("설정하기"))
                {
                    button.onClick.AddListener(mainManager.OnClickSettings);
                    LogVerbose("설정하기 버튼 이벤트 연결됨");
                }
                else if (buttonName.Contains("종료하기"))
                {
                    button.onClick.AddListener(mainManager.OnClickExit);
                    LogVerbose("종료하기 버튼 이벤트 연결됨");
                }
            }
            
            // 씬 저장
            EditorSceneManager.SaveScene(mainScene);
            LogInfo("MainScene 설정 완료!");
        }
        catch (System.Exception e)
        {
            LogError($"MainScene 설정 중 오류: {e.Message}");
        }
    }

    private static void SetupCharacterSelectScene()
    {
        LogInfo("CharacterSelectScene 설정 중...");
        
        try
        {
            // CharacterSelectScene 로드
            Scene charScene = EditorSceneManager.OpenScene("Assets/Scenes/CharacterSelectScene.unity");
            
            // Canvas 찾기
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                LogError("CharacterSelectScene에 Canvas가 없습니다!");
                return;
            }

            // EventSystem 확인/생성
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
                LogVerbose("CharacterSelectManager가 이미 존재합니다.");
            }

            // UI 생성
            CreateCharacterSelectUI(canvas.transform, manager);
            
            // 씬 저장
            EditorSceneManager.SaveScene(charScene);
            LogInfo("CharacterSelectScene 설정 완료!");
        }
        catch (System.Exception e)
        {
            LogError($"CharacterSelectScene 설정 중 오류: {e.Message}");
        }
    }

    public static void CreateCharacterSelectUI(Transform canvasTransform, CharacterSelectManager manager)
    {
        try
        {
            // 기존 UI 제거
            Transform existingUI = canvasTransform.Find("MainUI");
            if (existingUI != null)
            {
                Object.DestroyImmediate(existingUI.gameObject);
            }

            // MainUI 생성
            GameObject mainUI = new GameObject("MainUI", typeof(RectTransform));
            mainUI.transform.SetParent(canvasTransform, false);
            RectTransform mainUIRect = mainUI.GetComponent<RectTransform>();
            mainUIRect.anchorMin = Vector2.zero;
            mainUIRect.anchorMax = Vector2.one;
            mainUIRect.sizeDelta = Vector2.zero;

            // 배경 생성
            GameObject background = new GameObject("Background", typeof(Image));
            background.transform.SetParent(mainUI.transform, false);
            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            Image bgImage = background.GetComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.1f, 1f);

            // 제목 생성
            GameObject titleGO = new GameObject("Title", typeof(Text));
            titleGO.transform.SetParent(mainUI.transform, false);
            RectTransform titleRect = titleGO.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.8f);
            titleRect.anchorMax = new Vector2(0.5f, 0.9f);
            titleRect.sizeDelta = new Vector2(400, 50);
            titleRect.anchoredPosition = Vector2.zero;
            Text titleText = titleGO.GetComponent<Text>();
            titleText.text = "캐릭터 선택";
            titleText.fontSize = 30;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;

            // 캐릭터 선택 영역 생성
            CreateCharacterSelectionArea(mainUI.transform, manager);
            
            LogVerbose("CharacterSelectScene UI 생성 완료");
        }
        catch (System.Exception e)
        {
            LogError($"CharacterSelectScene UI 생성 중 오류: {e.Message}");
        }
    }

    private static void CreateCharacterSelectionArea(Transform parent, CharacterSelectManager manager)
    {
        // 캐릭터 선택 영역
        GameObject charSelectArea = new GameObject("CharacterSelectionArea", typeof(RectTransform));
        charSelectArea.transform.SetParent(parent, false);
        RectTransform charSelectRect = charSelectArea.GetComponent<RectTransform>();
        charSelectRect.anchorMin = new Vector2(0.1f, 0.2f);
        charSelectRect.anchorMax = new Vector2(0.9f, 0.7f);
        charSelectRect.sizeDelta = Vector2.zero;

        // 캐릭터 바 생성 (10개)
        for (int i = 0; i < 10; i++)
        {
            GameObject charBar = new GameObject($"CharacterBar_{i}", typeof(Image), typeof(Button));
            charBar.transform.SetParent(charSelectArea.transform, false);
            RectTransform charBarRect = charBar.GetComponent<RectTransform>();
            charBarRect.anchorMin = new Vector2(0.1f + (i * 0.08f), 0.5f);
            charBarRect.anchorMax = new Vector2(0.18f + (i * 0.08f), 0.6f);
            charBarRect.sizeDelta = Vector2.zero;
            
            Button charButton = charBar.GetComponent<Button>();
            int characterIndex = i;
            charButton.onClick.AddListener(() => manager.OnClickCharacter(characterIndex));
        }

        // 전체 캐릭터 이미지 영역
        GameObject fullCharArea = new GameObject("FullCharacterArea", typeof(RectTransform));
        fullCharArea.transform.SetParent(charSelectArea.transform, false);
        RectTransform fullCharRect = fullCharArea.GetComponent<RectTransform>();
        fullCharRect.anchorMin = new Vector2(0.1f, 0.1f);
        fullCharRect.anchorMax = new Vector2(0.9f, 0.4f);
        fullCharRect.sizeDelta = Vector2.zero;

        GameObject fullCharImageGO = new GameObject("FullCharacterImage", typeof(Image));
        fullCharImageGO.transform.SetParent(fullCharArea.transform, false);
        RectTransform fullCharImageRect = fullCharImageGO.GetComponent<RectTransform>();
        fullCharImageRect.anchorMin = Vector2.zero;
        fullCharImageRect.anchorMax = Vector2.one;
        fullCharImageRect.sizeDelta = Vector2.zero;

        // 선택 라벨
        GameObject selectLabelGO = new GameObject("SelectLabel", typeof(Text));
        selectLabelGO.transform.SetParent(fullCharArea.transform, false);
        RectTransform selectLabelRect = selectLabelGO.GetComponent<RectTransform>();
        selectLabelRect.anchorMin = new Vector2(0.5f, 0.9f);
        selectLabelRect.anchorMax = new Vector2(0.5f, 1f);
        selectLabelRect.sizeDelta = new Vector2(200, 30);
        selectLabelRect.anchoredPosition = Vector2.zero;
        Text selectLabelText = selectLabelGO.GetComponent<Text>();
        selectLabelText.text = "캐릭터를 선택하세요";
        selectLabelText.fontSize = 16;
        selectLabelText.alignment = TextAnchor.MiddleCenter;
        selectLabelText.color = Color.white;
    }

    private static void SetupSettingsScene()
    {
        LogInfo("SettingsScene 설정 중...");
        
        try
        {
            // SettingsScene 로드
            Scene settingsScene = EditorSceneManager.OpenScene("Assets/Scenes/SettingsScene.unity");
            
            // Canvas 찾기
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                LogError("SettingsScene에 Canvas가 없습니다!");
                return;
            }

            // EventSystem 확인/생성
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
                LogVerbose("SettingsManager가 이미 존재합니다.");
            }

            // UI 생성
            CreateSettingsUI(canvas.transform);
            
            // 씬 저장
            EditorSceneManager.SaveScene(settingsScene);
            LogInfo("SettingsScene 설정 완료!");
        }
        catch (System.Exception e)
        {
            LogError($"SettingsScene 설정 중 오류: {e.Message}");
        }
    }

    public static void CreateSettingsUI(Transform canvasTransform)
    {
        try
        {
            // 기존 UI 제거
            Transform existingUI = canvasTransform.Find("MainUI");
            if (existingUI != null)
            {
                Object.DestroyImmediate(existingUI.gameObject);
            }

            // MainUI 생성
            GameObject mainUI = new GameObject("MainUI", typeof(RectTransform));
            mainUI.transform.SetParent(canvasTransform, false);
            RectTransform mainUIRect = mainUI.GetComponent<RectTransform>();
            mainUIRect.anchorMin = Vector2.zero;
            mainUIRect.anchorMax = Vector2.one;
            mainUIRect.sizeDelta = Vector2.zero;

            // 배경 생성
            GameObject background = new GameObject("Background", typeof(Image));
            background.transform.SetParent(mainUI.transform, false);
            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            Image bgImage = background.GetComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.1f, 1f);

            // 제목 생성
            GameObject titleGO = new GameObject("Title", typeof(Text));
            titleGO.transform.SetParent(mainUI.transform, false);
            RectTransform titleRect = titleGO.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.9f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.sizeDelta = new Vector2(400, 50);
            titleRect.anchoredPosition = Vector2.zero;
            Text titleText = titleGO.GetComponent<Text>();
            titleText.text = "설정";
            titleText.fontSize = 30;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;

            // 설정 영역들 생성
            CreateVolumeSettings(mainUI.transform);
            CreateScreenSizeSettings(mainUI.transform);
            CreateDifficultySettings(mainUI.transform);
            
            LogVerbose("SettingsScene UI 생성 완료");
        }
        catch (System.Exception e)
        {
            LogError($"SettingsScene UI 생성 중 오류: {e.Message}");
        }
    }

    private static void CreateVolumeSettings(Transform canvasTransform)
    {
        // 볼륨 설정 영역
        GameObject volumeArea = new GameObject("VolumeArea", typeof(RectTransform));
        volumeArea.transform.SetParent(canvasTransform, false);
        RectTransform volumeRect = volumeArea.GetComponent<RectTransform>();
        volumeRect.anchorMin = new Vector2(0.1f, 0.7f);
        volumeRect.anchorMax = new Vector2(0.9f, 0.8f);
        volumeRect.sizeDelta = Vector2.zero;

        // BGM 라벨
        GameObject bgmLabel = new GameObject("BGMLabel", typeof(Text));
        bgmLabel.transform.SetParent(volumeArea.transform, false);
        RectTransform bgmLabelRect = bgmLabel.GetComponent<RectTransform>();
        bgmLabelRect.anchorMin = new Vector2(0.1f, 0.5f);
        bgmLabelRect.anchorMax = new Vector2(0.3f, 1f);
        bgmLabelRect.sizeDelta = Vector2.zero;
        Text bgmLabelText = bgmLabel.GetComponent<Text>();
        bgmLabelText.text = "BGM 볼륨";
        bgmLabelText.fontSize = 16;
        bgmLabelText.alignment = TextAnchor.MiddleLeft;
        bgmLabelText.color = Color.white;

        // BGM 슬라이더
        GameObject bgmSliderGO = new GameObject("BGMSlider", typeof(Slider));
        bgmSliderGO.transform.SetParent(volumeArea.transform, false);
        RectTransform bgmSliderRect = bgmSliderGO.GetComponent<RectTransform>();
        bgmSliderRect.anchorMin = new Vector2(0.3f, 0.5f);
        bgmSliderRect.anchorMax = new Vector2(0.9f, 1f);
        bgmSliderRect.sizeDelta = Vector2.zero;
        Slider bgmSlider = bgmSliderGO.GetComponent<Slider>();
        bgmSlider.minValue = 0f;
        bgmSlider.maxValue = 1f;
        bgmSlider.value = 0.5f;

        // SFX 설정 (비슷한 구조)
        GameObject sfxLabel = new GameObject("SFXLabel", typeof(Text));
        sfxLabel.transform.SetParent(volumeArea.transform, false);
        RectTransform sfxLabelRect = sfxLabel.GetComponent<RectTransform>();
        sfxLabelRect.anchorMin = new Vector2(0.1f, 0f);
        sfxLabelRect.anchorMax = new Vector2(0.3f, 0.5f);
        sfxLabelRect.sizeDelta = Vector2.zero;
        Text sfxLabelText = sfxLabel.GetComponent<Text>();
        sfxLabelText.text = "SFX 볼륨";
        sfxLabelText.fontSize = 16;
        sfxLabelText.alignment = TextAnchor.MiddleLeft;
        sfxLabelText.color = Color.white;

        GameObject sfxSliderGO = new GameObject("SFXSlider", typeof(Slider));
        sfxSliderGO.transform.SetParent(volumeArea.transform, false);
        RectTransform sfxSliderRect = sfxSliderGO.GetComponent<RectTransform>();
        sfxSliderRect.anchorMin = new Vector2(0.3f, 0f);
        sfxSliderRect.anchorMax = new Vector2(0.9f, 0.5f);
        sfxSliderRect.sizeDelta = Vector2.zero;
        Slider sfxSlider = sfxSliderGO.GetComponent<Slider>();
        sfxSlider.minValue = 0f;
        sfxSlider.maxValue = 1f;
        sfxSlider.value = 0.7f;
    }

    private static void CreateScreenSizeSettings(Transform canvasTransform)
    {
        // 화면 크기 설정 영역
        GameObject screenArea = new GameObject("ScreenSizeArea", typeof(RectTransform));
        screenArea.transform.SetParent(canvasTransform, false);
        RectTransform screenRect = screenArea.GetComponent<RectTransform>();
        screenRect.anchorMin = new Vector2(0.1f, 0.5f);
        screenRect.anchorMax = new Vector2(0.9f, 0.6f);
        screenRect.sizeDelta = Vector2.zero;

        // 화면 크기 라벨
        GameObject screenLabel = new GameObject("ScreenSizeLabel", typeof(Text));
        screenLabel.transform.SetParent(screenArea.transform, false);
        RectTransform screenLabelRect = screenLabel.GetComponent<RectTransform>();
        screenLabelRect.anchorMin = new Vector2(0.1f, 0.5f);
        screenLabelRect.anchorMax = new Vector2(0.3f, 1f);
        screenLabelRect.sizeDelta = Vector2.zero;
        Text screenLabelText = screenLabel.GetComponent<Text>();
        screenLabelText.text = "화면 크기";
        screenLabelText.fontSize = 16;
        screenLabelText.alignment = TextAnchor.MiddleLeft;
        screenLabelText.color = Color.white;

        // 화면 크기 드롭다운
        GameObject screenDropdownGO = new GameObject("ScreenSizeDropdown", typeof(Dropdown));
        screenDropdownGO.transform.SetParent(screenArea.transform, false);
        RectTransform screenDropdownRect = screenDropdownGO.GetComponent<RectTransform>();
        screenDropdownRect.anchorMin = new Vector2(0.3f, 0.5f);
        screenDropdownRect.anchorMax = new Vector2(0.9f, 1f);
        screenDropdownRect.sizeDelta = Vector2.zero;
        Dropdown screenDropdown = screenDropdownGO.GetComponent<Dropdown>();
        screenDropdown.options.Clear();
        screenDropdown.options.Add(new Dropdown.OptionData("1920x1080"));
        screenDropdown.options.Add(new Dropdown.OptionData("1280x720"));
        screenDropdown.options.Add(new Dropdown.OptionData("800x600"));
        screenDropdown.value = 0;
    }

    private static void CreateDifficultySettings(Transform canvasTransform)
    {
        // 난이도 설정 영역
        GameObject difficultyArea = new GameObject("DifficultyArea", typeof(RectTransform));
        difficultyArea.transform.SetParent(canvasTransform, false);
        RectTransform difficultyRect = difficultyArea.GetComponent<RectTransform>();
        difficultyRect.anchorMin = new Vector2(0.1f, 0.3f);
        difficultyRect.anchorMax = new Vector2(0.9f, 0.4f);
        difficultyRect.sizeDelta = Vector2.zero;

        // 난이도 라벨
        GameObject difficultyLabel = new GameObject("DifficultyLabel", typeof(Text));
        difficultyLabel.transform.SetParent(difficultyArea.transform, false);
        RectTransform difficultyLabelRect = difficultyLabel.GetComponent<RectTransform>();
        difficultyLabelRect.anchorMin = new Vector2(0.1f, 0.5f);
        difficultyLabelRect.anchorMax = new Vector2(0.3f, 1f);
        difficultyLabelRect.sizeDelta = Vector2.zero;
        Text difficultyLabelText = difficultyLabel.GetComponent<Text>();
        difficultyLabelText.text = "난이도";
        difficultyLabelText.fontSize = 16;
        difficultyLabelText.alignment = TextAnchor.MiddleLeft;
        difficultyLabelText.color = Color.white;

        // 난이도 드롭다운
        GameObject difficultyDropdownGO = new GameObject("DifficultyDropdown", typeof(Dropdown));
        difficultyDropdownGO.transform.SetParent(difficultyArea.transform, false);
        RectTransform difficultyDropdownRect = difficultyDropdownGO.GetComponent<RectTransform>();
        difficultyDropdownRect.anchorMin = new Vector2(0.3f, 0.5f);
        difficultyDropdownRect.anchorMax = new Vector2(0.9f, 1f);
        difficultyDropdownRect.sizeDelta = Vector2.zero;
        Dropdown difficultyDropdown = difficultyDropdownGO.GetComponent<Dropdown>();
        difficultyDropdown.options.Clear();
        difficultyDropdown.options.Add(new Dropdown.OptionData("쉬움"));
        difficultyDropdown.options.Add(new Dropdown.OptionData("보통"));
        difficultyDropdown.options.Add(new Dropdown.OptionData("어려움"));
        difficultyDropdown.value = 1;
    }

    private static void SetupGameScene()
    {
        LogInfo("GameScene 설정 중...");
        
        try
        {
            // GameScene 로드
            Scene gameScene = EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");
            
            // Canvas 찾기
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                LogError("GameScene에 Canvas가 없습니다!");
                return;
            }

            // EventSystem 확인/생성
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
                LogVerbose("GameManager가 이미 존재합니다.");
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
                LogVerbose("BoardManager가 이미 존재합니다.");
            }

            // UI 생성
            CreateGameUI(canvas.transform, gameManager, boardManager);
            
            // 씬 저장
            EditorSceneManager.SaveScene(gameScene);
            LogInfo("GameScene 설정 완료!");
        }
        catch (System.Exception e)
        {
            LogError($"GameScene 설정 중 오류: {e.Message}");
        }
    }

    public static void CreateGameUI(Transform canvasTransform, GameManager gameManager, BoardManager boardManager)
    {
        try
        {
            // 기존 UI 제거
            Transform existingUI = canvasTransform.Find("MainUI");
            if (existingUI != null)
            {
                Object.DestroyImmediate(existingUI.gameObject);
            }

            // MainUI 생성
            GameObject mainUI = new GameObject("MainUI", typeof(RectTransform));
            mainUI.transform.SetParent(canvasTransform, false);
            RectTransform mainUIRect = mainUI.GetComponent<RectTransform>();
            mainUIRect.anchorMin = Vector2.zero;
            mainUIRect.anchorMax = Vector2.one;
            mainUIRect.sizeDelta = Vector2.zero;

            // 배경 생성
            GameObject background = new GameObject("Background", typeof(Image));
            background.transform.SetParent(mainUI.transform, false);
            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            Image bgImage = background.GetComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.1f, 1f);

            // 게임 영역들 생성
            CreateFightingArea(mainUI.transform, gameManager);
            CreateOthelloArea(mainUI.transform, gameManager, boardManager);
            
            LogVerbose("GameScene UI 생성 완료");
        }
        catch (System.Exception e)
        {
            LogError($"GameScene UI 생성 중 오류: {e.Message}");
        }
    }

    private static void CreateFightingArea(Transform canvasTransform, GameManager gameManager)
    {
        // 격투 영역
        GameObject fightingArea = new GameObject("FightingArea", typeof(RectTransform));
        fightingArea.transform.SetParent(canvasTransform, false);
        RectTransform fightingRect = fightingArea.GetComponent<RectTransform>();
        fightingRect.anchorMin = new Vector2(0f, 0.5f);
        fightingRect.anchorMax = new Vector2(1f, 1f);
        fightingRect.sizeDelta = Vector2.zero;

        // HP 바들 생성
        CreateHealthBars(fightingArea.transform, gameManager);
        
        // 캐릭터 영역
        CreateCharacterArea(fightingArea.transform, gameManager);
        
        // 배경 영역
        CreateBackgroundArea(fightingArea.transform, gameManager);
    }

    private static void CreateHealthBars(Transform fightingArea, GameManager gameManager)
    {
        // 플레이어 HP 바
        GameObject playerHpBarGO = new GameObject("PlayerHPBar", typeof(Slider));
        playerHpBarGO.transform.SetParent(fightingArea.transform, false);
        RectTransform playerHpBarRect = playerHpBarGO.GetComponent<RectTransform>();
        playerHpBarRect.anchorMin = new Vector2(0.1f, 0.9f);
        playerHpBarRect.anchorMax = new Vector2(0.4f, 0.95f);
        playerHpBarRect.sizeDelta = Vector2.zero;
        Slider playerHpBar = playerHpBarGO.GetComponent<Slider>();
        playerHpBar.minValue = 0f;
        playerHpBar.maxValue = 100f;
        playerHpBar.value = 100f;

        // 플레이어 HP 라벨
        GameObject playerHpLabel = new GameObject("PlayerHPLabel", typeof(Text));
        playerHpLabel.transform.SetParent(fightingArea.transform, false);
        RectTransform playerHpLabelRect = playerHpLabel.GetComponent<RectTransform>();
        playerHpLabelRect.anchorMin = new Vector2(0.1f, 0.85f);
        playerHpLabelRect.anchorMax = new Vector2(0.4f, 0.9f);
        playerHpLabelRect.sizeDelta = Vector2.zero;
        Text playerHpLabelText = playerHpLabel.GetComponent<Text>();
        playerHpLabelText.text = "플레이어 HP";
        playerHpLabelText.fontSize = 14;
        playerHpLabelText.alignment = TextAnchor.MiddleLeft;
        playerHpLabelText.color = Color.white;

        // CPU HP 바
        GameObject cpuHpBarGO = new GameObject("CPUHPBar", typeof(Slider));
        cpuHpBarGO.transform.SetParent(fightingArea.transform, false);
        RectTransform cpuHpBarRect = cpuHpBarGO.GetComponent<RectTransform>();
        cpuHpBarRect.anchorMin = new Vector2(0.6f, 0.9f);
        cpuHpBarRect.anchorMax = new Vector2(0.9f, 0.95f);
        cpuHpBarRect.sizeDelta = Vector2.zero;
        Slider cpuHpBar = cpuHpBarGO.GetComponent<Slider>();
        cpuHpBar.minValue = 0f;
        cpuHpBar.maxValue = 100f;
        cpuHpBar.value = 100f;

        // CPU HP 라벨
        GameObject cpuHpLabel = new GameObject("CPUHPLabel", typeof(Text));
        cpuHpLabel.transform.SetParent(fightingArea.transform, false);
        RectTransform cpuHpLabelRect = cpuHpLabel.GetComponent<RectTransform>();
        cpuHpLabelRect.anchorMin = new Vector2(0.6f, 0.85f);
        cpuHpLabelRect.anchorMax = new Vector2(0.9f, 0.9f);
        cpuHpLabelRect.sizeDelta = Vector2.zero;
        Text cpuHpLabelText = cpuHpLabel.GetComponent<Text>();
        cpuHpLabelText.text = "CPU HP";
        cpuHpLabelText.fontSize = 14;
        cpuHpLabelText.alignment = TextAnchor.MiddleRight;
        cpuHpLabelText.color = Color.white;
    }

    private static void CreateCharacterArea(Transform fightingArea, GameManager gameManager)
    {
        // 캐릭터 영역
        GameObject characterArea = new GameObject("CharacterArea", typeof(RectTransform));
        characterArea.transform.SetParent(fightingArea.transform, false);
        RectTransform characterRect = characterArea.GetComponent<RectTransform>();
        characterRect.anchorMin = new Vector2(0.1f, 0.6f);
        characterRect.anchorMax = new Vector2(0.9f, 0.8f);
        characterRect.sizeDelta = Vector2.zero;

        // 플레이어 캐릭터
        GameObject playerCharGO = new GameObject("PlayerCharacter", typeof(Image));
        playerCharGO.transform.SetParent(characterArea.transform, false);
        RectTransform playerCharRect = playerCharGO.GetComponent<RectTransform>();
        playerCharRect.anchorMin = new Vector2(0.1f, 0.1f);
        playerCharRect.anchorMax = new Vector2(0.4f, 0.9f);
        playerCharRect.sizeDelta = Vector2.zero;
        Image playerCharImage = playerCharGO.GetComponent<Image>();
        playerCharImage.color = Color.blue;

        // CPU 캐릭터
        GameObject cpuCharGO = new GameObject("CPUCharacter", typeof(Image));
        cpuCharGO.transform.SetParent(characterArea.transform, false);
        RectTransform cpuCharRect = cpuCharGO.GetComponent<RectTransform>();
        cpuCharRect.anchorMin = new Vector2(0.6f, 0.1f);
        cpuCharRect.anchorMax = new Vector2(0.9f, 0.9f);
        cpuCharRect.sizeDelta = Vector2.zero;
        Image cpuCharImage = cpuCharGO.GetComponent<Image>();
        cpuCharImage.color = Color.red;
    }

    private static void CreateBackgroundArea(Transform fightingArea, GameManager gameManager)
    {
        // 배경 영역
        GameObject backgroundGO = new GameObject("BackgroundImage", typeof(Image));
        backgroundGO.transform.SetParent(fightingArea.transform, false);
        RectTransform backgroundRect = backgroundGO.GetComponent<RectTransform>();
        backgroundRect.anchorMin = new Vector2(0.1f, 0.1f);
        backgroundRect.anchorMax = new Vector2(0.9f, 0.5f);
        backgroundRect.sizeDelta = Vector2.zero;
        Image backgroundImage = backgroundGO.GetComponent<Image>();
        backgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
    }

    private static void CreateOthelloArea(Transform canvasTransform, GameManager gameManager, BoardManager boardManager)
    {
        // 오셀로 영역
        GameObject othelloArea = new GameObject("OthelloArea", typeof(RectTransform));
        othelloArea.transform.SetParent(canvasTransform, false);
        RectTransform othelloRect = othelloArea.GetComponent<RectTransform>();
        othelloRect.anchorMin = new Vector2(0f, 0f);
        othelloRect.anchorMax = new Vector2(1f, 0.5f);
        othelloRect.sizeDelta = Vector2.zero;

        // 턴 텍스트
        GameObject turnTextGO = new GameObject("TurnText", typeof(Text));
        turnTextGO.transform.SetParent(othelloArea.transform, false);
        RectTransform turnTextRect = turnTextGO.GetComponent<RectTransform>();
        turnTextRect.anchorMin = new Vector2(0.5f, 0.9f);
        turnTextRect.anchorMax = new Vector2(0.5f, 1f);
        turnTextRect.sizeDelta = new Vector2(300, 30);
        turnTextRect.anchoredPosition = Vector2.zero;
        Text turnText = turnTextGO.GetComponent<Text>();
        turnText.text = "플레이어 턴";
        turnText.fontSize = 20;
        turnText.alignment = TextAnchor.MiddleCenter;
        turnText.color = Color.white;

        // 결과 텍스트
        GameObject resultTextGO = new GameObject("ResultText", typeof(Text));
        resultTextGO.transform.SetParent(othelloArea.transform, false);
        RectTransform resultTextRect = resultTextGO.GetComponent<RectTransform>();
        resultTextRect.anchorMin = new Vector2(0.5f, 0.8f);
        resultTextRect.anchorMax = new Vector2(0.5f, 0.9f);
        resultTextRect.sizeDelta = new Vector2(300, 30);
        resultTextRect.anchoredPosition = Vector2.zero;
        Text resultText = resultTextGO.GetComponent<Text>();
        resultText.text = "";
        resultText.fontSize = 18;
        resultText.alignment = TextAnchor.MiddleCenter;
        resultText.color = Color.yellow;

        // 스킬 버튼들 생성
        CreateSkillButtons(othelloArea.transform, gameManager);
        
        // 오셀로 보드 생성
        CreateOthelloBoard(othelloArea.transform, gameManager, boardManager);
    }

    private static void CreateSkillButtons(Transform othelloArea, GameManager gameManager)
    {
        // 스킬 버튼들 (3개)
        for (int i = 0; i < 3; i++)
        {
            GameObject skillButtonGO = new GameObject($"PlayerSkillButton_{i}", typeof(Button), typeof(Image));
            skillButtonGO.transform.SetParent(othelloArea.transform, false);
            RectTransform skillButtonRect = skillButtonGO.GetComponent<RectTransform>();
            skillButtonRect.anchorMin = new Vector2(0.1f + (i * 0.25f), 0.7f);
            skillButtonRect.anchorMax = new Vector2(0.3f + (i * 0.25f), 0.8f);
            skillButtonRect.sizeDelta = Vector2.zero;
            
            Button skillButton = skillButtonGO.GetComponent<Button>();
            int skillIndex = i;
            skillButton.onClick.AddListener(() => gameManager.OnClickPlayerSkill(skillIndex));

            // 스킬 텍스트
            GameObject skillTextGO = new GameObject("SkillText", typeof(Text));
            skillTextGO.transform.SetParent(skillButtonGO.transform, false);
            RectTransform skillTextRect = skillTextGO.GetComponent<RectTransform>();
            skillTextRect.anchorMin = Vector2.zero;
            skillTextRect.anchorMax = Vector2.one;
            skillTextRect.sizeDelta = Vector2.zero;
            Text skillText = skillTextGO.GetComponent<Text>();
            skillText.text = $"스킬 {i}";
            skillText.fontSize = 14;
            skillText.alignment = TextAnchor.MiddleCenter;
            skillText.color = Color.white;
        }
    }

    private static void CreateOthelloBoard(Transform othelloArea, GameManager gameManager, BoardManager boardManager)
    {
        // 오셀로 보드 영역
        GameObject boardArea = new GameObject("BoardArea", typeof(RectTransform));
        boardArea.transform.SetParent(othelloArea.transform, false);
        RectTransform boardRect = boardArea.GetComponent<RectTransform>();
        boardRect.anchorMin = new Vector2(0.1f, 0.1f);
        boardRect.anchorMax = new Vector2(0.9f, 0.6f);
        boardRect.sizeDelta = Vector2.zero;

        // 8x8 보드 생성
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                GameObject cellGO = new GameObject($"Cell_{row}_{col}", typeof(Image), typeof(Button));
                cellGO.transform.SetParent(boardArea.transform, false);
                RectTransform cellRect = cellGO.GetComponent<RectTransform>();
                
                float cellWidth = 1f / 8f;
                float cellHeight = 1f / 8f;
                cellRect.anchorMin = new Vector2(col * cellWidth, (7 - row) * cellHeight);
                cellRect.anchorMax = new Vector2((col + 1) * cellWidth, (8 - row) * cellHeight);
                cellRect.sizeDelta = Vector2.zero;
                
                Image cellImage = cellGO.GetComponent<Image>();
                cellImage.color = (row + col) % 2 == 0 ? Color.green : Color.darkGreen;
                
                Button cellButton = cellGO.GetComponent<Button>();
                int rowIndex = row;
                int colIndex = col;
                //cellButton.onClick.AddListener(() => boardManager.OnCellClicked(rowIndex, colIndex)); // BoardManager에 OnCellClicked가 없으므로 주석 처리
            }
        }
    }

    private static void SetupBuildSettings()
    {
        LogInfo("빌드 설정에 씬들 추가 중...");
        
        try
        {
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
            
            LogInfo("빌드 설정에 씬들이 추가되었습니다!");
        }
        catch (System.Exception e)
        {
            LogError($"빌드 설정 중 오류: {e.Message}");
        }
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

    // 로깅 유틸리티 메서드들
    private static void LogInfo(string message)
    {
        Debug.Log(message);
    }
    
    private static void LogVerbose(string message)
    {
        if (enableVerboseLogging)
        {
            Debug.Log($"[Verbose] {message}");
        }
    }
    
    private static void LogError(string message)
    {
        Debug.LogError(message);
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Setup/Setup AudioManager & Sound")]
    public static void SetupAudioManagerAndSound()
    {
        try
        {
            // 1. AudioManager 오브젝트 찾기/생성
            var audioManager = GameObject.FindFirstObjectByType<AudioManager>();
            if (audioManager == null)
            {
                var go = new GameObject("AudioManager");
                audioManager = go.AddComponent<AudioManager>();
                LogInfo("AudioManager 오브젝트가 자동 생성되었습니다.");
            }

            // 2. Assets/Audio 폴더 내 mp3/wav 파일 임포트
            string audioFolder = "Assets/Audio";
            if (!Directory.Exists(audioFolder))
            {
                LogError("Assets/Audio 폴더가 없습니다. 사운드 파일을 직접 넣어주세요.");
                return;
            }
            
            var audioFiles = Directory.GetFiles(audioFolder, "*.*").Where(f => f.EndsWith(".mp3") || f.EndsWith(".wav")).ToArray();
            if (audioFiles.Length == 0)
            {
                LogError("Assets/Audio 폴더에 mp3/wav 파일이 없습니다. 사운드 파일을 직접 넣어주세요.");
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
            LogInfo("AudioManager Inspector 사운드 자동 연결 완료");
        }
        catch (System.Exception e)
        {
            LogError($"AudioManager 설정 중 오류: {e.Message}");
        }
    }
#endif
} 
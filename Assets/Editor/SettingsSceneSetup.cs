using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class SettingsSceneSetup : MonoBehaviour
{
    [MenuItem("Tools/Setup SettingsScene")]
    public static void SetupSettingsScene()
    {
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

        // SettingsManager GameObject 생성 (나중에 구현할 스크립트)
        GameObject managerGO = new GameObject("SettingsManager");
        
        // UI 요소들 생성
        CreateSettingsUI(canvas.transform);
        
        // 씬 저장
        EditorSceneManager.MarkSceneDirty(settingsScene);
        EditorSceneManager.SaveScene(settingsScene);
        
        Debug.Log("SettingsScene UI 설정이 완료되었습니다!");
    }

    private static void CreateSettingsUI(Transform canvasTransform)
    {
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
        // backButton.onClick.AddListener(() => SceneManager.LoadScene("MainScene"));

        // 적용 버튼
        Button applyButton = CreateButton(canvasTransform, "적용", new Vector2(200, -300));
        // applyButton.onClick.AddListener(() => ApplySettings());

        // 취소 버튼
        Button cancelButton = CreateButton(canvasTransform, "취소", new Vector2(-200, -300));
        // cancelButton.onClick.AddListener(() => CancelSettings());
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
} 
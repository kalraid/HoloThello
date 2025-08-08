using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using TMPro; // TextMeshPro 사용을 위해 추가

public class AutoSceneUISetup : MonoBehaviour
{
    [MenuItem("Tools/Auto HoloThello Setup")]
    public static void SetupHoloThelloProject()
    {
        // 씬 경로 정의
        string mainScenePath = "Assets/Scenes/MainScene.unity";
        string characterSelectScenePath = "Assets/Scenes/CharacterSelectScene.unity";
        string settingsScenePath = "Assets/Scenes/SettingsScene.unity";
        string gameScenePath = "Assets/Scenes/GameScene.unity";

        // Scenes 폴더 생성 (없으면)
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }

        // 씬 생성 및 빌드 설정에 추가
        CreateAndAddSceneToBuildSettings(mainScenePath);
        CreateAndAddSceneToBuildSettings(characterSelectScenePath);
        CreateAndAddSceneToBuildSettings(settingsScenePath);
        CreateAndAddSceneToBuildSettings(gameScenePath);

        // MainScene 로드 및 설정
        Scene mainScene = EditorSceneManager.OpenScene(mainScenePath);
        SetupMainScene(mainScene);

        EditorUtility.DisplayDialog("HoloThello Setup", "Unity 프로젝트 자동 설정이 완료되었습니다!", "확인");
    }

    private static void CreateAndAddSceneToBuildSettings(string scenePath)
    {
        if (!System.IO.File.Exists(scenePath))
        {
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            EditorSceneManager.SaveScene(newScene, scenePath);
            EditorSceneManager.CloseScene(newScene, true);
            Debug.Log($"Created new scene: {scenePath}");
        }

        // 빌드 설정에 씬 추가
        EditorBuildSettingsScene[] currentScenes = EditorBuildSettings.scenes;
        bool sceneExistsInBuildSettings = false;
        foreach (var s in currentScenes)
        {
            if (s.path == scenePath)
            {
                sceneExistsInBuildSettings = true;
                break;
            }
        }

        if (!sceneExistsInBuildSettings)
        {
            EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[currentScenes.Length + 1];
            System.Array.Copy(currentScenes, newScenes, currentScenes.Length);
            newScenes[currentScenes.Length] = new EditorBuildSettingsScene(scenePath, true);
            EditorBuildSettings.scenes = newScenes;
            Debug.Log($"Added scene to build settings: {scenePath}");
        }
    }

    private static void SetupMainScene(Scene scene)
    {
        EditorSceneManager.SetActiveScene(scene);

        // Canvas 찾기 또는 생성
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.transform.SetAsLastSibling(); // Canvas를 Hierarchy의 마지막으로 이동
        }

        // EventSystem 찾기 또는 생성
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        // MainMenuManager 찾기 또는 생성
        MainMenuManager mainManager = FindFirstObjectByType<MainMenuManager>();
        if (mainManager == null)
        {
            GameObject mainManagerGO = new GameObject("MainMenuManager");
            mainManager = mainManagerGO.AddComponent<MainMenuManager>();
        }

        // --- UI 요소 생성 (중복 방지 추가) ---

        // Background Image
        Image backgroundImage = FindComponentInChildren<Image>(canvas.transform, "BackgroundImage");
        if (backgroundImage == null)
        {
            GameObject bgImageGO = new GameObject("BackgroundImage", typeof(Image));
            bgImageGO.transform.SetParent(canvas.transform, false);
            RectTransform bgRect = bgImageGO.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            mainManager.backgroundImage = bgImageGO.GetComponent<Image>();
            mainManager.backgroundImage.color = Color.gray;
        }
        else
        {
            mainManager.backgroundImage = backgroundImage;
        }

        // Character Image
        Image characterImage = FindComponentInChildren<Image>(canvas.transform, "CharacterImage");
        if (characterImage == null)
        {
            GameObject charImageGO = new GameObject("CharacterImage", typeof(Image));
            charImageGO.transform.SetParent(canvas.transform, false);
            RectTransform charRect = charImageGO.GetComponent<RectTransform>();
            charRect.anchoredPosition = new Vector2(0, -100);
            charRect.sizeDelta = new Vector2(300, 300);
            mainManager.characterImage = charImageGO.GetComponent<Image>();
            mainManager.characterImage.color = Color.white;
        }
        else
        {
            mainManager.characterImage = characterImage;
        }


        // Buttons
        Button startButton = FindOrCreateButton(canvas.transform, "Button_시작하기", "시작하기", new Vector2(0, 100));
        Button settingsButton = FindOrCreateButton(canvas.transform, "Button_설정하기", "설정하기", new Vector2(0, 0));
        Button exitButton = FindOrCreateButton(canvas.transform, "Button_종료하기", "종료하기", new Vector2(0, -100));

        // 버튼 OnClick 이벤트 연결 (중복 방지)
        startButton.onClick.RemoveAllListeners();
        settingsButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
        
        startButton.onClick.AddListener(mainManager.OnStartButtonClicked);
        settingsButton.onClick.AddListener(mainManager.OnSettingsButtonClicked);
        exitButton.onClick.AddListener(mainManager.OnQuitButtonClicked);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }
    
    private static T FindComponentInChildren<T>(Transform parent, string name) where T : Component
    {
        Transform child = parent.Find(name);
        if (child != null)
        {
            return child.GetComponent<T>();
        }
        return null;
    }

    private static Button FindOrCreateButton(Transform parent, string gameObjectName, string text, Vector2 anchoredPosition)
    {
        Transform buttonTransform = parent.Find(gameObjectName);
        if (buttonTransform != null)
        {
            return buttonTransform.GetComponent<Button>();
        }
        return CreateButton(parent, text, anchoredPosition);
    }

    private static Button CreateButton(Transform parent, string text, Vector2 anchoredPosition)
    {
        GameObject buttonGO = new GameObject("Button_" + text, typeof(RectTransform), typeof(Button), typeof(Image));
        buttonGO.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonGO.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50);
        rectTransform.anchoredPosition = anchoredPosition;

        // 버튼 배경 이미지 (임시)
        Image buttonImage = buttonGO.GetComponent<Image>();
        buttonImage.color = Color.white;

        // 버튼 텍스트
        GameObject textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(buttonGO.transform, false);
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        TextMeshProUGUI tmpText = textGO.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = 24;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = Color.black;

        return buttonGO.GetComponent<Button>();
    }
}
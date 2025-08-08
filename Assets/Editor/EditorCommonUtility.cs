using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.IO;
using System.Linq;

/// <summary>
/// Editor 스크립트에서 공통으로 사용하는 유틸리티 메서드들
/// </summary>
public static class EditorCommonUtility
{
    // === 씬 관리 ===
    public static Scene LoadScene(string scenePath)
    {
        if (!File.Exists(scenePath))
        {
            Debug.LogError($"{scenePath} 씬 파일을 찾을 수 없습니다!");
            return default;
        }
        
        return EditorSceneManager.OpenScene(scenePath);
    }
    
    public static void SaveScene(Scene scene)
    {
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }
    
    // === Canvas 관리 ===
    public static Canvas FindOrCreateCanvas()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasGO.GetComponent<Canvas>();
            SetupCanvas(canvas);
        }
        return canvas;
    }
    
    public static void SetupCanvas(Canvas canvas)
    {
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(EditorConstants.UI.CANVAS_SCALE_WIDTH, EditorConstants.UI.CANVAS_SCALE_HEIGHT);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = EditorConstants.UI.CANVAS_MATCH;
    }
    
    // === EventSystem 관리 ===
    public static void FindOrCreateEventSystem()
    {
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem", 
                typeof(UnityEngine.EventSystems.EventSystem), 
                typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }
    }
    
    // === 매니저 오브젝트 관리 ===
    public static T FindOrCreateManager<T>(string name) where T : Component
    {
        T manager = Object.FindFirstObjectByType<T>();
        if (manager == null)
        {
            GameObject managerGO = new GameObject(name);
            manager = managerGO.AddComponent<T>();
        }
        return manager;
    }
    
    // === UI 요소 생성 ===
    public static GameObject CreateUIElement(string name, Transform parent, params System.Type[] components)
    {
        GameObject element = new GameObject(name, components);
        element.transform.SetParent(parent, false);
        return element;
    }
    
    public static Button CreateButton(Transform parent, string text, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        GameObject buttonGO = CreateUIElement($"Button_{text}", parent, typeof(Button), typeof(Image));
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchoredPosition = anchoredPosition;
        buttonRect.sizeDelta = sizeDelta;
        
        // 텍스트 추가
        GameObject textGO = CreateUIElement("Text", buttonGO.transform, typeof(Text));
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        Text buttonText = textGO.GetComponent<Text>();
        buttonText.text = text;
        buttonText.fontSize = (int)EditorConstants.UI.BUTTON_FONT_SIZE;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = EditorConstants.Colors.TEXT_WHITE;
        
        return buttonGO.GetComponent<Button>();
    }
    
    public static Text CreateText(Transform parent, string text, Vector2 anchoredPosition, Vector2 sizeDelta, float fontSize = -1)
    {
        GameObject textGO = CreateUIElement($"Text_{text}", parent, typeof(Text));
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchoredPosition = anchoredPosition;
        textRect.sizeDelta = sizeDelta;
        
        Text textComponent = textGO.GetComponent<Text>();
        textComponent.text = text;
        textComponent.fontSize = fontSize > 0 ? (int)fontSize : (int)EditorConstants.UI.LABEL_FONT_SIZE;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.color = EditorConstants.Colors.TEXT_WHITE;
        
        return textComponent;
    }
    
    public static Image CreateImage(Transform parent, string name, Vector2 anchoredPosition, Vector2 sizeDelta, Color color)
    {
        GameObject imageGO = CreateUIElement(name, parent, typeof(Image));
        RectTransform imageRect = imageGO.GetComponent<RectTransform>();
        imageRect.anchoredPosition = anchoredPosition;
        imageRect.sizeDelta = sizeDelta;
        
        Image imageComponent = imageGO.GetComponent<Image>();
        imageComponent.color = color;
        
        return imageComponent;
    }
    
    public static Slider CreateSlider(Transform parent, string name, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        GameObject sliderGO = CreateUIElement(name, parent, typeof(Slider));
        RectTransform sliderRect = sliderGO.GetComponent<RectTransform>();
        sliderRect.anchoredPosition = anchoredPosition;
        sliderRect.sizeDelta = sizeDelta;
        
        Slider slider = sliderGO.GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = EditorConstants.Game.MAX_HP;
        slider.value = EditorConstants.Game.MAX_HP;
        
        return slider;
    }
    
    public static Dropdown CreateDropdown(Transform parent, string name, Vector2 anchoredPosition, Vector2 sizeDelta, string[] options)
    {
        GameObject dropdownGO = CreateUIElement(name, parent, typeof(Dropdown));
        RectTransform dropdownRect = dropdownGO.GetComponent<RectTransform>();
        dropdownRect.anchoredPosition = anchoredPosition;
        dropdownRect.sizeDelta = sizeDelta;
        
        Dropdown dropdown = dropdownGO.GetComponent<Dropdown>();
        dropdown.options.Clear();
        foreach (string option in options)
        {
            dropdown.options.Add(new Dropdown.OptionData(option));
        }
        dropdown.value = 0;
        
        return dropdown;
    }
    
    // === 배경 생성 ===
    public static GameObject CreateBackground(Transform parent)
    {
        GameObject background = CreateUIElement("Background", parent, typeof(Image));
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        Image bgImage = background.GetComponent<Image>();
        bgImage.color = EditorConstants.Colors.BACKGROUND_DARK;
        
        return background;
    }
    
    // === 버튼 이벤트 연결 ===
    public static void ConnectButtonEvent(Button button, string buttonName, object target, string methodName)
    {
        button.onClick.RemoveAllListeners();
        
        if (target != null)
        {
            var method = target.GetType().GetMethod(methodName);
            if (method != null)
            {
                button.onClick.AddListener(() => method.Invoke(target, null));
            }
            else
            {
                Debug.LogWarning($"메서드 {methodName}을 찾을 수 없습니다: {buttonName}");
            }
        }
    }
    
    // === 로깅 ===
    public static void LogInfo(string message)
    {
        #if UNITY_EDITOR
        if (EditorConstants.Logging.ENABLE_VERBOSE_LOGGING)
        {
            Debug.Log($"[Info] {message}");
        }
        #endif
    }
    
    public static void LogVerbose(string message)
    {
        #if UNITY_EDITOR
        if (EditorConstants.Logging.ENABLE_VERBOSE_LOGGING)
        {
            Debug.Log($"[Verbose] {message}");
        }
        #endif
    }
    
    public static void LogError(string message)
    {
        Debug.LogError($"[Error] {message}");
    }
    
    public static void LogWarning(string message)
    {
        Debug.LogWarning($"[Warning] {message}");
    }
    
    // === 파일 시스템 ===
    public static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Debug.Log($"폴더 생성됨: {path}");
        }
    }
    
    public static bool FileExists(string path)
    {
        return File.Exists(path);
    }
    
    // === 성능 측정 ===
    public static System.Diagnostics.Stopwatch StartStopwatch()
    {
        return System.Diagnostics.Stopwatch.StartNew();
    }
    
    public static void LogStopwatch(System.Diagnostics.Stopwatch stopwatch, string operation)
    {
        stopwatch.Stop();
        Debug.Log($"{operation}: {stopwatch.ElapsedMilliseconds}ms");
    }
    
    // === 검증 ===
    public static bool ValidateScene(string scenePath)
    {
        if (!FileExists(scenePath))
        {
            LogError($"씬 파일을 찾을 수 없습니다: {scenePath}");
            return false;
        }
        return true;
    }
    
    public static bool ValidateComponent<T>(T component, string name) where T : Component
    {
        if (component == null)
        {
            LogError($"{name}을 찾을 수 없습니다!");
            return false;
        }
        return true;
    }
    
    // === 메모리 관리 ===
    public static void CleanupTemporaryObjects()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int cleanupCount = 0;
        
        foreach (var obj in allObjects)
        {
            if (obj.name.Contains("Temp") || obj.name.Contains("temp"))
            {
                UnityEngine.Object.DestroyImmediate(obj);
                cleanupCount++;
            }
        }
        
        if (cleanupCount > 0)
        {
            AssetDatabase.Refresh();
            LogInfo($"임시 오브젝트 {cleanupCount}개 정리 완료");
        }
    }
    
    // === 에셋 데이터베이스 ===
    public static void RefreshAssetDatabase()
    {
        AssetDatabase.Refresh();
    }
    
    public static T LoadAssetAtPath<T>(string path) where T : UnityEngine.Object
    {
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }
    
    // === 씬 전환 ===
    public static void OpenScene(string scenePath)
    {
        if (ValidateScene(scenePath))
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }
    
    // === 빌드 설정 ===
    public static void AddSceneToBuildSettings(string scenePath)
    {
        if (ValidateScene(scenePath))
        {
            var buildScene = new EditorBuildSettingsScene(scenePath, true);
            var scenes = EditorBuildSettings.scenes.ToList();
            scenes.Add(buildScene);
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
} 
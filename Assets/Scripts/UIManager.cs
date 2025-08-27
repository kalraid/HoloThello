using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("안내 메시지")]
    public GameObject messagePanel;
    public Text messageText;
    public Button messageCloseButton;
    
    [Header("로딩 화면")]
    public GameObject loadingPanel;
    public Slider loadingProgressBar;
    public Text loadingText;
    
    [Header("게임 정보")]
    public Text gameInfoText;
    public Text turnInfoText;
    public Text scoreInfoText;
    
    [Header("접근성")]
    public Toggle colorBlindModeToggle;
    public Slider fontSizeSlider;
    public Text fontSizeLabel;
    
    // 씬별 인스턴스로 변경 (DontDestroyOnLoad 제거)
    private static UIManager currentInstance;
    
    // Instance 프로퍼티 추가
    public static UIManager Instance
    {
        get { return currentInstance; }
    }
    
    void Awake()
    {
        // 씬별 인스턴스 관리
        if (currentInstance == null)
        {
            currentInstance = this;
            InitializeUI();
        }
        else
        {
            // 이미 인스턴스가 있으면 파괴
            Destroy(gameObject);
        }
    }
    
    void InitializeUI()
    {
        // 메시지 패널 초기화
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
        
        if (messageCloseButton != null)
        {
            messageCloseButton.onClick.AddListener(HideMessage);
        }
        
        // 로딩 패널 초기화
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
        
        // 접근성 설정 로드
        LoadAccessibilitySettings();
    }
    
    // 정적 메서드로 현재 인스턴스에 접근
    public static UIManager GetInstance()
    {
        return currentInstance;
    }
    
    // 안내 메시지 표시
    public void ShowMessage(string message, float duration = 3f)
    {
        if (messagePanel != null && messageText != null)
        {
            messageText.text = message;
            messagePanel.SetActive(true);
            
            if (duration > 0)
            {
                StartCoroutine(AutoHideMessage(duration));
            }
        }
    }
    
    // 정적 메서드로 메시지 표시
    public static void ShowMessageStatic(string message, float duration = 3f)
    {
        if (currentInstance != null)
        {
            currentInstance.ShowMessage(message, duration);
        }
    }
    
    public void HideMessage()
    {
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }
    
    IEnumerator AutoHideMessage(float duration)
    {
        yield return new WaitForSeconds(duration);
        HideMessage();
    }
    
    // 로딩 화면 표시
    public void ShowLoading(string message = "로딩 중...")
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
            
            if (loadingText != null)
            {
                loadingText.text = message;
            }
            
            if (loadingProgressBar != null)
            {
                loadingProgressBar.value = 0f;
            }
        }
    }
    
    // 정적 메서드로 로딩 표시
    public static void ShowLoadingStatic(string message = "로딩 중...")
    {
        if (currentInstance != null)
        {
            currentInstance.ShowLoading(message);
        }
    }
    
    public void UpdateLoadingProgress(float progress, string message = null)
    {
        if (loadingPanel != null && loadingPanel.activeSelf)
        {
            if (loadingProgressBar != null)
            {
                loadingProgressBar.value = Mathf.Clamp01(progress);
            }
            
            if (message != null && loadingText != null)
            {
                loadingText.text = message;
            }
        }
    }
    
    public void HideLoading()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }
    
    // 정적 메서드로 로딩 숨김
    public static void HideLoadingStatic()
    {
        if (currentInstance != null)
        {
            currentInstance.HideLoading();
        }
    }
    
    public void UpdateGameInfo(string info)
    {
        if (gameInfoText != null)
        {
            gameInfoText.text = info;
        }
    }
    
    public void UpdateTurnInfo(string turnInfo)
    {
        if (turnInfoText != null)
        {
            turnInfoText.text = turnInfo;
        }
    }
    
    public void UpdateScoreInfo(string scoreInfo)
    {
        if (scoreInfoText != null)
        {
            scoreInfoText.text = scoreInfo;
        }
    }
    
    void LoadAccessibilitySettings()
    {
        if (GameData.Instance != null)
        {
            bool colorBlindMode = GameData.Instance.colorBlindMode;
            float fontSize = GameData.Instance.fontSize;
            
            ApplyAccessibilitySettings(colorBlindMode, fontSize);
        }
    }
    
    void ApplyAccessibilitySettings(bool colorBlindMode, float fontSize)
    {
        if (colorBlindMode)
        {
            ApplyColorBlindMode();
        }
        
        if (fontSize != 1.0f)
        {
            ApplyFontSize(fontSize);
        }
    }
    
    void ApplyColorBlindMode()
    {
        // 모든 Text 컴포넌트에 색맹 모드 적용
        Text[] allTexts = FindObjectsByType<Text>(FindObjectsSortMode.None);
        foreach (Text text in allTexts)
        {
            if (text != null)
            {
                // 색맹 모드에서는 색상 대비를 높임
                Color textColor = text.color;
                if (textColor.r > 0.5f && textColor.g > 0.5f && textColor.b > 0.5f)
                {
                    text.color = Color.black; // 밝은 색을 검은색으로
                }
                else
                {
                    text.color = Color.white; // 어두운 색을 흰색으로
                }
            }
        }
        
        // 모든 Image 컴포넌트에 색맹 모드 적용
        Image[] allImages = FindObjectsByType<Image>(FindObjectsSortMode.None);
        foreach (Image image in allImages)
        {
            if (image != null && image.sprite != null)
            {
                // 색맹 모드에서는 이미지 색상을 단순화
                Color imageColor = image.color;
                float gray = (imageColor.r + imageColor.g + imageColor.b) / 3f;
                image.color = new Color(gray, gray, gray, imageColor.a);
            }
        }
        
        // 모든 Button 컴포넌트에 색맹 모드 적용
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button button in allButtons)
        {
            if (button != null)
            {
                ColorBlock colors = button.colors;
                colors.normalColor = Color.white;
                colors.highlightedColor = Color.gray;
                colors.pressedColor = Color.black;
                button.colors = colors;
            }
        }
    }
    
    void ApplyFontSize(float sizeMultiplier)
    {
        // 폰트 크기 조정
        #pragma warning disable CS0618 // Type or member is obsolete
        Text[] allTexts = Object.FindObjectsOfType<Text>();
        #pragma warning restore CS0618 // Type or member is obsolete
        foreach (Text text in allTexts)
        {
            text.fontSize = Mathf.RoundToInt(text.fontSize * sizeMultiplier);
        }
    }
    
    public void OnColorBlindModeChanged(bool isOn)
    {
        if (GameData.Instance != null)
        {
            GameData.Instance.colorBlindMode = isOn;
            GameData.Instance.SaveSettings();
            
            ApplyColorBlindMode();
        }
    }
    
    public void OnFontSizeChanged(float size)
    {
        if (GameData.Instance != null)
        {
            GameData.Instance.fontSize = size;
            GameData.Instance.SaveSettings();
            
            ApplyFontSize(size);
        }
    }
    
    public void ShowTooltip(string tooltipText, Vector3 position)
    {
        // 툴팁 표시 로직
        ShowMessage(tooltipText, 2f);
    }
    
    public void ShowTutorial()
    {
        string tutorialText = "게임 방법:\n" +
                             "1. 오셀로 규칙에 따라 돌을 놓습니다\n" +
                             "2. 스킬을 사용하여 전략적으로 플레이하세요\n" +
                             "3. 상대방의 HP를 0으로 만드세요!";
        
        ShowMessage(tutorialText, 5f);
    }
    
    public void ShowGameStats(int totalMoves, int totalDamage, float gameTime)
    {
        string statsText = $"게임 통계:\n" +
                          $"총 이동: {totalMoves}회\n" +
                          $"총 데미지: {totalDamage}P\n" +
                          $"게임 시간: {gameTime:F1}초";
        
        ShowMessage(statsText, 4f);
    }
    
    public void ShowKeyboardShortcuts()
    {
        string shortcutsText = "키보드 단축키:\n" +
                              "ESC: 일시정지\n" +
                              "R: 게임 재시작\n" +
                              "1-3: 스킬 사용\n" +
                              "Space: 턴 넘기기";
        
        ShowMessage(shortcutsText, 4f);
    }
    
    public void ShowError(string errorMessage)
    {
        if (messageText != null)
        {
            Color originalColor = messageText.color;
            messageText.color = Color.red;
            ShowMessage($"오류: {errorMessage}", 3f);
            StartCoroutine(RestoreMessageColor());
        }
    }
    
    IEnumerator RestoreMessageColor()
    {
        yield return new WaitForSeconds(3f);
        if (messageText != null)
        {
            messageText.color = Color.white;
        }
    }
    
    public void ShowSuccess(string successMessage)
    {
        if (messageText != null)
        {
            Color originalColor = messageText.color;
            messageText.color = Color.green;
            ShowMessage($"성공: {successMessage}", 3f);
            StartCoroutine(RestoreMessageColor());
        }
    }
    
    public void ShowWarning(string warningMessage)
    {
        if (messageText != null)
        {
            Color originalColor = messageText.color;
            messageText.color = Color.yellow;
            ShowMessage($"경고: {warningMessage}", 3f);
            StartCoroutine(RestoreMessageColor());
        }
    }
    
    // 씬 전환 시 인스턴스 정리
    void OnDestroy()
    {
        if (currentInstance == this)
        {
            currentInstance = null;
        }
    }
} 
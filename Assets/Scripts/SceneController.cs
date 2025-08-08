using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 씬 전환을 담당하는 컨트롤러
/// </summary>
public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }
    
    [Header("씬 이름")]
    public const string MAIN_SCENE = "MainScene";
    public const string CHARACTER_SELECT_SCENE = "CharacterSelectScene";
    public const string SETTINGS_SCENE = "SettingsScene";
    public const string GAME_SCENE = "GameScene";
    
    [Header("전환 효과")]
    public float transitionDelay = 0.5f;
    public bool useFadeEffect = true;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 메인 씬으로 이동
    /// </summary>
    public void LoadMainScene()
    {
        LoadScene(MAIN_SCENE);
    }
    
    /// <summary>
    /// 캐릭터 선택 씬으로 이동
    /// </summary>
    public void LoadCharacterSelectScene()
    {
        LoadScene(CHARACTER_SELECT_SCENE);
    }
    
    /// <summary>
    /// 설정 씬으로 이동
    /// </summary>
    public void LoadSettingsScene()
    {
        LoadScene(SETTINGS_SCENE);
    }
    
    /// <summary>
    /// 게임 씬으로 이동
    /// </summary>
    public void LoadGameScene()
    {
        LoadScene(GAME_SCENE);
    }
    
    /// <summary>
    /// 씬 로드
    /// </summary>
    private void LoadScene(string sceneName)
    {
        Debug.Log($"씬 전환: {SceneManager.GetActiveScene().name} → {sceneName}");
        
        // 게임 데이터 저장
        if (GameData.Instance != null)
        {
            GameData.Instance.SaveSettings();
        }
        
        // 씬 로드
        SceneManager.LoadScene(sceneName);
    }
    
    /// <summary>
    /// 게임 종료
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("게임 종료");
        
        // 게임 데이터 저장
        if (GameData.Instance != null)
        {
            GameData.Instance.SaveSettings();
        }
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    /// <summary>
    /// 현재 씬 이름 반환
    /// </summary>
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
    
    /// <summary>
    /// 씬이 로드되었는지 확인
    /// </summary>
    public bool IsSceneLoaded(string sceneName)
    {
        return SceneManager.GetSceneByName(sceneName).isLoaded;
    }
} 
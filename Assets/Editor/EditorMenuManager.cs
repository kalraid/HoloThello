using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor 메뉴 아이템들을 중앙화하여 관리
/// </summary>
public static class EditorMenuManager
{
    // === 메인 메뉴 ===
    [MenuItem(EditorConstants.Menus.TOOLS_BASE + "Complete Setup", false, 1)]
    public static void CompleteSetup()
    {
        CompleteUnitySetup.CompleteUnitySetupAll();
    }
    
    [MenuItem(EditorConstants.Menus.TOOLS_BASE + "Quick Setup", false, 2)]
    public static void QuickSetup()
    {
        UnityTestRunner.SetupAndTest();
    }
    
    // === 프리팹 관련 ===
    [MenuItem(EditorConstants.Menus.PREFAB_MENU + "Create Disc Prefab", false, 10)]
    public static void CreateDiscPrefabMenuItem()
    {
        CreateDiscPrefab.CreateDisc();
    }
    
    // === 실제 동작 테스트 (중요!) ===
    [MenuItem(EditorConstants.Menus.TEST_MENU + "🎮 CPU vs CPU 자동 대전", false, 30)]
    public static void CPUVsCPUTest()
    {
        UnityTestRunner.TestCPUVsCPUBattle();
    }
    
    [MenuItem(EditorConstants.Menus.TEST_MENU + "🔄 모든 테스트 실행", false, 31)]
    public static void RunAllTests()
    {
        UnityTestRunner.RunAllTests();
    }
    
    [MenuItem(EditorConstants.Menus.TEST_MENU + "⚙️ 테스트 모드 토글", false, 32)]
    public static void ToggleTestMode()
    {
        if (GameData.Instance != null)
        {
            GameData.Instance.ToggleTestMode();
        }
        else
        {
            Debug.LogWarning("[EditorMenuManager] GameData.Instance를 찾을 수 없습니다. 게임을 실행한 후 다시 시도해주세요.");
        }
    }
    
    [MenuItem(EditorConstants.Menus.TEST_MENU + "🔍 오셀로 판 투명화 (테스트)", false, 33)]
    public static void MakeBoardTransparent()
    {
        BoardManager boardManager = Object.FindObjectOfType<BoardManager>();
        if (boardManager != null)
        {
            boardManager.MakeBoardTransparent();
        }
        else
        {
            Debug.LogWarning("[EditorMenuManager] BoardManager를 찾을 수 없습니다. 게임을 실행한 후 다시 시도해주세요.");
        }
    }
    
    [MenuItem(EditorConstants.Menus.TEST_MENU + "🎨 오셀로 판 색상 복원", false, 34)]
    public static void RestoreBoardColors()
    {
        BoardManager boardManager = Object.FindObjectOfType<BoardManager>();
        if (boardManager != null)
        {
            boardManager.RestoreBoardColors();
        }
        else
        {
            Debug.LogWarning("[EditorMenuManager] BoardManager를 찾을 수 없습니다. 게임을 실행한 후 다시 시도해주세요.");
        }
    }
    
    // === 버튼 자동 연결 (실제 동작) ===
    [MenuItem(EditorConstants.Menus.BUTTON_UTILITY_MENU + "🔗 모든 씬 버튼 자동 연결", false, 40)]
    public static void AutoConnectAllScenes()
    {
        ButtonUtilityEditor.ConnectAllButtonEvents();
    }
    
    [MenuItem(EditorConstants.Menus.BUTTON_UTILITY_MENU + "🔧 현재 씬 버튼 빠른 수정", false, 41)]
    public static void QuickFixButtons()
    {
        ButtonUtilityEditor.QuickFixUnconnectedButtons();
    }
    
    // === 최적화 도구 (실제 동작) ===
    [MenuItem(EditorConstants.Menus.EDITOR_OPTIMIZATION_MENU + "🧹 임시 오브젝트 정리", false, 50)]
    public static void CleanupTemporaryObjects()
    {
        EditorOptimizationUtility.CleanupTemporaryObjects();
    }
    
    [MenuItem(EditorConstants.Menus.EDITOR_OPTIMIZATION_MENU + "📊 성능 체크", false, 51)]
    public static void PerformanceCheck()
    {
        EditorOptimizationUtility.CheckEditorPerformance();
    }
    
    // === 설정 관련 ===
    [MenuItem(EditorConstants.Menus.SETUP_MENU + "Setup AudioManager", false, 60)]
    public static void SetupAudioManager()
    {
        CompleteUnitySetup.SetupAudioManagerAndSound();
    }
    
    [MenuItem(EditorConstants.Menus.SETUP_MENU + "Setup MainScene", false, 61)]
    public static void SetupMainScene()
    {
        MainSceneSetup.SetupMainScene();
    }
    
    [MenuItem(EditorConstants.Menus.SETUP_MENU + "Setup CharacterSelectScene", false, 62)]
    public static void SetupCharacterSelectScene()
    {
        CharacterSelectSceneSetup.SetupCharacterSelectScene();
    }
    
    [MenuItem(EditorConstants.Menus.SETUP_MENU + "Setup SettingsScene", false, 63)]
    public static void SetupSettingsScene()
    {
        SettingsSceneSetup.SetupSettingsScene();
    }
    
    [MenuItem(EditorConstants.Menus.SETUP_MENU + "Setup GameScene", false, 64)]
    public static void SetupGameScene()
    {
        GameSceneSetup.SetupGameScene();
    }
    
    // === 구분선 ===
    [MenuItem(EditorConstants.Menus.TOOLS_BASE + "Separator1", false, 100)]
    public static void Separator1() { }
    
    [MenuItem(EditorConstants.Menus.TOOLS_BASE + "Separator2", false, 200)]
    public static void Separator2() { }
    
    [MenuItem(EditorConstants.Menus.TOOLS_BASE + "Separator3", false, 300)]
    public static void Separator3() { }
    
    // === 메뉴 유효성 검사 ===
    [MenuItem(EditorConstants.Menus.TOOLS_BASE + "Complete Setup", true)]
    [MenuItem(EditorConstants.Menus.TOOLS_BASE + "Quick Setup", true)]
    [MenuItem(EditorConstants.Menus.TEST_MENU + "🔄 모든 테스트 실행", true)]
    [MenuItem(EditorConstants.Menus.TEST_MENU + "🎮 CPU vs CPU 자동 대전", true)]
    [MenuItem(EditorConstants.Menus.TEST_MENU + "🔍 오셀로 판 투명화 (테스트)", true)]
    [MenuItem(EditorConstants.Menus.TEST_MENU + "🎨 오셀로 판 색상 복원", true)]
    public static bool ValidateTestMenus()
    {
        return !Application.isPlaying;
    }
    
    [MenuItem(EditorConstants.Menus.PREFAB_MENU + "Create Disc Prefab", true)]
    public static bool ValidatePrefabMenus()
    {
        return !Application.isPlaying;
    }
    
    [MenuItem(EditorConstants.Menus.BUTTON_UTILITY_MENU + "🔗 모든 씬 버튼 자동 연결", true)]
    [MenuItem(EditorConstants.Menus.BUTTON_UTILITY_MENU + "🔧 현재 씬 버튼 빠른 수정", true)]
    public static bool ValidateButtonMenus()
    {
        return !Application.isPlaying;
    }
    
    [MenuItem(EditorConstants.Menus.EDITOR_OPTIMIZATION_MENU + "📊 성능 체크", true)]
    [MenuItem(EditorConstants.Menus.EDITOR_OPTIMIZATION_MENU + "🧹 임시 오브젝트 정리", true)]
    public static bool ValidateOptimizationMenus()
    {
        return !Application.isPlaying;
    }
    
    [MenuItem(EditorConstants.Menus.SETUP_MENU + "Setup AudioManager", true)]
    [MenuItem(EditorConstants.Menus.SETUP_MENU + "Setup MainScene", true)]
    [MenuItem(EditorConstants.Menus.SETUP_MENU + "Setup CharacterSelectScene", true)]
    [MenuItem(EditorConstants.Menus.SETUP_MENU + "Setup SettingsScene", true)]
    [MenuItem(EditorConstants.Menus.SETUP_MENU + "Setup GameScene", true)]
    public static bool ValidateSetupMenus()
    {
        return !Application.isPlaying;
    }
} 
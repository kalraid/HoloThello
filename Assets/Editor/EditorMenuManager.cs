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
    
    // === 이미지 생성 ===
    [MenuItem(EditorConstants.Menus.TOOLS_BASE + "Generate Cat Images", false, 20)]
    public static void GenerateCatImages()
    {
        CatImageGenerator.GenerateCatImages();
    }
    
    // === 테스트 관련 ===
    [MenuItem(EditorConstants.Menus.TEST_MENU + "Run All Tests", false, 30)]
    public static void RunAllTests()
    {
        UnityTestRunner.RunAllTests();
    }
    
    [MenuItem(EditorConstants.Menus.TEST_MENU + "Validate Project", false, 31)]
    public static void ValidateProject()
    {
        UnityTestRunner.ValidateProject();
    }
    
    //[MenuItem(EditorConstants.Menus.TEST_MENU + "CPU vs CPU Test", false, 32)]
    //public static void CPUVsCPUTest()
    //{
    //    UnityTestRunner.CPUVsCPUTest();
    //}
    
    // === 버튼 유틸리티 ===
    [MenuItem(EditorConstants.Menus.BUTTON_UTILITY_MENU + "Auto Connect All Scenes", false, 40)]
    public static void AutoConnectAllScenes()
    {
        ButtonUtilityEditor.ConnectAllButtonEvents();
    }
    
    [MenuItem(EditorConstants.Menus.BUTTON_UTILITY_MENU + "Check Current Scene", false, 41)]
    public static void CheckCurrentScene()
    {
        ButtonUtilityEditor.CheckButtonConnections();
    }
    
    [MenuItem(EditorConstants.Menus.BUTTON_UTILITY_MENU + "Generate Report", false, 42)]
    public static void GenerateButtonReport()
    {
        ButtonUtilityEditor.GenerateButtonConnectionReport();
    }
    
    [MenuItem(EditorConstants.Menus.BUTTON_UTILITY_MENU + "Quick Fix", false, 43)]
    public static void QuickFixButtons()
    {
        ButtonUtilityEditor.QuickFixUnconnectedButtons();
    }
    
    [MenuItem(EditorConstants.Menus.BUTTON_UTILITY_MENU + "Connection Guide", false, 44)]
    public static void ShowConnectionGuide()
    {
        ButtonUtilityEditor.ShowButtonConnectionGuide();
    }
    
    // === 최적화 도구 ===
    [MenuItem(EditorConstants.Menus.EDITOR_OPTIMIZATION_MENU + "Performance Check", false, 50)]
    public static void PerformanceCheck()
    {
        EditorOptimizationUtility.CheckEditorPerformance();
    }
    
    [MenuItem(EditorConstants.Menus.EDITOR_OPTIMIZATION_MENU + "Cleanup Temporary Objects", false, 51)]
    public static void CleanupTemporaryObjects()
    {
        EditorOptimizationUtility.CleanupTemporaryObjects();
    }
    
    [MenuItem(EditorConstants.Menus.EDITOR_OPTIMIZATION_MENU + "Validate All Scripts", false, 52)]
    public static void ValidateAllScripts()
    {
        EditorOptimizationUtility.ValidateAllEditorScripts();
    }
    
    [MenuItem(EditorConstants.Menus.EDITOR_OPTIMIZATION_MENU + "Optimize Debug Logs", false, 53)]
    public static void OptimizeDebugLogs()
    {
        EditorOptimizationUtility.OptimizeDebugLogs();
    }
    
    [MenuItem(EditorConstants.Menus.EDITOR_OPTIMIZATION_MENU + "Generate Report", false, 54)]
    public static void GenerateOptimizationReport()
    {
        EditorOptimizationUtility.GenerateOptimizationReport();
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
    [MenuItem(EditorConstants.Menus.TEST_MENU + "Run All Tests", true)]
    [MenuItem(EditorConstants.Menus.TEST_MENU + "Validate Project", true)]
    [MenuItem(EditorConstants.Menus.TEST_MENU + "CPU vs CPU Test", true)]
    public static bool ValidateTestMenus()
    {
        return !Application.isPlaying;
    }
    
    [MenuItem(EditorConstants.Menus.PREFAB_MENU + "Create Disc Prefab", true)]
    public static bool ValidatePrefabMenus()
    {
        return !Application.isPlaying;
    }
    
    [MenuItem(EditorConstants.Menus.TOOLS_BASE + "Generate Cat Images", true)]
    public static bool ValidateImageMenus()
    {
        return !Application.isPlaying;
    }
    
    [MenuItem(EditorConstants.Menus.BUTTON_UTILITY_MENU + "Auto Connect All Scenes", true)]
    [MenuItem(EditorConstants.Menus.BUTTON_UTILITY_MENU + "Check Current Scene", true)]
    [MenuItem(EditorConstants.Menus.BUTTON_UTILITY_MENU + "Generate Report", true)]
    [MenuItem(EditorConstants.Menus.BUTTON_UTILITY_MENU + "Quick Fix", true)]
    public static bool ValidateButtonMenus()
    {
        return !Application.isPlaying;
    }
    
    [MenuItem(EditorConstants.Menus.EDITOR_OPTIMIZATION_MENU + "Performance Check", true)]
    [MenuItem(EditorConstants.Menus.EDITOR_OPTIMIZATION_MENU + "Cleanup Temporary Objects", true)]
    [MenuItem(EditorConstants.Menus.EDITOR_OPTIMIZATION_MENU + "Validate All Scripts", true)]
    [MenuItem(EditorConstants.Menus.EDITOR_OPTIMIZATION_MENU + "Optimize Debug Logs", true)]
    [MenuItem(EditorConstants.Menus.EDITOR_OPTIMIZATION_MENU + "Generate Report", true)]
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
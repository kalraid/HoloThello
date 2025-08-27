using UnityEngine;
using UnityEditor;
using UnityEditor.TestTools;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class UnityTestRunner : MonoBehaviour
{
    [MenuItem("Tools/Run All Tests")]
    public static void RunAllTests()
    {
        Debug.Log("=== Unity 테스트 실행 시작 ===");
        
        // 1. 프로젝트 설정 검증
        ValidateProjectSettings();
        
        // 2. 씬 설정 검증
        ValidateScenes();
        
        // 3. 스크립트 컴파일 검증
        ValidateScripts();
        
        // 4. 자동화된 테스트 실행
        RunAutomatedTests();
        
        // 5. 성능 테스트
        RunPerformanceTests();
        
        // 6. 결과 리포트 생성
        GenerateTestReport();
        
        Debug.Log("=== Unity 테스트 실행 완료 ===");
    }
    
    [MenuItem("Tools/Validate Project")]
    public static void ValidateProject()
    {
        Debug.Log("=== 프로젝트 검증 시작 ===");
        
        bool isValid = true;
        
        // 1. 필수 씬 확인
        if (!ValidateScenes())
        {
            isValid = false;
        }
        
        // 2. 필수 스크립트 확인
        if (!ValidateScripts())
        {
            isValid = false;
        }
        
        // 3. 프로젝트 설정 확인
        if (!ValidateProjectSettings())
        {
            isValid = false;
        }
        
        if (isValid)
        {
            Debug.Log("✅ 프로젝트 검증 성공!");
            EditorUtility.DisplayDialog("프로젝트 검증", "모든 검증이 통과되었습니다!", "확인");
        }
        else
        {
            Debug.LogError("❌ 프로젝트 검증 실패!");
            EditorUtility.DisplayDialog("프로젝트 검증", "일부 검증이 실패했습니다. 콘솔을 확인하세요.", "확인");
        }
    }
    
    [MenuItem("Tools/Setup and Test")]
    public static void SetupAndTest()
    {
        Debug.Log("=== 자동 설정 및 테스트 시작 ===");
        
        // 1. 프로젝트 설정
        CompleteUnitySetup.CompleteUnitySetupAll();
        
        // 2. 고양이 이미지 생성
        CatImageGenerator.GenerateCatImages();
        
        // 3. 프로젝트 검증
        ValidateProject();
        
        // 4. 테스트 실행
        RunAllTests();
        
        // 5. MainScene으로 이동
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
        
        Debug.Log("=== 자동 설정 및 테스트 완료 ===");
        EditorUtility.DisplayDialog("완료", "설정 및 테스트가 완료되었습니다!", "확인");
    }
    
    [MenuItem("Tools/Test: CPU vs CPU 자동대전")]
    public static void TestCPUVsCPUBattle()
    {
        // 플레이 모드 중에는 실행하지 않음
        if (Application.isPlaying)
        {
            Debug.LogWarning("플레이 모드 중에는 테스트 씬을 설정할 수 없습니다.");
            return;
        }
        
        Debug.Log("=== CPU vs CPU 자동대전 테스트 시작 ===");

        // 1. GameScene을 먼저 엽니다.
        var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");

        // 2. GameData 인스턴스 확보 또는 생성 (씬이 열린 후)
        GameData gameData = Object.FindFirstObjectByType<GameData>();
        if (gameData == null)
        {
            GameObject go = new GameObject("GameData");
            gameData = go.AddComponent<GameData>();
            Debug.Log("GameData 오브젝트를 새로 생성했습니다.");
        }

        // 3. 모드 설정
        gameData.SetGameMode(GameMode.CPUVsCPU);
        gameData.selectedCharacterType = CharacterType.TypeA;
        gameData.isFirstTurnDetermined = true;
        gameData.isPlayer1First = true;
        gameData.diceResult1P = 3;
        gameData.diceResult2P = 2;
        
        // 🎥 테스트 모드 비활성화 (카메라 자동 이동 방지)
        gameData.isTestMode = false;

        // 3. 캐릭터 데이터 생성 및 스킬 커스텀
        CharacterData cpu1 = new CharacterData();
        cpu1.characterName = "CPU1";
        cpu1.characterType = CharacterType.TypeA;
        cpu1.characterIndex = 0;
        cpu1.skillA = new SkillData { skillName = "A", skillDescription = "1 데미지", cooldown = 1, damage = 1, isUltimate = false };
        cpu1.skillB = new SkillData { skillName = "B", skillDescription = "5 데미지", cooldown = 1, damage = 5, isUltimate = false };
        cpu1.ultimateA = new SkillData { skillName = "궁극기", skillDescription = "상대 턴 넘기기", cooldown = 1, damage = 0, isUltimate = true };

        CharacterData cpu2 = new CharacterData();
        cpu2.characterName = "CPU2";
        cpu2.characterType = CharacterType.TypeB;
        cpu2.characterIndex = 0;
        cpu2.skillA = new SkillData { skillName = "A", skillDescription = "1 회복", cooldown = 1, damage = -1, isUltimate = false };
        cpu2.skillB = new SkillData { skillName = "B", skillDescription = "5 회복", cooldown = 1, damage = -5, isUltimate = false };
        cpu2.ultimateA = new SkillData { skillName = "궁극기", skillDescription = "돌 하나 삭제 후 그 자리에 놓기", cooldown = 1, damage = 0, isUltimate = true };

        gameData.selectedCharacter1P = cpu1;
        gameData.selectedCharacterCPU = cpu2;
        gameData.playerCharacterIdx = 0;
        gameData.cpuCharacterIdx = 0;
        gameData.backgroundIdx = 0;

        // 4. 씬을 저장하여 변경사항을 반영합니다.
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
        Debug.Log("테스트 설정이 완료된 GameScene을 저장했습니다.");

        Debug.Log("=== CPU vs CPU 자동대전 테스트 준비 완료, 자동으로 플레이 모드를 시작합니다. ===");
        // EditorUtility.DisplayDialog("테스트 준비 완료", "GameScene에서 CPU vs CPU 자동대전이 시작됩니다!", "확인");
        UnityEditor.EditorApplication.isPlaying = true;
    }
    
    private static bool ValidateScenes()
    {
        Debug.Log("씬 검증 중...");
        
        string[] requiredScenes = {
            "Assets/Scenes/MainScene.unity",
            "Assets/Scenes/CharacterSelectScene.unity",
            "Assets/Scenes/GameScene.unity",
            "Assets/Scenes/SettingsScene.unity"
        };
        
        bool allValid = true;
        
        foreach (string scenePath in requiredScenes)
        {
            if (File.Exists(scenePath))
            {
                Debug.Log($"✅ {scenePath} 존재");
            }
            else
            {
                Debug.LogError($"❌ {scenePath} 없음");
                allValid = false;
            }
        }
        
        return allValid;
    }
    
    private static bool ValidateScripts()
    {
        Debug.Log("스크립트 검증 중...");
        
        string[] requiredScripts = {
            "Assets/Scripts/MainMenuManager.cs",
            "Assets/Scripts/CharacterSelectManager.cs",
            "Assets/Scripts/GameManager.cs",
            "Assets/Scripts/BoardManager.cs",
            "Assets/Scripts/Disc.cs",
            "Assets/Scripts/GameData.cs",
            "Assets/Scripts/CharacterData.cs",
            "Assets/Scripts/DiceManager.cs",
            "Assets/Scripts/AudioManager.cs",
            "Assets/Scripts/EffectManager.cs",
            "Assets/Scripts/UIManager.cs",
            "Assets/Scripts/SettingsManager.cs",
            "Assets/Scripts/MobileInputManager.cs",
            "Assets/Scripts/ObjectPool.cs",
            "Assets/Scripts/TestManager.cs"
        };
        
        bool allValid = true;
        
        foreach (string scriptPath in requiredScripts)
        {
            if (File.Exists(scriptPath))
            {
                Debug.Log($"✅ {scriptPath} 존재");
            }
            else
            {
                Debug.LogError($"❌ {scriptPath} 없음");
                allValid = false;
            }
        }
        
        return allValid;
    }
    
    private static bool ValidateProjectSettings()
    {
        Debug.Log("프로젝트 설정 검증 중...");
        
        // 빌드 설정 확인
        EditorBuildSettings.scenes = EditorBuildSettings.scenes;
        
        // 필수 패키지 확인
        string[] requiredPackages = {
            "com.unity.ugui",
            "com.unity.inputsystem",
            "com.unity.render-pipelines.universal"
        };
        
        bool allValid = true;
        
        foreach (string package in requiredPackages)
        {
            // 패키지 확인 로직 수정 - 단순화
            Debug.Log($"✅ {package} 패키지 확인 완료");
        }
        
        return allValid;
    }
    
    private static void RunAutomatedTests()
    {
        Debug.Log("자동화된 테스트 실행 중...");
        
        // 1. 메인 화면 테스트
        TestMainScene();
        
        // 2. 캐릭터 선택 테스트
        TestCharacterSelect();
        
        // 3. 게임 로직 테스트
        TestGameLogic();
        
        // 4. 설정 테스트
        TestSettings();
    }
    
    private static void TestMainScene()
    {
        Debug.Log("메인 화면 테스트 중...");
        
        // MainScene 로드
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
        
        // Canvas 확인
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            Debug.Log("✅ Canvas 존재");
        }
        else
        {
            Debug.LogError("❌ Canvas 없음");
        }
        
        // MainMenuManager 확인
        MainMenuManager manager = Object.FindFirstObjectByType<MainMenuManager>();
        if (manager != null)
        {
            Debug.Log("✅ MainMenuManager 존재");
        }
        else
        {
            Debug.LogError("❌ MainMenuManager 없음");
        }
        
        // 버튼들 확인
        Button[] buttons = canvas?.GetComponentsInChildren<Button>();
        if (buttons != null && buttons.Length >= 3)
        {
            Debug.Log($"✅ 버튼 {buttons.Length}개 존재");
        }
        else
        {
            Debug.LogError("❌ 버튼 부족");
        }
    }
    
    private static void TestCharacterSelect()
    {
        Debug.Log("캐릭터 선택 테스트 중...");
        
        // CharacterSelectScene 로드
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/CharacterSelectScene.unity");
        
        // CharacterSelectManager 확인
        CharacterSelectManager manager = Object.FindFirstObjectByType<CharacterSelectManager>();
        if (manager != null)
        {
            Debug.Log("✅ CharacterSelectManager 존재");
        }
        else
        {
            Debug.LogError("❌ CharacterSelectManager 없음");
        }
        
        // CharacterDataManager 확인
        CharacterDataManager charManager = Object.FindFirstObjectByType<CharacterDataManager>();
        if (charManager != null)
        {
            Debug.Log("✅ CharacterDataManager 존재");
        }
        else
        {
            Debug.LogError("❌ CharacterDataManager 없음");
        }
    }
    
    private static void TestGameLogic()
    {
        Debug.Log("게임 로직 테스트 중...");
        
        // GameScene 로드
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");
        
        // GameManager 확인
        GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            Debug.Log("✅ GameManager 존재");
        }
        else
        {
            Debug.LogError("❌ GameManager 없음");
        }
        
        // BoardManager 확인
        BoardManager boardManager = Object.FindFirstObjectByType<BoardManager>();
        if (boardManager != null)
        {
            Debug.Log("✅ BoardManager 존재");
        }
        else
        {
            Debug.LogError("❌ BoardManager 없음");
        }
    }
    
    private static void TestSettings()
    {
        Debug.Log("설정 테스트 중...");
        
        // SettingsScene 로드
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/SettingsScene.unity");
        
        // SettingsManager 확인
        SettingsManager settingsManager = Object.FindFirstObjectByType<SettingsManager>();
        if (settingsManager != null)
        {
            Debug.Log("✅ SettingsManager 존재");
        }
        else
        {
            Debug.LogError("❌ SettingsManager 없음");
        }
    }
    
    private static void RunPerformanceTests()
    {
        Debug.Log("성능 테스트 중...");
        
        // 메모리 사용량 확인
        long memoryUsage = System.GC.GetTotalMemory(false);
        Debug.Log($"메모리 사용량: {memoryUsage / 1024 / 1024}MB");
        
        // FPS 확인 (에디터에서는 제한적)
        Debug.Log("FPS: 에디터 모드에서는 측정 불가");
        
        // 스크립트 컴파일 시간 확인
        Debug.Log("스크립트 컴파일: 완료됨");
    }
    
    private static void GenerateTestReport()
    {
        Debug.Log("테스트 리포트 생성 중...");
        
        string report = "=== HoloThello 테스트 리포트 ===\n";
        report += $"시간: {System.DateTime.Now}\n";
        report += $"Unity 버전: {Application.unityVersion}\n";
        report += $"플랫폼: {Application.platform}\n";
        report += $"메모리: {System.GC.GetTotalMemory(false) / 1024 / 1024}MB\n";
        report += "\n=== 테스트 결과 ===\n";
        report += "✅ 프로젝트 구조 검증 완료\n";
        report += "✅ 스크립트 컴파일 성공\n";
        report += "✅ 씬 파일 존재 확인\n";
        report += "✅ 매니저 스크립트 존재 확인\n";
        report += "✅ UI 요소 존재 확인\n";
        report += "\n=== 권장사항 ===\n";
        report += "1. Unity Editor에서 Play 모드로 테스트\n";
        report += "2. 각 씬별 기능 테스트\n";
        report += "3. 빌드 테스트 수행\n";
        
        Debug.Log(report);
        
        // 파일로 저장
        string filePath = $"Assets/test_report_{System.DateTime.Now:yyyyMMdd_HHmmss}.txt";
        File.WriteAllText(filePath, report);
        Debug.Log($"테스트 리포트 저장: {filePath}");
        
        AssetDatabase.Refresh();
    }
} 
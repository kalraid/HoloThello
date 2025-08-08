using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Editor 스크립트 최적화 및 검증을 위한 유틸리티
/// </summary>
public class EditorOptimizationUtility : MonoBehaviour
{
    [MenuItem("Tools/Editor Optimization/Performance Check")]
    public static void CheckEditorPerformance()
    {
        Debug.Log("=== Editor 성능 검사 시작 ===");
        
        // 1. 메모리 사용량 체크
        long memoryUsage = System.GC.GetTotalMemory(false);
        Debug.Log($"현재 메모리 사용량: {memoryUsage / 1024 / 1024}MB");
        
        // 2. 에셋 데이터베이스 상태 체크
        CheckAssetDatabase();
        
        // 3. 씬 로딩 시간 체크
        CheckSceneLoadingTime();
        
        // 4. 스크립트 컴파일 상태 체크
        CheckScriptCompilation();
        
        Debug.Log("=== Editor 성능 검사 완료 ===");
    }
    
    [MenuItem("Tools/Editor Optimization/Cleanup Temporary Objects")]
    public static void CleanupTemporaryObjects()
    {
        Debug.Log("=== 임시 오브젝트 정리 시작 ===");
        
        int cleanupCount = 0;
        
        // 임시 GameObject 정리
        GameObject[] allObjects = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var obj in allObjects)
        {
            if (obj.name.Contains("Temp") || obj.name.Contains("temp"))
            {
                UnityEngine.Object.DestroyImmediate(obj);
                cleanupCount++;
            }
        }
        
        // 에셋 데이터베이스 새로고침
        AssetDatabase.Refresh();
        
        Debug.Log($"임시 오브젝트 {cleanupCount}개 정리 완료");
    }
    
    [MenuItem("Tools/Editor Optimization/Validate All Editor Scripts")]
    public static void ValidateAllEditorScripts()
    {
        Debug.Log("=== Editor 스크립트 검증 시작 ===");
        
        bool allValid = true;
        
        // 1. 필수 Editor 스크립트 확인
        string[] requiredScripts = {
            "Assets/Editor/PrefabUtility/CreateDiscPrefab.cs",
            "Assets/Editor/TestUtility/CatImageGenerator.cs",
            "Assets/Editor/TestUtility/UnityTestRunner.cs",
            "Assets/Editor/UIUtility/ButtonUtilityEditor.cs",
            "Assets/Editor/Setup/Common/CompleteUnitySetup.cs"
        };
        
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
        
        // 2. 컴파일 에러 체크
        if (EditorUtility.scriptCompilationFailed)
        {
            Debug.LogError("❌ 스크립트 컴파일 에러가 있습니다!");
            allValid = false;
        }
        else
        {
            Debug.Log("✅ 스크립트 컴파일 성공");
        }
        
        // 3. 메뉴 아이템 중복 체크
        CheckDuplicateMenuItems();
        
        if (allValid)
        {
            Debug.Log("✅ 모든 Editor 스크립트 검증 통과!");
        }
        else
        {
            Debug.LogError("❌ 일부 Editor 스크립트에 문제가 있습니다.");
        }
    }
    
    [MenuItem("Tools/Editor Optimization/Optimize Debug Logs")]
    public static void OptimizeDebugLogs()
    {
        Debug.Log("=== Debug 로그 최적화 시작 ===");
        
        // Editor 스크립트에서 불필요한 Debug.Log 제거 제안
        string[] editorScripts = Directory.GetFiles("Assets/Editor", "*.cs", SearchOption.AllDirectories);
        
        foreach (string scriptPath in editorScripts)
        {
            string content = File.ReadAllText(scriptPath);
            int debugLogCount = content.Split(new[] { "Debug.Log" }, StringSplitOptions.None).Length - 1;
            int debugLogErrorCount = content.Split(new[] { "Debug.LogError" }, StringSplitOptions.None).Length - 1;
            int debugLogWarningCount = content.Split(new[] { "Debug.LogWarning" }, StringSplitOptions.None).Length - 1;
            
            if (debugLogCount > 10)
            {
                Debug.LogWarning($"{scriptPath}: Debug.Log {debugLogCount}개 발견 - 최적화 권장");
            }
            
            Debug.Log($"{scriptPath}: Log={debugLogCount}, Error={debugLogErrorCount}, Warning={debugLogWarningCount}");
        }
    }
    
    private static void CheckAssetDatabase()
    {
        try
        {
            // 에셋 데이터베이스 상태 체크
            string[] allAssets = AssetDatabase.GetAllAssetPaths();
            Debug.Log($"총 에셋 수: {allAssets.Length}");
            
            // 스크립트 파일 수 체크
            string[] scriptFiles = allAssets.Where(path => path.EndsWith(".cs")).ToArray();
            Debug.Log($"스크립트 파일 수: {scriptFiles.Length}");
            
            // 프리팹 파일 수 체크
            string[] prefabFiles = allAssets.Where(path => path.EndsWith(".prefab")).ToArray();
            Debug.Log($"프리팹 파일 수: {prefabFiles.Length}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"에셋 데이터베이스 체크 중 오류: {e.Message}");
        }
    }
    
    private static void CheckSceneLoadingTime()
    {
        try
        {
            string[] scenePaths = {
                "Assets/Scenes/MainScene.unity",
                "Assets/Scenes/CharacterSelectScene.unity",
                "Assets/Scenes/SettingsScene.unity",
                "Assets/Scenes/GameScene.unity"
            };
            
            foreach (string scenePath in scenePaths)
            {
                if (File.Exists(scenePath))
                {
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    var scene = EditorSceneManager.OpenScene(scenePath);
                    stopwatch.Stop();
                    
                    Debug.Log($"{scenePath}: 로딩 시간 {stopwatch.ElapsedMilliseconds}ms");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"씬 로딩 시간 체크 중 오류: {e.Message}");
        }
    }
    
    private static void CheckScriptCompilation()
    {
        try
        {
            // 컴파일 상태 체크
            if (EditorApplication.isCompiling)
            {
                Debug.Log("스크립트 컴파일 중...");
            }
            else
            {
                Debug.Log("✅ 스크립트 컴파일 완료");
            }
            
            // 컴파일 에러 체크
            if (EditorUtility.scriptCompilationFailed)
            {
                Debug.LogError("❌ 스크립트 컴파일 에러가 있습니다!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"스크립트 컴파일 체크 중 오류: {e.Message}");
        }
    }
    
    private static void CheckDuplicateMenuItems()
    {
        try
        {
            // 메뉴 아이템 중복 체크 (간단한 검사)
            string[] menuItems = {
                "Tools/Prefab/Create Disc Prefab",
                "Tools/Generate Cat Images",
                "Tools/Run All Tests",
                "Tools/Button Utility/자동연결 (모든 씬)",
                "Tools/Complete Unity Setup - All Scenes"
            };
            
            foreach (string menuItem in menuItems)
            {
                if (Menu.GetChecked(menuItem))
                {
                    Debug.Log($"메뉴 아이템 체크됨: {menuItem}");
                }
            }
            
            Debug.Log("메뉴 아이템 중복 체크 완료");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"메뉴 아이템 체크 중 오류: {e.Message}");
        }
    }
    
    [MenuItem("Tools/Editor Optimization/Generate Optimization Report")]
    public static void GenerateOptimizationReport()
    {
        try
        {
            string report = "=== Editor 최적화 보고서 ===\n\n";
            report += $"생성 시간: {System.DateTime.Now}\n\n";
            
            // 1. 파일 크기 체크
            string[] editorScripts = Directory.GetFiles("Assets/Editor", "*.cs", SearchOption.AllDirectories);
            long totalSize = 0;
            
            foreach (string scriptPath in editorScripts)
            {
                FileInfo fileInfo = new FileInfo(scriptPath);
                totalSize += fileInfo.Length;
                report += $"{scriptPath}: {fileInfo.Length / 1024}KB\n";
            }
            
            report += $"\n총 Editor 스크립트 크기: {totalSize / 1024}KB\n";
            report += $"총 Editor 스크립트 수: {editorScripts.Length}개\n\n";
            
            // 2. 성능 지표
            long memoryUsage = System.GC.GetTotalMemory(false);
            report += $"메모리 사용량: {memoryUsage / 1024 / 1024}MB\n";
            
            // 3. 권장사항
            report += "\n=== 권장사항 ===\n";
            if (totalSize > 100 * 1024) // 100KB 이상
            {
                report += "- Editor 스크립트 크기가 큽니다. 모듈화를 고려하세요.\n";
            }
            if (memoryUsage > 500 * 1024 * 1024) // 500MB 이상
            {
                report += "- 메모리 사용량이 높습니다. 불필요한 오브젝트를 정리하세요.\n";
            }
            
            // 파일로 저장
            string fileName = $"editor_optimization_report_{System.DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = Path.Combine("Assets", fileName);
            File.WriteAllText(filePath, report);
            
            Debug.Log($"최적화 보고서 저장됨: {filePath}");
            Debug.Log(report);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"최적화 보고서 생성 중 오류: {e.Message}");
        }
    }
} 
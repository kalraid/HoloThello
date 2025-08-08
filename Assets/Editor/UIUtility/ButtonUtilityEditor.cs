using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class ButtonUtilityEditor : MonoBehaviour
{
    // === 메뉴 등록 ===
    [MenuItem("Tools/Button Utility/자동연결 (모든 씬)")]
    public static void ConnectAllButtonEvents() => ForEachScene(ConnectButtonsInScene);

    [MenuItem("Tools/Button Utility/상태체크 (현재 씬)")]
    public static void CheckButtonConnections() => CheckButtonsInScene(SceneManager.GetActiveScene().name);

    [MenuItem("Tools/Button Utility/보고서 생성 (현재 씬)")]
    public static void GenerateButtonConnectionReport() => GenerateReport(SceneManager.GetActiveScene().name);

    [MenuItem("Tools/Button Utility/퀵픽스 (현재 씬)")]
    public static void QuickFixUnconnectedButtons() => QuickFixButtonsInScene(SceneManager.GetActiveScene().name);

    [MenuItem("Tools/Button Utility/버튼 연동 가이드")]
    public static void ShowButtonConnectionGuide()
    {
        string guide = @"=== 버튼 연동 가이드 ===\n\n1. MainScene 버튼들:\n   - '시작하기' 또는 'Start' -> MainMenuManager.OnStartButtonClicked()\n   - '설정하기' 또는 'Settings' -> MainMenuManager.OnSettingsButtonClicked()\n   - '종료하기' 또는 'Exit/Quit' -> MainMenuManager.OnQuitButtonClicked()\n\n2. CharacterSelectScene 버튼들:\n   - 'TypeA' 또는 '타입A' -> CharacterSelectManager.OnClickTypeA()\n   - 'TypeB' 또는 '타입B' -> CharacterSelectManager.OnClickTypeB()\n   - '1P_0~9' -> CharacterSelectManager.OnClickCharacter(인덱스)\n   - '2P_0~9' 또는 'CPU_0~9' -> CharacterSelectManager.OnClickCharacter(인덱스)\n   - '확인' 또는 'Confirm' -> CharacterSelectManager.OnClickConfirm()\n   - '뒤로' 또는 'Back' -> MainScene으로 이동\n\n3. SettingsScene 버튼들:\n   - 'TypeA' 또는 '타입A' -> SettingsManager.OnTypeAButtonClicked()\n   - 'TypeB' 또는 '타입B' -> SettingsManager.OnTypeBButtonClicked()\n   - '뒤로' 또는 'Back' -> SettingsManager.OnBackButtonClicked()\n\n4. GameScene 버튼들:\n   - 'Skill0~2' 또는 '스킬0~2' -> GameManager.OnClickPlayerSkill(인덱스)\n   - '일시정지' 또는 'Pause' -> 일시정지 기능\n   - '재시작' 또는 'Restart' -> 게임 재시작\n   - '메뉴' 또는 'Menu' -> 메인 메뉴로 이동\n\n자동 연결: Tools > Button Utility > 자동연결 (모든 씬)\n상태 체크: Tools > Button Utility > 상태체크 (현재 씬)\n보고서: Tools > Button Utility > 보고서 생성 (현재 씬)\n퀵픽스: Tools > Button Utility > 퀵픽스 (현재 씬)\n";
        EditorUtility.DisplayDialog("버튼 연동 가이드", guide, "확인");
    }

    // === 공통 처리 ===
    static readonly string[] scenePaths = new[] {
        "Assets/Scenes/MainScene.unity",
        "Assets/Scenes/CharacterSelectScene.unity",
        "Assets/Scenes/SettingsScene.unity",
        "Assets/Scenes/GameScene.unity"
    };

    static void ForEachScene(System.Action<string> action)
    {
        try
        {
            foreach (var path in scenePaths)
            {
                if (File.Exists(path))
                {
                    action(path);
                }
                else
                {
                    Debug.LogWarning($"씬 파일을 찾을 수 없습니다: {path}");
                }
            }
            Debug.Log("모든 씬 처리 완료!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"씬 처리 중 오류가 발생했습니다: {e.Message}");
        }
    }

    // === 버튼 자동연결 ===
    static void ConnectButtonsInScene(string scenePath)
    {
        try
        {
            var scene = EditorSceneManager.OpenScene(scenePath);
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null) 
            { 
                Debug.LogWarning($"{scenePath}: Canvas 없음"); 
                return; 
            }
            
            Button[] buttons = canvas.GetComponentsInChildren<Button>(true);
            int count = 0;
            
            foreach (var button in buttons)
            {
                string name = button.gameObject.name.ToLower();
                button.onClick.RemoveAllListeners();
                if (TryConnectButton(button, name)) count++;
            }
            
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"{scenePath}: {count}개 버튼 자동연결 완료");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"{scenePath} 버튼 연결 중 오류: {e.Message}");
        }
    }

    // === 버튼 상태체크 ===
    static void CheckButtonsInScene(string sceneName)
    {
        try
        {
            Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
            int connected = 0, unconnected = 0;
            
            foreach (var button in buttons)
            {
                int cnt = button.onClick.GetPersistentEventCount();
                if (cnt > 0) connected++; else unconnected++;
                Debug.Log($"버튼: {button.gameObject.name} / 연결: {cnt}개");
            }
            
            Debug.Log($"연결된 버튼: {connected}, 연결 안됨: {unconnected}, 총: {buttons.Length}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"버튼 상태 체크 중 오류: {e.Message}");
        }
    }

    // === 버튼 보고서 ===
    static void GenerateReport(string sceneName)
    {
        try
        {
            Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
            string report = $"=== 버튼 연결 상태 보고서 ===\n\n현재 씬: {sceneName}\n\n총 버튼 수: {buttons.Length}개\n\n";
            int connected = 0, unconnected = 0;
            
            foreach (var button in buttons)
            {
                int cnt = button.onClick.GetPersistentEventCount();
                report += $"버튼: {button.gameObject.name}\n  연결된 이벤트: {cnt}개\n";
                if (cnt > 0)
                {
                    connected++;
                    report += "  상태: ✅ 연결됨\n";
                    for (int i = 0; i < cnt; i++)
                    {
                        string target = button.onClick.GetPersistentTarget(i)?.name ?? "Unknown";
                        string method = button.onClick.GetPersistentMethodName(i);
                        report += $"    - {target}.{method}()\n";
                    }
                }
                else
                {
                    unconnected++;
                    report += "  상태: ❌ 연결 안됨\n";
                    string suggestion = GetSuggestedFunction(button.gameObject.name);
                    if (!string.IsNullOrEmpty(suggestion))
                    {
                        report += $"  제안: {suggestion}\n";
                    }
                }
                report += "\n";
            }
            
            report += $"=== 요약 ===\n연결된 버튼: {connected}개\n연결 안된 버튼: {unconnected}개\n총 버튼: {buttons.Length}개";
            
            Debug.Log(report);
            
            // 파일로 저장
            string fileName = $"button_report_{sceneName}_{System.DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = Path.Combine("Assets", fileName);
            File.WriteAllText(filePath, report);
            Debug.Log($"보고서 저장됨: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"보고서 생성 중 오류: {e.Message}");
        }
    }

    // === 퀵픽스 ===
    static void QuickFixButtonsInScene(string sceneName)
    {
        try
        {
            Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
            int fixedCount = 0;
            
            foreach (var button in buttons)
            {
                if (button.onClick.GetPersistentEventCount() == 0)
                {
                    string name = button.gameObject.name.ToLower();
                    if (TryConnectButton(button, name))
                    {
                        fixedCount++;
                    }
                }
            }
            
            Debug.Log($"퀵픽스 완료: {fixedCount}개 버튼 수정됨");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"퀵픽스 중 오류: {e.Message}");
        }
    }

    // === 버튼 연결 시도 ===
    static bool TryConnectButton(Button button, string name)
    {
        try
        {
            // MainScene 버튼들
            if (name.Contains("시작") || name.Contains("start"))
            {
                var main = Object.FindFirstObjectByType<MainMenuManager>();
                if (main != null)
                {
                    button.onClick.AddListener(main.OnStartButtonClicked);
                    return true;
                }
            }
            else if (name.Contains("설정") || name.Contains("settings"))
            {
                var main = Object.FindFirstObjectByType<MainMenuManager>();
                if (main != null)
                {
                    button.onClick.AddListener(main.OnSettingsButtonClicked);
                    return true;
                }
            }
            else if (name.Contains("종료") || name.Contains("exit") || name.Contains("quit"))
            {
                var main = Object.FindFirstObjectByType<MainMenuManager>();
                if (main != null)
                {
                    button.onClick.AddListener(main.OnQuitButtonClicked);
                    return true;
                }
            }
            
            // CharacterSelectScene 버튼들
            else if (name.Contains("typea") || name.Contains("타입a"))
            {
                var charSel = Object.FindFirstObjectByType<CharacterSelectManager>();
                if (charSel != null)
                {
                    button.onClick.AddListener(charSel.OnClickTypeA);
                    return true;
                }
            }
            else if (name.Contains("typeb") || name.Contains("타입b"))
            {
                var charSel = Object.FindFirstObjectByType<CharacterSelectManager>();
                if (charSel != null)
                {
                    button.onClick.AddListener(charSel.OnClickTypeB);
                    return true;
                }
            }
            else if (name.Contains("확인") || name.Contains("confirm"))
            {
                var charSel = Object.FindFirstObjectByType<CharacterSelectManager>();
                if (charSel != null)
                {
                    button.onClick.AddListener(charSel.OnClickConfirm);
                    return true;
                }
            }
            
            // SettingsScene 버튼들
            else if (name.Contains("typea") || name.Contains("타입a"))
            {
                var settings = Object.FindFirstObjectByType<SettingsManager>();
                if (settings != null)
                {
                    button.onClick.AddListener(settings.OnTypeAButtonClicked);
                    return true;
                }
            }
            else if (name.Contains("typeb") || name.Contains("타입b"))
            {
                var settings = Object.FindFirstObjectByType<SettingsManager>();
                if (settings != null)
                {
                    button.onClick.AddListener(settings.OnTypeBButtonClicked);
                    return true;
                }
            }
            else if (name.Contains("뒤로") || name.Contains("back"))
            {
                var settings = Object.FindFirstObjectByType<SettingsManager>();
                if (settings != null)
                {
                    button.onClick.AddListener(settings.OnBackButtonClicked);
                    return true;
                }
            }
            
            // GameScene 버튼들
            else if (name.Contains("skill"))
            {
                var game = Object.FindFirstObjectByType<GameManager>();
                if (game != null)
                {
                    // 스킬 인덱스 추출
                    for (int i = 0; i <= 2; i++)
                    {
                        if (name.Contains(i.ToString()))
                        {
                            int skillIndex = i;
                            button.onClick.AddListener(() => game.OnClickPlayerSkill(skillIndex));
                            return true;
                        }
                    }
                }
            }
            
            return false;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"버튼 연결 중 오류 ({button.gameObject.name}): {e.Message}");
            return false;
        }
    }

    // === 제안 함수 가져오기 ===
    static string GetSuggestedFunction(string buttonName)
    {
        string name = buttonName.ToLower();
        
        if (name.Contains("시작") || name.Contains("start")) return "MainMenuManager.OnStartButtonClicked()";
        if (name.Contains("설정") || name.Contains("settings")) return "MainMenuManager.OnSettingsButtonClicked()";
        if (name.Contains("종료") || name.Contains("exit") || name.Contains("quit")) return "MainMenuManager.OnQuitButtonClicked()";
        if (name.Contains("typea") || name.Contains("타입a")) return "CharacterSelectManager.OnClickTypeA()";
        if (name.Contains("typeb") || name.Contains("타입b")) return "CharacterSelectManager.OnClickTypeB()";
        if (name.Contains("확인") || name.Contains("confirm")) return "CharacterSelectManager.OnClickConfirm()";
        if (name.Contains("뒤로") || name.Contains("back")) return "SettingsManager.OnBackButtonClicked()";
        if (name.Contains("skill")) return "GameManager.OnClickPlayerSkill(인덱스)";
        
        return "";
    }
} 
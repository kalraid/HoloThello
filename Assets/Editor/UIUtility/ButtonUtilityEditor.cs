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
        string guide = @"=== 버튼 연동 가이드 ===\n\n1. MainScene 버튼들:\n   - '시작하기' 또는 'Start' -> MainMenuManager.OnClickStart()\n   - '설정하기' 또는 'Settings' -> MainMenuManager.OnClickSettings()\n   - '종료하기' 또는 'Exit/Quit' -> MainMenuManager.OnClickExit()\n\n2. CharacterSelectScene 버튼들:\n   - 'TypeA' 또는 '타입A' -> CharacterSelectManager.OnClickTypeA()\n   - 'TypeB' 또는 '타입B' -> CharacterSelectManager.OnClickTypeB()\n   - '1P_0~9' -> CharacterSelectManager.OnClickCharacter(인덱스)\n   - '2P_0~9' 또는 'CPU_0~9' -> CharacterSelectManager.OnClickCharacter(인덱스)\n   - '확인' 또는 'Confirm' -> CharacterSelectManager.OnClickConfirm()\n   - '뒤로' 또는 'Back' -> MainScene으로 이동\n\n3. SettingsScene 버튼들:\n   - 'TypeA' 또는 '타입A' -> SettingsManager.OnTypeAButtonClicked()\n   - 'TypeB' 또는 '타입B' -> SettingsManager.OnTypeBButtonClicked()\n   - '뒤로' 또는 'Back' -> SettingsManager.OnBackButtonClicked()\n\n4. GameScene 버튼들:\n   - 'Skill0~2' 또는 '스킬0~2' -> GameManager.OnClickPlayerSkill(인덱스)\n   - '일시정지' 또는 'Pause' -> 일시정지 기능\n   - '재시작' 또는 'Restart' -> 게임 재시작\n   - '메뉴' 또는 'Menu' -> 메인 메뉴로 이동\n\n자동 연결: Tools > Button Utility > 자동연결 (모든 씬)\n상태 체크: Tools > Button Utility > 상태체크 (현재 씬)\n보고서: Tools > Button Utility > 보고서 생성 (현재 씬)\n퀵픽스: Tools > Button Utility > 퀵픽스 (현재 씬)\n";
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
        foreach (var path in scenePaths)
        {
            action(path);
        }
        Debug.Log("모든 씬 처리 완료!");
    }

    // === 버튼 자동연결 ===
    static void ConnectButtonsInScene(string scenePath)
    {
        var scene = EditorSceneManager.OpenScene(scenePath);
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) { Debug.LogWarning($"{scenePath}: Canvas 없음"); return; }
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

    // === 버튼 상태체크 ===
    static void CheckButtonsInScene(string sceneName)
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

    // === 버튼 보고서 ===
    static void GenerateReport(string sceneName)
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
                report += "  상태: ❌ 연결되지 않음\n";
                string suggestion = GetSuggestedFunction(button.gameObject.name);
                if (!string.IsNullOrEmpty(suggestion))
                    report += $"    추천: {suggestion}\n";
            }
            report += "\n";
        }
        report += $"=== 요약 ===\n연결된 버튼: {connected}개\n연결되지 않은 버튼: {unconnected}개\n연결률: {(buttons.Length > 0 ? (float)connected/buttons.Length*100f : 0):F1}%\n";
        string fileName = $"button_report_{sceneName}_{System.DateTime.Now:yyyyMMdd_HHmmss}.txt";
        string filePath = Path.Combine("Assets", fileName);
        File.WriteAllText(filePath, report);
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("보고서 생성 완료", $"{filePath}에 저장됨", "확인");
    }

    // === 퀵픽스 ===
    static void QuickFixButtonsInScene(string sceneName)
    {
        Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
        int fixedCount = 0;
        foreach (var button in buttons)
        {
            if (button.onClick.GetPersistentEventCount() == 0)
            {
                string name = button.gameObject.name.ToLower();
                if (TryConnectButton(button, name)) fixedCount++;
            }
        }
        if (fixedCount > 0)
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
        EditorUtility.DisplayDialog("퀵픽스 완료", $"{fixedCount}개 버튼 자동연결", "확인");
    }

    // === 버튼 추천 함수 및 연결 ===
    static string GetSuggestedFunction(string buttonName)
    {
        string n = buttonName.ToLower();
        if (n.Contains("시작") || n.Contains("start")) return "MainMenuManager.OnClickStart()";
        if (n.Contains("설정") || n.Contains("settings")) return "MainMenuManager.OnClickSettings()";
        if (n.Contains("종료") || n.Contains("exit") || n.Contains("quit")) return "MainMenuManager.OnClickExit()";
        if (n.Contains("typea") || n.Contains("타입a")) return "CharacterSelectManager.OnClickTypeA()";
        if (n.Contains("typeb") || n.Contains("타입b")) return "CharacterSelectManager.OnClickTypeB()";
        if (n.Contains("확인") || n.Contains("confirm")) return "CharacterSelectManager.OnClickConfirm()";
        if (n.Contains("back") || n.Contains("뒤로")) return "SceneManager.LoadScene('MainScene')";
        for (int i = 0; i < 10; i++)
        {
            if ((n.Contains("1p") || n.Contains("player1") || n.Contains("2p") || n.Contains("player2") || n.Contains("cpu")) && n.Contains(i.ToString()))
                return $"CharacterSelectManager.OnClickCharacter({i})"; // 1P, 2P, CPU 모두 OnClickCharacter로 통일
        }
        if (n.Contains("typea") || n.Contains("타입a")) return "SettingsManager.OnTypeAButtonClicked()";
        if (n.Contains("typeb") || n.Contains("타입b")) return "SettingsManager.OnTypeBButtonClicked()";
        if (n.Contains("back") || n.Contains("뒤로") || n.Contains("돌아가기")) return "SettingsManager.OnBackButtonClicked()";
        for (int i = 0; i < 3; i++)
        {
            if ((n.Contains("skill") || n.Contains("스킬")) && (n.Contains(i.ToString()) || n.Contains("skill" + (i + 1))))
                return $"GameManager.OnClickPlayerSkill({i})";
        }
        if (n.Contains("pause") || n.Contains("일시정지")) return "Time.timeScale = Time.timeScale == 0 ? 1 : 0";
        if (n.Contains("restart") || n.Contains("재시작")) return "SceneManager.LoadScene('GameScene')";
        if (n.Contains("menu") || n.Contains("메뉴")) return "SceneManager.LoadScene('MainScene')";
        return "";
    }

    static bool TryConnectButton(Button button, string name)
    {
        // MainMenuManager
        var main = Object.FindFirstObjectByType<MainMenuManager>();
        if (main != null)
        {
            if (name.Contains("시작") || name.Contains("start")) { button.onClick.AddListener(main.OnClickStart); return true; }
            if (name.Contains("설정") || name.Contains("settings")) { button.onClick.AddListener(main.OnClickSettings); return true; }
            if (name.Contains("종료") || name.Contains("exit") || name.Contains("quit")) { button.onClick.AddListener(main.OnClickExit); return true; }
        }
        // CharacterSelectManager
        var charSel = Object.FindFirstObjectByType<CharacterSelectManager>();
        if (charSel != null)
        {
            if (name.Contains("typea") || name.Contains("타입a")) { button.onClick.AddListener(charSel.OnClickTypeA); return true; }
            if (name.Contains("typeb") || name.Contains("타입b")) { button.onClick.AddListener(charSel.OnClickTypeB); return true; }
            if (name.Contains("확인") || name.Contains("confirm")) { button.onClick.AddListener(charSel.OnClickConfirm); return true; }
            if (name.Contains("back") || name.Contains("뒤로")) { button.onClick.AddListener(() => SceneManager.LoadScene("MainScene")); return true; }
            for (int i = 0; i < 10; i++)
            {
                // 1P, 2P, CPU 버튼 모두 OnClickCharacter(index)를 호출하도록 통일
                if ((name.Contains("1p") || name.Contains("player1") || name.Contains("2p") || name.Contains("player2") || name.Contains("cpu")) && name.Contains(i.ToString()))
                { int idx = i; button.onClick.AddListener(() => charSel.OnClickCharacter(idx)); return true; }
            }
        }
        // SettingsManager
        var settings = Object.FindFirstObjectByType<SettingsManager>();
        if (settings != null)
        {
            if (name.Contains("typea") || name.Contains("타입a")) { button.onClick.AddListener(settings.OnTypeAButtonClicked); return true; }
            if (name.Contains("typeb") || name.Contains("타입b")) { button.onClick.AddListener(settings.OnTypeBButtonClicked); return true; }
            if (name.Contains("back") || name.Contains("뒤로") || name.Contains("돌아가기")) { button.onClick.AddListener(settings.OnBackButtonClicked); return true; }
        }
        // GameManager
        var game = Object.FindFirstObjectByType<GameManager>();
        if (game != null)
        {
            for (int i = 0; i < 3; i++)
            {
                if ((name.Contains("skill") || name.Contains("스킬")) && (name.Contains(i.ToString()) || name.Contains("skill" + (i + 1))))
                { int idx = i; button.onClick.AddListener(() => game.OnClickPlayerSkill(idx)); return true; }
            }
            if (name.Contains("pause") || name.Contains("일시정지")) { button.onClick.AddListener(() => { Time.timeScale = Time.timeScale == 0 ? 1 : 0; }); return true; }
            if (name.Contains("restart") || name.Contains("재시작")) { button.onClick.AddListener(() => SceneManager.LoadScene("GameScene")); return true; }
            if (name.Contains("menu") || name.Contains("메뉴")) { button.onClick.AddListener(() => SceneManager.LoadScene("MainScene")); return true; }
        }
        return false;
    }
} 
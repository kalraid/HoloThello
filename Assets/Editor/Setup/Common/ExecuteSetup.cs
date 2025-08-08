using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class ExecuteSetup : MonoBehaviour
{
    [MenuItem("Tools/Connect Button Events")]
    public static void ConnectButtonEvents()
    {
        // MainScene 로드
        Scene mainScene = EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
        
        // Canvas 찾기
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas를 찾을 수 없습니다!");
            return;
        }

        // MainMenuManager 찾기
        MainMenuManager mainManager = FindFirstObjectByType<MainMenuManager>();
        if (mainManager == null)
        {
            Debug.LogError("MainMenuManager를 찾을 수 없습니다!");
            return;
        }

        // 버튼들 찾기
        Button[] buttons = canvas.GetComponentsInChildren<Button>();
        
        foreach (Button button in buttons)
        {
            string buttonName = button.gameObject.name;
            
            // 기존 이벤트 제거
            button.onClick.RemoveAllListeners();
            
            // 버튼 이름에 따라 이벤트 연결
            if (buttonName.Contains("시작하기"))
            {
                button.onClick.AddListener(mainManager.OnStartButtonClicked);
                Debug.Log("시작하기 버튼 이벤트 연결됨");
            }
            else if (buttonName.Contains("설정하기"))
            {
                button.onClick.AddListener(mainManager.OnSettingsButtonClicked);
                Debug.Log("설정하기 버튼 이벤트 연결됨");
            }
            else if (buttonName.Contains("종료하기"))
            {
                button.onClick.AddListener(mainManager.OnQuitButtonClicked);
                Debug.Log("종료하기 버튼 이벤트 연결됨");
            }
        }

        // 씬 저장
        EditorSceneManager.MarkSceneDirty(mainScene);
        EditorSceneManager.SaveScene(mainScene);
        
        Debug.Log("버튼 이벤트 연결이 완료되었습니다!");
    }

    [MenuItem("Tools/Setup Build Settings")]
    public static void SetupBuildSettings()
    {
        // 빌드 설정에 씬들 추가
        string[] scenes = {
            "Assets/Scenes/MainScene.unity",
            "Assets/Scenes/CharacterSelectScene.unity", 
            "Assets/Scenes/SettingsScene.unity",
            "Assets/Scenes/GameScene.unity"
        };

        EditorBuildSettings.scenes = new EditorBuildSettingsScene[scenes.Length];
        
        for (int i = 0; i < scenes.Length; i++)
        {
            EditorBuildSettings.scenes[i] = new EditorBuildSettingsScene(scenes[i], true);
        }
        
        Debug.Log("빌드 설정에 씬들이 추가되었습니다!");
    }

    [MenuItem("Tools/Complete Unity Setup (Legacy)")]
    public static void CompleteUnitySetup()
    {
        ConnectButtonEvents();
        SetupBuildSettings();
        
        Debug.Log("Unity 설정이 완료되었습니다!");
    }
}

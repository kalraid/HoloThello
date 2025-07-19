using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class EventSystemSetup : MonoBehaviour
{
    [MenuItem("Tools/Setup EventSystem")]
    public static void SetupEventSystem()
    {
        Debug.Log("EventSystem 설정 시작...");
        
        // 현재 씬의 EventSystem 찾기
        EventSystem existingEventSystem = Object.FindFirstObjectByType<EventSystem>();
        
        if (existingEventSystem != null)
        {
            Debug.Log("기존 EventSystem 발견, Input System 호환으로 업데이트...");
            
            // 기존 StandaloneInputModule 제거
            StandaloneInputModule oldInputModule = existingEventSystem.GetComponent<StandaloneInputModule>();
            if (oldInputModule != null)
            {
                Object.DestroyImmediate(oldInputModule);
                Debug.Log("기존 StandaloneInputModule 제거됨");
            }
            
            // InputSystemUIInputModule 추가
            InputSystemUIInputModule newInputModule = existingEventSystem.GetComponent<InputSystemUIInputModule>();
            if (newInputModule == null)
            {
                newInputModule = existingEventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
                Debug.Log("InputSystemUIInputModule 추가됨");
            }
            
            // Input Action Asset 설정
            var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/InputSystem_Actions.inputactions");
            if (inputActions != null)
            {
                newInputModule.actionsAsset = inputActions;
                Debug.Log("Input Action Asset 설정됨");
            }
            else
            {
                Debug.LogWarning("InputSystem_Actions.inputactions 파일을 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.Log("새 EventSystem 생성...");
            
            // 새 EventSystem 생성
            GameObject eventSystemGO = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            EventSystem eventSystem = eventSystemGO.GetComponent<EventSystem>();
            
            // Input Action Asset 설정
            var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/InputSystem_Actions.inputactions");
            if (inputActions != null)
            {
                InputSystemUIInputModule inputModule = eventSystemGO.GetComponent<InputSystemUIInputModule>();
                inputModule.actionsAsset = inputActions;
                Debug.Log("새 EventSystem에 Input Action Asset 설정됨");
            }
            else
            {
                Debug.LogWarning("InputSystem_Actions.inputactions 파일을 찾을 수 없습니다.");
            }
        }
        
        Debug.Log("EventSystem 설정 완료!");
    }
    
    [MenuItem("Tools/Fix Input System Conflicts")]
    public static void FixInputSystemConflicts()
    {
        Debug.Log("Input System 충돌 해결 시작...");
        
        // 모든 씬의 EventSystem 확인
        string[] scenePaths = {
            "Assets/Scenes/MainScene.unity",
            "Assets/Scenes/CharacterSelectScene.unity", 
            "Assets/Scenes/GameScene.unity",
            "Assets/Scenes/SettingsScene.unity"
        };
        
        foreach (string scenePath in scenePaths)
        {
            if (System.IO.File.Exists(scenePath))
            {
                Debug.Log($"씬 확인: {scenePath}");
                
                // 씬 로드
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
                
                // EventSystem 설정
                SetupEventSystem();
                
                // 씬 저장
                UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            }
        }
        
        Debug.Log("모든 씬의 Input System 충돌 해결 완료!");
    }
} 
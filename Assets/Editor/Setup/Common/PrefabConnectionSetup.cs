using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace EditorSetup.Common
{
    public static class PrefabConnectionSetup
    {
        [MenuItem("Tools/Setup/Connect Prefabs")]
        public static void ConnectAllPrefabs()
        {
            Debug.Log("모든 프리팹 연결을 시작합니다...");
            
            // EffectManager 확인
            #pragma warning disable CS0618 // Type or member is obsolete
            var effectManager = Object.FindObjectOfType<EffectManager>();
            #pragma warning restore CS0618 // Type or member is obsolete
            if (effectManager != null)
            {
                ConnectEffectManagerPrefabs();
            }
            
            // BoardManager 확인
            #pragma warning disable CS0618 // Type or member is obsolete
            var boardManager = Object.FindObjectOfType<BoardManager>();
            #pragma warning restore CS0618 // Type or member is obsolete
            if (boardManager != null)
            {
                ConnectBoardManagerPrefabs();
            }
            
            // GameManager 확인
            #pragma warning disable CS0618 // Type or member is obsolete
            var gameManager = Object.FindObjectOfType<GameManager>();
            #pragma warning restore CS0618 // Type or member is obsolete
            if (gameManager != null)
            {
                ConnectGameManagerPrefabs();
            }
            
            Debug.Log("모든 프리팹 연결이 완료되었습니다.");
        }
        
        [MenuItem("Tools/Setup/Connect Prefabs with Scene Switch")]
        public static void ConnectAllPrefabsWithSceneSwitch()
        {
            // GameScene으로 전환
            if (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name != "GameScene")
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");
                Debug.Log("GameScene으로 전환 완료");
            }
            
            // 잠시 대기 후 프리팹 연결
            EditorApplication.delayCall += () =>
            {
                ConnectEffectManagerPrefabs();
                ConnectBoardManagerPrefabs();
                ConnectGameManagerPrefabs();
                ConnectAudioManagerPrefabs();
                
                EditorUtility.DisplayDialog("프리팹 연결 완료", "모든 프리팹이 연결되었습니다!", "확인");
            };
        }
        
        [MenuItem("Tools/Setup/Connect EffectManager Prefabs")]
        public static void ConnectEffectManagerPrefabs()
        {
            #pragma warning disable CS0618 // Type or member is obsolete
            var effectManager = Object.FindObjectOfType<EffectManager>();
            #pragma warning restore CS0618 // Type or member is obsolete
            if (effectManager != null)
            {
                // EffectManager 프리팹 연결
                ConnectPrefabToField(effectManager, "damageTextPrefab", "DamageText");
                ConnectPrefabToField(effectManager, "skillEffectPrefab", "SkillEffect");
                ConnectPrefabToField(effectManager, "victoryDefeatEffectPrefab", "VictoryDefeatEffect");
                
                Debug.Log("EffectManager 프리팹 연결 완료");
            }
            else
            {
                Debug.LogWarning("EffectManager를 찾을 수 없습니다.");
            }
        }
        
        [MenuItem("Tools/Setup/Connect BoardManager Prefabs")]
        public static void ConnectBoardManagerPrefabs()
        {
            #pragma warning disable CS0618 // Type or member is obsolete
            var boardManager = Object.FindObjectOfType<BoardManager>();
            #pragma warning restore CS0618 // Type or member is obsolete
            if (boardManager != null)
            {
                // BoardManager 프리팹 연결
                ConnectPrefabToField(boardManager, "discPrefab", "Disc");
                
                Debug.Log("BoardManager 프리팹 연결 완료");
            }
            else
            {
                Debug.LogWarning("BoardManager를 찾을 수 없습니다.");
            }
        }
        
        [MenuItem("Tools/Setup/Connect GameManager Prefabs")]
        public static void ConnectGameManagerPrefabs()
        {
            #pragma warning disable CS0618 // Type or member is obsolete
            var gameManager = Object.FindObjectOfType<GameManager>();
            #pragma warning restore CS0618 // Type or member is obsolete
            if (gameManager != null)
            {
                // GameManager 프리팹 연결
                ConnectPrefabToField(gameManager, "skillButtonPrefab", "SkillButton");
                ConnectPrefabToField(gameManager, "messagePopupPrefab", "MessagePopup");
                
                Debug.Log("GameManager 프리팹 연결 완료");
            }
            else
            {
                Debug.LogWarning("GameManager를 찾을 수 없습니다.");
            }
        }
        
        [MenuItem("Tools/Setup/Connect AudioManager Prefabs")]
        public static void ConnectAudioManagerPrefabs()
        {
            #pragma warning disable CS0618 // Type or member is obsolete
            var audioManager = Object.FindObjectOfType<AudioManager>();
            #pragma warning restore CS0618 // Type or member is obsolete
            if (audioManager != null)
            {
                // AudioManager 프리팹 연결
                ConnectPrefabToField(audioManager, "audioSourcePrefab", "AudioSource");
                
                Debug.Log("AudioManager 프리팹 연결 완료");
            }
            else
            {
                Debug.LogWarning("AudioManager를 찾을 수 없습니다.");
            }
        }
        
        [MenuItem("Tools/Setup/Validate Prefab Connections")]
        public static void ValidatePrefabConnections()
        {
            Debug.Log("=== 프리팹 연결 상태 확인 ===");
            
            // EffectManager 확인
            var effectManager = FindFirstObjectByType<EffectManager>();
            if (effectManager != null)
            {
                Debug.Log($"EffectManager.damageTextPrefab: {(effectManager.damageTextPrefab != null ? "연결됨" : "연결 안됨")}");
                Debug.Log($"EffectManager.skillButtonPrefab: {(effectManager.skillButtonPrefab != null ? "연결됨" : "연결 안됨")}");
            }
            
            // BoardManager 확인
            var boardManager = FindFirstObjectByType<BoardManager>();
            if (boardManager != null)
            {
                Debug.Log($"BoardManager.discPrefab: {(boardManager.discPrefab != null ? "연결됨" : "연결 안됨")}");
            }
            
            // GameManager 확인
            var gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                Debug.Log($"GameManager.discPrefab: {(gameManager.discPrefab != null ? "연결됨" : "연결 안됨")}");
            }
            
            Debug.Log("=== 프리팹 연결 상태 확인 완료 ===");
        }
        
        [MenuItem("Tools/Setup/Switch to GameScene")]
        public static void SwitchToGameScene()
        {
            if (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name != "GameScene")
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");
                Debug.Log("GameScene으로 전환 완료");
            }
            else
            {
                Debug.Log("이미 GameScene에 있습니다.");
            }
        }
    }
} 
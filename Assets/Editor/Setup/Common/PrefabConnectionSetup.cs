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
            ConnectEffectManagerPrefabs();
            ConnectBoardManagerPrefabs();
            ConnectGameManagerPrefabs();
            ConnectAudioManagerPrefabs();
            
            EditorUtility.DisplayDialog("프리팹 연결 완료", "모든 프리팹이 연결되었습니다!", "확인");
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
            var effectManager = Object.FindObjectOfType<EffectManager>();
            if (effectManager != null)
            {
                var damageTextPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/DamageText.prefab");
                var skillButtonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SkillButton.prefab");
                
                if (damageTextPrefab != null)
                {
                    effectManager.damageTextPrefab = damageTextPrefab;
                    Debug.Log("EffectManager: damageTextPrefab 연결 완료");
                }
                
                if (skillButtonPrefab != null)
                {
                    effectManager.skillButtonPrefab = skillButtonPrefab;
                    Debug.Log("EffectManager: skillButtonPrefab 연결 완료");
                }
                
                EditorUtility.SetDirty(effectManager);
            }
            else
            {
                Debug.LogWarning("EffectManager를 찾을 수 없습니다. GameScene에서 실행해주세요.");
            }
        }
        
        [MenuItem("Tools/Setup/Connect BoardManager Prefabs")]
        public static void ConnectBoardManagerPrefabs()
        {
            var boardManager = Object.FindObjectOfType<BoardManager>();
            if (boardManager != null)
            {
                var discPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Disc.prefab");
                
                if (discPrefab != null)
                {
                    boardManager.discPrefab = discPrefab;
                    Debug.Log("BoardManager: discPrefab 연결 완료");
                    EditorUtility.SetDirty(boardManager);
                }
            }
            else
            {
                Debug.LogWarning("BoardManager를 찾을 수 없습니다. GameScene에서 실행해주세요.");
            }
        }
        
        [MenuItem("Tools/Setup/Connect GameManager Prefabs")]
        public static void ConnectGameManagerPrefabs()
        {
            var gameManager = Object.FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                var discPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Disc.prefab");
                
                if (discPrefab != null)
                {
                    gameManager.discPrefab = discPrefab;
                    Debug.Log("GameManager: discPrefab 연결 완료");
                    EditorUtility.SetDirty(gameManager);
                }
            }
            else
            {
                Debug.LogWarning("GameManager를 찾을 수 없습니다. GameScene에서 실행해주세요.");
            }
        }
        
        [MenuItem("Tools/Setup/Connect AudioManager Prefabs")]
        public static void ConnectAudioManagerPrefabs()
        {
            var audioManager = Object.FindObjectOfType<AudioManager>();
            if (audioManager != null)
            {
                // AudioSource 프리팹이 있다면 연결
                var audioSourcePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/AudioSource.prefab");
                if (audioSourcePrefab != null)
                {
                    audioManager.audioSourcePrefab = audioSourcePrefab;
                    Debug.Log("AudioManager: audioSourcePrefab 연결 완료");
                    EditorUtility.SetDirty(audioManager);
                }
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
            var effectManager = Object.FindObjectOfType<EffectManager>();
            if (effectManager != null)
            {
                Debug.Log($"EffectManager.damageTextPrefab: {(effectManager.damageTextPrefab != null ? "연결됨" : "연결 안됨")}");
                Debug.Log($"EffectManager.skillButtonPrefab: {(effectManager.skillButtonPrefab != null ? "연결됨" : "연결 안됨")}");
            }
            
            // BoardManager 확인
            var boardManager = Object.FindObjectOfType<BoardManager>();
            if (boardManager != null)
            {
                Debug.Log($"BoardManager.discPrefab: {(boardManager.discPrefab != null ? "연결됨" : "연결 안됨")}");
            }
            
            // GameManager 확인
            var gameManager = Object.FindObjectOfType<GameManager>();
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
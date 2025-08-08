using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace EditorSetup.Common
{
    public static class MissingComponentsSetup
    {
        [MenuItem("Tools/Setup/Create Missing Components")]
        public static void CreateMissingComponents()
        {
            CreateHPBars();
            CreateBoardContainer();
            CreateEffectCanvas();
            CreateAnimators();
            
            EditorUtility.DisplayDialog("누락된 컴포넌트 생성 완료", "모든 누락된 컴포넌트가 생성되었습니다!", "확인");
        }
        
        [MenuItem("Tools/Setup/Create HP Bars")]
        public static void CreateHPBars()
        {
            // PlayerHpBar 생성
            GameObject playerHpBar = GameObject.Find("PlayerHpBar");
            if (playerHpBar == null)
            {
                playerHpBar = new GameObject("PlayerHpBar", typeof(Slider));
                Slider playerSlider = playerHpBar.GetComponent<Slider>();
                playerSlider.minValue = 0;
                playerSlider.maxValue = 100;
                playerSlider.value = 100;
                Debug.Log("PlayerHpBar 생성 완료");
            }
            
            // CpuHpBar 생성
            GameObject cpuHpBar = GameObject.Find("CpuHpBar");
            if (cpuHpBar == null)
            {
                cpuHpBar = new GameObject("CpuHpBar", typeof(Slider));
                Slider cpuSlider = cpuHpBar.GetComponent<Slider>();
                cpuSlider.minValue = 0;
                cpuSlider.maxValue = 100;
                cpuSlider.value = 100;
                Debug.Log("CpuHpBar 생성 완료");
            }
        }
        
        [MenuItem("Tools/Setup/Create Board Container")]
        public static void CreateBoardContainer()
        {
            GameObject boardContainer = GameObject.Find("BoardContainer");
            if (boardContainer == null)
            {
                boardContainer = new GameObject("BoardContainer", typeof(RectTransform));
                Debug.Log("BoardContainer 생성 완료");
            }
        }
        
        [MenuItem("Tools/Setup/Create Effect Canvas")]
        public static void CreateEffectCanvas()
        {
            GameObject effectCanvas = GameObject.Find("EffectCanvas");
            if (effectCanvas == null)
            {
                effectCanvas = new GameObject("EffectCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                Canvas canvas = effectCanvas.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 10; // UI 위에 표시
                Debug.Log("EffectCanvas 생성 완료");
            }
        }
        
        [MenuItem("Tools/Setup/Create Animators")]
        public static void CreateAnimators()
        {
            // Player1Animator 생성
            GameObject player1Animator = GameObject.Find("Player1Animator");
            if (player1Animator == null)
            {
                player1Animator = new GameObject("Player1Animator", typeof(Animator));
                Debug.Log("Player1Animator 생성 완료");
            }
            
            // CpuAnimator 생성
            GameObject cpuAnimator = GameObject.Find("CpuAnimator");
            if (cpuAnimator == null)
            {
                cpuAnimator = new GameObject("CpuAnimator", typeof(Animator));
                Debug.Log("CpuAnimator 생성 완료");
            }
        }
        
        [MenuItem("Tools/Setup/Connect All Components")]
        public static void ConnectAllComponents()
        {
            // MainMenuManager 버튼 연결
            var mainMenuManager = Object.FindFirstObjectByType<MainMenuManager>();
            if (mainMenuManager != null)
            {
                var startButton = GameObject.Find("StartButton");
                var settingsButton = GameObject.Find("SettingsButton");
                var quitButton = GameObject.Find("QuitButton");
                
                if (startButton != null) mainMenuManager.startButton = startButton.GetComponent<Button>();
                if (settingsButton != null) mainMenuManager.settingsButton = settingsButton.GetComponent<Button>();
                if (quitButton != null) mainMenuManager.quitButton = quitButton.GetComponent<Button>();
                
                Debug.Log("MainMenuManager 버튼 연결 완료");
            }
            
            // BoardManager 연결
            var boardManager = Object.FindFirstObjectByType<BoardManager>();
            if (boardManager != null)
            {
                var discPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Disc.prefab");
                var boardContainer = GameObject.Find("BoardContainer");
                
                if (discPrefab != null) boardManager.discPrefab = discPrefab;
                if (boardContainer != null) boardManager.boardContainer = boardContainer.transform;
                
                Debug.Log("BoardManager 연결 완료");
            }
            
            // GameManager 연결
            var gameManager = Object.FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                var playerHpBar = GameObject.Find("PlayerHpBar");
                var cpuHpBar = GameObject.Find("CpuHpBar");
                var boardManagerComponent = Object.FindFirstObjectByType<BoardManager>();
                
                if (playerHpBar != null) gameManager.playerHpBar = playerHpBar.GetComponent<Slider>();
                if (cpuHpBar != null) gameManager.cpuHpBar = cpuHpBar.GetComponent<Slider>();
                if (boardManagerComponent != null) gameManager.boardManager = boardManagerComponent;
                
                Debug.Log("GameManager 연결 완료");
            }
            
            // EffectManager 연결
            var effectManager = Object.FindFirstObjectByType<EffectManager>();
            if (effectManager != null)
            {
                var damageTextPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/DamageText.prefab");
                var effectCanvas = GameObject.Find("EffectCanvas");
                var player1Animator = GameObject.Find("Player1Animator");
                var cpuAnimator = GameObject.Find("CpuAnimator");
                
                if (damageTextPrefab != null) effectManager.damageTextPrefab = damageTextPrefab;
                if (effectCanvas != null) effectManager.effectCanvas = effectCanvas.transform;
                if (player1Animator != null) effectManager.player1Animator = player1Animator.GetComponent<Animator>();
                if (cpuAnimator != null) effectManager.cpuAnimator = cpuAnimator.GetComponent<Animator>();
                
                Debug.Log("EffectManager 연결 완료");
            }
            
            EditorUtility.DisplayDialog("컴포넌트 연결 완료", "모든 컴포넌트가 연결되었습니다!", "확인");
        }
    }
} 
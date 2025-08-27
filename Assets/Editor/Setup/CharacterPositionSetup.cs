using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace EditorSetup
{
    public static class CharacterPositionSetup
    {
        [MenuItem("Tools/Setup/Create Character Position Markers")]
        public static void CreateCharacterPositionMarkers()
        {
            // 현재 씬이 GameScene인지 확인
            if (EditorSceneManager.GetActiveScene().name != "GameScene")
            {
                EditorUtility.DisplayDialog("씬 오류", "GameScene에서만 실행할 수 있습니다.", "확인");
                return;
            }

            // 기존 위치 마커들이 있는지 확인
            bool hasExistingMarkers = CheckExistingPositionMarkers();
            
            if (hasExistingMarkers)
            {
                bool shouldOverwrite = EditorUtility.DisplayDialog(
                    "기존 마커 발견", 
                    "이미 캐릭터 위치 마커들이 존재합니다.\n\n기존 마커들을 모두 제거하고 새로 생성하시겠습니까?", 
                    "예, 새로 생성", 
                    "아니오, 취소"
                );
                
                if (!shouldOverwrite)
                {
                    Debug.Log("캐릭터 위치 마커 생성이 취소되었습니다.");
                    return;
                }
            }

            // 1단계: CharacterBattleMotion 컴포넌트 자동 추가
            CharacterBattleMotion battleMotion = SetupCharacterBattleMotionComponent();
            if (battleMotion == null)
            {
                EditorUtility.DisplayDialog("오류", "CharacterBattleMotion 컴포넌트 설정에 실패했습니다.", "확인");
                return;
            }

            // 2단계: 기존 위치 마커들 정리
            CleanupExistingPositionMarkers();

            // 3단계: 캐릭터 위치 마커들을 담을 부모 오브젝트 생성
            GameObject positionParent = new GameObject("CharacterPositionMarkers");
            positionParent.tag = "EditorOnly";

            // 4단계: 1P 캐릭터 위치 마커들 생성
            CreatePlayerPositionMarkers(positionParent.transform);

            // 5단계: 2P/CPU 캐릭터 위치 마커들 생성
            CreateCPUPositionMarkers(positionParent.transform);

            // 6단계: CharacterBattleMotion 컴포넌트에 위치 마커들 연결
            ConnectPositionMarkersToCharacterBattleMotion();

            // 7단계: 캐릭터 이미지 자동 연결 시도
            TryAutoConnectCharacterImages(battleMotion);

            string message = hasExistingMarkers ? 
                "기존 마커들을 제거하고 새로운 캐릭터 위치 마커드가 성공적으로 생성되었습니다!" : 
                "캐릭터 위치 마커드가 성공적으로 생성되었습니다!";
                
            EditorUtility.DisplayDialog("완료", message, "확인");
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        /// <summary>
        /// 메뉴 항목 상태 확인 (GameScene에서만 활성화)
        /// </summary>
        [MenuItem("Tools/Setup/Create Character Position Markers", true)]
        public static bool ValidateCreateCharacterPositionMarkers()
        {
            return EditorSceneManager.GetActiveScene().name == "GameScene";
        }

        /// <summary>
        /// 기존 위치 마커들이 존재하는지 확인
        /// </summary>
        private static bool CheckExistingPositionMarkers()
        {
            // CharacterPositionMarkers 부모 오브젝트가 있는지 확인
            GameObject existingParent = GameObject.Find("CharacterPositionMarkers");
            if (existingParent != null)
            {
                return true;
            }

            // 개별 위치 마커들이 있는지 확인
            string[] markerNames = {
                "PlayerStartPosition", "PlayerCenterPosition", "PlayerFarPosition",
                "CPUStartPosition", "CPUCenterPosition", "CPUFarPosition"
            };

            foreach (string markerName in markerNames)
            {
                if (GameObject.Find(markerName) != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static void CleanupExistingPositionMarkers()
        {
            // 기존 위치 마커들 찾아서 제거
            GameObject[] existingMarkers = GameObject.FindGameObjectsWithTag("EditorOnly");
            foreach (GameObject marker in existingMarkers)
            {
                if (marker.name.Contains("PositionMarker") || marker.name.Contains("CharacterPositionMarkers"))
                {
                    Object.DestroyImmediate(marker);
                }
            }

            // EditorOnly 태그가 아닌 개별 마커들도 제거
            string[] markerNames = {
                "PlayerStartPosition", "PlayerCenterPosition", "PlayerFarPosition",
                "CPUStartPosition", "CPUCenterPosition", "CPUFarPosition"
            };

            foreach (string markerName in markerNames)
            {
                GameObject marker = GameObject.Find(markerName);
                if (marker != null)
                {
                    Object.DestroyImmediate(marker);
                }
            }

            Debug.Log("기존 캐릭터 위치 마커드들이 정리되었습니다.");
        }

        private static void CreatePlayerPositionMarkers(Transform parent)
        {
            // 1P 시작 위치 (기본 위치)
            GameObject startPos = CreatePositionMarker(parent, "PlayerStartPosition", new Vector3(-300f, 0f, 0f), Color.blue);
            
            // 1P 중앙 위치 (우위일 때)
            GameObject centerPos = CreatePositionMarker(parent, "PlayerCenterPosition", new Vector3(-200f, 0f, 0f), Color.cyan);
            
            // 1P 멀리 위치 (열위일 때)
            GameObject farPos = CreatePositionMarker(parent, "PlayerFarPosition", new Vector3(-400f, 0f, 0f), Color.darkBlue);
        }

        private static void CreateCPUPositionMarkers(Transform parent)
        {
            // 2P/CPU 시작 위치 (기본 위치)
            GameObject startPos = CreatePositionMarker(parent, "CPUStartPosition", new Vector3(300f, 0f, 0f), Color.red);
            
            // 2P/CPU 중앙 위치 (우위일 때)
            GameObject centerPos = CreatePositionMarker(parent, "CPUCenterPosition", new Vector3(200f, 0f, 0f), Color.magenta);
            
            // 2P/CPU 멀리 위치 (열위일 때)
            GameObject farPos = CreatePositionMarker(parent, "CPUFarPosition", new Vector3(400f, 0f, 0f), Color.darkRed);
        }

        private static GameObject CreatePositionMarker(Transform parent, string name, Vector3 position, Color color)
        {
            GameObject marker = new GameObject(name);
            marker.transform.SetParent(parent, false);
            marker.transform.position = position;
            marker.tag = "EditorOnly";

            // 시각적 표시를 위한 SpriteRenderer 추가
            SpriteRenderer spriteRenderer = marker.AddComponent<SpriteRenderer>();
            
            // 기본 스프라이트 생성 (원형)
            Texture2D texture = CreateCircleTexture(32, color);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            spriteRenderer.sprite = sprite;
            
            // 크기 조정
            marker.transform.localScale = Vector3.one * 0.5f;
            
            // Sorting Order 설정
            spriteRenderer.sortingOrder = 1000;
            
            return marker;
        }

        private static Texture2D CreateCircleTexture(int size, Color color)
        {
            Texture2D texture = new Texture2D(size, size);
            Color[] pixels = new Color[size * size];
            
            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size / 2f;
            
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    if (distance <= radius)
                    {
                        pixels[y * size + x] = color;
                    }
                    else
                    {
                        pixels[y * size + x] = Color.clear;
                    }
                }
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        private static void ConnectPositionMarkersToCharacterBattleMotion()
        {
            CharacterBattleMotion battleMotion = Object.FindObjectOfType<CharacterBattleMotion>();
            if (battleMotion == null)
            {
                Debug.LogWarning("CharacterBattleMotion 컴포넌트를 찾을 수 없습니다. 수동으로 연결해주세요.");
                return;
            }

            // 위치 마커들 찾기
            Transform positionParent = GameObject.Find("CharacterPositionMarkers")?.transform;
            if (positionParent == null)
            {
                Debug.LogWarning("CharacterPositionMarkers를 찾을 수 없습니다.");
                return;
            }

            // 1P 위치 마커들 연결
            Transform playerStartPos = positionParent.Find("PlayerStartPosition");
            Transform playerCenterPos = positionParent.Find("PlayerCenterPosition");
            Transform playerFarPos = positionParent.Find("PlayerFarPosition");

            // 2P/CPU 위치 마커들 연결
            Transform cpuStartPos = positionParent.Find("CPUStartPosition");
            Transform cpuCenterPos = positionParent.Find("CPUCenterPosition");
            Transform cpuFarPos = positionParent.Find("CPUFarPosition");

            // SerializedObject를 사용하여 컴포넌트 업데이트
            SerializedObject serializedObject = new SerializedObject(battleMotion);
            
            if (playerStartPos != null)
                serializedObject.FindProperty("playerStartPosition").objectReferenceValue = playerStartPos;
            if (playerCenterPos != null)
                serializedObject.FindProperty("playerCenterPosition").objectReferenceValue = playerCenterPos;
            if (playerFarPos != null)
                serializedObject.FindProperty("playerFarPosition").objectReferenceValue = playerFarPos;
            
            if (cpuStartPos != null)
                serializedObject.FindProperty("cpuStartPosition").objectReferenceValue = cpuStartPos;
            if (cpuCenterPos != null)
                serializedObject.FindProperty("cpuCenterPosition").objectReferenceValue = cpuCenterPos;
            if (cpuFarPos != null)
                serializedObject.FindProperty("cpuFarPosition").objectReferenceValue = cpuFarPos;

            serializedObject.ApplyModifiedProperties();
            
            Debug.Log("캐릭터 위치 마커드가 CharacterBattleMotion에 연결되었습니다.");
        }

        /// <summary>
        /// CharacterBattleMotion 컴포넌트 자동 설정
        /// </summary>
        private static CharacterBattleMotion SetupCharacterBattleMotionComponent()
        {
            // GameManager 찾기
            GameManager gameManager = Object.FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("GameManager를 찾을 수 없습니다.");
                return null;
            }

            // 이미 CharacterBattleMotion이 있는지 확인
            CharacterBattleMotion existingMotion = gameManager.GetComponent<CharacterBattleMotion>();
            if (existingMotion != null)
            {
                Debug.Log("GameManager에 이미 CharacterBattleMotion 컴포넌트가 있습니다.");
                return existingMotion;
            }

            // CharacterBattleMotion 컴포넌트 추가
            CharacterBattleMotion battleMotion = gameManager.gameObject.AddComponent<CharacterBattleMotion>();
            Debug.Log("GameManager에 CharacterBattleMotion 컴포넌트를 추가했습니다.");

            return battleMotion;
        }

        /// <summary>
        /// 캐릭터 이미지 자동 연결 시도
        /// </summary>
        private static void TryAutoConnectCharacterImages(CharacterBattleMotion battleMotion)
        {
            // Canvas 찾기
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("Canvas를 찾을 수 없어 캐릭터 이미지 자동 연결을 건너뜁니다.");
                return;
            }

            // 1P 캐릭터 이미지 찾기 (Player로 시작하는 Image 컴포넌트)
            Image[] allImages = canvas.GetComponentsInChildren<Image>();
            Image playerImage = null;
            Image cpuImage = null;

            foreach (Image img in allImages)
            {
                if (img.name.ToLower().Contains("player") || img.name.ToLower().Contains("1p"))
                {
                    playerImage = img;
                }
                else if (img.name.ToLower().Contains("cpu") || img.name.ToLower().Contains("2p") || img.name.ToLower().Contains("enemy"))
                {
                    cpuImage = img;
                }
            }

            // SerializedObject를 사용하여 연결
            SerializedObject serializedObject = new SerializedObject(battleMotion);
            
            if (playerImage != null)
            {
                serializedObject.FindProperty("playerCharacterImage").objectReferenceValue = playerImage;
                Debug.Log($"1P 캐릭터 이미지 '{playerImage.name}'를 자동 연결했습니다.");
            }
            else
            {
                Debug.LogWarning("1P 캐릭터 이미지를 찾을 수 없습니다. 수동으로 연결해주세요.");
            }

            if (cpuImage != null)
            {
                serializedObject.FindProperty("cpuCharacterImage").objectReferenceValue = cpuImage;
                Debug.Log($"2P/CPU 캐릭터 이미지 '{cpuImage.name}'를 자동 연결했습니다.");
            }
            else
            {
                Debug.LogWarning("2P/CPU 캐릭터 이미지를 찾을 수 없습니다. 수동으로 연결해주세요.");
            }

            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("Tools/Setup/Cleanup Character Position Markers")]
        public static void CleanupCharacterPositionMarkers()
        {
            if (EditorUtility.DisplayDialog("정리 확인", "캐릭터 위치 마커드들을 모두 제거하시겠습니까?", "예", "아니오"))
            {
                CleanupExistingPositionMarkers();
                EditorUtility.DisplayDialog("완료", "캐릭터 위치 마커드들이 제거되었습니다.", "확인");
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }
}

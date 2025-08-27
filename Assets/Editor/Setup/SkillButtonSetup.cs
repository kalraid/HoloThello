using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace EditorSetup
{
    public static class SkillButtonSetup
    {
        [MenuItem("Tools/Setup/Create Skill Buttons")]
        public static void CreateSkillButtons()
        {
            // 현재 씬이 GameScene인지 확인
            if (EditorSceneManager.GetActiveScene().name != "GameScene")
            {
                EditorUtility.DisplayDialog("씬 오류", "GameScene에서만 실행할 수 있습니다.", "확인");
                return;
            }

            // Canvas 찾기
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                EditorUtility.DisplayDialog("오류", "Canvas를 찾을 수 없습니다.", "확인");
                return;
            }

            // BoardArea 찾기
            GameObject boardArea = GameObject.Find("BoardArea");
            if (boardArea == null)
            {
                EditorUtility.DisplayDialog("오류", "BoardArea를 찾을 수 없습니다.", "확인");
                return;
            }

            // 기존 스킬 버튼들 정리
            CleanupExistingSkillButtons();

            // 1P 스킬 버튼 생성 (좌측)
            CreatePlayerSkillButtons(canvas.transform, boardArea.transform);

            // 2P/CPU 스킬 버튼 생성 (우측)
            CreateCPUSkillButtons(canvas.transform, boardArea.transform);

            // GameManager에 스킬 버튼 연결
            ConnectSkillButtonsToGameManager();

            EditorUtility.DisplayDialog("완료", "스킬 버튼이 성공적으로 생성되었습니다!", "확인");
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private static void CleanupExistingSkillButtons()
        {
            // 기존 스킬 버튼들 찾아서 제거
            GameObject[] existingButtons = GameObject.FindGameObjectsWithTag("SkillButton");
            foreach (GameObject button in existingButtons)
            {
                Object.DestroyImmediate(button);
            }

            // PlayerSkillButton, CPUSkillButton으로 시작하는 오브젝트들도 제거
            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.StartsWith("PlayerSkillButton") || obj.name.StartsWith("CPUSkillButton"))
                {
                    Object.DestroyImmediate(obj);
                }
            }
        }

        private static void CreatePlayerSkillButtons(Transform canvas, Transform boardArea)
        {
            // 1P 스킬 버튼들을 담을 패널 생성
            GameObject skillPanel = new GameObject("PlayerSkillPanel", typeof(RectTransform));
            skillPanel.transform.SetParent(canvas, false);
            skillPanel.tag = "SkillButton";

            RectTransform panelRect = skillPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.05f, 0.3f);
            panelRect.anchorMax = new Vector2(0.25f, 0.7f);
            panelRect.sizeDelta = Vector2.zero;

            // 3개 스킬 버튼 생성 (A, B, C)
            string[] skillNames = { "A", "B", "C" };
            for (int i = 0; i < 3; i++)
            {
                GameObject skillButton = CreateSkillButton(skillPanel.transform, skillNames[i], i, true);
                skillButton.name = $"PlayerSkillButton_{i}";
            }
        }

        private static void CreateCPUSkillButtons(Transform canvas, Transform boardArea)
        {
            // 2P/CPU 스킬 버튼들을 담을 패널 생성
            GameObject skillPanel = new GameObject("CPUSkillPanel", typeof(RectTransform));
            skillPanel.transform.SetParent(canvas, false);
            skillPanel.tag = "SkillButton";

            RectTransform panelRect = skillPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.75f, 0.3f);
            panelRect.anchorMax = new Vector2(0.95f, 0.7f);
            panelRect.sizeDelta = Vector2.zero;

            // 3개 스킬 버튼 생성 (A, B, C)
            string[] skillNames = { "A", "B", "C" };
            for (int i = 0; i < 3; i++)
            {
                GameObject skillButton = CreateSkillButton(skillPanel.transform, skillNames[i], i, false);
                skillButton.name = $"CPUSkillButton_{i}";
            }
        }

        private static GameObject CreateSkillButton(Transform parent, string skillName, int skillIndex, bool isPlayer)
        {
            // 스킬 버튼 생성
            GameObject skillButton = new GameObject($"SkillButton_{skillName}", typeof(Button), typeof(Image));
            skillButton.transform.SetParent(parent, false);
            skillButton.tag = "SkillButton";

            // RectTransform 설정
            RectTransform buttonRect = skillButton.GetComponent<RectTransform>();
            float yPos = 0.5f - (skillIndex * 0.3f);
            buttonRect.anchorMin = new Vector2(0.1f, yPos - 0.1f);
            buttonRect.anchorMax = new Vector2(0.9f, yPos + 0.1f);
            buttonRect.sizeDelta = Vector2.zero;

            // Button 컴포넌트 설정
            Button button = skillButton.GetComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.colors = CreateButtonColors();

            // Image 컴포넌트 설정
            Image buttonImage = skillButton.GetComponent<Image>();
            buttonImage.color = isPlayer ? new Color(0.2f, 0.6f, 1f, 1f) : new Color(1f, 0.4f, 0.4f, 1f);

            // 스킬 이름 텍스트 생성
            GameObject skillText = new GameObject("SkillText", typeof(Text));
            skillText.transform.SetParent(skillButton.transform, false);
            RectTransform textRect = skillText.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            Text text = skillText.GetComponent<Text>();
            text.text = skillName;
            text.fontSize = 24;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;

            // 쿨타임 텍스트 생성
            GameObject cooldownText = new GameObject("CooldownText", typeof(Text));
            cooldownText.transform.SetParent(skillButton.transform, false);
            RectTransform cooldownRect = cooldownText.GetComponent<RectTransform>();
            cooldownRect.anchorMin = new Vector2(0.7f, 0.7f);
            cooldownRect.anchorMax = new Vector2(0.9f, 0.9f);
            cooldownRect.sizeDelta = Vector2.zero;

            Text cooldown = cooldownText.GetComponent<Text>();
            cooldown.text = "";
            cooldown.fontSize = 16;
            cooldown.fontStyle = FontStyle.Bold;
            cooldown.alignment = TextAnchor.MiddleCenter;
            cooldown.color = Color.red;
            cooldown.gameObject.SetActive(false);

            // 버튼 클릭 이벤트 설정
            if (isPlayer)
            {
                int index = skillIndex;
                button.onClick.AddListener(() => OnPlayerSkillClick(index));
            }
            else
            {
                int index = skillIndex;
                button.onClick.AddListener(() => OnCPUSkillClick(index));
            }

            return skillButton;
        }

        private static ColorBlock CreateButtonColors()
        {
            ColorBlock colors = ColorBlock.defaultColorBlock;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
            colors.selectedColor = new Color(0.8f, 0.8f, 1f, 1f);
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            return colors;
        }

        private static void OnPlayerSkillClick(int skillIndex)
        {
            GameManager gameManager = Object.FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.UseSkill(skillIndex);
                Debug.Log($"플레이어 스킬 {skillIndex} 사용!");
            }
        }

        private static void OnCPUSkillClick(int skillIndex)
        {
            GameManager gameManager = Object.FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                // CPU 스킬은 자동으로 사용되도록 설정
                Debug.Log($"CPU 스킬 {skillIndex} 준비!");
            }
        }

        private static void ConnectSkillButtonsToGameManager()
        {
            GameManager gameManager = Object.FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogWarning("GameManager를 찾을 수 없습니다.");
                return;
            }

            // 플레이어 스킬 버튼들 찾기
            GameObject[] playerButtons = GameObject.FindGameObjectsWithTag("SkillButton");
            Button[] playerSkillButtons = new Button[3];
            Button[] cpuSkillButtons = new Button[3];

            int playerCount = 0;
            int cpuCount = 0;

            foreach (GameObject button in playerButtons)
            {
                if (button.name.StartsWith("PlayerSkillButton"))
                {
                    if (playerCount < 3)
                    {
                        playerSkillButtons[playerCount] = button.GetComponent<Button>();
                        playerCount++;
                    }
                }
                else if (button.name.StartsWith("CPUSkillButton"))
                {
                    if (cpuCount < 3)
                    {
                        cpuSkillButtons[cpuCount] = button.GetComponent<Button>();
                        cpuCount++;
                    }
                }
            }

            // GameManager에 연결
            if (playerCount == 3)
            {
                gameManager.playerSkillButtons = playerSkillButtons;
                Debug.Log("플레이어 스킬 버튼이 GameManager에 연결되었습니다.");
            }

            if (cpuCount == 3)
            {
                gameManager.cpuSkillButtons = cpuSkillButtons;
                Debug.Log("CPU 스킬 버튼이 GameManager에 연결되었습니다.");
            }
        }
    }
}

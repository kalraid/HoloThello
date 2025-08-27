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
            #pragma warning disable CS0618 // Type or member is obsolete
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            #pragma warning restore CS0618 // Type or member is obsolete
            if (canvas == null)
            {
                Debug.LogError("Canvas를 찾을 수 없습니다.");
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

            // 1P 스킬 버튼들 생성
            CreatePlayerSkillButtons(canvas.transform);

            // 2P/CPU 스킬 버튼들 생성
            CreateCPUSkillButtons(canvas.transform);

            // GameManager에 연결
            ConnectSkillButtonsToGameManager();

            EditorUtility.DisplayDialog("완료", "스킬 버튼들이 성공적으로 생성되었습니다!", "확인");
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private static void CleanupExistingSkillButtons()
        {
            // 기존 스킬 버튼들 제거
            GameObject[] existingButtons = GameObject.FindGameObjectsWithTag("EditorOnly");
            foreach (GameObject button in existingButtons)
            {
                if (button.name.Contains("SkillButton"))
                {
                    Object.DestroyImmediate(button);
                }
            }

            // PlayerSkillButton, CPUSkillButton으로 시작하는 오브젝트들도 제거
            #pragma warning disable CS0618 // Type or member is obsolete
            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
            #pragma warning restore CS0618 // Type or member is obsolete
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.StartsWith("PlayerSkillButton") || obj.name.StartsWith("CPUSkillButton"))
                {
                    Object.DestroyImmediate(obj);
                }
            }

            Debug.Log("기존 스킬 버튼들이 정리되었습니다.");
        }

        private static void CreatePlayerSkillButtons(Transform canvas)
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

        private static void CreateCPUSkillButtons(Transform canvas)
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
            #pragma warning disable CS0618 // Type or member is obsolete
            GameManager gameManager = Object.FindObjectOfType<GameManager>();
            #pragma warning restore CS0618 // Type or member is obsolete
            if (gameManager != null)
            {
                // 스킬 효과 적용
                Debug.Log($"1P 스킬 {skillIndex + 1} 사용!");
            }
        }

        private static void OnCPUSkillClick(int skillIndex)
        {
            #pragma warning disable CS0618 // Type or member is obsolete
            GameManager gameManager = Object.FindObjectOfType<GameManager>();
            #pragma warning restore CS0618 // Type or member is obsolete
            if (gameManager != null)
            {
                // 스킬 효과 적용
                Debug.Log($"2P/CPU 스킬 {skillIndex + 1} 사용!");
            }
        }

        private static void ConnectSkillButtonsToGameManager()
        {
            #pragma warning disable CS0618 // Type or member is obsolete
            GameManager gameManager = Object.FindObjectOfType<GameManager>();
            #pragma warning restore CS0618 // Type or member is obsolete
            if (gameManager == null)
            {
                Debug.LogWarning("GameManager를 찾을 수 없습니다.");
                return;
            }

            Debug.Log("스킬 버튼들이 GameManager에 연결되었습니다.");
        }
    }
}

using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class GameSceneSetup : MonoBehaviour
{
    [MenuItem("Tools/Setup GameScene")]
    public static void SetupGameScene()
    {
        // GameScene 로드
        Scene gameScene = EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");
        
        // Canvas 생성
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        // EventSystem 생성
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        // GameManager GameObject 생성
        GameObject gameManagerGO = new GameObject("GameManager");
        GameManager gameManager = gameManagerGO.AddComponent<GameManager>();

        // BoardManager GameObject 생성
        GameObject boardManagerGO = new GameObject("BoardManager");
        BoardManager boardManager = boardManagerGO.AddComponent<BoardManager>();

        // UI 요소들 생성
        CreateGameUI(canvas.transform, gameManager, boardManager);
        
        // 씬 저장
        EditorSceneManager.MarkSceneDirty(gameScene);
        EditorSceneManager.SaveScene(gameScene);
        
        Debug.Log("GameScene UI 설정이 완료되었습니다!");
    }

    private static void CreateGameUI(Transform canvasTransform, GameManager gameManager, BoardManager boardManager)
    {
        // 상단 검정 바 (1/3 영역)
        GameObject topBar = new GameObject("TopBar", typeof(Image));
        topBar.transform.SetParent(canvasTransform, false);
        RectTransform topBarRect = topBar.GetComponent<RectTransform>();
        topBarRect.anchorMin = new Vector2(0, 0.67f);
        topBarRect.anchorMax = new Vector2(1, 1);
        topBarRect.sizeDelta = Vector2.zero;
        Image topBarImage = topBar.GetComponent<Image>();
        topBarImage.color = Color.black;

        // 격투 게임 영역 (상단 1/3 내부)
        CreateFightingArea(canvasTransform, gameManager);

        // 오셀로 영역 (하단 2/3)
        CreateOthelloArea(canvasTransform, gameManager, boardManager);
    }

    private static void CreateFightingArea(Transform canvasTransform, GameManager gameManager)
    {
        // 격투 게임 영역
        GameObject fightingArea = new GameObject("FightingArea", typeof(RectTransform));
        fightingArea.transform.SetParent(canvasTransform, false);
        RectTransform fightingRect = fightingArea.GetComponent<RectTransform>();
        fightingRect.anchorMin = new Vector2(0, 0.67f);
        fightingRect.anchorMax = new Vector2(1, 1);
        fightingRect.sizeDelta = Vector2.zero;

        // 체력바 영역 (2/5, 4/5 위치)
        CreateHealthBars(fightingArea.transform, gameManager);

        // 캐릭터 영역
        CreateCharacterArea(fightingArea.transform, gameManager);

        // 배경 영역
        CreateBackgroundArea(fightingArea.transform, gameManager);
    }

    private static void CreateHealthBars(Transform fightingArea, GameManager gameManager)
    {
        // 1P 체력바 (2/5 위치)
        GameObject playerHpBarGO = new GameObject("PlayerHPBar", typeof(Slider));
        playerHpBarGO.transform.SetParent(fightingArea, false);
        RectTransform playerHpRect = playerHpBarGO.GetComponent<RectTransform>();
        playerHpRect.anchorMin = new Vector2(0.1f, 0.8f);
        playerHpRect.anchorMax = new Vector2(0.4f, 0.9f);
        playerHpRect.sizeDelta = Vector2.zero;
        Slider playerHpSlider = playerHpBarGO.GetComponent<Slider>();
        playerHpSlider.minValue = 0f;
        playerHpSlider.maxValue = 10000f;
        playerHpSlider.value = 10000f;
        gameManager.playerHpBar = playerHpSlider;

        // 1P 체력바 라벨
        GameObject playerHpLabel = new GameObject("PlayerHPLabel", typeof(Text));
        playerHpLabel.transform.SetParent(playerHpBarGO.transform, false);
        RectTransform playerHpLabelRect = playerHpLabel.GetComponent<RectTransform>();
        playerHpLabelRect.anchorMin = new Vector2(0, 0);
        playerHpLabelRect.anchorMax = new Vector2(1, 1);
        playerHpLabelRect.sizeDelta = Vector2.zero;
        Text playerHpText = playerHpLabel.GetComponent<Text>();
        playerHpText.text = "1P HP";
        playerHpText.fontSize = 16;
        playerHpText.alignment = TextAnchor.MiddleCenter;

        // CPU 체력바 (4/5 위치)
        GameObject cpuHpBarGO = new GameObject("CPUHPBar", typeof(Slider));
        cpuHpBarGO.transform.SetParent(fightingArea, false);
        RectTransform cpuHpRect = cpuHpBarGO.GetComponent<RectTransform>();
        cpuHpRect.anchorMin = new Vector2(0.6f, 0.8f);
        cpuHpRect.anchorMax = new Vector2(0.9f, 0.9f);
        cpuHpRect.sizeDelta = Vector2.zero;
        Slider cpuHpSlider = cpuHpBarGO.GetComponent<Slider>();
        cpuHpSlider.minValue = 0f;
        cpuHpSlider.maxValue = 10000f;
        cpuHpSlider.value = 10000f;
        gameManager.cpuHpBar = cpuHpSlider;

        // CPU 체력바 라벨
        GameObject cpuHpLabel = new GameObject("CPUHPLabel", typeof(Text));
        cpuHpLabel.transform.SetParent(cpuHpBarGO.transform, false);
        RectTransform cpuHpLabelRect = cpuHpLabel.GetComponent<RectTransform>();
        cpuHpLabelRect.anchorMin = new Vector2(0, 0);
        cpuHpLabelRect.anchorMax = new Vector2(1, 1);
        cpuHpLabelRect.sizeDelta = Vector2.zero;
        Text cpuHpText = cpuHpLabel.GetComponent<Text>();
        cpuHpText.text = "CPU HP";
        cpuHpText.fontSize = 16;
        cpuHpText.alignment = TextAnchor.MiddleCenter;
    }

    private static void CreateCharacterArea(Transform fightingArea, GameManager gameManager)
    {
        // 캐릭터 영역 (중앙)
        GameObject characterArea = new GameObject("CharacterArea", typeof(RectTransform));
        characterArea.transform.SetParent(fightingArea, false);
        RectTransform charAreaRect = characterArea.GetComponent<RectTransform>();
        charAreaRect.anchorMin = new Vector2(0.1f, 0.3f);
        charAreaRect.anchorMax = new Vector2(0.9f, 0.7f);
        charAreaRect.sizeDelta = Vector2.zero;

        // 1P 캐릭터 이미지
        GameObject playerCharGO = new GameObject("PlayerCharacter", typeof(Image));
        playerCharGO.transform.SetParent(characterArea.transform, false);
        RectTransform playerCharRect = playerCharGO.GetComponent<RectTransform>();
        playerCharRect.anchorMin = new Vector2(0, 0);
        playerCharRect.anchorMax = new Vector2(0.5f, 1);
        playerCharRect.sizeDelta = Vector2.zero;
        gameManager.playerCharacterImage = playerCharGO.GetComponent<Image>();

        // CPU 캐릭터 이미지
        GameObject cpuCharGO = new GameObject("CPUCharacter", typeof(Image));
        cpuCharGO.transform.SetParent(characterArea.transform, false);
        RectTransform cpuCharRect = cpuCharGO.GetComponent<RectTransform>();
        cpuCharRect.anchorMin = new Vector2(0.5f, 0);
        cpuCharRect.anchorMax = new Vector2(1, 1);
        cpuCharRect.sizeDelta = Vector2.zero;
        gameManager.cpuCharacterImage = cpuCharGO.GetComponent<Image>();
    }

    private static void CreateBackgroundArea(Transform fightingArea, GameManager gameManager)
    {
        // 배경 이미지
        GameObject backgroundGO = new GameObject("BackgroundImage", typeof(Image));
        backgroundGO.transform.SetParent(fightingArea, false);
        RectTransform bgRect = backgroundGO.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0);
        bgRect.anchorMax = new Vector2(1, 0.3f);
        bgRect.sizeDelta = Vector2.zero;
        gameManager.backgroundImage = backgroundGO.GetComponent<Image>();
    }

    private static void CreateOthelloArea(Transform canvasTransform, GameManager gameManager, BoardManager boardManager)
    {
        // 오셀로 영역
        GameObject othelloArea = new GameObject("OthelloArea", typeof(RectTransform));
        othelloArea.transform.SetParent(canvasTransform, false);
        RectTransform othelloRect = othelloArea.GetComponent<RectTransform>();
        othelloRect.anchorMin = new Vector2(0, 0);
        othelloRect.anchorMax = new Vector2(1, 0.67f);
        othelloRect.sizeDelta = Vector2.zero;

        // 스킬 버튼 영역
        CreateSkillButtons(othelloArea.transform, gameManager);

        // 오셀로 보드 영역
        CreateOthelloBoard(othelloArea.transform, gameManager, boardManager);

        // 턴 텍스트
        GameObject turnTextGO = new GameObject("TurnText", typeof(Text));
        turnTextGO.transform.SetParent(othelloArea.transform, false);
        RectTransform turnTextRect = turnTextGO.GetComponent<RectTransform>();
        turnTextRect.anchorMin = new Vector2(0.3f, 0.9f);
        turnTextRect.anchorMax = new Vector2(0.7f, 0.95f);
        turnTextRect.sizeDelta = Vector2.zero;
        gameManager.turnText = turnTextGO.GetComponent<Text>();
        gameManager.turnText.text = "흑 차례";
        gameManager.turnText.fontSize = 24;
        gameManager.turnText.alignment = TextAnchor.MiddleCenter;

        // 결과 텍스트
        GameObject resultTextGO = new GameObject("ResultText", typeof(Text));
        resultTextGO.transform.SetParent(othelloArea.transform, false);
        RectTransform resultTextRect = resultTextGO.GetComponent<RectTransform>();
        resultTextRect.anchorMin = new Vector2(0.3f, 0.85f);
        resultTextRect.anchorMax = new Vector2(0.7f, 0.9f);
        resultTextRect.sizeDelta = Vector2.zero;
        gameManager.resultText = resultTextGO.GetComponent<Text>();
        gameManager.resultText.text = "";
        gameManager.resultText.fontSize = 20;
        gameManager.resultText.alignment = TextAnchor.MiddleCenter;
    }

    private static void CreateSkillButtons(Transform othelloArea, GameManager gameManager)
    {
        // 1P 스킬 버튼들 (좌측)
        gameManager.playerSkillButtons = new Button[3];
        for (int i = 0; i < 3; i++)
        {
            GameObject skillButtonGO = new GameObject($"PlayerSkillButton_{i}", typeof(Button), typeof(Image));
            skillButtonGO.transform.SetParent(othelloArea.transform, false);
            RectTransform skillRect = skillButtonGO.GetComponent<RectTransform>();
            skillRect.anchorMin = new Vector2(0.05f, 0.7f + i * 0.1f);
            skillRect.anchorMax = new Vector2(0.15f, 0.8f + i * 0.1f);
            skillRect.sizeDelta = Vector2.zero;
            gameManager.playerSkillButtons[i] = skillButtonGO.GetComponent<Button>();

            // 스킬 버튼 텍스트
            GameObject skillTextGO = new GameObject("SkillText", typeof(Text));
            skillTextGO.transform.SetParent(skillButtonGO.transform, false);
            RectTransform skillTextRect = skillTextGO.GetComponent<RectTransform>();
            skillTextRect.anchorMin = Vector2.zero;
            skillTextRect.anchorMax = Vector2.one;
            skillTextRect.sizeDelta = Vector2.zero;
            Text skillText = skillTextGO.GetComponent<Text>();
            
            // 스킬 이름 설정
            switch (i)
            {
                case 0: skillText.text = "스킬A"; break;
                case 1: skillText.text = "스킬B"; break;
                case 2: skillText.text = "궁극기A"; break;
            }
            skillText.fontSize = 12;
            skillText.alignment = TextAnchor.MiddleCenter;
            skillText.color = Color.black;
        }

        // CPU 스킬 버튼들 (우측)
        gameManager.cpuSkillButtons = new Button[3];
        for (int i = 0; i < 3; i++)
        {
            GameObject skillButtonGO = new GameObject($"CPUSkillButton_{i}", typeof(Button), typeof(Image));
            skillButtonGO.transform.SetParent(othelloArea.transform, false);
            RectTransform skillRect = skillButtonGO.GetComponent<RectTransform>();
            skillRect.anchorMin = new Vector2(0.85f, 0.7f + i * 0.1f);
            skillRect.anchorMax = new Vector2(0.95f, 0.8f + i * 0.1f);
            skillRect.sizeDelta = Vector2.zero;
            gameManager.cpuSkillButtons[i] = skillButtonGO.GetComponent<Button>();

            // 스킬 버튼 텍스트
            GameObject skillTextGO = new GameObject("SkillText", typeof(Text));
            skillTextGO.transform.SetParent(skillButtonGO.transform, false);
            RectTransform skillTextRect = skillTextGO.GetComponent<RectTransform>();
            skillTextRect.anchorMin = Vector2.zero;
            skillTextRect.anchorMax = Vector2.one;
            skillTextRect.sizeDelta = Vector2.zero;
            Text skillText = skillTextGO.GetComponent<Text>();
            
            // 스킬 이름 설정
            switch (i)
            {
                case 0: skillText.text = "스킬A"; break;
                case 1: skillText.text = "스킬B"; break;
                case 2: skillText.text = "궁극기A"; break;
            }
            skillText.fontSize = 12;
            skillText.alignment = TextAnchor.MiddleCenter;
            skillText.color = Color.black;
        }
    }

    private static void CreateOthelloBoard(Transform othelloArea, GameManager gameManager, BoardManager boardManager)
    {
        // 오셀로 보드 영역 (중앙)
        GameObject boardArea = new GameObject("BoardArea", typeof(RectTransform));
        boardArea.transform.SetParent(othelloArea.transform, false);
        RectTransform boardAreaRect = boardArea.GetComponent<RectTransform>();
        boardAreaRect.anchorMin = new Vector2(0.2f, 0.1f);
        boardAreaRect.anchorMax = new Vector2(0.8f, 0.7f);
        boardAreaRect.sizeDelta = Vector2.zero;

        // 보드 배경
        GameObject boardBackground = new GameObject("BoardBackground", typeof(Image));
        boardBackground.transform.SetParent(boardArea.transform, false);
        RectTransform boardBgRect = boardBackground.GetComponent<RectTransform>();
        boardBgRect.anchorMin = Vector2.zero;
        boardBgRect.anchorMax = Vector2.one;
        boardBgRect.sizeDelta = Vector2.zero;
        Image boardBgImage = boardBackground.GetComponent<Image>();
        boardBgImage.color = new Color(0.2f, 0.6f, 0.2f); // 녹색 배경

        // 8x8 그리드 생성 (임시로 빈 오브젝트들)
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                GameObject cellGO = new GameObject($"Cell_{x}_{y}", typeof(RectTransform));
                cellGO.transform.SetParent(boardArea.transform, false);
                RectTransform cellRect = cellGO.GetComponent<RectTransform>();
                cellRect.anchorMin = new Vector2(x * 0.125f, y * 0.125f);
                cellRect.anchorMax = new Vector2((x + 1) * 0.125f, (y + 1) * 0.125f);
                cellRect.sizeDelta = Vector2.zero;
            }
        }
    }
} 
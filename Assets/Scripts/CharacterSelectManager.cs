using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CharacterSelectManager : MonoBehaviour
{
    public Image[] characterBarImages; // 캐릭터 바(작은 사진) UI
    public Image fullCharacterImage;   // 선택된 캐릭터 풀사진
    public Sprite[] characterSprites;  // 캐릭터 스프라이트(작은/큰 동일 순서)
    public Image[] backgroundBarImages; // 배경 선택 바 UI
    public Sprite[] backgroundSprites;  // 배경 스프라이트
    public Image selectedBackgroundImage; // 선택된 배경 미리보기
    public GameObject cpuBlockOverlay; // CPU가 선택 불가한 캐릭터에 덮을 오브젝트(회색 처리)
    public Text selectLabel; // '1P 선택', 'CPU 선택', '배경 선택' 등 안내

    [Header("캐릭터 타입 선택")]
    public Button typeAButton;
    public Button typeBButton;
    public Text typeLabel;
    
    [Header("게임 모드 선택")]
    public Button playerVsCPUButton;
    public Button playerVsPlayerButton;
    public Button cpuVsCPUButton;
    public Text gameModeLabel;
    
    [Header("2P 캐릭터 선택")]
    public GameObject player2SelectionPanel;
    public Image[] player2CharacterBarImages;
    public Image player2FullCharacterImage;
    public Text player2SelectLabel;
    
    private CharacterType currentCharacterType = CharacterType.TypeA;
    private CharacterData[] currentTypeCharacters;
    private GameMode currentGameMode = GameMode.PlayerVsCPU;
    
    // --- 상태 관리 ---
    private enum SelectionState { Player1, Player2, CPU, Background, Done }
    private SelectionState currentState;
    
    // selectStep 변수는 더 이상 사용되지 않으므로 삭제
    private int selected1P = -1;
    private int selectedCPU = -1;
    private int selectedBackground = 0;

    #region Unity Lifecycle Methods

    void Start()
    {
        if (!ValidateComponents())
        {
            enabled = false;
            return;
        }

        InitializeState();
        InitializeListeners();
        UpdateAllUI();
    }
    
    // 키보드 입력은 Update에서 계속 처리
    void Update()
    {
        HandleKeyboardInput();
    }

    #endregion

    #region Initialization

    bool ValidateComponents()
    {
        // 주요 UI 컴포넌트들이 연결되었는지 확인
        if (fullCharacterImage == null || selectLabel == null || typeAButton == null || playerVsCPUButton == null)
        {
            Debug.LogError("CharacterSelectManager의 필수 UI 컴포넌트가 연결되지 않았습니다!");
            return false;
        }
        if (GameData.Instance == null)
        {
            Debug.LogError("GameData.Instance가 없습니다. 씬에 GameData 오브젝트를 추가하세요.");
            return false;
        }
        if (CharacterDataManager.Instance == null)
        {
            Debug.LogError("CharacterDataManager.Instance가 없습니다. 씬에 CharacterDataManager 오브젝트를 추가하세요.");
            return false;
        }
        return true;
    }

    void InitializeState()
    {
        currentCharacterType = GameData.Instance.GetCharacterType();
        currentGameMode = GameData.Instance.GetGameMode();
        LoadCharactersByType(currentCharacterType);
        ResetSelection();
    }
    
    void InitializeListeners()
    {
        // 버튼 이벤트 리스너 설정 (중복 방지)
        typeAButton.onClick.RemoveAllListeners();
        typeAButton.onClick.AddListener(OnClickTypeA);
        
        typeBButton.onClick.RemoveAllListeners();
        typeBButton.onClick.AddListener(OnClickTypeB);
        
        playerVsCPUButton.onClick.RemoveAllListeners();
        playerVsCPUButton.onClick.AddListener(OnClickPlayerVsCPU);
        
        playerVsPlayerButton.onClick.RemoveAllListeners();
        playerVsPlayerButton.onClick.AddListener(OnClickPlayerVsPlayer);
        
        cpuVsCPUButton.onClick.RemoveAllListeners();
        cpuVsCPUButton.onClick.AddListener(OnClickCPUVsCPU);

        // 캐릭터/배경 버튼 리스너는 동적으로 생성되므로 UI 업데이트 시 처리
    }
    
    #endregion

    #region State Machine

    void ChangeState(SelectionState newState)
    {
        currentState = newState;
        UpdateAllUI();
    }

    void ResetSelection()
    {
        selected1P = -1;
        selectedCPU = -1;
        selectedBackground = 0;
        
        // 게임 모드에 따라 초기 상태 결정
        if (currentGameMode == GameMode.CPUVsCPU)
        {
            // CPU vs CPU 모드는 바로 배경 선택으로 (또는 자동 선택 로직 추가)
            // 예시: 자동으로 캐릭터 선택 후 배경 선택으로
            selected1P = Random.Range(0, 10);
            selectedCPU = (selected1P + 1) % 10;
            ChangeState(SelectionState.Background);
        }
        else
        {
            ChangeState(SelectionState.Player1);
        }
    }

    #endregion

    #region UI Update

    void UpdateAllUI()
    {
        UpdateTypeUI();
        UpdateGameModeUI();
        UpdateCharacterSelectionUI();
        UpdateBackgroundSelectionUI();
        UpdateStateSpecificUI();
    }

    void LoadCharactersByType(CharacterType type)
    {
        // CharacterDataManager 인스턴스 확인 및 초기화
        if (CharacterDataManager.Instance == null)
        {
            Debug.LogWarning("CharacterDataManager.Instance가 null입니다. 초기화를 시도합니다.");
            
            // CharacterDataManager 찾기
            CharacterDataManager cdm = FindFirstObjectByType<CharacterDataManager>();
            if (cdm == null)
            {
                // CharacterDataManager가 없으면 생성
                GameObject cdmGO = new GameObject("CharacterDataManager");
                cdm = cdmGO.AddComponent<CharacterDataManager>();
                Debug.Log("CharacterDataManager를 새로 생성했습니다.");
            }
            
            // 다시 확인
            if (CharacterDataManager.Instance == null)
            {
                Debug.LogError("CharacterDataManager 초기화에 실패했습니다!");
                return;
            }
        }
        
        currentTypeCharacters = CharacterDataManager.Instance.GetCharactersByType(type);
        currentCharacterType = type;
        
        // 캐릭터 데이터 유효성 검사
        if (currentTypeCharacters == null || currentTypeCharacters.Length == 0)
        {
            Debug.LogWarning($"타입 {type}의 캐릭터 데이터가 없습니다. 기본 캐릭터를 생성합니다.");
            CreateDefaultCharacters(type);
        }
        
        // 선택 초기화
        selected1P = -1;
        selectedCPU = -1;
        
        UpdateAllUI();
    }
    
    /// <summary>
    /// 기본 캐릭터 데이터 생성 (CharacterDataManager가 없을 때)
    /// </summary>
    private void CreateDefaultCharacters(CharacterType type)
    {
        int characterCount = 10; // 기본 10개 캐릭터
        currentTypeCharacters = new CharacterData[characterCount];
        
        for (int i = 0; i < characterCount; i++)
        {
            currentTypeCharacters[i] = new CharacterData
            {
                id = i,
                name = type == CharacterType.TypeA ? $"Hololive_{i + 1}" : $"Cat_{i + 1}",
                type = type,
                sprite = characterSprites != null && i < characterSprites.Length ? characterSprites[i] : null,
                color = GetDefaultCharacterColor(i)
            };
        }
        
        Debug.Log($"{type} 타입의 기본 캐릭터 {characterCount}개를 생성했습니다.");
    }
    
    /// <summary>
    /// 기본 캐릭터 색상 반환
    /// </summary>
    private Color GetDefaultCharacterColor(int index)
    {
        Color[] defaultColors = {
            Color.red, Color.blue, Color.green, Color.yellow, Color.magenta,
            Color.cyan, Color.white, Color.gray, Color.black, Color.orange
        };
        
        return index < defaultColors.Length ? defaultColors[index] : Color.white;
    }
    
    public void OnClickTypeA()
    {
        LoadCharactersByType(CharacterType.TypeA);
        GameData.Instance.SetCharacterType(CharacterType.TypeA);
        UpdateTypeUI();
    }
    
    public void OnClickTypeB()
    {
        LoadCharactersByType(CharacterType.TypeB);
        GameData.Instance.SetCharacterType(CharacterType.TypeB);
        UpdateTypeUI();
    }
    
    void UpdateTypeUI()
    {
        if (typeLabel != null)
        {
            typeLabel.text = currentCharacterType == CharacterType.TypeA ? "Hololive 타입" : "고양이 타입";
        }
        
        if (typeAButton != null)
        {
            typeAButton.interactable = currentCharacterType != CharacterType.TypeA;
        }
        
        if (typeBButton != null)
        {
            typeBButton.interactable = currentCharacterType != CharacterType.TypeB;
        }
    }
    
    public void OnClickPlayerVsCPU()
    {
        currentGameMode = GameMode.PlayerVsCPU;
        GameData.Instance.SetGameMode(GameMode.PlayerVsCPU);
        UpdateGameModeUI();
        ResetSelection();
    }
    
    public void OnClickPlayerVsPlayer()
    {
        currentGameMode = GameMode.PlayerVsPlayer;
        GameData.Instance.SetGameMode(GameMode.PlayerVsPlayer);
        UpdateGameModeUI();
        ResetSelection();
    }
    
    public void OnClickCPUVsCPU()
    {
        currentGameMode = GameMode.CPUVsCPU;
        GameData.Instance.SetGameMode(GameMode.CPUVsCPU);
        UpdateGameModeUI();
        ResetSelection();
    }
    
    void UpdateGameModeUI()
    {
        if (gameModeLabel != null)
        {
            switch (currentGameMode)
            {
                case GameMode.PlayerVsCPU:
                    gameModeLabel.text = "1P vs CPU";
                    break;
                case GameMode.PlayerVsPlayer:
                    gameModeLabel.text = "1P vs 2P";
                    break;
                case GameMode.CPUVsCPU:
                    gameModeLabel.text = "CPU vs CPU";
                    break;
            }
        }
        
        if (playerVsCPUButton != null)
        {
            playerVsCPUButton.interactable = currentGameMode != GameMode.PlayerVsCPU;
        }
        
        if (playerVsPlayerButton != null)
        {
            playerVsPlayerButton.interactable = currentGameMode != GameMode.PlayerVsPlayer;
        }
        
        if (cpuVsCPUButton != null)
        {
            cpuVsCPUButton.interactable = currentGameMode != GameMode.CPUVsCPU;
        }
    }
    
    void UpdateCharacterSelectionUI()
    {
        // 1P, 2P 캐릭터 바 공통 로직
        UpdateCharacterBars(characterBarImages, selected1P, selectedCPU);
        if (player2CharacterBarImages != null)
        {
            UpdateCharacterBars(player2CharacterBarImages, selected1P, selectedCPU);
        }

        // 풀 이미지 업데이트
        int focusedPlayer = (currentState == SelectionState.Player1) ? selected1P : selectedCPU;
        UpdateFullCharacterImage(fullCharacterImage, focusedPlayer);
        if (player2FullCharacterImage != null)
        {
            UpdateFullCharacterImage(player2FullCharacterImage, selectedCPU);
        }
    }

    void UpdateCharacterBars(Image[] bars, int p1, int p2)
    {
        if (bars == null) return;
        for (int i = 0; i < bars.Length && i < 10; i++)
        {
            // 캐릭터 스프라이트 설정
            if (currentTypeCharacters != null && i < currentTypeCharacters.Length && currentTypeCharacters[i] != null)
            {
                bars[i].sprite = currentTypeCharacters[i].characterSprite;
            }
            
            // 선택 상태에 따른 색상 변경
            Color color = Color.white;
            if (i == p1) color = (currentState == SelectionState.Player1) ? Color.yellow : Color.gray;
            if (i == p2) color = (currentState == SelectionState.Player2 || currentState == SelectionState.CPU) ? Color.cyan : Color.white;
            bars[i].color = color;
        }
    }

    void UpdateFullCharacterImage(Image image, int selectedIndex)
    {
        if (image == null || currentTypeCharacters == null || selectedIndex < 0 || selectedIndex >= currentTypeCharacters.Length) return;
        
        CharacterData selectedChar = currentTypeCharacters[selectedIndex];
        if (selectedChar != null)
        {
            image.sprite = selectedChar.characterSprite;
            image.color = Color.white;
        }
    }

    void UpdateBackgroundSelectionUI()
    {
        if (backgroundBarImages == null || backgroundSprites == null) return;
        for (int i = 0; i < backgroundBarImages.Length; i++)
        {
            if (i < backgroundSprites.Length)
            {
                backgroundBarImages[i].sprite = backgroundSprites[i];
                backgroundBarImages[i].color = (i == selectedBackground) ? Color.yellow : Color.white;
            }
        }
        if (selectedBackgroundImage != null && selectedBackground < backgroundSprites.Length)
        {
            selectedBackgroundImage.sprite = backgroundSprites[selectedBackground];
        }
    }
    
    void UpdateStateSpecificUI()
    {
        if (selectLabel == null) return;
        
        switch (currentState)
        {
            case SelectionState.Player1:
                selectLabel.text = "1P 캐릭터를 선택하세요";
                if(player2SelectionPanel != null) player2SelectionPanel.SetActive(false);
                break;
            case SelectionState.Player2:
                selectLabel.text = "2P 캐릭터를 선택하세요";
                if(player2SelectionPanel != null) player2SelectionPanel.SetActive(true);
                break;
            case SelectionState.CPU:
                selectLabel.text = "CPU 캐릭터를 선택하세요";
                if(player2SelectionPanel != null) player2SelectionPanel.SetActive(false);
                break;
            case SelectionState.Background:
                selectLabel.text = "배경을 선택하세요";
                if(player2SelectionPanel != null) player2SelectionPanel.SetActive(false);
                break;
        }
    }

    #endregion

    #region Input Handlers

    private void HandleKeyboardInput()
    {
        // 키보드 좌우 이동/선택 처리 (예시: 좌우 화살표, Z키)
        if (currentState < SelectionState.Background)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                MoveSelection(-1);
            if (Input.GetKeyDown(KeyCode.RightArrow))
                MoveSelection(1);
            if (Input.GetKeyDown(KeyCode.Z))
                ConfirmSelection();
        }
        else if (currentState == SelectionState.Background)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                MoveBackground(-1);
            if (Input.GetKeyDown(KeyCode.RightArrow))
                MoveBackground(1);
            if (Input.GetKeyDown(KeyCode.Z))
                ConfirmBackground();
        }
    }

    void MoveSelection(int dir)
    {
        int max = 10; // 10개 캐릭터
        if (currentState == SelectionState.Player1)
        {
            if (selected1P == -1) selected1P = 0;
            else selected1P = (selected1P + dir + max) % max;
            if (selected1P == selectedCPU) selected1P = (selected1P + dir + max) % max;
        }
        else if (currentState == SelectionState.CPU)
        {
            if (selectedCPU == -1) selectedCPU = 0;
            else selectedCPU = (selectedCPU + dir + max) % max;
            if (selectedCPU == selected1P) selectedCPU = (selectedCPU + dir + max) % max;
        }
        UpdateAllUI();
    }

    void MoveBackground(int dir)
    {
        int max = backgroundSprites.Length;
        selectedBackground = (selectedBackground + dir + max) % max;
        UpdateAllUI();
    }

    void ConfirmSelection()
    {
        if (currentState == SelectionState.Player1 && selected1P != -1)
        {
            ChangeState(SelectionState.CPU);
            // 1P가 고른 캐릭터는 CPU가 선택 불가
            if (selectedCPU == selected1P) selectedCPU = -1;
        }
        else if (currentState == SelectionState.CPU && selectedCPU != -1)
        {
            ChangeState(SelectionState.Background);
        }
        UpdateAllUI();
    }

    void ConfirmBackground()
    {
        // 선택 완료 → 게임 씬으로 이동, 선택 정보 저장
        if (GameData.Instance)
        {
            GameData.Instance.playerCharacterIdx = selected1P;
            GameData.Instance.cpuCharacterIdx = selectedCPU;
            GameData.Instance.backgroundIdx = selectedBackground;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void OnClickCharacter(int index)
    {
        if (index < 0 || index >= 10) return;

        switch (currentState)
        {
            case SelectionState.Player1:
                selected1P = index;
                if (currentGameMode == GameMode.PlayerVsPlayer) ChangeState(SelectionState.Player2);
                else ChangeState(SelectionState.CPU);
                break;
            case SelectionState.Player2:
            case SelectionState.CPU:
                if (index != selected1P)
                {
                    selectedCPU = index;
                    ChangeState(SelectionState.Background);
                }
                break;
        }
    }
    
    public void OnClickBackground(int index)
    {
        selectedBackground = index;
        UpdateAllUI();
    }

    public void OnClickConfirm()
    {
        if (selected1P != -1 && selectedCPU != -1)
        {
            // 데이터 저장
            GameData.Instance.selectedCharacter1P = currentTypeCharacters[selected1P];
            if (currentGameMode == GameMode.PlayerVsPlayer)
                GameData.Instance.selectedCharacter2P = currentTypeCharacters[selectedCPU];
            else
                GameData.Instance.selectedCharacterCPU = currentTypeCharacters[selectedCPU];

            // 씬 이동
            SceneManager.LoadScene("GameScene");
        }
    }

    // 2P 캐릭터 선택 버튼 클릭 시 호출 (ButtonUtilityEditor 등에서 사용)
    public void OnClickPlayer2Character(int characterIndex)
    {
        if (characterIndex < 0 || characterIndex >= 10) return;

        if (characterIndex != selected1P) // 1P와 다른 캐릭터 선택
        {
            selectedCPU = characterIndex;
            UpdateAllUI();
        }
    }

    /// <summary>
    /// 뒤로가기 버튼 클릭
    /// </summary>
    public void OnClickBack()
    {
        Debug.Log("뒤로가기 버튼 클릭됨");
        
        // MainScene으로 이동
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadMainScene();
        }
        else
        {
            // SceneController가 없으면 직접 로드
            SceneManager.LoadScene("MainScene");
        }
    }

    #endregion
} 
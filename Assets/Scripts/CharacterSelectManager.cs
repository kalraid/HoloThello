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
    
    private int selectStep = 0; // 0:1P, 1:CPU, 2:배경
    private int selected1P = -1;
    private int selectedCPU = -1;
    private int selectedBackground = 0;

    void Start()
    {
        // GameData 인스턴스 확인
        if (GameData.Instance == null)
        {
            Debug.LogError("GameData.Instance가 null입니다. GameData 오브젝트가 씬에 있는지 확인하세요.");
            return;
        }
        
        // 캐릭터 타입 설정 로드
        currentCharacterType = GameData.Instance.GetCharacterType();
        
        // 게임 모드 설정 로드
        currentGameMode = GameData.Instance.GetGameMode();
        
        // 현재 타입의 캐릭터들 로드
        LoadCharactersByType(currentCharacterType);
        
        // UI 초기화
        UpdateTypeUI();
        UpdateGameModeUI();
        UpdateUI();
    }

    void Update()
    {
        // 키보드 좌우 이동/선택 처리 (예시: 좌우 화살표, Z키)
        if (selectStep < 2)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                MoveSelection(-1);
            if (Input.GetKeyDown(KeyCode.RightArrow))
                MoveSelection(1);
            if (Input.GetKeyDown(KeyCode.Z))
                ConfirmSelection();
        }
        else if (selectStep == 2)
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
        if (selectStep == 0)
        {
            if (selected1P == -1) selected1P = 0;
            else selected1P = (selected1P + dir + max) % max;
            if (selected1P == selectedCPU) selected1P = (selected1P + dir + max) % max;
        }
        else if (selectStep == 1)
        {
            if (selectedCPU == -1) selectedCPU = 0;
            else selectedCPU = (selectedCPU + dir + max) % max;
            if (selectedCPU == selected1P) selectedCPU = (selectedCPU + dir + max) % max;
        }
        UpdateUI();
    }

    void MoveBackground(int dir)
    {
        int max = backgroundSprites.Length;
        selectedBackground = (selectedBackground + dir + max) % max;
        UpdateUI();
    }

    void ConfirmSelection()
    {
        if (selectStep == 0 && selected1P != -1)
        {
            selectStep = 1;
            // 1P가 고른 캐릭터는 CPU가 선택 불가
            if (selectedCPU == selected1P) selectedCPU = -1;
        }
        else if (selectStep == 1 && selectedCPU != -1)
        {
            selectStep = 2;
        }
        UpdateUI();
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

    void UpdateUI()
    {
        // 캐릭터 바/풀사진/회색 처리/선택 박스 등 UI 갱신
        for (int i = 0; i < characterBarImages.Length && i < 10; i++)
        {
            // 현재 타입의 캐릭터 데이터에서 스프라이트 가져오기
            if (currentTypeCharacters != null && i < currentTypeCharacters.Length)
            {
                CharacterData charData = currentTypeCharacters[i];
                if (charData != null && charData.characterSprite != null)
                {
                    characterBarImages[i].sprite = charData.characterSprite;
                    characterBarImages[i].color = Color.white; // 원래 색상으로 복원
                }
                else
                {
                    // 스프라이트가 없으면 임시 색상 사용
                    characterBarImages[i].color = new Color(
                        Random.Range(0.5f, 1f),
                        Random.Range(0.5f, 1f),
                        Random.Range(0.5f, 1f),
                        1f
                    );
                }
            }
            else
            {
                // 캐릭터 데이터가 없으면 임시 색상 사용
                characterBarImages[i].color = new Color(
                    Random.Range(0.5f, 1f),
                    Random.Range(0.5f, 1f),
                    Random.Range(0.5f, 1f),
                    1f
                );
            }
            
            // 1P/CPU가 선택한 캐릭터는 테두리/색상 등으로 표시
            if (selectStep == 0 && i == selected1P) characterBarImages[i].color = Color.yellow;
            if (selectStep == 1 && i == selectedCPU) characterBarImages[i].color = Color.cyan;
            // CPU가 선택 불가한 캐릭터(1P가 고른 것)는 회색 처리
            if (selectStep == 1 && i == selected1P) characterBarImages[i].color = Color.gray;
        }
        
        // 풀사진 갱신
        if (selectStep == 0 && selected1P != -1 && selected1P < currentTypeCharacters.Length)
        {
            CharacterData selectedChar = currentTypeCharacters[selected1P];
            if (selectedChar != null && selectedChar.characterSprite != null)
            {
                fullCharacterImage.sprite = selectedChar.characterSprite;
                fullCharacterImage.color = Color.white;
            }
        }
        else if (selectStep == 1 && selectedCPU != -1 && selectedCPU < currentTypeCharacters.Length)
        {
            CharacterData selectedChar = currentTypeCharacters[selectedCPU];
            if (selectedChar != null && selectedChar.characterSprite != null)
            {
                fullCharacterImage.sprite = selectedChar.characterSprite;
                fullCharacterImage.color = Color.white;
            }
        }
        
        // 안내 텍스트
        if (selectStep == 0) selectLabel.text = "1P 캐릭터를 선택하세요";
        else if (selectStep == 1) selectLabel.text = "CPU 캐릭터를 선택하세요";
        else selectLabel.text = "배경을 선택하세요";
        
        // 배경 바/미리보기
        for (int i = 0; i < backgroundBarImages.Length; i++)
        {
            backgroundBarImages[i].sprite = backgroundSprites[i];
            backgroundBarImages[i].color = (i == selectedBackground) ? Color.yellow : Color.white;
        }
        selectedBackgroundImage.sprite = backgroundSprites[selectedBackground];
    }
    
    void LoadCharactersByType(CharacterType type)
    {
        if (CharacterDataManager.Instance == null)
        {
            Debug.LogError("CharacterDataManager.Instance가 null입니다!");
            return;
        }
        
        currentTypeCharacters = CharacterDataManager.Instance.GetCharactersByType(type);
        currentCharacterType = type;
        
        // 선택 초기화
        selected1P = -1;
        selectedCPU = -1;
        selectStep = 0;
        
        UpdateUI();
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
    
    void ResetSelection()
    {
        selected1P = -1;
        selectedCPU = -1;
        selectStep = 0;
        UpdateUI();
    }
    
    public void OnClickCharacter(int characterIndex)
    {
        if (characterIndex < 0 || characterIndex >= 10) return;
        
        if (selectStep == 0) // 1P 선택
        {
            selected1P = characterIndex;
            
            if (currentGameMode == GameMode.PlayerVsPlayer)
            {
                // 2P 모드면 2P 캐릭터 선택으로
                selectStep = 1;
                ShowPlayer2Selection();
            }
            else
            {
                // CPU 모드면 CPU 캐릭터 선택으로
                selectStep = 1;
            }
            UpdateUI();
        }
        else if (selectStep == 1) // CPU/2P 선택
        {
            if (currentGameMode == GameMode.PlayerVsPlayer)
            {
                // 2P 모드
                if (characterIndex != selected1P) // 1P와 다른 캐릭터 선택
                {
                    selectedCPU = characterIndex;
                    selectStep = 2; // 배경 선택 단계로
                    HidePlayer2Selection();
                    UpdateUI();
                }
            }
            else
            {
                // CPU 모드
                if (characterIndex != selected1P) // 1P와 다른 캐릭터 선택
                {
                    selectedCPU = characterIndex;
                    selectStep = 2; // 배경 선택 단계로
                    UpdateUI();
                }
            }
        }
    }
    
    void ShowPlayer2Selection()
    {
        if (player2SelectionPanel != null)
        {
            player2SelectionPanel.SetActive(true);
        }
        
        if (player2SelectLabel != null)
        {
            player2SelectLabel.text = "2P 캐릭터를 선택하세요";
        }
        
        UpdatePlayer2UI();
    }
    
    void HidePlayer2Selection()
    {
        if (player2SelectionPanel != null)
        {
            player2SelectionPanel.SetActive(false);
        }
    }
    
    void UpdatePlayer2UI()
    {
        // 2P 캐릭터 바 업데이트
        for (int i = 0; i < player2CharacterBarImages.Length && i < 10; i++)
        {
            if (currentTypeCharacters != null && i < currentTypeCharacters.Length)
            {
                CharacterData charData = currentTypeCharacters[i];
                if (charData != null && charData.characterSprite != null)
                {
                    player2CharacterBarImages[i].sprite = charData.characterSprite;
                    player2CharacterBarImages[i].color = Color.white;
                }
                else
                {
                    player2CharacterBarImages[i].color = new Color(
                        Random.Range(0.5f, 1f),
                        Random.Range(0.5f, 1f),
                        Random.Range(0.5f, 1f),
                        1f
                    );
                }
            }
            
            // 선택된 캐릭터 표시
            if (i == selectedCPU)
            {
                player2CharacterBarImages[i].color = Color.cyan;
            }
            // 1P가 선택한 캐릭터는 회색 처리
            else if (i == selected1P)
            {
                player2CharacterBarImages[i].color = Color.gray;
            }
        }
        
        // 2P 풀사진 업데이트
        if (selectedCPU != -1 && selectedCPU < currentTypeCharacters.Length)
        {
            CharacterData selectedChar = currentTypeCharacters[selectedCPU];
            if (selectedChar != null && selectedChar.characterSprite != null)
            {
                player2FullCharacterImage.sprite = selectedChar.characterSprite;
                player2FullCharacterImage.color = Color.white;
            }
        }
    }
    
    public void OnClickPlayer2Character(int characterIndex)
    {
        if (characterIndex < 0 || characterIndex >= 10) return;
        
        if (characterIndex != selected1P) // 1P와 다른 캐릭터 선택
        {
            selectedCPU = characterIndex;
            UpdatePlayer2UI();
        }
    }
    
    public void OnClickConfirm()
    {
        if (selectStep == 2 && selected1P != -1 && selectedCPU != -1)
        {
            // 선택된 캐릭터 데이터 저장
            if (currentTypeCharacters != null && selected1P < currentTypeCharacters.Length && selectedCPU < currentTypeCharacters.Length)
            {
                GameData.Instance.selectedCharacter1P = currentTypeCharacters[selected1P];
                
                if (currentGameMode == GameMode.PlayerVsPlayer)
                {
                    GameData.Instance.selectedCharacter2P = currentTypeCharacters[selectedCPU];
                }
                else
                {
                    GameData.Instance.selectedCharacterCPU = currentTypeCharacters[selectedCPU];
                }
            }
            
            // 주사위 굴리기 화면으로 이동
            if (DiceManager.Instance != null)
            {
                DiceManager.Instance.ShowDicePanel();
            }
            else
            {
                // DiceManager가 없으면 바로 게임으로
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
            }
        }
    }
    
    public void OnClickBackground(int backgroundIndex)
    {
        selectedBackground = backgroundIndex;
        UpdateUI();
    }
} 
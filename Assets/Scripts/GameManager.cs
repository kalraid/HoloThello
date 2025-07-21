using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // SceneManager 사용을 위해 추가

// [ExecuteInEditMode] // 제거
public class GameManager : MonoBehaviour
{
    public BoardManager boardManager;
    public Text turnText;
    public Text resultText;
    public int currentPlayer = 1; // 1:흑, 2:백
    public Sprite highlightSprite; // 하이라이트용 스프라이트

    // --- 격투/오셀로 UI 및 데이터 ---
    public Slider playerHpBar;
    public Slider cpuHpBar;
    public Image playerCharacterImage;
    public Image cpuCharacterImage;
    public Image backgroundImage;
    public Button[] playerSkillButtons; // 0,1:스킬, 2:궁극기
    public Button[] cpuSkillButtons;
    public Sprite[] characterDiscSprites; // 캐릭터별 돌 스프라이트
    public Sprite[] backgroundSprites;
    public GameObject discPrefab;
    public int playerHp = 10000;
    public int cpuHp = 10000;
    private int playerCharacterIdx;
    private int cpuCharacterIdx;
    private int backgroundIdx;
    // 스킬 쿨타임, 사용 여부 등은 추후 확장

    // --- 스킬 쿨타임/사용 관리 ---
    private int[] playerSkillCooldowns = new int[3]; // 0,1:스킬, 2:궁극기
    private int[] cpuSkillCooldowns = new int[3];
    private bool[] playerSkillUsed = new bool[3]; // 궁극기 1회 제한
    private bool[] cpuSkillUsed = new bool[3];
    public int skillCooldownValue = 5; // 일반 스킬 쿨타임(예시)
    public int ultimateCooldownValue = 99; // 궁극기 쿨타임(사실상 1회)

    public AudioSource sfxSource;
    public AudioClip flipSfx;
    public AudioClip hitSfx;
    public Animator playerAnimator;
    public Animator cpuAnimator;

    // --- 5개 단위/연속턴/배수 데미지/이펙트 ---
    public AudioClip specialEffectSfx;
    public GameObject specialEffectPrefab;
    // 연속 턴 시스템
    private int consecutiveTurnCount = 0;
    private const float CONSECUTIVE_DAMAGE_MULTIPLIER = 1.5f;

    // --- K.O/FINISH 연출 ---
    public GameObject koEffectPrefab;
    public AudioClip koSfx;
    public GameObject finishEffectPrefab;
    public AudioClip finishSfx;
    private bool isKO = false;
    private bool isFinish = false;
    private float koBounceTimer = 0f;
    private GameObject koBoxInstance;

    // --- 자동 데미지(1P 3초 미입력 시) ---
    private float idleTimer = 0f;
    public float idleDamageInterval = 3f;
    public int idleDamageValue = 1;
    
    // 자동 데미지 시스템
    private float autoDamageTimer = 0f;
    private const float AUTO_DAMAGE_INTERVAL = 5f;

    // --- 스킬 효과 샘플 ---
    public AudioClip skill1Sfx;
    public AudioClip skill2Sfx;
    public AudioClip ultimateSfx;
    public GameObject ultimateEffectPrefab;

    [Header("게임 정보 UI")]
    public Text gameInfoText;
    public Text turnInfoText;
    public Text scoreInfoText;
    public Text consecutiveTurnText;
    public Text gameModeText;
    public Text diceResultText;
    
    // 게임 정보 업데이트
    void UpdateGameInfo()
    {
        if (gameInfoText != null)
        {
            string info = $"1P HP: {playerHp} | 2P HP: {cpuHp}";
            gameInfoText.text = info;
        }
        
        if (turnInfoText != null)
        {
            BoardManager boardManager = FindFirstObjectByType<BoardManager>();
            if (boardManager != null)
            {
                string turnInfo = boardManager.IsBlackTurn() ? "1P 턴" : "2P 턴";
                turnInfoText.text = turnInfo;
            }
        }
        
        if (scoreInfoText != null)
        {
            BoardManager boardManager = FindFirstObjectByType<BoardManager>();
            if (boardManager != null)
            {
                string scoreInfo = $"1P: {boardManager.GetBlackScore()} | 2P: {boardManager.GetWhiteScore()}";
                scoreInfoText.text = scoreInfo;
            }
        }
        
        if (consecutiveTurnText != null)
        {
            if (consecutiveTurnCount > 0)
            {
                consecutiveTurnText.text = $"연속 턴: {consecutiveTurnCount}";
                consecutiveTurnText.color = Color.orange;
            }
            else
            {
                consecutiveTurnText.text = "";
            }
        }
        
        if (gameModeText != null)
        {
            string modeText = "";
            switch (GameData.Instance.GetGameMode())
            {
                case GameMode.PlayerVsCPU:
                    modeText = "1P vs CPU";
                    break;
                case GameMode.PlayerVsPlayer:
                    modeText = "1P vs 2P";
                    break;
                case GameMode.CPUVsCPU:
                    modeText = "CPU vs CPU";
                    break;
            }
            gameModeText.text = modeText;
        }
        
        if (diceResultText != null && GameData.Instance.isFirstTurnDetermined)
        {
            string diceText = $"주사위 - 1P: {GameData.Instance.diceResult1P}, 2P: {GameData.Instance.diceResult2P}";
            diceResultText.text = diceText;
        }
    }
    
    void UpdateHPBars()
    {
        if (playerHpBar != null)
        {
            playerHpBar.value = playerHp;
        }
        if (cpuHpBar != null)
        {
            cpuHpBar.value = cpuHp;
        }
    }

    // --- 게임 상태 관리 ---
    public enum GameState { Playing, Paused, GameOver }
    private GameState currentState;

    [Header("조작계")]
    public GameObject boardSelector; // 보드 위 커서 오브젝트
    public Text controlsInfoText; // 조작 안내 텍스트
    private Vector2Int selectorPosition = new Vector2Int(0, 0);

    #region Unity Lifecycle Methods

    void Start()
    {
        // 필수 컴포넌트 확인
        if (!ValidateComponents())
        {
            enabled = false; // 필수 컴포넌트 없으면 동작 중지
            return;
        }

        currentState = GameState.Playing;
        
        // 주사위 결과가 없으면 주사위 굴리기
        if (GameData.Instance != null && !GameData.Instance.isFirstTurnDetermined)
        {
            GameData.Instance.DetermineFirstTurn();
        }
        
        // 초기화
        InitializeGameMode();
        SetupCharacters();
        UpdateAllUI(); // 통합된 UI 업데이트 호출

        // 조작 안내 UI 초기화
        UpdateControlsInfo();

        // 보드 커서 초기화
        if (boardSelector != null)
        {
            boardSelector.SetActive(true);
            UpdateSelectorPosition();
        }
    }

    void Update()
    {
        if (currentState != GameState.Playing) return; // 게임 진행 중이 아니면 Update 로직 실행 안함

        // --- 입력 처리 ---
        HandleKeyboardInput();
        HandleMouseInput();

        // 타이머 업데이트
        UpdateTimers();

        // 게임 상태 업데이트
        UpdateGameState();
    }

    #endregion

    #region Initialization Methods

    bool ValidateComponents()
    {
        bool isValid = true;
        if (boardManager == null) { Debug.LogError("BoardManager가 연결되지 않았습니다."); isValid = false; }
        if (playerHpBar == null) { Debug.LogError("PlayerHpBar가 연결되지 않았습니다."); isValid = false; }
        if (cpuHpBar == null) { Debug.LogError("CpuHpBar가 연결되지 않았습니다."); isValid = false; }
        // ... 다른 주요 컴포넌트들도 필요에 따라 추가 ...
        return isValid;
    }

    #region Game Flow
    System.Collections.IEnumerator CPUVsCPUGameLoop()
    {
        if (boardManager == null) yield break;
        
        while (!boardManager.IsGameEnded())
        {
            boardManager.MakeAIMove();
            yield return new WaitForSeconds(1.0f);
        }
    }
    
    void InitializeGameMode()
    {
        if (GameData.Instance == null) return;
        
        GameMode currentMode = GameData.Instance.GetGameMode();
        
        switch (currentMode)
        {
            case GameMode.PlayerVsCPU:
                Debug.Log("1P vs CPU 모드 시작");
                break;
            case GameMode.PlayerVsPlayer:
                Debug.Log("1P vs 2P 모드 시작");
                break;
            case GameMode.CPUVsCPU:
                Debug.Log("CPU vs CPU 모드 시작");
                StartCoroutine(CPUVsCPUGameLoop()); // 삭제되었던 코루틴 호출 복구
                break;
        }
    }
    
    void ApplyAutoDamage()
    {
        if (boardManager != null && !boardManager.IsGameEnded())
        {
            if (boardManager.IsBlackTurn())
            {
                ApplyDamageToCPU(1);
            }
            else
            {
                ApplyDamageToPlayer1(1);
            }
        }
    }
    #endregion

    void SetupCharacters()
    {
        if (GameData.Instance == null) return;

        CharacterData player1Char = GameData.Instance.GetPlayer1Character();
        if (player1Char != null && playerCharacterImage != null)
        {
            playerCharacterImage.sprite = player1Char.characterSprite;
        }
        
        CharacterData player2Char = GameData.Instance.GetPlayer2Character();
        if (player2Char != null && cpuCharacterImage != null)
        {
            cpuCharacterImage.sprite = player2Char.characterSprite;
        }
    }

    #endregion

    #region Update Logic Methods

    void UpdateTimers()
    {
        // CPU vs CPU 모드가 아닐 때만 타이머 작동
        if (GameData.Instance != null && GameData.Instance.GetGameMode() != GameMode.CPUVsCPU)
        {
            autoDamageTimer += Time.deltaTime;
            if (autoDamageTimer >= AUTO_DAMAGE_INTERVAL)
            {
                autoDamageTimer = 0f;
                ApplyAutoDamage(); // 삭제되었던 메서드 호출 복구
            }
            
            idleTimer += Time.deltaTime;
        }

        if (isKO && koBounceTimer > 0)
        {
            koBounceTimer -= Time.deltaTime;
        }
    }

    void UpdateGameState()
    {
        // 게임 정보 UI 업데이트
        UpdateAllUI();
        
        if (isFinish)
        {
            Debug.Log("FINISH 상태");
        }
    }

    #endregion

    #region UI Update Methods

    void UpdateAllUI()
    {
        UpdateGameInfo();
        UpdateHPBars();
        // 스킬 버튼 UI 갱신은 UpdateGameInfo 또는 별도 메서드에서 처리
    }
    
    // ... 기존 UpdateGameInfo, UpdateHPBars, UpdateSkillButtons 메서드 ...
    
    #endregion
    
    // ... 이하 기존 메서드들 (ApplyDamage, EndGame, OnClickPlayerSkill 등) ...
    // 각 메서드 내부에서도 사용하는 컴포넌트에 대해 null 체크를 강화하면 더 안전해집니다.

    // 예시: ApplyDamageToPlayer1 메서드 보완
    void ApplyDamageToPlayer1(int damage)
    {
        playerHp -= damage;
        if (playerHp < 0) playerHp = 0;
        
        UpdateHPBars();
        if(playerHpBar != null) ShowDamageEffect(playerHpBar, damage);
        
        if (EffectManager.Instance != null && playerHpBar != null)
        {
            EffectManager.Instance.FlashHealthBar(playerHpBar);
            EffectManager.Instance.PlayCharacterAnimation(true, "Hit");
        }
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDamage();
        }
        
        Debug.Log($"1P 데미지: {damage}, 남은 HP: {playerHp}");
        CheckGameEnd(); // 데미지를 입은 후 게임 종료 여부 확인
    }

    // 예시: ApplyDamageToCPU 메서드 보완
    void ApplyDamageToCPU(int damage)
    {
        cpuHp -= damage;
        if (cpuHp < 0) cpuHp = 0;
        
        UpdateHPBars();
        if(cpuHpBar != null) ShowDamageEffect(cpuHpBar, damage);
        
        if (EffectManager.Instance != null && cpuHpBar != null)
        {
            EffectManager.Instance.FlashHealthBar(cpuHpBar);
            EffectManager.Instance.PlayCharacterAnimation(false, "Hit");
        }
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDamage();
        }
        
        Debug.Log($"CPU 데미지: {damage}, 남은 HP: {cpuHp}");
        CheckGameEnd(); // 데미지를 입은 후 게임 종료 여부 확인
    }
    
    System.Collections.IEnumerator FlashHealthBar(Slider healthBar)
    {
        Image fillImage = healthBar.fillRect.GetComponent<Image>();
        Color originalColor = fillImage.color;
        
        for (int i = 0; i < 3; i++)
        {
            fillImage.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            fillImage.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    void ShowDamageEffect(Slider healthBar, int damage)
    {
        // 데미지 텍스트 이펙트
        if (EffectManager.Instance != null)
        {
            Vector3 effectPosition = healthBar.transform.position;
            bool isCritical = damage >= 5; // 5 이상이면 크리티컬
            EffectManager.Instance.ShowDamageEffect(effectPosition, damage, isCritical);
        }
        
        Debug.Log($"데미지 효과: {damage}");
    }
    
    void ShowSpecialEffect(string message)
    {
        Debug.Log($"특별 이펙트: {message}");
        
        // 특별 이펙트 UI 표시
        if (EffectManager.Instance != null)
        {
            EffectManager.Instance.ShowSpecialEffect(message, Color.yellow);
        }
        
        if (resultText != null)
        {
            resultText.text = message;
            StartCoroutine(ClearResultText());
        }
    }
    
    System.Collections.IEnumerator ClearResultText()
    {
        yield return new WaitForSeconds(2f);
        if (resultText != null)
        {
            resultText.text = "";
        }
    }

    void UpdateTurnText()
    {
        turnText.text = (currentPlayer == 1) ? "흑 차례" : "백 차례";
        // ShowCurrentPlayerHighlights(); // BoardManager에서 처리
    }

    // GetFlippableDiscs와 같은 기존의 오셀로 로직은 BoardManager로 이전되었으므로,
    // GameManager에 중복으로 남아있는 관련 메서드들은 삭제합니다.
    // (PlaceAndFlip, GetFlippableDiscs, HasValidMove, IsValidMove 등)
    // ShowCurrentPlayerHighlights 메서드도 BoardManager의 ShowHighlights가 삭제되었으므로 함께 삭제합니다.

    void EndGame()
    {
        if (currentState == GameState.GameOver) return; // 이미 게임이 끝났으면 중복 실행 방지
        currentState = GameState.GameOver;

        // KO/FINISH 연출 및 게임 종료 처리
        int black = 0, white = 0;
        for (int y = 0; y < 8; y++)
        for (int x = 0; x < 8; x++)
        {
            if (boardManager.board[x, y].HasPiece() && boardManager.board[x, y].IsBlack()) black++;
            if (boardManager.board[x, y].HasPiece() && !boardManager.board[x, y].IsBlack()) white++;
        }
        if (playerHp <= 0 || cpuHp <= 0)
        {
            isKO = true;
            // K.O 이펙트/사운드/애니메이션
            if (koEffectPrefab)
            {
                koBoxInstance = Instantiate(koEffectPrefab, Vector3.zero, Quaternion.identity);
            }
            if (sfxSource && koSfx) sfxSource.PlayOneShot(koSfx);
            resultText.text = "K.O! 아무 키나 누르세요";
        }
        else
        {
            // 돌을 모두 못 둘 때 FINISH 처리
            int diff = Mathf.Abs(black - white);
            if (black > white)
            {
                cpuHp -= diff * 10;
                cpuHpBar.value = cpuHp;
            }
            else if (white > black)
            {
                playerHp -= diff * 10;
                playerHpBar.value = playerHp;
            }
            isFinish = true;
            // FINISH 이펙트/사운드/애니메이션
            if (finishEffectPrefab)
            {
                Instantiate(finishEffectPrefab, Vector3.zero, Quaternion.identity);
            }
            if (sfxSource && finishSfx) sfxSource.PlayOneShot(finishSfx);
            resultText.text = "FINISH! 아무 키나 누르세요";
        }
    }

    // GetFlippableDiscs와 같은 기존의 오셀로 로직은 BoardManager로 이전되었으므로,
    // GameManager에 중복으로 남아있는 관련 메서드들은 삭제합니다.
    // (PlaceAndFlip, GetFlippableDiscs, HasValidMove, IsValidMove 등)
    // ShowCurrentPlayerHighlights 메서드도 BoardManager의 ShowHighlights가 삭제되었으므로 함께 삭제합니다.

    // 예시: ApplyDamageToPlayer1 메서드 보완
    // 예시: ApplyDamageToCPU 메서드 보완
    
    System.Collections.IEnumerator BarFlash(Slider bar)
    {
        Color originalColor = bar.fillRect.GetComponent<Image>().color;
        bar.fillRect.GetComponent<Image>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        bar.fillRect.GetComponent<Image>().color = originalColor;
    }

    // 스킬 사용 시 이펙트와 사운드 추가
    public void UseSkill(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= 3) return;
        
        // 스킬 쿨타임 체크
        if (!CanUseSkill(skillIndex)) return;
        
        // 스킬 사용
        CharacterData currentChar = GameData.Instance.selectedCharacter1P;
        if (currentChar != null)
        {
            SkillData skill = GetSkillData(currentChar, skillIndex);
            if (skill != null)
            {
                // 스킬 이펙트
                if (EffectManager.Instance != null)
                {
                    Vector3 effectPosition = transform.position;
                    EffectManager.Instance.ShowSkillEffect(effectPosition, skill.skillName);
                }
                
                // 스킬 사운드
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySkillUse();
                }
                
                // 스킬 사용 애니메이션
                if (EffectManager.Instance != null)
                {
                    EffectManager.Instance.PlayCharacterAnimation(true, "Attack");
                }
                
                // 데미지 적용
                ApplyDamageToCPU(skill.damage);
                
                // 스킬 사용 처리
                UseSkillInternal(skillIndex);
                
                Debug.Log($"{skill.skillName} 발동!");
            }
        }
    }
    
    // 게임 종료 처리 개선
    void CheckGameEnd()
    {
        if (playerHp <= 0 || cpuHp <= 0)
        {
            isKO = true;
            isFinish = true;
            
            if (playerHp <= 0)
            {
                // CPU 승리
                ShowGameEndEffect(false);
            }
            else
            {
                // 1P 승리
                ShowGameEndEffect(true);
            }
        }
    }
    
    void ShowGameEndEffect(bool player1Wins)
    {
        string winnerText = "";
        
        if (player1Wins)
        {
            // 1P 승리
            winnerText = "1P 승리!";
            if (EffectManager.Instance != null)
            {
                EffectManager.Instance.ShowKOEffect();
            }
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayKO();
                AudioManager.Instance.PlayVictory();
            }
        }
        else
        {
            // 2P 승리
            switch (GameData.Instance.GetGameMode())
            {
                case GameMode.PlayerVsCPU:
                    winnerText = "CPU 승리!";
                    break;
                case GameMode.PlayerVsPlayer:
                    winnerText = "2P 승리!";
                    break;
                case GameMode.CPUVsCPU:
                    winnerText = "CPU2 승리!";
                    break;
            }
            
            if (EffectManager.Instance != null)
            {
                EffectManager.Instance.ShowKOEffect();
            }
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayKO();
                AudioManager.Instance.PlayDefeat();
            }
        }
        
        if (resultText != null)
        {
            resultText.text = winnerText;
        }
        
        // 3초 후 캐릭터 선택 화면으로 이동
        StartCoroutine(ReturnToCharacterSelect());
    }
    
    System.Collections.IEnumerator ReturnToCharacterSelect()
    {
        yield return new WaitForSeconds(3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterSelectScene");
    }
    
    // 누락된 메서드들 추가
    bool CanUseSkill(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= 3) return false;
        return playerSkillCooldowns[skillIndex] == 0 && !playerSkillUsed[skillIndex];
    }
    
    SkillData GetSkillData(CharacterData character, int skillIndex)
    {
        if (character == null) return null;
        
        switch (skillIndex)
        {
            case 0: return character.skillA;
            case 1: return character.skillB;
            case 2: return character.ultimateA;
            default: return null;
        }
    }
    
    void UseSkillInternal(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= 3) return;
        
        // 쿨타임 설정
        CharacterData currentChar = GameData.Instance.selectedCharacter1P;
        if (currentChar != null)
        {
            SkillData skill = GetSkillData(currentChar, skillIndex);
            if (skill != null)
            {
                playerSkillCooldowns[skillIndex] = skill.cooldown;
                if (skill.isUltimate)
                {
                    playerSkillUsed[skillIndex] = true;
                }
            }
        }
        
        UpdateAllUI();
    }

    void HandleKeyboardInput()
    {
        // 커서 이동
        if (Input.GetKeyDown(KeyCode.UpArrow)) MoveSelector(0, 1);
        if (Input.GetKeyDown(KeyCode.DownArrow)) MoveSelector(0, -1);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveSelector(-1, 0);
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveSelector(1, 0);

        // 결정 (돌 놓기)
        if (Input.GetKeyDown(KeyCode.Z))
        {
            TryPlacePieceAt(selectorPosition.x, selectorPosition.y);
        }

        // 스킬 사용
        if (Input.GetKeyDown(KeyCode.Alpha1)) OnClickPlayerSkill(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) OnClickPlayerSkill(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) OnClickPlayerSkill(2);
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                Disc clickedDisc = hit.collider.GetComponent<Disc>();
                if (clickedDisc != null)
                {
                    TryPlacePieceAt(clickedDisc.x, clickedDisc.y);
                }
            }
        }
    }

    void MoveSelector(int dx, int dy)
    {
        if (boardManager == null) return;
        selectorPosition.x = Mathf.Clamp(selectorPosition.x + dx, 0, boardManager.boardSize - 1);
        selectorPosition.y = Mathf.Clamp(selectorPosition.y + dy, 0, boardManager.boardSize - 1);
        UpdateSelectorPosition();
    }

    void UpdateSelectorPosition()
    {
        if (boardSelector != null && boardManager != null)
        {
            // BoardManager의 Transform을 기준으로 로컬 위치 계산
            boardSelector.transform.position = boardManager.transform.position + new Vector3(selectorPosition.x, selectorPosition.y, -1f);
        }
    }

    void TryPlacePieceAt(int x, int y)
    {
        if (boardManager != null && boardManager.TryPlacePiece(x, y))
        {
            // 성공 시 로직 (예: 사운드 재생)
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayFlip();
            }
        }
        else
        {
            // 실패 시 로직 (예: 효과음, UI 피드백)
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayError();
            }
        }
    }

    void UpdateControlsInfo()
    {
        if (controlsInfoText != null)
        {
            controlsInfoText.text = "이동: ←→↑↓ | 결정: Z | 스킬: 1, 2, 3 | 마우스 클릭 가능";
        }
    }

    // UpdateSkillButtons() 호출을 모두 UpdateAllUI()로 대체

    // OnClickPlayerSkill 메서드가 없을 경우 추가
    public void OnClickPlayerSkill(int idx)
    {
        // 실제 스킬 사용 로직 구현 (예시)
        // 예: 플레이어 스킬 사용 처리, 쿨타임 적용, UI 갱신 등
        // 아래는 예시 코드, 실제 프로젝트 로직에 맞게 구현 필요
        if (playerSkillCooldowns[idx] > 0 || playerSkillUsed[idx]) return;
        // ... (스킬 사용 로직) ...
        UpdateAllUI();
    }
} 
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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

    void Start()
    {
        // 주사위 결과가 없으면 주사위 굴리기
        if (!GameData.Instance.isFirstTurnDetermined)
        {
            GameData.Instance.DetermineFirstTurn();
        }
        
        // 게임 모드에 따른 초기화
        InitializeGameMode();
        
        // 캐릭터 설정
        SetupCharacters();
        
        // UI 업데이트
        UpdateGameInfo();
    }
    
    void InitializeGameMode()
    {
        GameMode currentMode = GameData.Instance.GetGameMode();
        
        switch (currentMode)
        {
            case GameMode.PlayerVsCPU:
                // 1P vs CPU 모드
                Debug.Log("1P vs CPU 모드 시작");
                break;
                
            case GameMode.PlayerVsPlayer:
                // 1P vs 2P 모드
                Debug.Log("1P vs 2P 모드 시작");
                break;
                
            case GameMode.CPUVsCPU:
                // CPU vs CPU 모드
                Debug.Log("CPU vs CPU 모드 시작");
                StartCPUVsCPUGame();
                break;
        }
    }
    
    void SetupCharacters()
    {
        // 1P 캐릭터 설정
        CharacterData player1Char = GameData.Instance.GetPlayer1Character();
        if (player1Char != null && playerCharacterImage != null)
        {
            playerCharacterImage.sprite = player1Char.characterSprite;
        }
        
        // 2P/CPU 캐릭터 설정
        CharacterData player2Char = GameData.Instance.GetPlayer2Character();
        if (player2Char != null && cpuCharacterImage != null)
        {
            cpuCharacterImage.sprite = player2Char.characterSprite;
        }
    }
    
    void StartCPUVsCPUGame()
    {
        // CPU vs CPU 모드에서는 자동으로 게임 진행
        StartCoroutine(CPUVsCPUGameLoop());
    }
    
    System.Collections.IEnumerator CPUVsCPUGameLoop()
    {
        BoardManager boardManager = FindFirstObjectByType<BoardManager>();
        if (boardManager == null) yield break;
        
        while (!boardManager.IsGameEnded())
        {
            // CPU가 자동으로 수를 둠
            boardManager.MakeAIMove();
            
            // 잠시 대기
            yield return new WaitForSeconds(1.0f);
        }
    }

    void Update()
    {
        // CPU vs CPU 모드가 아니면 일반 업데이트
        if (GameData.Instance.GetGameMode() != GameMode.CPUVsCPU)
        {
            // 자동 데미지 타이머
            autoDamageTimer += Time.deltaTime;
            if (autoDamageTimer >= AUTO_DAMAGE_INTERVAL)
            {
                autoDamageTimer = 0f;
                ApplyAutoDamage();
            }
            
            // 스킬 쿨타임 업데이트
            UpdateSkillCooldowns();
        }
        
        // 게임 정보 업데이트
        UpdateGameInfo();
        
        // KO 바운스 타이머 업데이트
        if (isKO && koBounceTimer > 0)
        {
            koBounceTimer -= Time.deltaTime;
        }
        
        // FINISH 상태 체크
        if (isFinish)
        {
            // FINISH 상태에서 추가 처리
            Debug.Log("FINISH 상태");
        }
        
        // 유휴 타이머 업데이트
        idleTimer += Time.deltaTime;
    }
    
    void ApplyAutoDamage()
    {
        // 현재 턴에 따라 자동 데미지 적용
        BoardManager boardManager = FindFirstObjectByType<BoardManager>();
        if (boardManager != null && !boardManager.IsGameEnded())
        {
            if (boardManager.IsBlackTurn())
            {
                // 1P 턴이면 CPU에게 데미지
                ApplyDamageToCPU(1);
            }
            else
            {
                // CPU 턴이면 1P에게 데미지
                ApplyDamageToPlayer1(1);
            }
        }
    }
    
    void ApplyDamageToPlayer1(int damage)
    {
        playerHp -= damage;
        if (playerHp < 0) playerHp = 0;
        
        UpdateHPBars();
        ShowDamageEffect(playerHpBar, damage);
        
        // 체력바 반짝임 효과
        if (EffectManager.Instance != null)
        {
            EffectManager.Instance.FlashHealthBar(playerHpBar);
        }
        
        // 데미지 사운드
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDamage();
        }
        
        // 캐릭터 애니메이션
        if (EffectManager.Instance != null)
        {
            EffectManager.Instance.PlayCharacterAnimation(true, "Hit");
        }
        
        Debug.Log($"1P 데미지: {damage}, 남은 HP: {playerHp}");
    }
    
    void ApplyDamageToCPU(int damage)
    {
        cpuHp -= damage;
        if (cpuHp < 0) cpuHp = 0;
        
        UpdateHPBars();
        ShowDamageEffect(cpuHpBar, damage);
        
        // 체력바 반짝임 효과
        if (EffectManager.Instance != null)
        {
            EffectManager.Instance.FlashHealthBar(cpuHpBar);
        }
        
        // 데미지 사운드
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDamage();
        }
        
        // 캐릭터 애니메이션
        if (EffectManager.Instance != null)
        {
            EffectManager.Instance.PlayCharacterAnimation(false, "Hit");
        }
        
        Debug.Log($"CPU 데미지: {damage}, 남은 HP: {cpuHp}");
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
        ShowCurrentPlayerHighlights();
    }

    void ShowCurrentPlayerHighlights()
    {
        var positions = new List<Vector2Int>();
        for (int y = 0; y < 8; y++)
        for (int x = 0; x < 8; x++)
            if (IsValidMove(x, y, currentPlayer))
                positions.Add(new Vector2Int(x, y));
        boardManager.ShowHighlights(positions, highlightSprite);
    }

    void EndGame()
    {
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

    bool IsValidMove(int x, int y, int color)
    {
        if (x < 0 || x >= 8 || y < 0 || y >= 8 || boardManager.board[x, y].HasPiece())
            return false;
        return GetFlippableDiscs(x, y, color).Count > 0;
    }

    bool HasValidMove(int color)
    {
        for (int y = 0; y < 8; y++)
        for (int x = 0; x < 8; x++)
            if (IsValidMove(x, y, color))
                return true;
        return false;
    }

    void PlaceAndFlip(int x, int y, int color)
    {
        bool isBlack = (color == 1);
        boardManager.PlaceDisc(x, y, isBlack);
        var flippable = GetFlippableDiscs(x, y, color);
        foreach (var pos in flippable)
        {
            boardManager.FlipDisc(pos.x, pos.y);
        }
        
        // 데미지 계산 및 적용
        int damage = flippable.Count;
        if (color == 1) // 1P가 뒤집었으면 CPU에게 데미지
        {
            cpuHp -= damage;
            cpuHpBar.value = cpuHp;
            if (cpuAnimator) cpuAnimator.SetTrigger("Hit");
        }
        else // CPU가 뒤집었으면 1P에게 데미지
        {
            playerHp -= damage;
            playerHpBar.value = playerHp;
            if (playerAnimator) playerAnimator.SetTrigger("Hit");
        }
        
        // 사운드 재생
        if (sfxSource && flipSfx) sfxSource.PlayOneShot(flipSfx);
        
        // HP 0 이하 시 게임 종료
        if (playerHp <= 0 || cpuHp <= 0) { EndGame(); return; }
    }

    List<Vector2Int> GetFlippableDiscs(int x, int y, int color)
    {
        var flippable = new List<Vector2Int>();
        int[] dx = {-1, -1, -1, 0, 0, 1, 1, 1};
        int[] dy = {-1, 0, 1, -1, 1, -1, 0, 1};
        
        for (int i = 0; i < 8; i++)
        {
            var temp = new List<Vector2Int>();
            int nx = x + dx[i];
            int ny = y + dy[i];
            
            while (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
            {
                if (!boardManager.board[nx, ny].HasPiece()) break;
                bool isCurrentColor = (color == 1) ? boardManager.board[nx, ny].IsBlack() : !boardManager.board[nx, ny].IsBlack();
                if (isCurrentColor)
                {
                    flippable.AddRange(temp);
                    break;
                }
                temp.Add(new Vector2Int(nx, ny));
                nx += dx[i];
                ny += dy[i];
            }
        }
        return flippable;
    }

    void DecrementSkillCooldowns()
    {
        for (int i = 0; i < 3; i++)
        {
            if (playerSkillCooldowns[i] > 0) playerSkillCooldowns[i]--;
            if (cpuSkillCooldowns[i] > 0) cpuSkillCooldowns[i]--;
        }
    }
    
    void UpdateSkillCooldowns()
    {
        DecrementSkillCooldowns();
        UpdateSkillButtons();
    }

    void UpdateSkillButtons()
    {
        // 1P 스킬 버튼 업데이트
        for (int i = 0; i < playerSkillButtons.Length; i++)
        {
            bool canUse = playerSkillCooldowns[i] == 0 && !playerSkillUsed[i];
            playerSkillButtons[i].interactable = canUse;
            
            // 버튼 텍스트 업데이트
            Text buttonText = playerSkillButtons[i].GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                CharacterData charData = CharacterDataManager.Instance.GetCharacterData(playerCharacterIdx);
                if (charData != null)
                {
                    SkillData skillData = null;
                    switch (i)
                    {
                        case 0: skillData = charData.skillA; break;
                        case 1: skillData = charData.skillB; break;
                        case 2: skillData = charData.ultimateA; break;
                    }
                    if (skillData != null)
                    {
                        buttonText.text = skillData.skillName;
                    }
                }
            }
        }
        
        // CPU 스킬 버튼 업데이트
        for (int i = 0; i < cpuSkillButtons.Length; i++)
        {
            bool canUse = cpuSkillCooldowns[i] == 0 && !cpuSkillUsed[i];
            cpuSkillButtons[i].interactable = canUse;
            
            // 버튼 텍스트 업데이트
            Text buttonText = cpuSkillButtons[i].GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                CharacterData charData = CharacterDataManager.Instance.GetCharacterData(cpuCharacterIdx);
                if (charData != null)
                {
                    SkillData skillData = null;
                    switch (i)
                    {
                        case 0: skillData = charData.skillA; break;
                        case 1: skillData = charData.skillB; break;
                        case 2: skillData = charData.ultimateA; break;
                    }
                    if (skillData != null)
                    {
                        buttonText.text = skillData.skillName;
                    }
                }
            }
        }
    }

    public void OnClickPlayerSkill(int idx)
    {
        if (playerSkillCooldowns[idx] > 0 || playerSkillUsed[idx]) return;
        
        CharacterData charData = CharacterDataManager.Instance.GetCharacterData(playerCharacterIdx);
        if (charData == null) return;
        
        SkillData skillData = null;
        switch (idx)
        {
            case 0: skillData = charData.skillA; break;
            case 1: skillData = charData.skillB; break;
            case 2: skillData = charData.ultimateA; break;
        }
        
        if (skillData != null)
        {
            // 데미지 적용
            cpuHp -= skillData.damage;
            cpuHpBar.value = cpuHp;
            
            // 메시지 출력
            resultText.text = $"{skillData.skillName} 발동!";
            
            // 쿨타임 설정
            playerSkillCooldowns[idx] = skillData.cooldown;
            if (skillData.isUltimate)
            {
                playerSkillUsed[idx] = true;
            }
            
            // 사운드 재생
            if (sfxSource)
            {
                if (idx == 0 && skill1Sfx) sfxSource.PlayOneShot(skill1Sfx);
                else if (idx == 1 && skill2Sfx) sfxSource.PlayOneShot(skill2Sfx);
                else if (idx == 2 && ultimateSfx) sfxSource.PlayOneShot(ultimateSfx);
            }
            
            // HP 0 이하 시 게임 종료
            if (cpuHp <= 0) { EndGame(); return; }
            
            UpdateSkillButtons();
        }
    }

    public void OnClickCPUSkill(int idx)
    {
        if (cpuSkillCooldowns[idx] > 0 || cpuSkillUsed[idx]) return;
        
        CharacterData charData = CharacterDataManager.Instance.GetCharacterData(cpuCharacterIdx);
        if (charData == null) return;
        
        SkillData skillData = null;
        switch (idx)
        {
            case 0: skillData = charData.skillA; break;
            case 1: skillData = charData.skillB; break;
            case 2: skillData = charData.ultimateA; break;
        }
        
        if (skillData != null)
        {
            // 데미지 적용
            playerHp -= skillData.damage;
            playerHpBar.value = playerHp;
            
            // 메시지 출력
            resultText.text = $"{skillData.skillName} 발동!";
            
            // 쿨타임 설정
            cpuSkillCooldowns[idx] = skillData.cooldown;
            if (skillData.isUltimate)
            {
                cpuSkillUsed[idx] = true;
            }
            
            // 사운드 재생
            if (sfxSource)
            {
                if (idx == 0 && skill1Sfx) sfxSource.PlayOneShot(skill1Sfx);
                else if (idx == 1 && skill2Sfx) sfxSource.PlayOneShot(skill2Sfx);
                else if (idx == 2 && ultimateSfx) sfxSource.PlayOneShot(ultimateSfx);
            }
            
            // HP 0 이하 시 게임 종료
            if (playerHp <= 0) { EndGame(); return; }
            
            UpdateSkillButtons();
        }
    }

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
        
        UpdateSkillButtons();
    }
} 
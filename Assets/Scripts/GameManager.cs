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
    private int consecutiveTurnCount = 0; // 연속 턴 카운트
    private int lastMovePlayer = 0;

    // --- K.O/FINISH 연출 ---
    public GameObject koEffectPrefab;
    public AudioClip koSfx;
    public GameObject finishEffectPrefab;
    public AudioClip finishSfx;
    private bool isGameOver = false;
    private bool isKO = false;
    private bool isFinish = false;
    private float koBounceTimer = 0f;
    private GameObject koBoxInstance;

    // --- 자동 데미지(1P 3초 미입력 시) ---
    private float idleTimer = 0f;
    public float idleDamageInterval = 3f;
    public int idleDamageValue = 1;

    // --- 스킬 효과 샘플 ---
    public AudioClip skill1Sfx;
    public AudioClip skill2Sfx;
    public AudioClip ultimateSfx;
    public GameObject ultimateEffectPrefab;

    void Start()
    {
        // GameData에서 선택 정보 받아오기
        if (GameData.Instance)
        {
            playerCharacterIdx = GameData.Instance.playerCharacterIdx;
            cpuCharacterIdx = GameData.Instance.cpuCharacterIdx;
            backgroundIdx = GameData.Instance.backgroundIdx;
        }
        playerHp = 10000;
        cpuHp = 10000;
        playerHpBar.maxValue = cpuHpBar.maxValue = 10000;
        playerHpBar.value = playerHp;
        cpuHpBar.value = cpuHp;
        playerCharacterImage.sprite = characterDiscSprites[playerCharacterIdx];
        cpuCharacterImage.sprite = characterDiscSprites[cpuCharacterIdx];
        backgroundImage.sprite = backgroundSprites[backgroundIdx];
        // 오셀로 보드 초기화 시 돌에 캐릭터별 스프라이트 적용
        InitBoardWithCharacterDiscs();
        for (int i = 0; i < 3; i++)
        {
            playerSkillCooldowns[i] = (i < 2) ? 0 : 0; // 궁극기도 0으로 시작
            cpuSkillCooldowns[i] = (i < 2) ? 0 : 0;
            playerSkillUsed[i] = false;
            cpuSkillUsed[i] = false;
        }
        UpdateSkillButtons();
    }

    void InitBoardWithCharacterDiscs()
    {
        // 오셀로 초기 돌 배치(캐릭터별 돌)
        for (int y = 0; y < 8; y++)
        for (int x = 0; x < 8; x++)
        {
            boardManager.board[x, y] = 0;
            if (boardManager.discs[x, y] != null)
                Destroy(boardManager.discs[x, y].gameObject);
            boardManager.discs[x, y] = null;
        }
        // 중앙 4개 돌 배치 (1:흑=player, 2:백=cpu)
        boardManager.PlaceDiscWithSprite(3, 3, 2, characterDiscSprites[cpuCharacterIdx]);
        boardManager.PlaceDiscWithSprite(3, 4, 1, characterDiscSprites[playerCharacterIdx]);
        boardManager.PlaceDiscWithSprite(4, 3, 1, characterDiscSprites[playerCharacterIdx]);
        boardManager.PlaceDiscWithSprite(4, 4, 2, characterDiscSprites[cpuCharacterIdx]);
    }

    void Update()
    {
        if (isGameOver)
        {
            // KO 박스 통통 튀기
            if (isKO && koBoxInstance)
            {
                koBounceTimer += Time.deltaTime;
                float bounce = Mathf.Abs(Mathf.Sin(koBounceTimer * Mathf.PI / 1.5f)) * 30f;
                koBoxInstance.transform.localPosition = new Vector3(0, -100f + bounce, 0);
            }
            // 아무 키나 클릭 시 캐릭터 선택 씬으로 이동
            if (Input.anyKeyDown)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterSelectScene");
            }
            return;
        }

        // 1P 차례일 때 자동 데미지 타이머
        if (currentPlayer == 1)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDamageInterval)
            {
                playerHp -= idleDamageValue;
                playerHpBar.value = playerHp;
                if (playerAnimator) playerAnimator.SetTrigger("Hit");
                if (sfxSource && hitSfx) sfxSource.PlayOneShot(hitSfx);
                StartCoroutine(BarFlash(playerHpBar));
                idleTimer = 0f;
                // 안내 메시지
                resultText.text = $"3초 미입력! 1P 데미지 {idleDamageValue}";
                // HP 0 이하 시 즉시 게임 종료
                if (playerHp <= 0) { EndGame(); return; }
            }
        }
        else
        {
            idleTimer = 0f;
        }

        if (Input.GetMouseButtonDown(0) && resultText.text == "")
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.RoundToInt(mousePos.x);
            int y = Mathf.RoundToInt(mousePos.y);

            if (IsValidMove(x, y, currentPlayer))
            {
                // 연속 턴 체크
                if (lastMovePlayer == currentPlayer)
                    consecutiveTurnCount++;
                else
                    consecutiveTurnCount = 0;
                lastMovePlayer = currentPlayer;

                PlaceAndFlip(x, y, currentPlayer);
                int nextPlayer = 3 - currentPlayer;
                if (HasValidMove(nextPlayer))
                {
                    currentPlayer = nextPlayer;
                }
                else if (HasValidMove(currentPlayer))
                {
                    // 연속 턴: 배수 데미지 적용
                    consecutiveTurnCount++;
                }
                else
                {
                    EndGame();
                }
                UpdateTurnText();
                ShowCurrentPlayerHighlights();
            }
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
            if (boardManager.board[x, y] == 1) black++;
            if (boardManager.board[x, y] == 2) white++;
        }
        if (playerHp <= 0 || cpuHp <= 0)
        {
            isGameOver = true;
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
            isGameOver = true;
            isFinish = true;
            // FINISH 이펙트/사운드/애니메이션
            if (finishEffectPrefab)
                Instantiate(finishEffectPrefab, Vector3.zero, Quaternion.identity);
            if (sfxSource && finishSfx) sfxSource.PlayOneShot(finishSfx);
            resultText.text = $"FINISH! (흑:{black} 백:{white}) 아무 키나 누르세요";
        }
    }

    bool IsValidMove(int x, int y, int color)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7) return false;
        if (boardManager.board[x, y] != 0) return false;
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

    // 돌 뒤집기 시 체력 연동, 이펙트/사운드/애니메이션 등은 추후 확장
    void PlaceAndFlip(int x, int y, int color)
    {
        int spriteIdx = (color == 1) ? playerCharacterIdx : cpuCharacterIdx;
        boardManager.PlaceDiscWithSprite(x, y, color, characterDiscSprites[spriteIdx]);
        var toFlip = GetFlippableDiscs(x, y, color);
        foreach (var pos in toFlip)
        {
            boardManager.FlipDisc(pos.x, pos.y, color);
            boardManager.discs[pos.x, pos.y].GetComponent<SpriteRenderer>().sprite = characterDiscSprites[spriteIdx];
            boardManager.discs[pos.x, pos.y].PlayFlipEffect();
            if (sfxSource && flipSfx) sfxSource.PlayOneShot(flipSfx);
        }
        // 5개 단위 이펙트/사운드
        if (toFlip.Count > 0 && toFlip.Count % 5 == 0)
        {
            if (sfxSource && specialEffectSfx) sfxSource.PlayOneShot(specialEffectSfx);
            if (specialEffectPrefab)
            {
                var eff = Instantiate(specialEffectPrefab, new Vector3(x, y, 0), Quaternion.identity);
                Destroy(eff, 1.5f);
            }
        }
        // 연속 턴 배수 데미지
        double damage = toFlip.Count;
        if (consecutiveTurnCount > 0)
            damage *= System.Math.Pow(1.5, consecutiveTurnCount);
        int intDamage = (int)System.Math.Round(damage);
        if (color == 1)
        {
            cpuHp -= intDamage;
            cpuHpBar.value = cpuHp;
            if (cpuAnimator) cpuAnimator.SetTrigger("Hit");
            if (sfxSource && hitSfx && intDamage > 0) sfxSource.PlayOneShot(hitSfx);
            StartCoroutine(BarFlash(cpuHpBar));
        }
        else
        {
            playerHp -= intDamage;
            playerHpBar.value = playerHp;
            if (playerAnimator) playerAnimator.SetTrigger("Hit");
            if (sfxSource && hitSfx && intDamage > 0) sfxSource.PlayOneShot(hitSfx);
            StartCoroutine(BarFlash(playerHpBar));
        }
        DecrementSkillCooldowns();
    }

    List<Vector2Int> GetFlippableDiscs(int x, int y, int color)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        int[] dx = {-1,0,1,-1,1,-1,0,1};
        int[] dy = {-1,-1,-1,0,0,1,1,1};
        for (int dir = 0; dir < 8; dir++)
        {
            int nx = x + dx[dir], ny = y + dy[dir];
            List<Vector2Int> temp = new List<Vector2Int>();
            while (nx >= 0 && nx < 8 && ny >= 0 && ny < 8 && boardManager.board[nx, ny] == 3 - color)
            {
                temp.Add(new Vector2Int(nx, ny));
                nx += dx[dir]; ny += dy[dir];
            }
            if (nx >= 0 && nx < 8 && ny >= 0 && ny < 8 && boardManager.board[nx, ny] == color && temp.Count > 0)
                result.AddRange(temp);
        }
        return result;
    }

    // 돌을 놓을 때마다 쿨타임 감소
    void DecrementSkillCooldowns()
    {
        for (int i = 0; i < 2; i++)
        {
            if (playerSkillCooldowns[i] > 0) playerSkillCooldowns[i]--;
            if (cpuSkillCooldowns[i] > 0) cpuSkillCooldowns[i]--;
        }
        UpdateSkillButtons();
    }

    // 스킬 버튼 UI 갱신(사용 가능 시 반짝임, 쿨타임 표시 등)
    void UpdateSkillButtons()
    {
        for (int i = 0; i < 3; i++)
        {
            // 플레이어
            playerSkillButtons[i].interactable = (i < 2 && playerSkillCooldowns[i] == 0) || (i == 2 && !playerSkillUsed[2]);
            var img = playerSkillButtons[i].GetComponent<Image>();
            if (playerSkillButtons[i].interactable)
                img.color = Color.yellow; // 사용 가능 시 반짝임(예시)
            else
                img.color = Color.gray;
            // 쿨타임 텍스트 등은 필요시 추가
            // CPU(관전 모드 등 확장 가능)
            cpuSkillButtons[i].interactable = (i < 2 && cpuSkillCooldowns[i] == 0) || (i == 2 && !cpuSkillUsed[2]);
            var img2 = cpuSkillButtons[i].GetComponent<Image>();
            if (cpuSkillButtons[i].interactable)
                img2.color = Color.yellow;
            else
                img2.color = Color.gray;
        }
    }

    // 스킬 버튼 클릭 시 호출
    public void OnClickPlayerSkill(int idx)
    {
        if (idx < 2)
        {
            if (playerSkillCooldowns[idx] == 0)
            {
                if (idx == 0)
                {
                    // 스킬1: CPU HP 10 감소
                    cpuHp -= 10;
                    cpuHpBar.value = cpuHp;
                    if (sfxSource && skill1Sfx) sfxSource.PlayOneShot(skill1Sfx);
                    if (cpuAnimator) cpuAnimator.SetTrigger("Hit");
                    StartCoroutine(BarFlash(cpuHpBar));
                }
                else if (idx == 1)
                {
                    // 스킬2: 1P HP 10 회복
                    playerHp += 10;
                    if (playerHp > 10000) playerHp = 10000;
                    playerHpBar.value = playerHp;
                    if (sfxSource && skill2Sfx) sfxSource.PlayOneShot(skill2Sfx);
                    if (playerAnimator) playerAnimator.SetTrigger("Heal");
                    StartCoroutine(BarFlash(playerHpBar));
                }
                playerSkillCooldowns[idx] = skillCooldownValue;
            }
        }
        else if (idx == 2 && !playerSkillUsed[2])
        {
            // 궁극기: CPU HP 50 감소 + 이펙트
            cpuHp -= 50;
            cpuHpBar.value = cpuHp;
            if (sfxSource && ultimateSfx) sfxSource.PlayOneShot(ultimateSfx);
            if (ultimateEffectPrefab)
                Instantiate(ultimateEffectPrefab, cpuCharacterImage.transform.position, Quaternion.identity);
            if (cpuAnimator) cpuAnimator.SetTrigger("Hit");
            StartCoroutine(BarFlash(cpuHpBar));
            playerSkillUsed[2] = true;
            playerSkillCooldowns[2] = ultimateCooldownValue;
        }
        UpdateSkillButtons();
    }
    public void OnClickCPUSkill(int idx)
    {
        if (idx < 2)
        {
            if (cpuSkillCooldowns[idx] == 0)
            {
                if (idx == 0)
                {
                    // 스킬1: 1P HP 10 감소
                    playerHp -= 10;
                    playerHpBar.value = playerHp;
                    if (sfxSource && skill1Sfx) sfxSource.PlayOneShot(skill1Sfx);
                    if (playerAnimator) playerAnimator.SetTrigger("Hit");
                    StartCoroutine(BarFlash(playerHpBar));
                }
                else if (idx == 1)
                {
                    // 스킬2: CPU HP 10 회복
                    cpuHp += 10;
                    if (cpuHp > 10000) cpuHp = 10000;
                    cpuHpBar.value = cpuHp;
                    if (sfxSource && skill2Sfx) sfxSource.PlayOneShot(skill2Sfx);
                    if (cpuAnimator) cpuAnimator.SetTrigger("Heal");
                    StartCoroutine(BarFlash(cpuHpBar));
                }
                cpuSkillCooldowns[idx] = skillCooldownValue;
            }
        }
        else if (idx == 2 && !cpuSkillUsed[2])
        {
            // 궁극기: 1P HP 50 감소 + 이펙트
            playerHp -= 50;
            playerHpBar.value = playerHp;
            if (sfxSource && ultimateSfx) sfxSource.PlayOneShot(ultimateSfx);
            if (ultimateEffectPrefab)
                Instantiate(ultimateEffectPrefab, playerCharacterImage.transform.position, Quaternion.identity);
            if (playerAnimator) playerAnimator.SetTrigger("Hit");
            StartCoroutine(BarFlash(playerHpBar));
            cpuSkillUsed[2] = true;
            cpuSkillCooldowns[2] = ultimateCooldownValue;
        }
        UpdateSkillButtons();
    }

    // 체력바 반짝임 코루틴
    System.Collections.IEnumerator BarFlash(Slider bar)
    {
        var img = bar.fillRect.GetComponent<UnityEngine.UI.Image>();
        Color orig = img.color;
        img.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        img.color = orig;
    }
} 
using UnityEngine;
// [ExecuteInEditMode] // 제거
public enum GameMode
{
    PlayerVsCPU,    // 1P vs CPU
    PlayerVsPlayer, // 1P vs 2P
    CPUVsCPU        // CPU vs CPU
}

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }
    
    [Header("선택된 캐릭터")]
    public CharacterData selectedCharacter1P;
    public CharacterData selectedCharacter2P;
    public CharacterData selectedCharacterCPU;
    
    [Header("캐릭터 타입 설정")]
    public CharacterType selectedCharacterType = CharacterType.TypeA;
    
    [Header("게임 모드")]
    public GameMode currentGameMode = GameMode.PlayerVsCPU;
    public GameMode gameMode = GameMode.PlayerVsCPU; // 추가
    
    [Header("접근성 설정")]
    public bool colorBlindMode = false; // 추가
    public float fontSize = 1.0f; // 추가
    
    [Header("주사위 시스템")]
    public int diceResult1P = 0;
    public int diceResult2P = 0;
    public bool isFirstTurnDetermined = false;
    public bool isPlayer1First = true;
    
    [Header("캐릭터 인덱스")]
    public int playerCharacterIdx = 0;
    public int cpuCharacterIdx = 0;
    public int backgroundIdx = 0;
    
    [Header("게임 설정")]
    public float masterVolume = 1.0f;
    public float bgmVolume = 0.8f;
    public float sfxVolume = 1.0f;
    public int screenWidth = 1920;
    public int screenHeight = 1080;
    public bool isFullscreen = true;
    
    [Header("테스트 모드")]
    public bool isTestMode = false; // 테스트 모드 활성화 플래그
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void LoadSettings() // 접근 제한자를 public으로 변경
    {
        // 설정값 로드
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        screenWidth = PlayerPrefs.GetInt("ScreenWidth", 1920);
        screenHeight = PlayerPrefs.GetInt("ScreenHeight", 1080);
        isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        
        // 캐릭터 타입 설정 로드
        int typeIndex = PlayerPrefs.GetInt("CharacterType", 0);
        selectedCharacterType = (CharacterType)typeIndex;
        
        // 게임 모드 로드
        int gameModeIndex = PlayerPrefs.GetInt("GameMode", 0);
        currentGameMode = (GameMode)gameModeIndex;
    }
    
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("ScreenWidth", screenWidth);
        PlayerPrefs.SetInt("ScreenHeight", screenHeight);
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.SetInt("CharacterType", (int)selectedCharacterType);
        PlayerPrefs.SetInt("GameMode", (int)currentGameMode);
        PlayerPrefs.Save();
    }
    
    public void SetCharacterType(CharacterType type)
    {
        selectedCharacterType = type;
        PlayerPrefs.SetInt("CharacterType", (int)type);
        PlayerPrefs.Save();
    }
    
    public CharacterType GetCharacterType()
    {
        return selectedCharacterType;
    }
    
    public void SetGameMode(GameMode mode)
    {
        currentGameMode = mode;
        PlayerPrefs.SetInt("GameMode", (int)mode);
        PlayerPrefs.Save();
    }
    
    public GameMode GetGameMode()
    {
        return currentGameMode;
    }
    
    // 주사위 굴리기
    public int RollDice()
    {
        return Random.Range(1, 7); // 1~6
    }
    
    // 선공 결정
    public void DetermineFirstTurn()
    {
        diceResult1P = RollDice();
        diceResult2P = RollDice();
        
        if (diceResult1P > diceResult2P)
        {
            isPlayer1First = true;
        }
        else if (diceResult2P > diceResult1P)
        {
            isPlayer1First = false;
        }
        else
        {
            // 같으면 랜덤
            isPlayer1First = Random.Range(0, 2) == 0;
        }
        
        isFirstTurnDetermined = true;
        
        Debug.Log($"주사위 결과 - 1P: {diceResult1P}, 2P: {diceResult2P}, 선공: {(isPlayer1First ? "1P" : "2P")}");
    }
    
    // 게임 모드에 따른 캐릭터 가져오기
    public CharacterData GetPlayer1Character()
    {
        return selectedCharacter1P;
    }
    
    public CharacterData GetPlayer2Character()
    {
        switch (currentGameMode)
        {
            case GameMode.PlayerVsPlayer:
                return selectedCharacter2P;
            case GameMode.PlayerVsCPU:
            case GameMode.CPUVsCPU:
                return selectedCharacterCPU;
            default:
                return selectedCharacterCPU;
        }
    }
    
    // 현재 게임 모드가 2P 모드인지 확인
    public bool IsTwoPlayerMode()
    {
        return currentGameMode == GameMode.PlayerVsPlayer;
    }
    
    // 현재 게임 모드가 CPU vs CPU인지 확인
    public bool IsCPUVsCPUMode()
    {
        return currentGameMode == GameMode.CPUVsCPU;
    }
    
    // 테스트 모드 확인
    public bool IsTestMode()
    {
        return isTestMode;
    }
    
    // 테스트 모드 토글
    public void ToggleTestMode()
    {
        isTestMode = !isTestMode;
        Debug.Log($"[GameData] 🧪 테스트 모드: {(isTestMode ? "활성화" : "비활성화")}");
        
        if (isTestMode)
        {
            Debug.Log("[GameData] 🎥 테스트 모드 활성화 - 오셀로 돌 놓을 때마다 카메라가 자동으로 이동합니다.");
        }
    }
} 
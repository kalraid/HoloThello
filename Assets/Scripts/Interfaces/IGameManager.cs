using UnityEngine;

/// <summary>
/// 게임 관리자 인터페이스
/// 게임의 핵심 로직을 관리하는 모든 클래스가 구현해야 하는 인터페이스
/// </summary>
public interface IGameManager
{
    /// <summary>
    /// 게임 상태
    /// </summary>
    GameState CurrentGameState { get; }
    
    /// <summary>
    /// 현재 플레이어 (1: 플레이어1, 2: 플레이어2/CPU)
    /// </summary>
    int CurrentPlayer { get; }
    
    /// <summary>
    /// 플레이어1 HP
    /// </summary>
    int Player1HP { get; }
    
    /// <summary>
    /// 플레이어2/CPU HP
    /// </summary>
    int Player2HP { get; }
    
    /// <summary>
    /// 게임 시작
    /// </summary>
    void StartGame();
    
    /// <summary>
    /// 게임 일시정지
    /// </summary>
    void PauseGame();
    
    /// <summary>
    /// 게임 재개
    /// </summary>
    void ResumeGame();
    
    /// <summary>
    /// 게임 종료
    /// </summary>
    void EndGame();
    
    /// <summary>
    /// 플레이어1에게 데미지 적용
    /// </summary>
    /// <param name="damage">데미지량</param>
    void ApplyDamageToPlayer1(int damage);
    
    /// <summary>
    /// 플레이어2/CPU에게 데미지 적용
    /// </summary>
    /// <param name="damage">데미지량</param>
    void ApplyDamageToPlayer2(int damage);
    
    /// <summary>
    /// 스킬 사용
    /// </summary>
    /// <param name="skillIndex">스킬 인덱스 (0-2)</param>
    void UseSkill(int skillIndex);
    
    /// <summary>
    /// 턴 변경
    /// </summary>
    void SwitchTurn();
    
    /// <summary>
    /// 게임 재시작
    /// </summary>
    void RestartGame();
    
    /// <summary>
    /// UI 업데이트
    /// </summary>
    void UpdateUI();
}

/// <summary>
/// 게임 상태 열거형
/// </summary>
public enum GameState
{
    /// <summary>
    /// 게임 진행 중
    /// </summary>
    Playing,
    
    /// <summary>
    /// 게임 일시정지
    /// </summary>
    Paused,
    
    /// <summary>
    /// 게임 종료
    /// </summary>
    GameOver,
    
    /// <summary>
    /// 게임 초기화 중
    /// </summary>
    Initializing
} 
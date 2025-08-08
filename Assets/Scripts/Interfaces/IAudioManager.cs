using UnityEngine;

/// <summary>
/// 오디오 관리자 인터페이스
/// 오디오 시스템을 관리하는 모든 클래스가 구현해야 하는 인터페이스
/// </summary>
public interface IAudioManager
{
    /// <summary>
    /// 마스터 볼륨
    /// </summary>
    float MasterVolume { get; set; }
    
    /// <summary>
    /// BGM 볼륨
    /// </summary>
    float BGMVolume { get; set; }
    
    /// <summary>
    /// SFX 볼륨
    /// </summary>
    float SFXVolume { get; set; }
    
    /// <summary>
    /// BGM 재생
    /// </summary>
    /// <param name="index">BGM 인덱스</param>
    void PlayBGM(int index);
    
    /// <summary>
    /// 다음 BGM 재생
    /// </summary>
    void PlayNextBGM();
    
    /// <summary>
    /// BGM 정지
    /// </summary>
    void StopBGM();
    
    /// <summary>
    /// BGM 일시정지/재개
    /// </summary>
    void ToggleBGM();
    
    /// <summary>
    /// 효과음 재생
    /// </summary>
    /// <param name="clip">오디오 클립</param>
    void PlaySFX(AudioClip clip);
    
    /// <summary>
    /// 버튼 클릭 사운드 재생
    /// </summary>
    void PlayButtonClick();
    
    /// <summary>
    /// 스킬 사용 사운드 재생
    /// </summary>
    /// <param name="level">스킬 레벨</param>
    void PlaySkillUse(int level = 1);
    
    /// <summary>
    /// 데미지 사운드 재생
    /// </summary>
    void PlayDamage();
    
    /// <summary>
    /// 돌 뒤집기 사운드 재생
    /// </summary>
    void PlayFlip();
    
    /// <summary>
    /// K.O 사운드 재생
    /// </summary>
    void PlayKO();
    
    /// <summary>
    /// FINISH 사운드 재생
    /// </summary>
    void PlayFinish();
    
    /// <summary>
    /// 승리 사운드 재생
    /// </summary>
    void PlayVictory();
    
    /// <summary>
    /// 패배 사운드 재생
    /// </summary>
    void PlayDefeat();
    
    /// <summary>
    /// 클릭 사운드 재생
    /// </summary>
    void PlayClick();
    
    /// <summary>
    /// 에러 사운드 재생
    /// </summary>
    void PlayError();
    
    /// <summary>
    /// 특별 효과음 재생
    /// </summary>
    void PlaySpecialEffect();
    
    /// <summary>
    /// 연속 턴 효과음 재생
    /// </summary>
    void PlayConsecutiveTurnEffect();
    
    /// <summary>
    /// 볼륨 설정 저장
    /// </summary>
    void SaveVolumeSettings();
    
    /// <summary>
    /// 볼륨 설정 로드
    /// </summary>
    void LoadVolumeSettings();
} 
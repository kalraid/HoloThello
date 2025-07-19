using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("오디오 믹서")]
    public AudioMixer audioMixer;
    
    [Header("배경음악")]
    public AudioSource bgmSource;
    public AudioClip[] bgmClips;
    private int currentBGMIndex = 0;
    
    [Header("효과음")]
    public AudioSource sfxSource;
    public AudioClip buttonClickSfx;
    public AudioClip skillUseSfx;
    public AudioClip damageSfx;
    public AudioClip flipSfx;
    public AudioClip koSfx;
    public AudioClip finishSfx;
    public AudioClip victorySfx;
    public AudioClip defeatSfx;
    
    [Header("볼륨 설정")]
    public float masterVolume = 1.0f;
    public float bgmVolume = 0.8f;
    public float sfxVolume = 1.0f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeAudio()
    {
        // 오디오 소스가 없으면 생성
        if (bgmSource == null)
        {
            GameObject bgmGO = new GameObject("BGMSource");
            bgmGO.transform.SetParent(transform);
            bgmSource = bgmGO.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
        }
        
        if (sfxSource == null)
        {
            GameObject sfxGO = new GameObject("SFXSource");
            sfxGO.transform.SetParent(transform);
            sfxSource = sfxGO.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
        
        // 볼륨 설정 로드
        LoadVolumeSettings();
        
        // 배경음악 시작
        PlayBGM(0);
    }
    
    void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        
        ApplyVolumeSettings();
    }
    
    void ApplyVolumeSettings()
    {
        // 마스터 볼륨
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20f);
        }
        
        // BGM 볼륨
        if (bgmSource != null)
        {
            bgmSource.volume = bgmVolume * masterVolume;
        }
        
        // SFX 볼륨
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume * masterVolume;
        }
    }
    
    // 배경음악 재생
    public void PlayBGM(int index)
    {
        if (bgmClips != null && index >= 0 && index < bgmClips.Length)
        {
            currentBGMIndex = index;
            bgmSource.clip = bgmClips[index];
            bgmSource.Play();
        }
    }
    
    // 다음 배경음악
    public void PlayNextBGM()
    {
        int nextIndex = (currentBGMIndex + 1) % bgmClips.Length;
        PlayBGM(nextIndex);
    }
    
    // 배경음악 정지
    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }
    
    // 배경음악 일시정지/재개
    public void ToggleBGM()
    {
        if (bgmSource != null)
        {
            if (bgmSource.isPlaying)
            {
                bgmSource.Pause();
            }
            else
            {
                bgmSource.UnPause();
            }
        }
    }
    
    // 효과음 재생
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    // 버튼 클릭 사운드
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSfx);
    }
    
    // 스킬 사용 사운드
    public void PlaySkillUse()
    {
        PlaySFX(skillUseSfx);
    }
    
    // 데미지 사운드
    public void PlayDamage()
    {
        PlaySFX(damageSfx);
    }
    
    // 돌 뒤집기 사운드
    public void PlayFlip()
    {
        PlaySFX(flipSfx);
    }
    
    // K.O. 사운드
    public void PlayKO()
    {
        PlaySFX(koSfx);
    }
    
    // FINISH 사운드
    public void PlayFinish()
    {
        PlaySFX(finishSfx);
    }
    
    // 승리 사운드
    public void PlayVictory()
    {
        PlaySFX(victorySfx);
    }
    
    // 패배 사운드
    public void PlayDefeat()
    {
        PlaySFX(defeatSfx);
    }
    
    // 클릭 사운드
    public void PlayClick()
    {
        PlaySFX(buttonClickSfx);
    }
    
    // 에러 사운드
    public void PlayError()
    {
        PlaySFX(buttonClickSfx); // 임시로 클릭 사운드 사용
    }
    
    // 볼륨 설정
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.Save();
        ApplyVolumeSettings();
    }
    
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.Save();
        ApplyVolumeSettings();
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
        ApplyVolumeSettings();
    }
    
    // 볼륨 가져오기
    public float GetMasterVolume()
    {
        return masterVolume;
    }
    
    public float GetBGMVolume()
    {
        return bgmVolume;
    }
    
    public float GetSFXVolume()
    {
        return sfxVolume;
    }
    
    // 특별 효과음 (5배수 데미지 등)
    public void PlaySpecialEffect()
    {
        // 특별 효과음을 위한 오버레이 사운드
        if (sfxSource != null)
        {
            // 피치를 높여서 특별한 효과
            float originalPitch = sfxSource.pitch;
            sfxSource.pitch = 1.5f;
            PlaySFX(skillUseSfx);
            StartCoroutine(ResetPitch(originalPitch));
        }
    }
    
    System.Collections.IEnumerator ResetPitch(float originalPitch)
    {
        yield return new WaitForSeconds(0.5f);
        if (sfxSource != null)
        {
            sfxSource.pitch = originalPitch;
        }
    }
    
    // 연속 턴 효과음
    public void PlayConsecutiveTurnEffect()
    {
        if (sfxSource != null)
        {
            // 연속 턴을 나타내는 특별한 사운드
            float originalPitch = sfxSource.pitch;
            sfxSource.pitch = 0.8f;
            PlaySFX(damageSfx);
            StartCoroutine(ResetPitch(originalPitch));
        }
    }
} 
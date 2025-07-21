using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic; // List 사용을 위해 추가

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
    
    [Header("오디오 소스 풀")]
    private List<AudioSource> sfxSourcePool = new List<AudioSource>();
    private int sfxPoolSize = 10; // 동시에 재생 가능한 효과음 수

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
        
        // SFX 소스 풀 생성
        if (sfxSource == null) // 대표 sfxSource가 없다면 하나 생성
        {
            sfxSource = CreateNewSfxSource();
        }
        sfxSourcePool.Add(sfxSource);

        for (int i = 0; i < sfxPoolSize - 1; i++)
        {
            sfxSourcePool.Add(CreateNewSfxSource());
        }

        // 믹서 그룹 연결
        SetupMixerGroups();

        // 볼륨 설정 로드 및 적용
        LoadVolumeSettings();
        
        // 배경음악 시작
        PlayBGM(0);
    }

    AudioSource CreateNewSfxSource()
    {
        GameObject sfxGO = new GameObject("SFXSource");
        sfxGO.transform.SetParent(transform);
        AudioSource source = sfxGO.AddComponent<AudioSource>();
        source.loop = false;
        source.playOnAwake = false;
        return source;
    }

    void SetupMixerGroups()
    {
        if (audioMixer == null) return;

        AudioMixerGroup[] bgmGroups = audioMixer.FindMatchingGroups("BGM");
        if (bgmGroups.Length > 0 && bgmSource != null)
        {
            bgmSource.outputAudioMixerGroup = bgmGroups[0];
        }

        AudioMixerGroup[] sfxGroups = audioMixer.FindMatchingGroups("SFX");
        if (sfxGroups.Length > 0)
        {
            foreach (var source in sfxSourcePool)
            {
                if(source != null) source.outputAudioMixerGroup = sfxGroups[0];
            }
        }
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
    
    // 효과음 재생 로직 (풀링 사용)
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        // 사용 가능한 (현재 재생 중이 아닌) 오디오 소스를 찾음
        AudioSource sourceToUse = null;
        foreach (var source in sfxSourcePool)
        {
            if (source != null && !source.isPlaying)
            {
                sourceToUse = source;
                break;
            }
        }

        // 모든 소스가 사용 중이면 새로 생성 (혹은 가장 오래된 소리 중단)
        if (sourceToUse == null)
        {
            sourceToUse = CreateNewSfxSource();
            sfxSourcePool.Add(sourceToUse);
            // 믹서 그룹 재설정
            if (audioMixer != null)
            {
                AudioMixerGroup[] sfxGroups = audioMixer.FindMatchingGroups("SFX");
                if (sfxGroups.Length > 0) sourceToUse.outputAudioMixerGroup = sfxGroups[0];
            }
        }

        if (sourceToUse != null)
        {
            sourceToUse.clip = clip;
            sourceToUse.volume = sfxVolume * masterVolume; // 볼륨 재설정
            sourceToUse.pitch = 1f; // 피치 초기화
            sourceToUse.Play();
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
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
    
    [Header("오디오 소스 풀 설정")]
    public GameObject audioSourcePrefab; // 오디오 소스 프리팹
    public int maxConcurrentSFX = 10; // 동시 재생 가능한 효과음 수
    private Queue<AudioSource> availableAudioSources = new Queue<AudioSource>();
    private List<AudioSource> activeAudioSources = new List<AudioSource>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
            Debug.Log("[AudioManager] 새로운 AudioManager 인스턴스 생성됨");
        }
        else
        {
            Debug.Log("[AudioManager] 기존 AudioManager가 존재하여 새 인스턴스 제거");
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
        
        // SFX 소스 풀 초기화
        InitializeAudioSourcePool();

        // 믹서 그룹 연결
        SetupMixerGroups();

        // 볼륨 설정 로드 및 적용
        LoadVolumeSettings();
        
        // 배경음악 시작 (기존 BGM 중지 후)
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
        PlayBGM(0);
    }

    void InitializeAudioSourcePool()
    {
        // ObjectPool에 오디오 소스 풀 등록
        if (ObjectPool.Instance != null && audioSourcePrefab != null)
        {
            ObjectPool.Instance.AddPool("AudioSource", audioSourcePrefab, maxConcurrentSFX);
        }
        else
        {
            // ObjectPool이 없으면 수동으로 풀 생성
            for (int i = 0; i < maxConcurrentSFX; i++)
            {
                AudioSource source = CreateNewSfxSource();
                availableAudioSources.Enqueue(source);
            }
        }
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

    AudioSource GetAudioSource()
    {
        if (ObjectPool.Instance != null)
        {
            GameObject pooledObj = ObjectPool.Instance.SpawnFromPool("AudioSource", Vector3.zero, Quaternion.identity);
            if (pooledObj != null)
            {
                AudioSource source = pooledObj.GetComponent<AudioSource>();
                if (source != null)
                {
                    activeAudioSources.Add(source);
                    return source;
                }
            }
        }
        
        // ObjectPool이 없으면 기존 방식 사용
        if (availableAudioSources.Count > 0)
        {
            AudioSource source = availableAudioSources.Dequeue();
            activeAudioSources.Add(source);
            return source;
        }
        
        // 풀이 비어있으면 새로 생성
        AudioSource newSource = CreateNewSfxSource();
        activeAudioSources.Add(newSource);
        return newSource;
    }

    void ReturnAudioSource(AudioSource source)
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.ReturnToPool("AudioSource", source.gameObject);
        }
        else
        {
            availableAudioSources.Enqueue(source);
        }
        
        activeAudioSources.Remove(source);
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
            // 모든 활성 오디오 소스에 SFX 그룹 적용
            foreach (AudioSource source in activeAudioSources)
            {
                source.outputAudioMixerGroup = sfxGroups[0];
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
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
            audioMixer.SetFloat("BGMVolume", Mathf.Log10(bgmVolume) * 20);
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
        }
        else
        {
            // 믹서가 없으면 직접 볼륨 설정
            if (bgmSource != null) bgmSource.volume = masterVolume * bgmVolume;
            foreach (AudioSource source in activeAudioSources)
            {
                source.volume = masterVolume * sfxVolume;
            }
        }
    }
    
    public void PlayBGM(int index)
    {
        if (bgmClips != null && index >= 0 && index < bgmClips.Length && bgmSource != null)
        {
            // 기존 BGM 중지
            StopBGM();
            
            currentBGMIndex = index;
            bgmSource.clip = bgmClips[index];
            bgmSource.Play();
            
            Debug.Log($"[AudioManager] BGM 재생: {bgmClips[index].name}");
        }
    }
    
    public void PlayNextBGM()
    {
        int nextIndex = (currentBGMIndex + 1) % bgmClips.Length;
        PlayBGM(nextIndex);
    }
    
    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
            Debug.Log("[AudioManager] BGM 중지됨");
        }
    }
    
    public void StopAllAudio()
    {
        // BGM 중지
        StopBGM();
        
        // 모든 활성 SFX 중지
        foreach (AudioSource source in activeAudioSources)
        {
            if (source != null && source.isPlaying)
            {
                source.Stop();
            }
        }
        
        Debug.Log("[AudioManager] 모든 오디오 중지됨");
    }
    
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
    
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        
        AudioSource source = GetAudioSource();
        if (source != null)
        {
            source.clip = clip;
            source.volume = masterVolume * sfxVolume;
            source.Play();
            
            // 재생 완료 후 풀로 반환
            StartCoroutine(ReturnAudioSourceAfterPlay(source, clip.length));
        }
    }
    
    System.Collections.IEnumerator ReturnAudioSourceAfterPlay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay + 0.1f); // 약간의 여유 시간 추가
        ReturnAudioSource(source);
    }
    
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSfx);
    }
    
    public void PlaySkillUse(int level = 1)
    {
        PlaySFX(skillUseSfx);
        // 레벨에 따른 피치 조절
        if (level > 1)
        {
            StartCoroutine(PlaySkillUseWithPitch(level));
        }
    }
    
    System.Collections.IEnumerator PlaySkillUseWithPitch(int level)
    {
        AudioSource source = GetAudioSource();
        if (source != null)
        {
            float originalPitch = source.pitch;
            source.pitch = originalPitch * (1f + (level - 1) * 0.2f);
            source.clip = skillUseSfx;
            source.volume = masterVolume * sfxVolume;
            source.Play();
            
            yield return new WaitForSeconds(skillUseSfx.length + 0.1f);
            source.pitch = originalPitch;
            ReturnAudioSource(source);
        }
    }
    
    public void PlayDamage()
    {
        PlaySFX(damageSfx);
    }
    
    public void PlayFlip()
    {
        PlaySFX(flipSfx);
    }
    
    public void PlayKO()
    {
        PlaySFX(koSfx);
    }
    
    public void PlayFinish()
    {
        PlaySFX(finishSfx);
    }
    
    public void PlayVictory()
    {
        PlaySFX(victorySfx);
    }
    
    public void PlayDefeat()
    {
        PlaySFX(defeatSfx);
    }
    
    public void PlayClick()
    {
        PlaySFX(buttonClickSfx);
    }
    
    public void PlayError()
    {
        // 에러 사운드 (버튼 클릭과 다른 피치)
        AudioSource source = GetAudioSource();
        if (source != null && buttonClickSfx != null)
        {
            source.clip = buttonClickSfx;
            source.pitch = 0.8f; // 낮은 피치로 에러 느낌
            source.volume = masterVolume * sfxVolume;
            source.Play();
            
            StartCoroutine(ReturnAudioSourceAfterPlay(source, buttonClickSfx.length));
        }
    }
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        ApplyVolumeSettings();
    }
    
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        ApplyVolumeSettings();
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        ApplyVolumeSettings();
    }
    
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
    
    public void PlaySpecialEffect()
    {
        AudioSource source = GetAudioSource();
        if (source != null && skillUseSfx != null)
        {
            source.clip = skillUseSfx;
            source.pitch = 1.5f; // 높은 피치
            source.volume = masterVolume * sfxVolume;
            source.Play();
            
            StartCoroutine(ResetPitchAndReturn(source, skillUseSfx.length, 1.0f));
        }
    }
    
    System.Collections.IEnumerator ResetPitchAndReturn(AudioSource source, float delay, float originalPitch)
    {
        yield return new WaitForSeconds(delay + 0.1f);
        source.pitch = originalPitch;
        ReturnAudioSource(source);
    }
    
    public void PlayConsecutiveTurnEffect()
    {
        AudioSource source = GetAudioSource();
        if (source != null && skillUseSfx != null)
        {
            source.clip = skillUseSfx;
            source.pitch = 1.2f;
            source.volume = masterVolume * sfxVolume * 0.8f;
            source.Play();
            
            StartCoroutine(ResetPitchAndReturn(source, skillUseSfx.length, 1.0f));
        }
    }
    
    // 메모리 정리
    void OnDestroy()
    {
        // 활성 오디오 소스들을 풀로 반환
        foreach (AudioSource source in activeAudioSources)
        {
            if (source != null)
            {
                ReturnAudioSource(source);
            }
        }
    }
}
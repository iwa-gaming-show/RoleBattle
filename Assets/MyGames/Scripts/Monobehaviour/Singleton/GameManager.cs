using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using static InitializationData;
using static PlayerPrefsKey;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager _instance;

    [SerializeField]
    [Header("キャラクターリストのスクリプタブルオブジェクトを設定")]
    SelectableCharacterList _selectableCharacterList;

    [SerializeField]
    [Header("SEリストのスクリプタブルオブジェクトを設定")]
    SEList _seList;

    [SerializeField]
    [Header("BGMリストのスクリプタブルオブジェクトを設定")]
    BgmList _bgmList;

    [SerializeField]
    [Header("SEの音量を設定")]
    [Range(0, 1.0f)]
    float _seVolume = 1.0f;
    [SerializeField]
    [Header("Bgmの音量を設定")]
    [Range(0, 1.0f)]
    float _bgmVolume = 1.0f;

    AudioSource _seAudioSource;
    AudioSource _bgmAudioSource;


    public SelectableCharacterList SelectableCharacterList => _selectableCharacterList;
    public float SEVolume => _seVolume;
    public float BgmVolume => _bgmVolume;

    private void Awake()
    {
        //シングルトン化する
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            _seAudioSource = gameObject.AddComponent<AudioSource>();
            _bgmAudioSource = gameObject.AddComponent<AudioSource>();
            SetAudioVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// シーンを読み込みます
    /// </summary>
    /// <param name="scene"></param>
    public void ClickToLoadScene(SceneType scene)
    {
        LoadScene(scene).Forget();
    }

    async UniTask LoadScene(SceneType scene)
    {
        await Fade._instance.StartFadeOut();
        SceneManager.LoadScene(CommonAttribute.GetStringValue(scene));
    }

    /// <summary>
    /// 音量を設定します
    /// </summary>
    public void SetAudioVolume()
    {
        SetSE();
        SetBgm();
    }

    /// <summary>
    /// SEの設定をします
    /// </summary>
    void SetSE()
    {
        if (PlayerPrefs.HasKey(SE_VOLUME))
            _seVolume = PlayerPrefs.GetFloat(SE_VOLUME);

        _seAudioSource.playOnAwake = false;
    }

    /// <summary>
    /// Bgmの設定をします
    /// </summary>
    void SetBgm()
    {
        if (PlayerPrefs.HasKey(BGM_VOLUME))
            _bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME);

        _bgmAudioSource.playOnAwake = false;
        _bgmAudioSource.volume = _bgmVolume;
        _bgmAudioSource.loop = true;
    }

    /// <summary>
    /// seの音量を設定します
    /// </summary>
    /// <param name="seVolume"></param>
    public void SetSEVolume(float seVolume)
    {
        _seVolume = seVolume;
    }

    /// <summary>
    /// bgmの音量を設定します
    /// </summary>
    /// <param name="bgmValue"></param>
    public void SetBgmVolume(float bgmValue)
    {
        _bgmVolume = bgmValue;
        _bgmAudioSource.volume = _bgmVolume;
    }

    /// <summary>
    /// SEを再生します
    /// </summary>
    /// <param name="seType"></param>
    public void PlaySE(SEType seType)
    {
        AudioClip seClip = _seList.FindSEClipByType(seType);
        if (seClip == null) return;

        _seAudioSource.PlayOneShot(seClip, _seVolume);
    }

    /// <summary>
    /// BGMを再生します
    /// </summary>
    public void PlayBgm(BgmType bgmType)
    {
        AudioClip bgmClip = _bgmList.FindBgmClipByType(bgmType);
        if (bgmClip == null) return;

        //流れているbgmを停止
        _bgmAudioSource.Stop();
        //曲を設定し、再生
        _bgmAudioSource.clip = bgmClip;
        _bgmAudioSource.Play();
    }

    /// <summary>
    /// プレイヤー名を取得します
    /// </summary>
    /// <returns></returns>
    public string GetPlayerName()
    {
        if (PlayerPrefs.HasKey(PLAYER_NAME))
            return PlayerPrefs.GetString(PLAYER_NAME);

        return PLAYER_NAME_FOR_UNEDITED_PLAYER;
    }

    /// <summary>
    /// プレイヤーのキャラクターを取得します
    /// </summary>
    /// <returns></returns>
    public SelectableCharacter GetPlayerCharacter()
    {
        int searchId;
        if (PlayerPrefs.HasKey(SELECTED_CHARACTER_ID))
            searchId = PlayerPrefs.GetInt(SELECTED_CHARACTER_ID);
        else
            searchId = CHARACTER_ID_FOR_UNSELECTED_PLAYER;//未選択時はフェンサーのidを指定する

        return _selectableCharacterList.FindCharacterById(searchId);
    }

    /// <summary>
    /// idからキャラクターを取得します
    /// </summary>
    /// <param name="characterId"></param>
    /// <returns></returns>
    public SelectableCharacter FindCharacterById(int characterId)
    {
        return _selectableCharacterList.FindCharacterById(characterId);
    }

    /// <summary>
    /// ランダムなプレイヤーのキャラクターを取得します
    /// </summary>
    /// <returns></returns>
    public SelectableCharacter GetRandomPlayerCharacter()
    {
        return _selectableCharacterList.GetRandomPlayerCharacter();
    }
}
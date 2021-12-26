using UnityEngine;
using UnityEngine.UI;
using static SEType;
using static PlayerPrefsKey;

public class AudioOption : MonoBehaviour,
    IGameOption,
    IToggleable
{
    [SerializeField]
    [Header("SettingCanvasのTransformを設定する")]
    Transform _settingCanvasTransform;

    [SerializeField]
    [Header("Bgmのスライダーを設定する")]
    Slider _bgmSlider;

    [SerializeField]
    [Header("SEのスライダーを設定する")]
    Slider _seSlider;

    //入力内容を保持
    float _seValue;
    float _bgmValue;
    bool _isEdited;

    public bool IsEdited => _isEdited;

    void Start()
    {
        InitAudioOption(GameManager._instance);
    }

    /// <summary>
    /// サウンドの初期設定をします
    /// </summary>
    void InitAudioOption(GameManager gm)
    {
        _seValue = gm.SEVolume;
        _seSlider.value = ConvertToSlider(gm.SEVolume, _seSlider);
        _bgmValue = gm.BgmVolume;
        _bgmSlider.value = ConvertToSlider(gm.BgmVolume, _bgmSlider);
    }

    /// <summary>
    /// 変更を保存する
    /// </summary>
    public bool Save()
    {
        //未編集なら保存扱いにして何もしない
        if (_isEdited == false) return true;

        bool isSESaved = SaveSEVolume();
        bool isBgmSaved = SaveBgmVolume();

        return (isSESaved && isBgmSaved);  
    }

    /// <summary>
    /// SEの音量を保存する
    /// </summary>
    /// <returns>saved</returns>
    bool SaveSEVolume()
    {
        //未編集なら保存扱いにして何もしない
        if (_seValue == PlayerPrefs.GetFloat(SE_VOLUME)) return true;
        PlayerPrefs.SetFloat(SE_VOLUME, _seValue);
        PlayerPrefs.Save();
        return true;
    }

    /// <summary>
    /// Bgmの音量を保存する
    /// </summary>
    /// <returns></returns>
    bool SaveBgmVolume()
    {
        //未編集なら保存扱いにして何もしない
        if (_bgmValue == PlayerPrefs.GetFloat(BGM_VOLUME)) return true;
        PlayerPrefs.SetFloat(BGM_VOLUME, _bgmValue);
        PlayerPrefs.Save();
        return true;
    }

    /// <summary>
    /// 入力でSEを設定します
    /// </summary>
    public void OnInputToSetSEVolume()
    {
        _seValue = ConvertToAudio(_seSlider);
        GameManager._instance.SetSEVolume(_seValue);
        GameManager._instance.PlaySE(OPTION_CLICK);
        _isEdited = true;
    }

    /// <summary>
    /// 入力でBgmを設定します
    /// </summary>
    public void OnInputToSetBgmVolume()
    {
        _bgmValue = ConvertToAudio(_bgmSlider);
        GameManager._instance.SetBgmVolume(_bgmValue);
        _isEdited = true;
    }

    /// <summary>
    /// オーディオの値をスライダー用に変換します
    /// </summary>
    /// <param name="seVolume"></param>
    /// <returns></returns>
    float ConvertToSlider(float audioVolume, Slider slider)
    {
        return audioVolume * slider.maxValue;//スライドを1~10の整数値で動かしたいため変換する
    }

    /// <summary>
    /// スライドの値をオーディオ用に変換します
    /// </summary>
    /// <param name="slideValue"></param>
    /// <returns></returns>
    float ConvertToAudio(Slider slider)
    {
        //スライド中は音がうるさいので整数値(1 ~ 10)でスライドさせる。
        //オーディオのvolume値に合わせるため最大値で割って小数点に変換する。例: 1 / 10 = 0.1f
        return slider.value / slider.maxValue;
    }

    /// <summary>
    /// UIの表示の切り替えを行います
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleUI(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(gameObject, isActive, _settingCanvasTransform);
    }
}

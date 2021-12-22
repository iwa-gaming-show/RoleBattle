using System.Collections;
using System.Collections.Generic;
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
        _bgmSlider.value = gm.BgmVolume;
    }

    /// <summary>
    /// 変更を保存する
    /// </summary>
    public bool Save()
    {
        //未編集なら保存扱いにして何もしない
        if (_seValue == PlayerPrefs.GetFloat(SE_VOLUME)) return true;

        PlayerPrefs.SetFloat(SE_VOLUME, _seValue);
        PlayerPrefs.Save();
        GameManager._instance.SetAudioVolume();//設定した値を反映するため
        return true;
    }

    /// <summary>
    /// 入力でSEを設定します
    /// </summary>
    public void OnInputToSetSEVolume()
    {
        _seValue = ConvertToSE(_seSlider);
        GameManager._instance.PlaySE(OPTION_CLICK, _seValue);
    }

    /// <summary>
    /// SEの値をスライダー用に変換します
    /// </summary>
    /// <param name="seVolume"></param>
    /// <returns></returns>
    float ConvertToSlider(float seVolume, Slider slider)
    {
        return seVolume * slider.maxValue;//スライドを1~10の整数値で動かしたいため変換する
    }

    /// <summary>
    /// スライドの値をSE用に変換します
    /// </summary>
    /// <param name="slideValue"></param>
    /// <returns></returns>
    float ConvertToSE(Slider slider)
    {
        //スライド中は音がうるさいので整数値(1 ~ 10)でスライドさせる。
        //seのvolume値に合わせるため最大値で割って小数点に変換する例: 1 / 10 = 0.1f
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

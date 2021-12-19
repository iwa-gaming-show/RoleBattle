using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOption : MonoBehaviour,
    IGameOption
{
    [SerializeField]
    [Header("SettingCanvasのTransformを設定する")]
    Transform _settingCanvasTransform;

    [SerializeField]
    [Header("CharacterOptionを設定する")]
    PlayerCharacterOption _playerCharacterOption;

    IGameOption _IplayerCharacterOption;

    void Start()
    {
        _IplayerCharacterOption = _playerCharacterOption;
    }

    /// <summary>
    /// 変更を保存する
    /// </summary>
    public bool Save()
    {
        //todo プレイヤー名の保存
        return _IplayerCharacterOption.Save();
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

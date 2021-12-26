using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOption : MonoBehaviour,
    IGameOption,
    IToggleable
{
    [SerializeField]
    [Header("SettingCanvasのTransformを設定する")]
    Transform _settingCanvasTransform;

    [SerializeField]
    [Header("CharacterOptionを設定する")]
    PlayerCharacterOption _playerCharacterOption;

    [SerializeField]
    [Header("PlayerNameを設定する")]
    EditPlayerNameField _editPlayerNameField;

    IGameOption _IplayerCharacterOption;
    IGameOption _IEditPlayerNameField;

    public bool IsEdited => (_IplayerCharacterOption.IsEdited || _IEditPlayerNameField.IsEdited);

    void Start()
    {
        _IplayerCharacterOption = _playerCharacterOption;
        _IEditPlayerNameField = _editPlayerNameField;
    }

    /// <summary>
    /// 変更を保存する
    /// </summary>
    public bool Save()
    {
        return (
            _IEditPlayerNameField.Save()
            && _IplayerCharacterOption.Save()
        );
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

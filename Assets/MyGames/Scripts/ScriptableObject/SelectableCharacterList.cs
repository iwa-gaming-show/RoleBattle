using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectableCharacterList", menuName = "Create SelectableCharacterList")]
public class SelectableCharacterList : ScriptableObject
{
    [SerializeField]
    List<SelectableCharacter> _selectableCharacterList = new List<SelectableCharacter>();

    public List<SelectableCharacter> GetSelectableCharacterList => _selectableCharacterList;
}

[System.Serializable]
public class SelectableCharacter
{
    [SerializeField]
    [Tooltip("キャラクター名")]
    string _name = "キャラクター名";

    [SerializeField]
    [Tooltip("キャラ説明文")]
    string _description;

    [SerializeField]
    [Tooltip("サイズとアイコン")]
    List<Icons> _icons;

    [SerializeField]
    [Tooltip("シチュエーションと音声")]
    List<Voices> _voices;
}

[System.Serializable]
public class Icons
{
    [SerializeField]
    [Tooltip("サイズ")]
    CharacterIconSizes _size;

    [SerializeField]
    [Tooltip("アイコン")]
    Sprite _icon;
}

[System.Serializable]
public class Voices
{
    [SerializeField]
    [Tooltip("シチュエーション")]
    CharacterVoiceSituations _situation;

    [SerializeField]
    [Tooltip("音声")]
    string _audio;
}
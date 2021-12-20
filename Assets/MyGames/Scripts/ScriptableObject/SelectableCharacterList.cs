using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectableCharacterList", menuName = "Create SelectableCharacterList")]
public class SelectableCharacterList : ScriptableObject
{
    [SerializeField]
    List<SelectableCharacter> _selectableCharacterList = new List<SelectableCharacter>();

    public List<SelectableCharacter> GetSelectableCharacterList => _selectableCharacterList;

    /// <summary>
    /// idからキャラクターを取得します
    /// </summary>
    /// <param name="characterId"></param>
    /// <returns></returns>
    public SelectableCharacter FindCharacterById(int characterId)
    {
        try
        {
            return _selectableCharacterList.Find(target => target.Id == characterId);
        }
        catch
        {
            Debug.Log("キャラクターが見つかりませんでした。");
            return null;
        }
    }

    /// <summary>
    /// ランダムなプレイヤーのキャラクターを取得します
    /// </summary>
    /// <returns></returns>
    public SelectableCharacter GetRandomPlayerCharacter()
    {
        return _selectableCharacterList[Random.Range(0, _selectableCharacterList.Count)];
    }
}

[System.Serializable]
public class SelectableCharacter
{
    [SerializeField]
    [Tooltip("id")]
    int _id;

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

    #region //プロパティ
    public int Id => _id;
    public string Name => _name;
    public string Description => _description;
    public List<Icons> Icons => _icons;
    public List<Voices> Voices => _voices;
    #endregion

    /// <summary>
    /// サイズによるアイコン画像を探します
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public Sprite FindIconImageBy(CharacterIconSizes size)
    {
        try
        {
            Icons icons = _icons.Find(icon => icon.Size == size);
            return icons.Icon;
        }
        catch
        {
            Debug.Log("画像が見つかりませんでした");
            return null;
        }
    }
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

    #region //プロパティ
    public CharacterIconSizes Size => _size;
    public Sprite Icon => _icon;
    #endregion
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

    #region //プロパティ
    public CharacterVoiceSituations Situation => _situation;
    public string Audio => _audio;
    #endregion
}
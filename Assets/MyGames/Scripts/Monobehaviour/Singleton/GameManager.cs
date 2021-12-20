using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InitializationData;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager _instance;

    [SerializeField]
    [Header("キャラクターリストのスクリプタブルオブジェクトを設定")]
    SelectableCharacterList _selectableCharacterList;

    public SelectableCharacterList SelectableCharacterList => _selectableCharacterList;

    private void Awake()
    {
        //シングルトン化する
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// プレイヤー名を取得します
    /// </summary>
    /// <returns></returns>
    public string GetPlayerName()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
            return PlayerPrefs.GetString("PlayerName");

        return PLAYER_NAME_FOR_UNEDITED_PLAYER;
    }

    /// <summary>
    /// プレイヤーのキャラクターを取得します
    /// </summary>
    /// <returns></returns>
    public SelectableCharacter GetPlayerCharacter()
    {
        int searchId;
        if (PlayerPrefs.HasKey("SelectedCharacterId"))
            searchId = PlayerPrefs.GetInt("SelectedCharacterId");
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
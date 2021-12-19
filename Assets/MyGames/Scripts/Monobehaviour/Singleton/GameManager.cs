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
}
using UnityEngine;
using UnityEngine.UI;
using static InitializationData;
using static SEType;
using static PlayerPrefsKey;

public class EditPlayerNameField : MonoBehaviour,
    IGameOption
{
    [SerializeField]
    [Header("表示用フィールド設定")]
    GameObject _viewField;

    [SerializeField]
    [Header("プレイヤー名の表示用テキストを設定")]
    Text _playerName;

    [SerializeField]
    [Header("編集用テキストフィールドを設定")]
    InputField _editInputField;

    bool _isEdited;

    public bool IsEdited => _isEdited;

    // Start is called before the first frame update
    void Start()
    {
        InitPlayerName(GameManager._instance.GetPlayerName());
    }

    /// <summary>
    /// プレイヤー名を表示します
    /// </summary>
    void InitPlayerName(string playerName)
    {
        //保存データがなければ編集フィールドを表示します
        if (playerName == PLAYER_NAME_FOR_UNEDITED_PLAYER)
            ActivateEditOrView(true);
        else
            _playerName.text = playerName;
    }

    /// <summary>
    /// Inputフィールドの入力完了時、プレイヤー名を変更する
    /// </summary>
    public void OnCompleteInputToSetPlayerName()
    {
        string input = _editInputField.textComponent.text;

        if (ValidateString(input))
            ReflectPlayerName(input);
        else
            DoWhenDisabled();
    }

    /// <summary>
    /// 文字列の入力チェックをします
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    bool ValidateString(string input)
    {
        //nullや空文字では無ければtrue
        return (string.IsNullOrWhiteSpace(input) == false);
    }

    /// <summary>
    /// 入力が無効の時の処理
    /// </summary>
    void DoWhenDisabled()
    {
        //すでに入力されていたらなら設定せずに編集画面を閉じる
        if (ValidateString(_playerName.text))
            ActivateEditOrView(false);
        else
            Debug.Log("入力内容は有効ではありません");
    }

    /// <summary>
    /// プレイヤー名を反映します
    /// </summary>
    void ReflectPlayerName(string input)
    {
        //プレイヤー名を設定し、編集画面を閉じる
        _playerName.text = input;
        _isEdited = true;
        ActivateEditOrView(false);
    }

    /// <summary>
    /// クリックで編集画面を表示します
    /// </summary>
    public void OnClickToShowEditField()
    {
        GameManager._instance.PlaySE(OPTION_CLICK);
        ActivateEditOrView(true);
    }

    /// <summary>
    /// 編集フィールド、または表示フィールドをアクティブにします
    /// </summary>
    /// <param name="isActive"></param>
    void ActivateEditOrView(bool isActivateEditField)
    {
        //編集フィールドがアクティブなら表示フィールドは閉じます
        ToggleEditInputField(isActivateEditField);
        ToggleViewField(isActivateEditField == false);
    }

    /// <summary>
    /// プレイヤー名UIの表示の切り替えをします
    /// </summary>
    /// <param name="isActive"></param>
    void ToggleViewField(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(_viewField, isActive, transform);
    }

    /// <summary>
    /// 編集UIの表示の切り替えをします
    /// </summary>
    /// <param name="isActive"></param>
    void ToggleEditInputField(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(_editInputField.gameObject, isActive, transform);
    }

    /// <summary>
    /// 変更を保存する
    /// </summary>
    /// <returns>保存の成功フラグ</returns>
    public bool Save()
    {
        //未編集なら保存扱いにして何もしない
        if (_playerName.text == PLAYER_NAME_FOR_UNEDITED_PLAYER) return true;
        if (_playerName.text == PlayerPrefs.GetString(PLAYER_NAME)) return true;
        if (_isEdited == false) return true;

        PlayerPrefs.SetString(PLAYER_NAME , _playerName.text);
        PlayerPrefs.Save();
        return true;
    }
}

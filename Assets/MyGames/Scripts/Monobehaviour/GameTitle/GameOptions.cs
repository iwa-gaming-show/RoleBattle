using UnityEngine;

public class GameOptions : MonoBehaviour
{
    GameOptionMenu _selectedMenu;

    [SerializeField]
    [Header("playerOptionのUIを設定")]
    PlayerOption _playerOption;

    [SerializeField]
    [Header("AudioOptionのUIを設定")]
    AudioOption _audioOption;

    IGameOption _IplayerOption;
    IGameOption _IaudioOption;

    void Start()
    {
        _IplayerOption = _playerOption;
        _IaudioOption = _audioOption;
    }

    /// <summary>
    /// playerOptionを表示する
    /// </summary>
    public void OnClickToShowPlayerOption()
    {
        if (_selectedMenu == GameOptionMenu.PLAYER) return;
        _selectedMenu = GameOptionMenu.PLAYER;

        _IplayerOption.Show();
        _IaudioOption.Close();
    }

    /// <summary>
    /// audioOptionを表示する
    /// </summary>
    public void OnClickToShowAudioOption()
    {
        if (_selectedMenu == GameOptionMenu.AUDIO) return;
        _selectedMenu = GameOptionMenu.AUDIO;

        _IplayerOption.Close();
        _IaudioOption.Show();
    }

    /// <summary>
    /// オプション画面を閉じる
    /// </summary>
    public void OnClickToCloseOptionWindow()
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(gameObject, false, transform);
    }
}

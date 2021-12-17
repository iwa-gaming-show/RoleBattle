using UnityEngine;

public class GameOptions : MonoBehaviour, IToggleable
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

        _IplayerOption.ToggleUI(true);
        _IaudioOption.ToggleUI(false);
    }

    /// <summary>
    /// audioOptionを表示する
    /// </summary>
    public void OnClickToShowAudioOption()
    {
        if (_selectedMenu == GameOptionMenu.AUDIO) return;
        _selectedMenu = GameOptionMenu.AUDIO;

        _IaudioOption.ToggleUI(true);
        _IplayerOption.ToggleUI(false);
    }

    /// <summary>
    /// オプション画面の表示切り替え
    /// </summary>
    public void OnClickToToggleOptionWindow(bool isActive)
    {
        ToggleUI(isActive);
    }

    /// <summary>
    /// オプション画面の表示切り替え
    /// </summary>
    public void ToggleUI(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(gameObject, isActive, transform);
    }
}

using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using static SEType;
using static WaitTimes;

public class GameOptions : MonoBehaviour, IToggleable
{
    GameOptionMenu _selectedMenu;

    [SerializeField]
    [Header("playerOptionのUIを設定")]
    PlayerOption _playerOption;

    [SerializeField]
    [Header("AudioOptionのUIを設定")]
    AudioOption _audioOption;

    [SerializeField]
    [Header("保存通知のダイアログを設定する")]
    GameObject _savedDialog;

    IGameOption _IplayerOption;
    IGameOption _IaudioOption;
    IToggleable _ItoggleablePlayerOp;
    IToggleable _ItoggleableAudioOp;

    void Start()
    {
        InitSetInterface();
    }

    void InitSetInterface()
    {
        _IplayerOption = _playerOption;
        _IaudioOption = _audioOption;
        _ItoggleablePlayerOp = _playerOption;
        _ItoggleableAudioOp = _audioOption;
    }

    /// <summary>
    /// playerOptionを表示する
    /// </summary>
    public void OnClickToShowPlayerOption()
    {
        if (_selectedMenu == GameOptionMenu.PLAYER) return;
        _selectedMenu = GameOptionMenu.PLAYER;

        GameManager._instance.PlaySE(OPTION_CLICK);
        _ItoggleablePlayerOp.ToggleUI(true);
        _ItoggleableAudioOp.ToggleUI(false);
    }

    /// <summary>
    /// audioOptionを表示する
    /// </summary>
    public void OnClickToShowAudioOption()
    {
        if (_selectedMenu == GameOptionMenu.AUDIO) return;
        _selectedMenu = GameOptionMenu.AUDIO;

        GameManager._instance.PlaySE(OPTION_CLICK);
        _ItoggleableAudioOp.ToggleUI(true);
        _ItoggleablePlayerOp.ToggleUI(false);
    }

    /// <summary>
    /// オプション画面の表示切り替え
    /// </summary>
    public void OnClickToToggleOptionWindow(bool isActive)
    {
        GameManager._instance.PlaySE(OPTION_CLICK);
        ToggleUI(isActive);
    }

    /// <summary>
    /// オプション画面の表示切り替え
    /// </summary>
    public void ToggleUI(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(gameObject, isActive, transform);
    }

    /// <summary>
    /// 設定を保存する
    /// </summary>
    public void OnClickToSave()
    {
        bool isPlayerOptionSaved = _IplayerOption.Save();
        bool isAudioOptionSaved = _IaudioOption.Save();

        if (isPlayerOptionSaved && isAudioOptionSaved)
        {
            SaveComplete().Forget();
        }
        else
        {
            //todo 保存無効時の処理
            Debug.Log("保存ができませんでした");
        }
    }

    /// <summary>
    /// 保存を完了する
    /// </summary>
    async UniTask SaveComplete()
    {
        GameManager._instance.PlaySE(SAVE_CLICK);
        await ViewSavedDialog();
        ToggleUI(false);
    }

    /// <summary>
    /// 保存通知のダイアログを表示する
    /// </summary>
    /// <returns></returns>
    async UniTask ViewSavedDialog()
    {
        ToggleDisplaySavedDialog(true);
        await UniTask.Delay(TimeSpan.FromSeconds(SAVED_DIALOG_DISPLAY_TIME));
        ToggleDisplaySavedDialog(false);
        await UniTask.Yield();
    }

    /// <summary>
    /// 保存通知のダイアログの表示を切り替える
    /// </summary>
    /// <param name="isActive"></param>
    void ToggleDisplaySavedDialog(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(_savedDialog, isActive, transform);
    }
}

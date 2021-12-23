using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using static SEType;
using static BgmType;
using static SceneType;

public class GameTitle : MonoBehaviour
{
    [SerializeField]
    [Header("OptionCanvasを設定")]
    GameObject _optionCanvas;

    [SerializeField]
    [Header("最初からフェードインが完了しているか")]
    bool _isFirstFadeComplete;

    bool _isFirstClick;

    void Start()
    {
        Fade._instance.StartFadeIn().Forget();
        GameManager._instance.PlayBgm(BgmType.GAME_TITLE);
        //最初はオプションをオフにする
        _optionCanvas.SetActive(false);
    }

    /// <summary>
    /// CPUバトルを開始する
    /// </summary>
    public void OnClickToStartCpuBattle()
    {
        if (_isFirstClick) return;
        _isFirstClick = true;
        GameManager._instance.PlaySE(SEType.BATTLE);
        GameManager._instance.ClickToLoadScene(Battle);
    }

    /// <summary>
    /// マルチバトルを開始する
    /// </summary>
    public void OnClickToStartMultiBattle()
    {
        if (_isFirstClick) return;
        _isFirstClick = true;
        GameManager._instance.PlaySE(SEType.BATTLE);
        GameManager._instance.ClickToLoadScene(MultiBattle);
    }

    /// <summary>
    /// オプション画面を表示する
    /// </summary>
    public void OnClickToShowOptionWindow()
    {
        GameManager._instance.PlaySE(OPTION_CLICK);
        CanvasForObjectPool._instance.ToggleUIGameObject(_optionCanvas, true, transform);
    }
}
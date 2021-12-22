using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTitle : MonoBehaviour
{
    [SerializeField]
    [Header("OptionCanvasを設定")]
    GameObject _optionCanvas;

    bool isFirstClick;

    void Start()
    {
        //最初はオプションをオフにする
        _optionCanvas.SetActive(false);
    }

    /// <summary>
    /// CPUバトルを開始する
    /// </summary>
    public void OnClickToStartCpuBattle()
    {
        ClickToLoadScene(SceneType.Battle);
    }

    /// <summary>
    /// マルチバトルを開始する
    /// </summary>
    public void OnClickToStartMultiBattle()
    {
        ClickToLoadScene(SceneType.MultiBattle);
    }

    /// <summary>
    /// オプション画面を表示する
    /// </summary>
    public void OnClickToShowOptionWindow()
    {
        GameManager._instance.PlaySE(SEType.OPTION_CLICK);
        CanvasForObjectPool._instance.ToggleUIGameObject(_optionCanvas, true, transform);
    }

    /// <summary>
    /// シーンを読み込みます
    /// </summary>
    /// <param name="scene"></param>
    void ClickToLoadScene(SceneType scene)
    {
        if (isFirstClick) return;
        isFirstClick = true;

        SceneManager.LoadScene(CommonAttribute.GetStringValue(scene));
    }
}
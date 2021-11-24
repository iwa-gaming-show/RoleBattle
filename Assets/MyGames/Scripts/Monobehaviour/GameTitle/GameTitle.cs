using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTitle : MonoBehaviour
{
    bool isFirstClick;

    /// <summary>
    /// CPUバトルを開始する
    /// </summary>
    public void OnClickCpuBattleStart()
    {
        ClickToLoadScene(SceneType.Battle);
    }

    /// <summary>
    /// マルチバトルを開始する
    /// </summary>
    public void OnClickMultiBattleStart()
    {
        ClickToLoadScene(SceneType.MultiBattle);
    }

    void ClickToLoadScene(SceneType scene)
    {
        if (isFirstClick) return;
        isFirstClick = true;

        SceneManager.LoadScene(CommonAttribute.GetStringValue(scene));
    }
}
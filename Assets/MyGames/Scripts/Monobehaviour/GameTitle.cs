using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTitle : MonoBehaviour
{
    bool isFirstClick;

    /// <summary>
    /// バトルを開始する
    /// </summary>
    public void OnClickCpuBattleStart()
    {
        if (isFirstClick) return;
        isFirstClick = true;

        SceneManager.LoadScene(CommonAttribute.GetStringValue(SceneType.Battle));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InitializationData;
using GM = GameManager;

public class PointManager : MonoBehaviour
{
    #region インスペクターから設定
    [SerializeField]
    [Header("ラウンド毎の勝者の獲得ポイント")]
    int _earnedPoint = 1;
    #endregion

    /// <summary>
    /// ポイントの加算
    /// </summary>
    /// <param name="isPlayer"></param>
    public void AddPointTo(bool isPlayer)
    {
        if (isPlayer)
        {
            GM._instance.Player.AddPoint(EarnPoint(GM._instance.RoundManager.IsUsingPlayerSkillInRound));
            return;
        }

        GM._instance.Enemy.AddPoint(EarnPoint(GM._instance.RoundManager.IsUsingEnemySkillInRound));
    }

    /// <summary>
    /// 獲得ポイント
    /// </summary>
    /// <returns></returns>
    int EarnPoint(bool isUsingSkillInRound)
    {
        //このラウンドの間必殺技を使用していた場合
        if (isUsingSkillInRound)
        {
            return _earnedPoint * SPECIAL_SKILL_MAGNIFICATION_BONUS;
        }
        return _earnedPoint;
    }
}

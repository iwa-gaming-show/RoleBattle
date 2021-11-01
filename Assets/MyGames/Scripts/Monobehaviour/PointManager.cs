using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InitializationData;

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
    public void AddPointTo(PlayerData targetPlayer, bool isUsingSkillInRound)
    {
        targetPlayer.AddPoint(EarnPoint(isUsingSkillInRound));
    }

    /// <summary>
    /// 獲得ポイント
    /// </summary>
    /// <returns></returns>
    public int EarnPoint(bool isUsingSkillInRound)
    {
        //このラウンドの間必殺技を使用していた場合
        if (isUsingSkillInRound)
        {
            return _earnedPoint * SPECIAL_SKILL_MAGNIFICATION_BONUS;
        }
        return _earnedPoint;
    }
}

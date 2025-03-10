using ExitGames.Client.Photon;
using Photon.Realtime;
using static InitializationData;

public static class RoomPropertiesExtensions
{
    static readonly Hashtable propsToSet = new Hashtable();
    static readonly string RoundCountKey = "RoundCount";
    static readonly string BattlePhaseKey = "BattlePhase";
    static readonly string EarnedPointKey = "EarnedPoint";
    static readonly string IsDuringDirectingSpSkillKey = "IsDuringDirectingSpSkill";

    /// <summary>
    /// ルームのラウンド数を取得する
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public static int GetRoundCount(this Room room)
    {
        return (room.CustomProperties[RoundCountKey] is int roundCount) ? roundCount : INITIAL_ROUND_COUNT;
    }

    /// <summary>
    /// ルームのラウンド数を設定する
    /// </summary>
    /// <param name="room"></param>
    /// <param name="roundCount"></param>
    public static void SetRoundCount(this Room room, int roundCount)
    {
        propsToSet[RoundCountKey] = roundCount;
        room.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// バトルのフェーズを取得する
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public static int GetIntBattlePhase(this Room room)
    {
        return (room.CustomProperties[BattlePhaseKey] is int battlePhase) ? battlePhase : 0;
    }

    /// <summary>
    /// バトルのフェーズを設定する
    /// </summary>
    /// <param name="room"></param>
    /// <param name="battlePhase"></param>
    public static void SetIntBattlePhase(this Room room, BattlePhase battlePhase)
    {
        propsToSet[BattlePhaseKey] = (int)battlePhase;
        room.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// 獲得ポイントを取得する
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public static int GetEarnedPoint(this Room room)
    {
        return (room.CustomProperties[EarnedPointKey] is int earnedPoint) ? earnedPoint : 1;
    }

    /// <summary>
    /// 獲得ポイントを設定する
    /// </summary>
    /// <param name="room"></param>
    /// <param name="earnedPoint"></param>
    public static void SetEarnedPoint(this Room room, int earnedPoint)
    {
        propsToSet[EarnedPointKey] = earnedPoint;
        room.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// 必殺技発動の演出中かどうかを取得する
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public static bool GetIsDuringDirecting(this Room room)
    {
        return (room.CustomProperties[IsDuringDirectingSpSkillKey] is bool isDuringDirectingSpSkill) ? isDuringDirectingSpSkill : false;
    }

    /// <summary>
    /// 必殺技発動の演出中かどうかを設定する
    /// </summary>
    /// <param name="room"></param>
    /// <param name="isDuringDirectingSpSkill"></param>
    public static void SetIsDuringDirecting(this Room room, bool isDuringDirectingSpSkill)
    {
        propsToSet[IsDuringDirectingSpSkillKey] = isDuringDirectingSpSkill;
        room.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }
}

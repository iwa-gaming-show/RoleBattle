using ExitGames.Client.Photon;
using Photon.Realtime;
using static InitializationData;

public static class PlayerPropertiesExtensions
{
    static readonly Hashtable propsToSet = new Hashtable();
    static readonly string PointKey = "Point";
    static readonly string CanUseSpSkillKey = "CanUseSpSkill";
    static readonly string IsMyTurnKey = "IsMyTurn";
    static readonly string IsMyTurnEndKey = "IsMyTurnEnd";
    static readonly string IsUsingSpInRoundKey = "IsUsingSpInRound";
    static readonly string IsFieldCardPlacedKey = "IsFieldCardPlaced";
    static readonly string IsCardJudgedKey = "IsCardJudged";
    static readonly string IsRetryingBattleKey = "IsRetryingBattle";
    static readonly string BattleCardTypeKey = "BattleCardType";
    static readonly string IsSelectedCharacterIdKey = "IsSelectedCharacterId";

    /// <summary>
    /// 選択したキャラクターのIDを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static int GetIsSelectedCharacterId(this Player player)
    {
        return (player.CustomProperties[IsSelectedCharacterIdKey] is int characterId) ? characterId : CHARACTER_ID_FOR_UNSELECTED_PLAYER;
    }

    /// <summary>
    /// 選択したキャラクターのIDを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="cardType"></param>
    public static void SetIsSelectedCharacterId(this Player player, int characterId)
    {
        propsToSet[IsSelectedCharacterIdKey] = characterId;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// プレイヤーのポイントを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static int GetPoint(this Player player)
    {
        return (player.CustomProperties[PointKey] is int point) ? point : 0;
    }

    /// <summary>
    /// プレイヤーのポイントを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="point"></param>
    public static void SetPoint(this Player player, int point)
    {
        propsToSet[PointKey] = point;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// プレイヤーの必殺技発動可能フラグを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool GetCanUseSpSkill(this Player player)
    {
        return (player.CustomProperties[CanUseSpSkillKey] is bool canUseSpSkill) ? canUseSpSkill : false;
    }

    /// <summary>
    /// プレイヤーの必殺技発動可能フラグを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="canUseSpSkill"></param>
    public static void SetCanUseSpSkill(this Player player, bool canUseSpSkill)
    {
        propsToSet[CanUseSpSkillKey] = canUseSpSkill;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// プレイヤーのターンかどうかを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool GetIsMyTurn(this Player player)
    {
        return (player.CustomProperties[IsMyTurnKey] is bool isMyTurn) ? isMyTurn : false;
    }

    /// <summary>
    /// プレイヤーのターンかどうかを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isMyTurn"></param>
    public static void SetIsMyTurn(this Player player, bool isMyTurn)
    {
        propsToSet[IsMyTurnKey] = isMyTurn;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    // <summary>
    /// プレイヤーのターンが終了したかどうかを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool GetIsMyTurnEnd(this Player player)
    {
        return (player.CustomProperties[IsMyTurnEndKey] is bool isMyTurnEnd) ? isMyTurnEnd : false;
    }

    /// <summary>
    /// プレイヤーのターンが終了したかどうかを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isMyTurnEnd"></param>
    public static void SetIsMyTurnEnd(this Player player, bool isMyTurnEnd)
    {
        propsToSet[IsMyTurnEndKey] = isMyTurnEnd;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// 必殺技の発動中かどうかを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool GetIsUsingSpInRound(this Player player)
    {
        return (player.CustomProperties[IsUsingSpInRoundKey] is bool isUsingSpInRound) ? isUsingSpInRound : false;
    }

    /// <summary>
    /// 必殺技の発動中かどうかを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isUsingSpInRound"></param>
    public static void SetIsUsingSpInRound(this Player player, bool isUsingSpInRound)
    {
        propsToSet[IsUsingSpInRoundKey] = isUsingSpInRound;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// フィールドへのカードのタイプを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static int GetIntBattleCardType(this Player player)
    {
        return (player.CustomProperties[BattleCardTypeKey] is int cardType) ? cardType : 0;
    }

    /// <summary>
    /// フィールドへのカードのタイプを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="cardType"></param>
    public static void SetIntBattleCardType(this Player player, CardType cardType)
    {
        propsToSet[BattleCardTypeKey] = (int)cardType;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// カードをフィールドに配置したかどうかを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool GetIsFieldCardPlaced(this Player player)
    {
        return (player.CustomProperties[IsFieldCardPlacedKey] is bool isFieldCardPlaced) ? isFieldCardPlaced : false;
    }

    /// <summary>
    /// カードをフィールドに配置したかどうかを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isFieldCardPlaced"></param>
    public static void SetIsFieldCardPlaced(this Player player, bool isFieldCardPlaced)
    {
        propsToSet[IsFieldCardPlacedKey] = isFieldCardPlaced;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// カードを判定したかどうかを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool GetIsCardJudged(this Player player)
    {
        return (player.CustomProperties[IsCardJudgedKey] is bool isCardJudged) ? isCardJudged : false;
    }

    /// <summary>
    /// カードの判定をしたかどうかを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isCardJudged"></param>
    public static void SetIsCardJudged(this Player player, bool isCardJudged)
    {
        propsToSet[IsCardJudgedKey] = isCardJudged;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// 再戦するかどうかを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool GetIsRetryingBattle(this Player player)
    {
        return (player.CustomProperties[IsRetryingBattleKey] is bool isRetryingBattle) ? isRetryingBattle : false;
    }

    /// <summary>
    /// 再戦するかどうかを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isRetryingBattle"></param>
    public static void SetIsRetryingBattle(this Player player, bool isRetryingBattle)
    {
        propsToSet[IsRetryingBattleKey] = isRetryingBattle;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }
}
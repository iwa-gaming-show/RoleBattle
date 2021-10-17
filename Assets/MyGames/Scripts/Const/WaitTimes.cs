/// <summary>
/// 定数クラス 待機時間のまとまり
/// </summary>
public static class WaitTimes
{
    /// <summary>
    /// カード移動時間
    /// </summary>
    public static readonly float CARD_MOVEMENT_TIME = 0.5f;

    /// <summary>
    /// ターン変更までの時間
    /// </summary>
    public static readonly float TIME_BEFORE_CHANGING_TURN = 2f;

    /// <summary>
    /// ラウンド変更までの時間
    /// </summary>
    public static readonly float TIME_BEFORE_CHANGING_ROUND = 2f;

    /// <summary>
    /// カードの判定結果の表示時間
    /// </summary>
    public static readonly float JUDGMENT_RESULT_DISPLAY_TIME = 1f;

    /// <summary>
    /// カウント数の表示時間
    /// </summary>
    public static readonly float ROUND_COUNT_DISPLAY_TIME = 1f;

    /// <summary>
    /// カードを開くアナウンスの表示時間
    /// </summary>
    public static readonly float ANNOUNCEMENT_TIME_TO_OPEN_CARD = 1f;

    /// <summary>
    /// ターンをアナウンスするテキストの表示時間
    /// </summary>
    public static readonly float ANNOUNCEMENT_TIME_TO_TURN_TEXT = 1f;
}
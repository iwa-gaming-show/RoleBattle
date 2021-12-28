using System.Collections;
using Cysharp.Threading.Tasks;
using Photon.Realtime;

public interface IMultiBattleDataManager
{
    Player Player { get; }
    Player Enemy { get; }
    Room Room { get; }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Init();

    /// <summary>
    /// 対戦相手を設定します
    /// </summary>
    /// <param name="player"></param>
    void SetEnemy(Player enemy);

    /// <summary>
    /// プレイヤーのデータの初期化
    /// </summary>
    void InitPlayerData();

    /// <summary>
    /// ルームのデータの初期化
    /// </summary>
    void InitRoomData();

    /// <summary>
    /// プレイヤーのデータを取得します
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    Player GetPlayerBy(bool isPlayer);

    /// <summary>
    /// 更新プレイヤーかどうか
    /// </summary>
    /// <param name="Player"></param>
    /// <returns></returns>
    bool IsUpdatePlayer(Player targetPlayer);

    /// <summary>
    /// マスタークライアントかどうか
    /// </summary>
    /// <returns></returns>
    bool IsMasterClient();

    /// <summary>
    /// お互いのプレイヤーがカードを判定したかどうか
    /// </summary>
    /// <returns></returns>
    bool IsCardJudgedForEachPlayer();

    /// <summary>
    /// カードの判定をしたかどうかを設定する
    /// </summary>
    /// <param name="Player"></param>
    /// <param name="isCardJudged"></param>
    void SetIsCardJudged(Player player, bool isCardJudged);

    /// <summary>
    /// プレイヤーのターンかどうかを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    bool GetIsMyTurn(Player player);

    /// <summary>
    /// プレイヤーのターンかどうかを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isMyTurn"></param>
    void SetIsMyTurn(Player player, bool isMyTurn);

    // <summary>
    /// プレイヤーのターンが終了したかどうかを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    bool GetIsMyTurnEnd(Player player);

    /// <summary>
    /// プレイヤーのターンが終了したかどうかを設定する
    /// </summary>
    /// <param name="Player"></param>
    /// <param name="isMyTurnEnd"></param>
    void SetIsMyTurnEnd(Player player, bool isMyTurnEnd);

    /// <summary>
    /// お互いのプレイヤーが再戦を希望したかどうか
    /// </summary>
    /// <returns></returns>
    bool IsRetryingBattleForEachPlayer();

    /// <summary>
    /// 再戦するかどうかを設定する
    /// </summary>
    /// <param name="Player"></param>
    /// <param name="isRetryingBattle"></param>
    void SetIsRetryingBattle(Player player, bool isRetryingBattle);

    /// <summary>
    /// プレイヤーの状態をリセットする
    /// </summary>
    void ResetPlayerState();

    /// <summary>
    /// 対戦相手かどうか
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    bool IsEnemy(string userId);

    /// <summary>
    /// マスタークライアントを取得します
    /// </summary>
    /// <returns></returns>
    Player GetMasterClient();

    /// <summary>
    /// 自分以外のプレイヤーを取得します
    /// </summary>
    /// <returns></returns>
    Player GetOtherPlayer();

    /// <summary>
    /// カードをフィールドに配置したかどうかを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    bool GetIsFieldCardPlaced(Player player);

    /// <summary>
    /// カードをフィールドに配置したかどうかを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isFieldCardPlaced"></param>
    void SetIsFieldCardPlaced(Player player, bool isFieldCardPlaced);

    /// <summary>
    /// お互いのプレイヤーがフィールドにカードを出しているか
    /// </summary>
    /// <returns></returns>
    bool IsEachPlayerFieldCardPlaced();

    /// <summary>
    /// プレイヤーのポイントを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    int GetPoint(Player player);

    /// <summary>
    /// プレイヤーのポイントを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="point"></param>
    void SetPoint(Player player, int point);

    /// <summary>
    /// 必殺技の発動中かどうかを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    bool GetIsUsingSpInRound(Player player);

    /// <summary>
    /// 必殺技の発動中かどうかを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isUsingSpInRound"></param>
    void SetIsUsingSpInRound(Player player, bool isUsingSpInRound);

    /// <summary>
    /// フィールドへのカードのタイプを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    int GetIntBattleCardType(Player player);

    /// <summary>
    /// フィールドへのカードのタイプを設定する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="cardType"></param>
    void SetIntBattleCardType(Player player, CardType cardType);

    /// <summary>
    /// プレイヤーの必殺技発動可能フラグを取得する
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    bool GetCanUseSpSkill(Player player);

    /// <summary>
    /// 必殺技を発動する状態にする
    /// </summary>
    void ActivatingSpSkillState(Player player);

    /// <summary>
    /// カードタイプを登録します
    /// </summary>
    void RegisterCardType(Player player, CardType cardType);

    /// <summary>
    /// 自身の選択ターンかどうかを返します
    /// </summary>
    /// <returns></returns>
    bool MySelectionTurn();
}
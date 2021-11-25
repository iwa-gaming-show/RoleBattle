using System.Collections;
using Cysharp.Threading.Tasks;

public interface IBattleUIManager
{
    ConfirmationPanelToField ConfirmationPanelToField
    {
        get;
    }

    /// <summary>
    /// プレイヤーのターン時にテキストを表示する
    /// </summary>
    /// <param name="isPlayer"></param>
    UniTask ShowThePlayerTurnText(bool isPlayer);

    /// <summary>
    /// 必殺技を発動する
    /// </summary>
    UniTask ActivateSpecialSkill(bool isPlayer);

    /// <summary>
    /// ポイントの表示
    /// </summary>
    void ShowPoint(int myPoint, int enemyPoint);

    /// <summary>
    /// ポイントの表示
    /// </summary>
    void ShowPointBy(int point, bool isPlayer);

    /// <summary>
    /// UIデータの初期設定
    /// </summary>
    void InitUIData();

    /// <summary>
    /// 開始時に非表示にするUI
    /// </summary>
    void HideUIAtStart();

    /// <summary>
    /// ラウンド数を表示する
    /// </summary>
    /// <param name="roundCount"></param>
    /// <param name="maxRoundCount"></param>
    UniTask ShowRoundCountText(int roundCount, int maxRoundCount);

    /// <summary>
    /// カウントダウンを表示
    /// </summary>
    void ShowCountDownText(int countDownTime);

    /// <summary>
    /// 確認画面UIを全てを非表示にする
    /// </summary>
    void CloseAllConfirmationPanels();

    /// <summary>
    /// ゲーム結果の表示の切り替え
    /// </summary>
    /// <param name="isAcitve"></param>
    void ToggleGameResultUI(bool isActive);

    /// <summary>
    /// ゲームの勝敗のテキストを表示する
    /// </summary>
    /// <returns></returns>
    void SetGameResultText(string text);

    /// <summary>
    /// ラウンドの勝敗の結果を表示
    /// </summary>
    UniTask ShowJudgementResultText(string result);

    /// <summary>
    /// カードを開くことをアナウンスします
    /// </summary>
    /// <returns></returns>
    UniTask AnnounceToOpenTheCard();

    /// <summary>
    /// フィールドへ移動するカードを選択したとき
    /// </summary>
    public void SelectedToFieldCard(CardController selectedCard);
}
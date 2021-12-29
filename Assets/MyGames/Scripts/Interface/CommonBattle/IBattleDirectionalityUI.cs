using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IBattleDirectionalityUI
{
    /// <summary>
    /// ラウンド数を表示する
    /// </summary>
    /// <param name="roundCount"></param>
    /// <returns></returns>
    UniTask ShowRoundCountText(int roundCount, int maxRoundCount);

    /// <summary>
    /// プレイヤーのターン時にテキストを表示する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    UniTask ShowThePlayerTurnText(bool isPlayer);

    /// <summary>
    /// カウントダウンを表示
    /// </summary>
    void ShowCountDownText(int countDownTime);

    /// <summary>
    /// ラウンドの勝敗の結果を表示
    /// </summary>
    UniTask ShowJudgementResultText(string result);

    /// <summary>
    /// バトル結果の表示
    /// </summary>
    /// <param name="isAcitve"></param>
    void ShowBattleResultUI(bool isActive, string resultText);

    /// <summary>
    /// カードを開くことをアナウンスします
    /// </summary>
    /// <returns></returns>
    UniTask AnnounceToOpenTheCard();

    /// <summary>
    /// 開始時にUIを非表示にします
    /// </summary>
    void HideUIAtStart();

    /// <summary>
    /// 切断されたことをダイアログで表示します
    /// </summary>
    /// <returns></returns>
    UniTask ViewDisconectedDialog();

    /// <summary>
    /// 部屋退室用のボタンの表示を切り替える
    /// </summary>
    /// <param name="isActive"></param>
    void ToggleDisplayLeaveRoomButton(bool isActive);
}
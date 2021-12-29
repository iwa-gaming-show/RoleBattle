using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;
using DG.Tweening;
using static UIStrings;
using static WaitTimes;

public class BattleDirectionalityUI : MonoBehaviour,
    IBattleDirectionalityUI
{
    [SerializeField]
    [Header("ラウンド数表示テキスト")]
    TextMeshProUGUI _roundCountText;

    [SerializeField]
    [Header("カードOPEN時のテキスト")]
    TextMeshProUGUI _openPhaseText;

    [SerializeField]
    [Header("自分のターンであることを知らせるUI")]
    GameObject _announceThePlayerTurn;

    [SerializeField]
    [Header("相手のターンであることを知らせるUI")]
    GameObject _announceTheEnemyTurn;

    [SerializeField]
    [Header("カウントダウンのテキスト")]
    TextMeshProUGUI _countDownText;

    [SerializeField]
    [Header("バトルの勝敗の結果表示用UI")]
    GameObject _battleResultUI;

    [SerializeField]
    [Header("ラウンドの勝敗の結果表示のテキスト")]
    TextMeshProUGUI _judgementResultText;

    [SerializeField]
    [Header("バトルの勝敗の結果表示のテキスト")]
    TextMeshProUGUI _battleResultText;

    [SerializeField]
    [Header("切断通知のダイアログを設定する")]
    GameObject _disconectedDialog;

    [SerializeField]
    [Header("部屋退室用のボタンを設定する")]
    GameObject _leaveRoomButton;

    /// <summary>
    /// ラウンド数を表示する
    /// </summary>
    /// <param name="roundCount"></param>
    /// <returns></returns>
    public async UniTask ShowRoundCountText(int roundCount, int maxRoundCount)
    {
        ToggleRoundCountText(true);
        SetRoundCountText(roundCount, maxRoundCount);

        await UniTask.Delay(TimeSpan.FromSeconds(ROUND_COUNT_DISPLAY_TIME));
        ToggleRoundCountText(false);
    }

    /// <summary>
    /// プレイヤーのターン時にテキストを表示する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public async UniTask ShowThePlayerTurnText(bool isPlayer)
    {
        ToggleAnnounceTurnFor(true, isPlayer);
        await UniTask.Delay(TimeSpan.FromSeconds(ANNOUNCEMENT_TIME_TO_TURN_TEXT));
        ToggleAnnounceTurnFor(false, isPlayer);
    }

    /// <summary>
    /// カウントダウンを表示
    /// </summary>
    public void ShowCountDownText(int countDownTime)
    {
        _countDownText.text = countDownTime.ToString();
    }

    /// <summary>
    /// バトル結果の表示
    /// </summary>
    /// <param name="isAcitve"></param>
    public void ShowBattleResultUI(bool isActive, string resultText)
    {
        ToggleBattleResultUI(true);
        SetBattleResultText(resultText);
    }

    /// <summary>
    /// ラウンド表示用のテキストを設定する
    /// </summary>
    void SetRoundCountText(int roundCount, int maxRoundCount)
    {
        if (roundCount == maxRoundCount)
            _roundCountText.text = FINAL_ROUND;//最終ラウンド
        else
            _roundCountText.text = ROUND_PREFIX + roundCount.ToString();
    }

    /// <summary>
    /// ラウンド数表示用テキストの切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleRoundCountText(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(_roundCountText.gameObject, isActive, transform);
    }

    /// <summary>
    /// カードを開くことをアナウンスします
    /// </summary>
    /// <returns></returns>
    public async UniTask AnnounceToOpenTheCard()
    {
        RectTransform textRectTransform = _openPhaseText.rectTransform;
        float screenEdgeX = UIUtil.GetScreenEdgeXFor(textRectTransform.sizeDelta.x);

        //右端→真ん中→左端へ移動する
        Sequence sequence = DOTween.Sequence();
        sequence.Append(UIUtil.MoveAnchorPosXByDOT(textRectTransform, screenEdgeX, 0)
            .OnStart(() => ToggleOpenPhaseText(true)));

        sequence.Append(UIUtil.MoveAnchorPosXByDOT(textRectTransform, 0f, 0.25f));

        sequence.Append(UIUtil.MoveAnchorPosXByDOT(textRectTransform, -screenEdgeX, 0.4f).SetDelay(1f)
            .OnComplete(() => ToggleOpenPhaseText(false)));

        await sequence
            .Play()
            .AsyncWaitForCompletion();
    }

    /// <summary>
    /// カードOPEN時のテキストの表示の切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleOpenPhaseText(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(_openPhaseText.gameObject, isActive, transform);
    }

    /// <summary>
    /// プレイヤーのターン時に表示するUIの切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleAnnounceTurnFor(bool isActive, bool isPlayer)
    {
        GameObject AnnounceThePlayerTurn = GetAnnounceThePlayerTurnBy(isPlayer);
        CanvasForObjectPool._instance.ToggleUIGameObject(AnnounceThePlayerTurn, isActive, transform);
    }

    /// <summary>
    /// プレイヤーのターンのアナウンス用のゲームオブジェクトを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public GameObject GetAnnounceThePlayerTurnBy(bool isPlayer)
    {
        if (isPlayer) return _announceThePlayerTurn;
        return _announceTheEnemyTurn;
    }

    /// <summary>
    /// 開始時にUIを非表示にします
    /// </summary>
    public void HideUIAtStart()
    {
        CanvasForObjectPool._instance
            .ToggleUIGameObject(_battleResultUI, false, transform);
    }

    /// <summary>
    /// ラウンドの勝敗の結果を表示
    /// </summary>
    public async UniTask ShowJudgementResultText(string result)
    {
        ToggleJudgementResultText(true);
        _judgementResultText.text = result + JUDGEMENT_RESULT_SUFFIX;

        await UniTask.Delay(TimeSpan.FromSeconds(JUDGMENT_RESULT_DISPLAY_TIME));
        ToggleJudgementResultText(false);
    }

    /// <summary>
    /// ラウンドの勝敗の結果の表示の切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleJudgementResultText(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(_judgementResultText.gameObject, isActive, transform);
    }

    /// <summary>
    /// バトル結果の表示の切り替え
    /// </summary>
    /// <param name="isAcitve"></param>
    public void ToggleBattleResultUI(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(_battleResultUI, isActive, transform);
    }

    /// <summary>
    /// バトルの勝敗のテキストを表示する
    /// </summary>
    /// <returns></returns>
    public void SetBattleResultText(string text)
    {
        _battleResultText.text = text;
    }

    /// <summary>
    /// 切断通知のダイアログを表示する
    /// </summary>
    /// <returns></returns>
    public async UniTask ViewDisconectedDialog()
    {
        ToggleDisplayDisconectedDialog(true);
        await UniTask.Delay(TimeSpan.FromSeconds(DISCONECTED_DIALOG_DISPLAY_TIME));
        ToggleDisplayDisconectedDialog(false);
        await UniTask.Yield();
    }

    /// <summary>
    /// 切断通知のダイアログの表示を切り替える
    /// </summary>
    /// <param name="isActive"></param>
    void ToggleDisplayDisconectedDialog(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(_disconectedDialog, isActive, transform);
    }

    /// <summary>
    /// 部屋退室用のボタンの表示を切り替える
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleDisplayLeaveRoomButton(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(_leaveRoomButton, isActive, transform);
    }
}

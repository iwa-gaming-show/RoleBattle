using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using GM = GameManager;
using static UIStrings;
using TMPro;
using static WaitTimes;

public class DirectionUIManager : MonoBehaviour, IHideableUIsAtStart
{
    [SerializeField]
    [Header("ラウンドの勝敗の結果表示のテキスト")]
    TextMeshProUGUI _judgementResultText;

    [SerializeField]
    [Header("ラウンド数表示テキスト")]
    TextMeshProUGUI _roundCountText;

    [SerializeField]
    [Header("ゲームの勝敗の結果表示のテキスト")]
    TextMeshProUGUI _gameResultText;

    [SerializeField]
    [Header("カードOPEN時のテキスト")]
    TextMeshProUGUI _openPhaseText;

    [SerializeField]
    [Header("カウントダウンのテキスト")]
    TextMeshProUGUI _countDownText;

    [SerializeField]
    [Header("自分のターンであることを知らせるUI")]
    GameObject _announceThePlayerTurn;

    [SerializeField]
    [Header("相手のターンであることを知らせるUI")]
    GameObject _announceTheEnemyTurn;

    [SerializeField]
    [Header("ゲームの勝敗の結果表示用UI")]
    GameObject _gameResultUI;

    /// <summary>
    /// 開始時にUIを非表示にする
    /// </summary>
    public void HideUIsAtStart()
    {
        ToggleGameResultUI(false);
        ToggleJudgementResultText(false);
        ToggleRoundCountText(false);
        ToggleAnnounceTurnFor(false, true);
        ToggleAnnounceTurnFor(false, false);
        ToggleOpenPhaseText(false);
    }

    /// <summary>
    /// ラウンド数を表示する
    /// </summary>
    /// <param name="roundCount"></param>
    /// <param name="maxRoundCount"></param>
    /// <returns></returns>
    public async UniTask ShowRoundCountText(int roundCount, int maxRoundCount)
    {
        ToggleRoundCountText(true);
        SetRoundCountText(roundCount, maxRoundCount);

        await UniTask.Delay(TimeSpan.FromSeconds(ROUND_COUNT_DISPLAY_TIME));
        ToggleRoundCountText(false);
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
    /// カードを開くことをアナウンスします
    /// </summary>
    /// <returns></returns>
    public async UniTask AnnounceToOpenTheCard()
    {
        RectTransform textRectTransform = _openPhaseText.rectTransform;
        float screenEdgeX = GM._instance.UIManager.GetScreenEdgeXFor(textRectTransform.sizeDelta.x);

        //右端→真ん中→左端へ移動する
        Sequence sequence = DOTween.Sequence();
        sequence.Append(GM._instance.UIManager.MoveAnchorPosX(textRectTransform, screenEdgeX, 0)
            .OnStart(() => ToggleOpenPhaseText(true)));

        sequence.Append(GM._instance.UIManager.MoveAnchorPosX(textRectTransform, 0f, 0.25f));

        sequence.Append(GM._instance.UIManager.MoveAnchorPosX(textRectTransform, -screenEdgeX, 0.4f).SetDelay(1f)
            .OnComplete(() => ToggleOpenPhaseText(false)));

        await sequence
            .Play()
            .AsyncWaitForCompletion();
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
    /// ゲームの勝敗のテキストを表示する
    /// </summary>
    /// <returns></returns>
    public void SetGameResultText(string text)
    {
        _gameResultText.text = text;
    }

    /// <summary>
    /// ラウンド表示用のテキストを設定する
    /// </summary>
    void SetRoundCountText(int roundCount, int maxRoundCount)
    {
        if (roundCount == maxRoundCount)
        {
            //最終ラウンド
            _roundCountText.text = FINAL_ROUND;
        }
        else
        {
            _roundCountText.text = ROUND_PREFIX + roundCount.ToString();
        }
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
        GameObject AnnounceThePlayerTurnGameObject;

        if (isPlayer)
        {
            AnnounceThePlayerTurnGameObject = _announceThePlayerTurn.gameObject;
        }
        else
        {
            AnnounceThePlayerTurnGameObject = _announceTheEnemyTurn.gameObject;
        }

        CanvasForObjectPool._instance.ToggleUIGameObject(AnnounceThePlayerTurnGameObject, isActive, transform);
    }

    /// <summary>
    /// ゲーム結果の表示の切り替え
    /// </summary>
    /// <param name="isAcitve"></param>
    public void ToggleGameResultUI(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(_gameResultUI, isActive, transform);
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
    /// ラウンド数表示用テキストの切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleRoundCountText(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(_roundCountText.gameObject, isActive, transform);
    }
}

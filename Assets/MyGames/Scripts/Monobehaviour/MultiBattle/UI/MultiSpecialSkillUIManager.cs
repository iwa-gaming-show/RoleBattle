using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static WaitTimes;
using static BattlePhase;
using DG.Tweening;

public class MultiSpecialSkillUIManager : MonoBehaviour, IHideableUIsAtStart
{
    [SerializeField]
    [Header("必殺技発動の確認UI")]
    ConfirmationPanelToSpecialSkill _confirmationPanel;

    [SerializeField]
    [Header("プレイヤーの必殺技発動の演出UI")]
    GameObject _playerProductionToSpecialSkill;

    [SerializeField]
    [Header("エネミーの必殺技発動の演出UI")]
    GameObject _enemyProductionToSpecialSkill;

    [SerializeField]
    [Header("必殺技の詳細を記述するテキストを格納する")]
    Text[] _descriptionsOfSpecialSkill;

    //[SerializeField]
    //[Header("未使用、使用済みの順にアイコン画像を必殺技ボタンに設定する")]
    //Sprite[] _spButtonIcons;

    [SerializeField]
    [Header("必殺技の説明用のテキストを設定")]
    string _specialSkillDescription;

    IBattleManager _battleManager;
    ITurnManager _turnManager;

    #region//プロパティ
    public ConfirmationPanelToSpecialSkill ConfirmationPanel => _confirmationPanel;
    #endregion

    void Start()
    {
        //_battleManager = ServiceLocator.Resolve<IBattleManager>();
        //_turnManager = ServiceLocator.Resolve<ITurnManager>();
    }

    /// <summary>
    /// 開始時にUIを非表示にする
    /// </summary>
    public void HideUIsAtStart()
    {
        ConfirmationPanel.ToggleUI(false);
        //ToggleProductionToSpecialSkill(false, true);
        //ToggleProductionToSpecialSkill(false, false);
    }

    /// <summary>
    /// 必殺技発動の演出
    /// </summary>
    //async UniTask ShowSpecialSkillDirection(bool isPlayer)
    //{
    //    RectTransform targetUIRectTranform = GetProductionToSpecialSkillBy(isPlayer).GetComponent<RectTransform>();
    //    float screenEdgeX = UIUtil.GetScreenEdgeXFor(targetUIRectTranform.sizeDelta.x);
    //    Sequence sequence = DOTween.Sequence();
    //    Tween firstMove;
    //    Tween lastMove;

    //    //プレイヤーなら右から左へ移動, エネミーなら左から右へ移動する
    //    if (isPlayer)
    //    {
    //        firstMove = UIUtil.MoveAnchorPosXByDOT(targetUIRectTranform, screenEdgeX, 0);
    //        lastMove = UIUtil.MoveAnchorPosXByDOT(targetUIRectTranform, -screenEdgeX, 0.4f);
    //    }
    //    else
    //    {
    //        firstMove = UIUtil.MoveAnchorPosXByDOT(targetUIRectTranform, -screenEdgeX, 0f);
    //        lastMove = UIUtil.MoveAnchorPosXByDOT(targetUIRectTranform, screenEdgeX, 0.4f);
    //    }

    //    sequence.Append(firstMove.OnStart(() => ToggleProductionToSpecialSkill(true, isPlayer)));
    //    sequence.Append(UIUtil.MoveAnchorPosXByDOT(targetUIRectTranform, 0f, 0.25f));
    //    sequence.Append(lastMove.SetDelay(SPECIAL_SKILL_PRODUCTION_DISPLAY_TIME).OnComplete(() => ToggleProductionToSpecialSkill(false, isPlayer)));

    //    await sequence
    //       .Play()
    //       .AsyncWaitForCompletion();
    //}

    ///// <summary>
    ///// 必殺技演出UIの表示の切り替え
    ///// </summary>
    ///// <param name="isActive"></param>
    //public void ToggleProductionToSpecialSkill(bool isActive, bool isPlayer)
    //{
    //    CanvasForObjectPool._instance.ToggleUIGameObject(GetProductionToSpecialSkillBy(isPlayer), isActive, transform);
    //}

    /// <summary>
    /// 必殺技演出UIを取得する
    /// </summary>
    /// <param name="isPlayer"></param>
    //GameObject GetProductionToSpecialSkillBy(bool isPlayer)
    //{
    //    if (isPlayer)
    //    {
    //        return _playerProductionToSpecialSkill;
    //    }
    //    return _enemyProductionToSpecialSkill;
    //}

    /// <summary>
    /// 必殺技を発動する
    /// </summary>
    //public async UniTask ActivateSpecialSkill(bool isPlayer)
    //{
    //    //必殺技を使用済みにする
    //    //UsedSpecialSkillButton(isPlayer);
    //    _battleManager.UsedSpecialSkill(isPlayer);

    //    //必殺技を演出、 演出中はカウントダウンが止まる
    //    _battleManager.SetIsDuringProductionOfSpecialSkill(true);
    //    await ShowSpecialSkillDirection(isPlayer);
    //    //カウントダウン再開
    //    _battleManager.SetIsDuringProductionOfSpecialSkill(false);
    //}

    /// <summary>
    /// 必殺技の説明文の設定
    /// </summary>
    public void InitSpecialSkillDescriptions()
    {
        foreach (Text description in _descriptionsOfSpecialSkill)
        {
            description.text = _specialSkillDescription;
        }
    }

    ///// <summary>
    ///// 必殺技ボタンを押した時
    ///// </summary>
    //public void OnClickSpecialSkillButton()
    //{
    //    //自分のターンのみ押せる
    //    if (_turnManager.IsMyTurn == false) return;
    //    //一度使用したら押せない
    //    if (_battleManager.Player.CanUseSpecialSkill == false) return;
    //    //選択フェイズでなければ押せない
    //    if (_battleManager.BattlePhase != SELECTION) return;

    //    _confirmationPanel.ToggleUI(true);
    //}

    /// <summary>
    /// 必殺技ボタンを使用済みにする
    /// </summary>
    //void UsedSpecialSkillButton(bool isPlayer)
    //{
    //    //SetSpecialSkillButtonSpriteForTarget(GetSpecialSkillButtonImageBy(isPlayer), _specialSkillButtonIcons[1]);
    //}

    /// <summary>
    /// 必殺技のIconを設定する
    /// </summary>
    /// <param name="targetImage"></param>
    /// <param name="setSprite"></param>
    //void SetSpecialSkillButtonSpriteForTarget(Image targetImage, Sprite setSprite)
    //{
    //    targetImage.sprite = setSprite;
    //}

    /// <summary>
    /// プレイヤー毎に必殺技のImageを初期化する
    /// </summary>
    //public void InitSpecialSkillButtonImageByPlayers()
    //{
    //    //foreach (Image specialSkillButtonImage in _specialSkillButtonImages)
    //    //{
    //    //    SetSpecialSkillButtonSpriteForTarget(specialSkillButtonImage, _specialSkillButtonIcons[0]);
    //    //}
    //}

    /// <summary>
    /// プレイヤーの必殺技ボタンのImageを取得します
    /// </summary>
    /// <returns></returns>
    //Image GetSpecialSkillButtonImageBy(bool isPlayer)
    //{
    //    //if (isPlayer) return _specialSkillButtonImages[0];
    //    //return _specialSkillButtonImages[1];
    //}
}

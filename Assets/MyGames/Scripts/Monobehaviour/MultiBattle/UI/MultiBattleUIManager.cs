using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Linq;
using UnityEngine.UI;
using static UIStrings;
using static WaitTimes;
using static BattlePhase;

public class MultiBattleUIManager : SuperBattleUIManager
{
    IMultiBattleDataManager _multiBattleDataManager;
    PhotonView _photonView;


    void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    new void Start()
    {
        base.Start();
        _multiBattleDataManager = ServiceLocator.Resolve<IMultiBattleDataManager>();
    }

    #region //override methods
    /// <summary>
    /// カードを移動する
    /// </summary>
    public override async UniTask MoveToBattleField(bool isPlayer, CardController movingCard)
    {
        if (isPlayer)
        {
            //すでにフィールドにカードが置かれているなら何もしない
            if (PhotonNetwork.LocalPlayer.GetIsFieldCardPlaced()) return;

            RegisterCardType(movingCard.CardType);
            //カードを配置済みにする
            PhotonNetwork.LocalPlayer.SetIsFieldCardPlaced(true);
            PhotonNetwork.CurrentRoom.SetIntBattlePhase(SELECTED);

            //対戦相手のゲーム側でenemyのカードを移動させます
            //演出用にランダムなカードを選び移動させます。
            //※実際にフィールドに出すカードは異なります、カンニングを阻止する意味もあります。
            _photonView.RPC("RpcMoveRandomCardToField", RpcTarget.Others, false);
        }

        await GetPlayerUI(isPlayer).MoveToBattleField(movingCard);

        if (isPlayer == false) return;
        await UniTask.Delay(TimeSpan.FromSeconds(TIME_BEFORE_CHANGING_TURN));
        //ターンを終了する
        PhotonNetwork.LocalPlayer.SetIsMyTurnEnd(true);
    }

    /// <summary>
    /// 必殺技を発動する
    /// </summary>
    /// <returns></returns>
    [PunRPC]
    public override async UniTask ActivateSpSkill(bool isPlayer)
    {
        if (isPlayer)
        {
            PhotonNetwork.LocalPlayer.SetIsUsingSpInRound(true);
            PhotonNetwork.LocalPlayer.SetCanUseSpSkill(false);
            PhotonNetwork.CurrentRoom.SetIsDuringDirecting(true);
            //対戦相手側のenemyで必殺技演出を行います
            _photonView.RPC("ActivateSpSkill", RpcTarget.Others, false);
        }

        await GetPlayerUI(isPlayer).ActivateDirectingOfSpSkill(isPlayer);
    }
    #endregion

    /// <summary>
    /// 相手のフィールドのカードを置き換えます
    /// </summary>
    /// <param name="cardType"></param>
    public async UniTask ReplaceEnemyFieldCard(CardType cardType)
    {
        CardController replacingCard = CreateCardFor(cardType, false);
        //元々フィールドに配置したカードは削除します
        _enemyUI.DestroyFieldCard();
        _enemyUI.SetFieldCard(replacingCard);
        await UniTask.Yield();
    }

    /// <summary>
    /// カードの種類に対応したカードを作成します
    /// </summary>
    /// <param name="cardType"></param>
    /// <returns></returns>
    CardController CreateCardFor(CardType cardType, bool isPlayer)
    {
        //cardTypeに対応したカードのindex番号を取得します
        var cardEntities = _cardEntityList.GetCardEntityList
            .Select((ce, i) => new { CardType = ce.CardType, Index = i });

        int cardIndex = (cardEntities.Where(ce => ce.CardType == cardType)
            .First().Index is int index) ? index: 0;

        return CreateCard(cardIndex, isPlayer);
    }

    /// <summary>
    /// ランダムなカードをフィールドに移動します
    /// </summary>
    [PunRPC]
    public async UniTask RpcMoveRandomCardToField(bool isPlayer)
    {
        await MoveRandomCardToField(isPlayer);
    }

    /// <summary>
    /// カードタイプを登録します
    /// </summary>
    void RegisterCardType(CardType cardType)
    {
        PhotonNetwork.LocalPlayer.SetIntBattleCardType(cardType);
    }
}
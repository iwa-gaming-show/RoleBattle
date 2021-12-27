using System;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Linq;
using static BattlePhase;
using static SEType;
using static WaitTimes;

public class MultiBattleUIManager : SuperBattleUIManager
{
    IMultiBattleDataManager _dataM;
    PhotonView _photonView;

    new void Awake()
    {
        base.Awake();
        _photonView = GetComponent<PhotonView>();
    }

    new void Start()
    {
        base.Start();
        _dataM = ServiceLocator.Resolve<IMultiBattleDataManager>();
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
            if (_dataM.GetIsFieldCardPlaced(_dataM.GetPlayerBy(true))) return;

            _dataM.RegisterCardType(_dataM.GetPlayerBy(true), movingCard.CardType);
            //RegisterCardType(movingCard.CardType);
            //カードを配置済みにする
            _dataM.SetIsFieldCardPlaced(_dataM.GetPlayerBy(true), true);
            _dataM.Room.SetIntBattlePhase(SELECTED);

            //対戦相手のゲーム側でenemyのカードを移動させます
            //演出用にランダムなカードを選び移動させます。
            //※実際にフィールドに出すカードは異なります、カンニングを阻止する意味もあります。
            _photonView.RPC("RpcMoveRandomCardToField", RpcTarget.Others, false);
        }

        await GetPlayerUI(isPlayer).MoveToBattleField(movingCard);

        if (isPlayer == false) return;
        await UniTask.Delay(TimeSpan.FromSeconds(TIME_BEFORE_CHANGING_TURN));
        //ターンを終了する
        _dataM.SetIsMyTurnEnd(_dataM.GetPlayerBy(true), true);
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
            _dataM.ActivatingSpSkillState(_dataM.GetPlayerBy(true));
            _photonView.RPC("ActivateSpSkill", RpcTarget.Others, false);
        }
        GameManager._instance.PlaySE(SP_SKILL);
        await GetPlayerUI(isPlayer).ActivateDirectingOfSpSkill(isPlayer);
    }
    #endregion

    /// <summary>
    /// プレイヤーキャラクターの初期化処理
    /// </summary>
    public void InitPlayerCharacter(bool isPlayer, SelectableCharacter selectableCharacter)
    {
        GetPlayerUI(isPlayer).SetPlayerCharacter(selectableCharacter);
    }

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
}
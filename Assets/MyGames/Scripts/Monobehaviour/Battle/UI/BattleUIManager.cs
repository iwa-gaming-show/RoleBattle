using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static WaitTimes;
using static BattlePhase;
using static CharacterIconSizes;

public class BattleUIManager : SuperBattleUIManager
{
    IBattleDataManager _battleDataManager;
    [SerializeField]
    [Header("Sサイズプレイヤーのアイコンのプレハブを設定する")]
    PlayerIcon _playerIconSPrefab;

    [SerializeField]
    [Header("Mサイズプレイヤーのアイコンのプレハブを設定する")]
    PlayerIcon _playerIconMPrefab;

    new void Start()
    {
        base.Start();
        _battleDataManager = ServiceLocator.Resolve<IBattleDataManager>();
    }

    #region // override methods
    /// <summary>
    /// カードを移動する
    /// </summary>
    public override async UniTask MoveToBattleField(bool isPlayer, CardController movingCard)
    {
        //すでにフィールドにカードが置かれているなら何もしない
        if (_battleDataManager.GetIsFieldCardPlacedBy(isPlayer)) return;

        _battleDataManager.RegisterCardTypeBy(isPlayer, movingCard.CardType);
        //カードを配置済みにする
        _battleDataManager.SetIsFieldCardPlacedBy(isPlayer, true);
        _battleDataManager.SetBattlePhase(SELECTED);

        //カードを移動する
        await GetPlayerUI(isPlayer).MoveToBattleField(movingCard);

        await UniTask.Delay(TimeSpan.FromSeconds(TIME_BEFORE_CHANGING_TURN));
        //ターンを終了する
        _battleDataManager.SetIsPlayerTurnEndBy(isPlayer, true);
        _battleDataManager.SetCanChangeTurn(true);
    }

    /// <summary>
    /// 必殺技を発動する
    /// </summary>
    /// <returns></returns>
    public override async UniTask ActivateSpSkill(bool isPlayer)
    {
        _battleDataManager.ActivatingSpSkillState(isPlayer);
        SetSpButtonImageBy(isPlayer, _battleDataManager.GetCanUseSpSkillBy(isPlayer));
        await GetPlayerUI(isPlayer).ActivateDirectingOfSpSkill(isPlayer);
    }

    /// <summary>
    /// プレイヤーのキャラクターを設定する
    /// </summary>
    public override void SetPlayerCharacter(SelectableCharacter selectableCharacter, bool isPlayer)
    {
        PlayerIcon iconS = CreatePlayerIconBy(_playerIconSPrefab, selectableCharacter.FindIconImageBy(S_SIZE));
        PlayerIcon iconM = CreatePlayerIconBy(_playerIconMPrefab, selectableCharacter.FindIconImageBy(M_SIZE));
        //自身のアイコンを配置
        PlacePlayerIconBy(isPlayer, iconS.gameObject, S_SIZE);
        PlacePlayerIconBy(isPlayer, iconM.gameObject, M_SIZE);
    }
    #endregion

    /// <summary>
    /// プレイヤーキャラクターの初期化処理
    /// </summary>
    public void InitPlayerCharacter()
    {
        SetPlayerCharacter(GameManager._instance.GetPlayerCharacter(), true);//playerの設定
        SetPlayerCharacter(GameManager._instance.GetRandomPlayerCharacter(), false);//enemyの設定
    }

    /// <summary>
    /// プレイヤーのアイコンを作成します
    /// </summary>
    /// <param name="size"></param>
    /// <param name="selectableCharacter"></param>
    PlayerIcon CreatePlayerIconBy(PlayerIcon iconPrefab, Sprite characterSprite)
    {
        PlayerIcon icon = Instantiate(iconPrefab);
        icon.SetPlayerCharacterIcon(characterSprite);
        return icon;
    }

    /// <summary>
    /// ポイントの表示
    /// </summary>
    public void ShowPoint(int playerPoint, int enemyPoint)
    {
        _playerUI.ShowPoint(playerPoint);
        _enemyUI.ShowPoint(enemyPoint);
    }

    ///<summary>
    //必殺技のImageの状態を設定する
    ///</summary>
    public void SetSpButtonImage(bool playerCanUseSpSkill, bool enemyCanUseSpSkill)
    {
        SetSpButtonImageBy(true, playerCanUseSpSkill);
        SetSpButtonImageBy(false, enemyCanUseSpSkill);
    }

    /// <summary>
    /// エネミーの行動をします
    /// </summary>
    /// <returns></returns>
    public async UniTask NpcEnemyAction()
    {
        //必殺技を使用するターンなら必殺技を発動
        bool useSpSkill
            = (_battleDataManager.RoundCount == _battleDataManager.EnemySpSkillRound);

        if (_battleDataManager.GetCanUseSpSkillBy(false) && useSpSkill)
        {
            await ActivateSpSkill(false);
        }

        //ランダムにカードを選びフィールドへ
        await MoveRandomCardToField(false);
    }
}
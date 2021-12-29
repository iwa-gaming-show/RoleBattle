using Cysharp.Threading.Tasks;
using UnityEngine;
using static SEType;

public abstract class SuperConfirmationPanelManager : MonoBehaviour,
    IConfirmationPanelManager
{
    [SerializeField]
    [Header("フィールドへ移動する確認画面を設定する")]
    ConfirmationPanelToField _confirmationPanelToField;

    [SerializeField]
    [Header("必殺技発動の確認画面を設定する")]
    ConfirmationPanelToSp _confirmationPanelToSp;

    [SerializeField]
    [Header("カウントダウン終了時に閉じるUIを設定する")]
    GameObject[] _inactiveUIWhenTimeOut;

    CardController _movingFieldCard;//フィールドへ移動するカードを保存する
    bool _isSpSkillActivating;//必殺技を発動するか

    public CardController MovingFieldCard => _movingFieldCard;
    public bool IsSpSkillActivating => _isSpSkillActivating;

    void Awake()
    {
        ServiceLocator.Register<IConfirmationPanelManager>(this);
    }

    void OnDestroy()
    {
        ServiceLocator.UnRegister<IConfirmationPanelManager>(this);
    }

    #region // abstract methods
    /// <summary>
    /// 必殺技が発動可能か返します
    /// </summary>
    /// <returns></returns>
    protected abstract bool canActivateSpSkill();

    /// <summary>
    /// 自身の選択ターンかどうかを返します
    /// </summary>
    /// <returns></returns>
    protected abstract bool MySelectionTurn();
    #endregion

    /// <summary>
    /// フィールドへの移動を確認します
    /// </summary>
    /// <returns></returns>
    public async UniTask ConfirmToMoveToField(CardController selectedCard)
    {
        //選択フェイズで自身のカードが配置可能な場合操作可能
        bool myCard = selectedCard.IsPlayerCard;
        bool controlable = myCard && MySelectionTurn();
        if (controlable == false) return;
        //すでに確認画面が表示されているなら何もしない
        if (IsActiveConfirmationPanel()) return;

        GameManager._instance.PlaySE(STANDARD_CLICK);
        //カードを選択し、確認画面を表示しYesならフィールドへ移動します
        ViewConfirmationPanelFor(_confirmationPanelToField);
        _confirmationPanelToField.SetFieldConfirmationText(selectedCard);
        await WaitFieldConfirmationButton(_confirmationPanelToField);
        //yesを押した時、フィールドへ移動するカードとして保持
        if (_confirmationPanelToField.CanMoveToField)
        {
            _movingFieldCard = selectedCard;
        }

        _confirmationPanelToField.SetCanMoveToField(false);
        _confirmationPanelToField.SetIsConfirmed(false);
    }

    /// <summary>
    /// 必殺技発動の確認をします
    /// </summary>
    public async UniTask ConfirmToActivateSpSkill()
    {
        if (canActivateSpSkill() == false) return;
        //すでに確認画面が表示されているなら何もしない
        if (IsActiveConfirmationPanel()) return;

        GameManager._instance.PlaySE(STANDARD_CLICK);
        //確認画面を表示しYesなら必殺技を発動します
        ViewConfirmationPanelFor(_confirmationPanelToSp);
        await WaitFieldConfirmationButton(_confirmationPanelToSp);

        if (_confirmationPanelToSp.CanActivateSpSkill)
        {
            _isSpSkillActivating = true;
        }

        _confirmationPanelToSp.SetCanActivateSpSkill(false);
        _confirmationPanelToSp.SetIsConfirmed(false);
    }

    /// <summary>
    /// 確認画面がアクティブになっているか
    /// </summary>
    /// <returns></returns>
    bool IsActiveConfirmationPanel()
    {
        if (_confirmationPanelToField.gameObject.activeInHierarchy) return true;
        if (_confirmationPanelToSp.gameObject.activeInHierarchy) return true;
        return false;
    }

    /// <summary>
    /// 選択したカードの確認画面を表示する
    /// </summary>
    public void ViewConfirmationPanelFor(IToggleable confirmationPanel)
    {
        confirmationPanel.ToggleUI(true);//確認画面を表示
    }

    /// <summary>
    /// フィールドへの確認画面の押下を待ちます
    /// </summary>
    /// <returns></returns>
    public async UniTask WaitFieldConfirmationButton(IRequiredConfirmation confirmationPanel)
    {
        await UniTask.WaitUntil(() => confirmationPanel.IsConfirmed);
        GameManager._instance.PlaySE(STANDARD_CLICK);
    }

    /// <summary>
    /// フィールドへ移動するカードの保存情報を破棄する
    /// </summary>
    public void DestroyMovingBattleCard()
    {
        _movingFieldCard = null;
    }

    /// <summary>
    /// //必殺技発動フラグを設定する
    /// </summary>
    /// <param name="isActivating"></param>
    public void SetIsSpSkillActivating(bool isActivating)
    {
        _isSpSkillActivating = isActivating;
    }

    /// <summary>
    /// カウントダウンがタイムアウトした場合に閉じる
    /// </summary>
    public void InactiveUIIfCountDownTimeOut()
    {
        foreach (GameObject _inactiveUI in _inactiveUIWhenTimeOut)
        {
            _inactiveUI.GetComponent<IToggleable>()?.ToggleUI(false);
        }
    }
}

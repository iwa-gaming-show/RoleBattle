using UnityEngine;

public class PlayerData
{
    string _name;
    Sprite _icon;
    int _point;
    bool _canUseSpSkill;
    bool _isMyTurn;
    bool _isMyTurnEnd;
    bool _isUsingSpInRound;
    bool _isFieldCardPlaced;
    bool _isCardJudged;
    bool _isRetryingBattle;
    CardType _battleCardType;
    SelectableCharacter _selectedCharacter;


    public int Point => _point;
    public bool IsMyTurn => _isMyTurn;
    public bool IsMyTurnEnd => _isMyTurnEnd;
    public bool IsFieldCardPlaced => _isFieldCardPlaced;
    public bool IsUsingSpInRound => _isUsingSpInRound;
    public bool CanUseSpSkill => _canUseSpSkill;
    public CardType BattleCardType => _battleCardType;
    public SelectableCharacter SelectedCharacter => _selectedCharacter;

    public PlayerData(int point)
    {
        _point = point;
    }

    /// <summary>
    /// ポイントを設定
    /// </summary>
    /// <param name="point"></param>
    public void SetPoint(int point)
    {
        _point = point;
    }

    /// <summary>
    /// 選択キャラクターを保持します
    /// </summary>
    /// <param name="isPlayer"></param>
    public void SetSelectedCharacter(SelectableCharacter character)
    {
        _selectedCharacter = character;
    }

    /// <summary>
    /// ポイントの加算
    /// </summary>
    public void AddPoint(int point)
    {
        _point += point;
    }

    /// <summary>
    /// 必殺技が使用可能かの設定
    /// </summary>
    /// <param name="canUseSp"></param>
    public void SetCanUseSpSkill(bool canUseSp)
    {
        _canUseSpSkill = canUseSp;
    }

    /// <summary>
    /// プレイヤーのターンかどうかを設定する
    /// </summary>
    /// <param name="isMyTurn"></param>
    public void SetIsMyTurn(bool isMyTurn)
    {
        _isMyTurn = isMyTurn;
    }

    /// <summary>
    /// プレイヤーのターンが終了したかどうかを設定する
    /// </summary>
    /// <param name="isMyTurnEnd"></param>
    public void SetIsMyTurnEnd(bool isMyTurnEnd)
    {
        _isMyTurnEnd  = isMyTurnEnd;
    }

    /// <summary>
    /// 必殺技の発動中かどうかを設定する
    /// </summary>
    /// <param name="isUsingSpInRound"></param>
    public void SetIsUsingSpInRound(bool isUsingSpInRound)
    {
        _isUsingSpInRound = isUsingSpInRound;
    }

    /// <summary>
    /// カードをフィールドに配置したかどうかを設定する
    /// </summary>
    /// <param name="isFieldCardPlaced"></param>
    public void SetIsFieldCardPlaced(bool isFieldCardPlaced)
    {
        _isFieldCardPlaced = isFieldCardPlaced;
    }

    /// <summary>
    /// フィールドに配置したカードの種類を設定する
    /// </summary>
    /// <param name="isFieldCardPlaced"></param>
    public void SetCardType(CardType battleCardType)
    {
        _battleCardType = battleCardType;
    }
}

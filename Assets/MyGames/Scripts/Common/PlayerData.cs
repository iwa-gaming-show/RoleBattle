using UnityEngine;

public class PlayerData
{
    string _name;
    Sprite _icon;
    int _point;
    bool _canUseSpecialSkill;

    public int Point => _point;
    public bool CanUseSpecialSkill => _canUseSpecialSkill;

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
    public void SetCanUseSpecialSkill(bool canUseSp)
    {
        _canUseSpecialSkill = canUseSp;
    }
}

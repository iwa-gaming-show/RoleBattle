using System.Collections;
using System.Collections.Generic;
using static UIStrings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    [Header("自身の獲得ポイントUI")]
    TextMeshProUGUI _myPointText;

    [SerializeField]
    [Header("必殺技ボタンを設定する")]
    Image _spButtonImage;

    [SerializeField]
    [Header("プレイヤーアイコンの設置場所を設定する")]
    Transform _playerIconField;

    /// <summary>
    /// ポイントの表示
    /// </summary>
    public void ShowPoint(int myPoint)
    {
        _myPointText.text = myPoint.ToString() + POINT_SUFFIX;
    }

    /// <summary>
    /// 必殺技のImageを初期化する
    /// </summary>
    //public void InitSpButtonImage(Sprite unUsedIcon)
    //{
    //    SetSpButtonSprite(unUsedIcon);
    //}

    /// <summary>
    /// 必殺技のIconを設定する
    /// </summary>
    /// <param name="targetImage"></param>
    /// <param name="setSprite"></param>
    public void SetSpButtonSprite(Sprite setSprite)
    {
        _spButtonImage.sprite = setSprite;
    }

    /// <summary>
    /// 必殺技ボタンを使用済みにする
    /// </summary>
    void UsedSpecialSkillButton(Sprite usedIcon)
    {
        SetSpButtonSprite(usedIcon);
    }

    /// <summary>
    /// プレイヤーアイコンを配置する
    /// </summary>
    public void PlacePlayerIcon(GameObject targetGo)
    {
        targetGo.transform.SetParent(_playerIconField, false);
    }
}

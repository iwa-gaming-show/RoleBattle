using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIcon : MonoBehaviour,
    IPlayerIcon
{
    Image _iconImage;
    public Image IconImage => _iconImage;

    /// <summary>
    /// プレイヤーキャラクターのアイコンを設定します
    /// </summary>
    /// <param name="sprite"></param>
    public void SetPlayerCharacterIcon(Sprite sprite)
    {
        if (TryGetComponent(out Image imageComp))
        {
            _iconImage = imageComp;
            _iconImage.sprite = sprite;
        }
    }
}

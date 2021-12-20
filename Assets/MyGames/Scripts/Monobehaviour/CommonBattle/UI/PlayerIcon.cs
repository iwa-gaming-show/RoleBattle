using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIcon : MonoBehaviour,
    IPlayerIcon
{
    Image _iconImage;
    public Image IconImage => _iconImage;

    void Awake()
    {
        _iconImage = GetComponent<Image>();
    }

    /// <summary>
    /// プレイヤーキャラクターのアイコンを設定します
    /// </summary>
    /// <param name="sprite"></param>
    public void SetPlayerCharacterIcon(Sprite sprite)
    {
        _iconImage.sprite = sprite;
    }
}

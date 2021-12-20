using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIcon : MonoBehaviour
{
    Image _iconImage;

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

using UnityEngine;
using UnityEngine.UI;

public interface IPlayerIcon
{
    Image IconImage
    {
        get;
    }
    /// <summary>
    /// プレイヤーキャラクターのアイコンを設定します
    /// </summary>
    /// <param name="sprite"></param>
    void SetPlayerCharacterIcon(Sprite sprite);
}
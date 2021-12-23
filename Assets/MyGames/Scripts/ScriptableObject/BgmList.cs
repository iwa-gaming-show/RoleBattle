using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BgmList", menuName = "Create BgmList")]
public class BgmList : ScriptableObject
{
    [SerializeField]
    List<Bgm> _bgmList = new List<Bgm>();

    public List<Bgm> GetBgmList => _bgmList;

    /// <summary>
    /// タイプからbgmのClipを取得します
    /// </summary>
    /// <param name="_bgmType"></param>
    /// <returns></returns>
    public AudioClip FindBgmClipByType(BgmType _bgmType)
    {
        try
        {
            return _bgmList.Find(bgm => bgm.Type == _bgmType).Clip;
        }
        catch
        {
            Debug.Log("Bgmが見つかりませんでした");
            return null;
        }
    }
}

[System.Serializable]
public class Bgm
{
    [SerializeField]
    [Header("Bgmの種類")]
    BgmType _bgmType;

    [SerializeField]
    [Header("Bgmを設定")]
    AudioClip _bgmClip;

    public BgmType Type => _bgmType;
    public AudioClip Clip => _bgmClip;
}
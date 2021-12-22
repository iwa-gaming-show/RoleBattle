using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SEList", menuName = "Create SEList")]
public class SEList : ScriptableObject
{
    [SerializeField]
    List<SE> _seList = new List<SE>();

    public List<SE> GetSEList => _seList;

    /// <summary>
    /// タイプからseのClipを取得します
    /// </summary>
    /// <param name="_seType"></param>
    /// <returns></returns>
    public AudioClip FindSEClipByType(SEType _seType)
    {
        try
        {
            return _seList.Find(se => se.Type == _seType).Clip;
        }
        catch
        {
            Debug.Log("SEが見つかりませんでした");
            return null;
        }
    }
}

[System.Serializable]
public class SE
{
    [SerializeField]
    [Header("seの種類")]
    SEType _seType;

    [SerializeField]
    [Header("SEを設定")]
    AudioClip _seClip;

    public SEType Type => _seType;
    public AudioClip Clip => _seClip;
}
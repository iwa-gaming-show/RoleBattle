using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOption : MonoBehaviour,
    IGameOption,
    IToggleable
{
    [SerializeField]
    [Header("SettingCanvasのTransformを設定する")]
    Transform _settingCanvasTransform;

    /// <summary>
    /// 変更を保存する
    /// </summary>
    public bool Save()
    {
        //throw new System.NotImplementedException();
        return true;
    }

    /// <summary>
    /// UIの表示の切り替えを行います
    /// </summary>
    /// <param name="isActive"></param>
    public void ToggleUI(bool isActive)
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(gameObject, isActive, _settingCanvasTransform);
    }
}

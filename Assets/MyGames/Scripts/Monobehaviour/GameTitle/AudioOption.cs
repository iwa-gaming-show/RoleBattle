using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOption : MonoBehaviour,
    IGameOption
{
    [SerializeField]
    [Header("SettingCanvasのTransformを設定する")]
    Transform _settingCanvasTransform;

    public void Show()
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(gameObject, true, _settingCanvasTransform);
    }

    public void Close()
    {
        CanvasForObjectPool._instance.ToggleUIGameObject(gameObject, false, _settingCanvasTransform);
    }

}

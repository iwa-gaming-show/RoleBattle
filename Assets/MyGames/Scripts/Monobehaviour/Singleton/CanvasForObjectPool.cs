using UnityEngine;

public class CanvasForObjectPool : MonoBehaviour
{
    [HideInInspector]
    public static CanvasForObjectPool _instance;

    void Awake()
    {
        //シングルトンで作成
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// UIオブジェクトの表示の切り替え
    /// </summary>
    public void ToggleUIGameObject(GameObject targetGameObject, bool isActive, Transform settingCanvasTransform = null)
    {
        //パフォーマンスの観点から使わなくなったUIは非表示にしてからCanvasを移動させます
        if (isActive)
        {
            //表示する時にはsettingCanvasが必要
            if (settingCanvasTransform == null) return;

            //ObjectPoolCanvasからsettingCanvasに移動してから表示
            targetGameObject?.transform.SetParent(settingCanvasTransform);
            targetGameObject?.SetActive(isActive);
        }
        else
        {
            //非表示にしてからObjectPoolCanvasに移動
            targetGameObject?.SetActive(isActive);
            targetGameObject?.transform.SetParent(transform);
        }
    }
}

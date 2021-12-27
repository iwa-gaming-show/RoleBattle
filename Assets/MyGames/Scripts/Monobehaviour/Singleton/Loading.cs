using UnityEngine;

public class Loading : MonoBehaviour,IToggleable
{
    [HideInInspector]
    public static Loading _instance;

    private void Awake()
    {
        //シングルトン化する
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

    public void ToggleUI(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}

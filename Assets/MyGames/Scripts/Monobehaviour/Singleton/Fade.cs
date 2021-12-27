using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class Fade : MonoBehaviour
{
    [HideInInspector]
    public static Fade _instance;

    [SerializeField]
    [Header("フェードする時間を指定する")]
    float fadeDuration;

    [SerializeField]
    [Header("初回起動時にフェードインが完了しているか")]
    bool _isFirstFadeInComp;

    CanvasGroup _canvasGroup;

    private void Awake()
    {
        //シングルトン化する
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// フェードアウトします
    /// </summary>
    public async UniTask StartFadeOut()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = true;
        await _canvasGroup.DOFade(endValue: 1, fadeDuration)
            .AsyncWaitForCompletion();
    }

    /// <summary>
    /// フェードインします
    /// </summary>
    public async UniTask StartFadeIn()
    {
        if (CheckFirstFadeInComp()) return;

        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = false;
        await _canvasGroup.DOFade(endValue: 0, fadeDuration)
            .AsyncWaitForCompletion();
    }

    /// <summary>
    /// 初回起動時にフェードインを完了させているか確認します
    /// </summary>
    /// <returns></returns>
    bool CheckFirstFadeInComp()
    {
        if (_isFirstFadeInComp)
        {
            _isFirstFadeInComp = false;//次回以降はフェードインさせる
            return true;
        }
        return false;
    }
}

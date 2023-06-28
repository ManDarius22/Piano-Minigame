using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class ScreenAnimations : MonoBehaviour
{
    [Header("Win Screen")]
    [SerializeField] private Image _winScreenBackground;
    [SerializeField] private TextMeshProUGUI _winScreenText;

    [Header("Lose Screen")]
    [SerializeField] private Image _loseScreenBackground;
    [SerializeField] private TextMeshProUGUI _loseScreenText;

    private void OnDestroy()
    {
        _winScreenBackground.DOKill();
        _winScreenText.DOKill();
    }

    public void WinScreenAnimation_Show()
    {
        _winScreenBackground.gameObject.SetActive(true);
        _winScreenBackground.DOFade(0.7f, 1f).SetEase(Ease.OutQuad).From(0);
        _winScreenText.DOFade(1f, 1f).SetEase(Ease.OutQuad).From(0);
    }

    public void WinScreenAnimation_Hide(Action onCompleteCallback)
    {
        _winScreenBackground.DOFade(0f, 1f).SetEase(Ease.OutQuad).From(0.7f);
        _winScreenText.DOFade(0f, 1f).SetEase(Ease.OutQuad).From(1f).OnComplete(()=> {
                _winScreenBackground.gameObject.SetActive(false);
                onCompleteCallback?.Invoke();
        });
    }

    public void LoseScreenAnimation_Show()
    {
        _loseScreenBackground.gameObject.SetActive(true);
        _loseScreenBackground.DOFade(0.7f, 1f).SetEase(Ease.OutQuad).From(0);
        _loseScreenText.DOFade(1f, 1f).SetEase(Ease.OutQuad).From(0);
    }

    public void LoseScreenAnimation_Hide(Action onCompleteCallback)
    {
        _loseScreenBackground.DOFade(0f, 1f).SetEase(Ease.OutQuad).From(0.7f);
        _loseScreenText.DOFade(0f, 1f).SetEase(Ease.OutQuad).From(1f).OnComplete(() => {
            _loseScreenBackground.gameObject.SetActive(false);
            onCompleteCallback?.Invoke();
        });
    }
}

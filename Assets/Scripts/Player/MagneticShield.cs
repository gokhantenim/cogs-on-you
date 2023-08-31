using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MagneticShield : MonoBehaviour
{
    public bool IsActive;
    [SerializeField] GameObject _shieldSphere;
    [SerializeField] RectTransform _shieldIcon;
    Vector2 _iconFullSize = new Vector2(100, 100);
    Vector2 _iconEmptySize = new Vector2(100, 0);
    float _chargeDelay = 2;
    float _dechargeDelay = 5;
    Tween _iconTween;
    Tween _sphereTween;

    private void OnDestroy()
    {
       if(_iconTween != null)
        {
            _iconTween.Kill();
        }
        if (_sphereTween != null)
        {
            _sphereTween.Kill();
        }
    }

    void Start()
    {
        gameObject.SetActive(false);
        _shieldIcon.sizeDelta = _iconEmptySize;
        _shieldSphere.SetActive(false);
        _shieldSphere.transform.localScale = Vector3.zero;
        ChargeShield();
    }

    void Update()
    {
        if (IsActive)
        {
            _shieldSphere.transform.Rotate(new Vector3(0, 0.5f));
        }
    }

    void ChargeShield()
    {
        _iconTween = _shieldIcon
            .DOSizeDelta(_iconFullSize, _chargeDelay)
            .SetEase(Ease.Linear)
            .OnComplete(OpenShield);
    }

    void DechargeShield()
    {
        _iconTween = _shieldIcon
            .DOSizeDelta(_iconEmptySize, _dechargeDelay)
            .SetEase(Ease.Linear)
            .OnComplete(CloseShield);
    }

    void OpenShield()
    {
        IsActive = true;
        _shieldSphere.SetActive(true);
        _sphereTween = _shieldSphere.transform
            .DOScale(3, 0.5f)
            .SetEase(Ease.OutBack);
        DechargeShield();
    }

    void CloseShield()
    {
        _sphereTween = _shieldSphere.transform
            .DOScale(0, 0.5f)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                IsActive = false;
                _shieldSphere.SetActive(false);
            });
        ChargeShield();
    }
}

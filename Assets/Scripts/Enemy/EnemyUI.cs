using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    Camera _cam;
    float _healthPercent = 1;
    [SerializeField] Image _greenBar;
    [SerializeField] Image _whiteBar;
    Tween _whiteBarTween;

    void Start()
    {
        _cam = Camera.main;
    }

    void OnDestroy()
    {
        if (_whiteBarTween != null)
        {
            _whiteBarTween.Kill();
        }
    }

    void Update()
    {
        //transform.LookAt(GameCamera.Instance.Camera.transform);
        transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position);
    }

    public void UpdateHealthBar(float healthPercent)
    {
        if (_whiteBarTween != null)
        {
            _whiteBarTween.Kill();
        }
        Vector3 whiteScale = _whiteBar.gameObject.transform.localScale;
        Vector3 greenScale = _greenBar.gameObject.transform.localScale;
        greenScale.x = healthPercent;
        _greenBar.gameObject.transform.localScale = greenScale;
        _whiteBarTween = DOTween.To((value) => {
            whiteScale.x = value;
            _whiteBar.gameObject.transform.localScale = whiteScale;
        }, whiteScale.x, healthPercent, 0.5f);

        _healthPercent = healthPercent;
    }
}

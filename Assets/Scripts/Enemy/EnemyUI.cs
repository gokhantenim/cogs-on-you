using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    Camera _cam;
    float _healthPercent = 1;
    [SerializeField] Slider _healthSlider;
    Tween _tween;

    void Start()
    {
        _cam = Camera.main;
    }

    void Update()
    {
        //transform.LookAt(GameCamera.Instance.Camera.transform);
        transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position);
    }

    public void UpdateHealthBar(float healthPercent)
    {
        _healthPercent = healthPercent;

        if (_tween != null)
        {
            _tween.Kill();
        }
        _tween = _healthSlider.DOValue(_healthPercent, 0.5f);
    }
}

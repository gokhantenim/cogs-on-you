using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    float _healthPercent = 1;
    [SerializeField] Slider _healthSlider;
    Tween _tween;

    void Start()
    {
        
    }

    void Update()
    {
        //transform.LookAt(GameCamera.Instance.Camera.transform);
        transform.rotation = Quaternion.LookRotation(transform.position - CameraManager.Instance.Camera.transform.position);
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

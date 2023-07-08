using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class GamePlayUI : MonoBehaviour
{
    public static GamePlayUI Instance;

    [SerializeField] Image _greenBar;
    [SerializeField] Image _whiteBar;
    [SerializeField] TextMeshProUGUI _healthPercentText;
    [SerializeField] TextMeshProUGUI _totalCogsText;
    float _textHealthPercent = 1;
    float _healthPercent = 1;
    Tween _whiteBarTween;
    Tween _healthTextTween;
    public int _totalCogs = 0;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        _whiteBarTween = DOTween.To((value)=> {
            whiteScale.x = value;
            _whiteBar.gameObject.transform.localScale = whiteScale;
        }, whiteScale.x, healthPercent, 0.5f);

        _healthTextTween = DOTween.To((value) => {
            _textHealthPercent = value;
            _healthPercentText.text = (value * 100).ToString(value < 0.1f ? "N2" : "N0") + "%";
        }, _textHealthPercent, healthPercent, 0.5f);

        _healthPercent = healthPercent;
    }

    public void UpdateTotalCogs(int totalCogs)
    {
        DOTween.To((val) =>
        {
            _totalCogsText.text = val.ToString("N0");
        }, _totalCogs, totalCogs, 1).SetId("total-cogs");
        _totalCogs = totalCogs;
    }
}

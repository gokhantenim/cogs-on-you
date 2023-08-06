using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GamePlayUI : MonoBehaviour
{
    public static GamePlayUI _instance;
    public static GamePlayUI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GamePlayUI>(true);
            }
            return _instance;
        }
    }

    [SerializeField] Image _greenBar;
    [SerializeField] Image _whiteBar;
    [SerializeField] TextMeshProUGUI _healthPercentText;
    [SerializeField] TextMeshProUGUI _totalCogsText;
    [SerializeField] TextMeshProUGUI _totalEnemiesText;
    [SerializeField] TextMeshProUGUI _levelNoText;
    float _textHealthPercent = 1;
    float _healthPercent = 1;
    Tween _whiteBarTween;
    Tween _healthTextTween;
    public int _totalCogs = 0;

    public void JumpButton()
    {
        InputManager.Instance.OnJump(null);
    }

    //public void UpdateHealthBarNoAnimation(float healthPercent)
    //{
    //    Vector3 whiteScale = _whiteBar.gameObject.transform.localScale;
    //    Vector3 greenScale = _greenBar.gameObject.transform.localScale;
    //    whiteScale.x = healthPercent;
    //    greenScale.x = healthPercent;
    //    _greenBar.gameObject.transform.localScale = greenScale;
    //    _whiteBar.gameObject.transform.localScale = whiteScale;
    //    _healthPercentText.text = (healthPercent * 100).ToString(healthPercent < 0.1f ? "N2" : "N0") + "%";
    //}

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
            SetHealthPercentText(value);
        }, _textHealthPercent, healthPercent, 0.5f);

        _healthPercent = healthPercent;
    }

    void SetHealthPercentText(float value)
    {
        _healthPercentText.text = (value * 100).ToString(value < 0.1f ? "N2" : "N0") + "%";
    }

    void SetTotalCogsText(float value)
    {
        _totalCogsText.text = value.ToString("N0");
    }

    public void UpdateTotalCogs(int totalCogs)
    {
        DOTween.To((val) =>
        {
            SetTotalCogsText(val);
        }, _totalCogs, totalCogs, 1).SetId("total-cogs");
        _totalCogs = totalCogs;
    }

    public void SetEnemyCounts(int remainEnemies, int totalEnemies)
    {
        _totalEnemiesText.text = remainEnemies.ToString() + " / " + totalEnemies.ToString();
    }

    public void SetLevelNo(int currentLevel, int totalLevel)
    {
        _levelNoText.text = currentLevel.ToString() + " / " + totalLevel.ToString();
    }

    public void ResetValues()
    {
        SetEnemyCounts(0, 0);
        SetTotalCogsText(0);
        SetHealthPercentText(1);
        _whiteBar.gameObject.transform.localScale = new Vector3(1, 1, 1);
        _greenBar.gameObject.transform.localScale = new Vector3(1, 1, 1);
    }
}

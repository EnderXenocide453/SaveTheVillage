using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Таймер с графической визуализацией
/// </summary>
public class Timer : MonoBehaviour
{
    #region События
    public delegate void TimerHandler();
    public TimerHandler onLoopEnds;
    #endregion

    #region UI элементы
    [SerializeField]
    private Image indicator;
    [SerializeField]
    private Image resourseImage;
    #endregion

    #region Время
    [SerializeField]
    private float maxTime;

    private float _curTime;
    private bool _isPlaying;
    #endregion

    private void Update()
    {
        if (_isPlaying)
            UpdateTime();
    }

    /// <summary>
    /// Приостанавливает таймер
    /// </summary>
    public void PauseTimer()
    {
        _isPlaying = false;
    }

    /// <summary>
    /// Запускает таймер вновь
    /// </summary>
    public void ResumeTimer()
    {
        _isPlaying = true;
    }

    /// <summary>
    /// Останавливает таймер и сбрасывает время
    /// </summary>
    public void StopTimer()
    {
        _isPlaying = false;
        _curTime = 0;
        UpdateUI();
    }

    /// <summary>
    /// Устанавливает время заполнения таймера равным time
    /// </summary>
    /// <param name="time">Время заполнения таймера</param>
    public void SetTime(float time)
    {
        maxTime = time;
    }

    /// <summary>
    /// Перерисовывает значки таймера
    /// </summary>
    /// <param name="resSprite">Спрайт ресурса</param>
    /// <param name="indSprite">Спрайт индикатора</param>
    public void RedrawUI(Sprite resSprite, Sprite indSprite)
    {
        resourseImage.sprite = resSprite;
        indicator.sprite = indSprite;
    }

    /// <summary>
    /// Обновление таймера. Вызывается каждый кадр
    /// </summary>
    private void UpdateTime()
    {
        _curTime += Time.deltaTime;

        if (_curTime >= maxTime) {
            _curTime %= maxTime;
            onLoopEnds.Invoke();
        }

        UpdateUI();
    }

    /// <summary>
    /// Графическое отображение времени через UI
    /// </summary>
    private void UpdateUI()
    {
        indicator.fillAmount = _curTime / maxTime;
    }
}

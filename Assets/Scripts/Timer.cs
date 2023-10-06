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
    public event TimerHandler onLoopEnds;
    #endregion

    #region UI элементы
    [SerializeField]
    private Image indicator;
    [SerializeField]
    private Image resourseImage;
    #endregion

    #region Время
    public bool isPlaying { get; private set; }
    
    private float _loopTime = 1;
    private float _curTime = 0;
    #endregion

    [SerializeField]
    private bool lookAtCamera = true;

    private void Update()
    {
        if (isPlaying)
            UpdateTimer();

        Animate();
    }

    /// <summary>
    /// Приостанавливает таймер
    /// </summary>
    public void Pause()
    {
        isPlaying = false;
    }

    /// <summary>
    /// Запускает таймер вновь
    /// </summary>
    public void Play()
    {
        isPlaying = true;
    }

    /// <summary>
    /// Останавливает таймер и сбрасывает время
    /// </summary>
    public void StopTimer()
    {
        isPlaying = false;
        _curTime = 0;
        UpdateUI();
    }

    /// <summary>
    /// Устанавливает время заполнения таймера равным time
    /// </summary>
    /// <param name="time">Время заполнения таймера</param>
    public void SetTime(float time)
    {
        //Таймер останавливается, если время равно 0
        isPlaying = (time == 0);

        //Сохранение прогресса производства с проверкой деления на ноль
        _curTime *= _loopTime > 0 ? time / _loopTime : 0;
        _loopTime = time;
    }

    /// <summary>
    /// Перерисовывает значки таймера
    /// </summary>
    /// <param name="resSprite">Спрайт ресурса</param>
    /// <param name="indSprite">Спрайт индикатора</param>
    public void RedrawUI(Sprite resSprite)
    {
        resourseImage.sprite = resSprite;
    }

    /// <summary>
    /// Обновление таймера. Вызывается каждый кадр
    /// </summary>
    private void UpdateTimer()
    {
        _curTime += Time.deltaTime;
        
        if (_curTime >= _loopTime) {
            _curTime %= _loopTime;
            onLoopEnds?.Invoke();
        }

        UpdateUI();
    }

    /// <summary>
    /// Графическое отображение времени через UI
    /// </summary>
    private void UpdateUI()
    {
        indicator.fillAmount = _curTime / _loopTime;
    }

    private void Animate()
    {
        transform.position += Vector3.up * Mathf.Sin(Time.realtimeSinceStartup) * 0.01f; ;

        if (lookAtCamera) {
            transform.LookAt(Camera.main.transform);
        }
    }
}

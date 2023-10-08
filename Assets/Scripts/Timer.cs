using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Таймер с графической визуализацией
/// </summary>
public class Timer : MonoBehaviour
{
    #region UI элементы
    [SerializeField]
    private Image indicator;
    [SerializeField]
    private Image resourseImage;
    #endregion

    #region Время
    public bool isPlaying { get; private set; }

    public float loopTime { get; private set; } = 1;
    
    private float _curTime = 0;
    #endregion

    #region Анимация
    [SerializeField]
    private bool lookAtCamera = false;
    [SerializeField]
    private bool animate = false;
    #endregion

    [SerializeField]
    private AudioClip loopSound;
    private AudioSource _audioSource;

    #region События
    public delegate void TimerHandler();
    public event TimerHandler onLoopEnds;
    #endregion

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = loopSound;
    }

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
    public void Stop()
    {
        isPlaying = false;
        _curTime = 0;
        UpdateUI();
    }

    public void Restart()
    {
        Stop();
        Play();
    }

    public void ResetTimer()
    {
        _curTime = 0;
        ClearListeners();
    }

    /// <summary>
    /// Устанавливает время заполнения таймера равным time
    /// </summary>
    /// <param name="time">Время заполнения таймера</param>
    public void SetTime(float time)
    {
        //Таймер останавливается, если время равно 0
        isPlaying = isPlaying && (time != 0);

        //Сохранение прогресса производства с проверкой деления на ноль
        _curTime *= loopTime > 0 ? time / loopTime : 0;
        loopTime = time;
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

    public void ClearListeners()
    {
        onLoopEnds = null;
    }

    /// <summary>
    /// Обновление таймера. Вызывается каждый кадр
    /// </summary>
    private void UpdateTimer()
    {
        _curTime += Time.deltaTime;
        
        if (_curTime >= loopTime) {
            _curTime %= loopTime;
            _audioSource.Play();
            onLoopEnds?.Invoke();
        }

        UpdateUI();
    }

    /// <summary>
    /// Графическое отображение времени через UI
    /// </summary>
    private void UpdateUI()
    {
        indicator.fillAmount = _curTime / loopTime;
    }

    private void Animate()
    {
        if (animate)
            transform.position += Vector3.up * Mathf.Sin(Time.realtimeSinceStartup) * 0.01f; ;

        if (lookAtCamera) {
            transform.LookAt(Camera.main.transform);
        }
    }
}

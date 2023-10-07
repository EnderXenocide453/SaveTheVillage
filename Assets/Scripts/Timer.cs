using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������ � ����������� �������������
/// </summary>
public class Timer : MonoBehaviour
{
    #region UI ��������
    [SerializeField]
    private Image indicator;
    [SerializeField]
    private Image resourseImage;
    #endregion

    #region �����
    public bool isPlaying { get; private set; }

    public float loopTime { get; private set; } = 1;
    
    private float _curTime = 0;
    #endregion

    #region �������
    public delegate void TimerHandler();
    public event TimerHandler onLoopEnds;
    #endregion

    [SerializeField]
    private bool lookAtCamera = false;
    [SerializeField]
    private bool animate = false;

    private void Update()
    {
        if (isPlaying)
            UpdateTimer();

        Animate();
    }

    /// <summary>
    /// ���������������� ������
    /// </summary>
    public void Pause()
    {
        isPlaying = false;
    }

    /// <summary>
    /// ��������� ������ �����
    /// </summary>
    public void Play()
    {
        isPlaying = true;
    }

    /// <summary>
    /// ������������� ������ � ���������� �����
    /// </summary>
    public void Stop()
    {
        isPlaying = false;
        _curTime = 0;
        UpdateUI();
    }

    /// <summary>
    /// ������������� ����� ���������� ������� ������ time
    /// </summary>
    /// <param name="time">����� ���������� �������</param>
    public void SetTime(float time)
    {
        //������ ���������������, ���� ����� ����� 0
        isPlaying = isPlaying && (time != 0);

        //���������� ��������� ������������ � ��������� ������� �� ����
        _curTime *= loopTime > 0 ? time / loopTime : 0;
        loopTime = time;
    }

    /// <summary>
    /// �������������� ������ �������
    /// </summary>
    /// <param name="resSprite">������ �������</param>
    /// <param name="indSprite">������ ����������</param>
    public void RedrawUI(Sprite resSprite)
    {
        resourseImage.sprite = resSprite;
    }

    public void ClearListeners()
    {
        onLoopEnds = null;
    }

    /// <summary>
    /// ���������� �������. ���������� ������ ����
    /// </summary>
    private void UpdateTimer()
    {
        _curTime += Time.deltaTime;
        
        if (_curTime >= loopTime) {
            _curTime %= loopTime;
            onLoopEnds?.Invoke();
        }

        UpdateUI();
    }

    /// <summary>
    /// ����������� ����������� ������� ����� UI
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

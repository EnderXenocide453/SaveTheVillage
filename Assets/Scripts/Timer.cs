using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������ � ����������� �������������
/// </summary>
public class Timer : MonoBehaviour
{
    #region �������
    public delegate void TimerHandler();
    public event TimerHandler onLoopEnds;
    #endregion

    #region UI ��������
    [SerializeField]
    private Image indicator;
    [SerializeField]
    private Image resourseImage;
    #endregion

    #region �����
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
    public void StopTimer()
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
        isPlaying = (time == 0);

        //���������� ��������� ������������ � ��������� ������� �� ����
        _curTime *= _loopTime > 0 ? time / _loopTime : 0;
        _loopTime = time;
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

    /// <summary>
    /// ���������� �������. ���������� ������ ����
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
    /// ����������� ����������� ������� ����� UI
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

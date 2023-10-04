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
    public TimerHandler onLoopEnds;
    #endregion

    #region UI ��������
    [SerializeField]
    private Image indicator;
    [SerializeField]
    private Image resourseImage;
    #endregion

    #region �����
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
    /// ���������������� ������
    /// </summary>
    public void PauseTimer()
    {
        _isPlaying = false;
    }

    /// <summary>
    /// ��������� ������ �����
    /// </summary>
    public void ResumeTimer()
    {
        _isPlaying = true;
    }

    /// <summary>
    /// ������������� ������ � ���������� �����
    /// </summary>
    public void StopTimer()
    {
        _isPlaying = false;
        _curTime = 0;
        UpdateUI();
    }

    /// <summary>
    /// ������������� ����� ���������� ������� ������ time
    /// </summary>
    /// <param name="time">����� ���������� �������</param>
    public void SetTime(float time)
    {
        maxTime = time;
    }

    /// <summary>
    /// �������������� ������ �������
    /// </summary>
    /// <param name="resSprite">������ �������</param>
    /// <param name="indSprite">������ ����������</param>
    public void RedrawUI(Sprite resSprite, Sprite indSprite)
    {
        resourseImage.sprite = resSprite;
        indicator.sprite = indSprite;
    }

    /// <summary>
    /// ���������� �������. ���������� ������ ����
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
    /// ����������� ����������� ������� ����� UI
    /// </summary>
    private void UpdateUI()
    {
        indicator.fillAmount = _curTime / maxTime;
    }
}

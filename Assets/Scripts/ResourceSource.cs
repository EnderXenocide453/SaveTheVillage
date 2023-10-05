using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����� ��������� �������. ������ � ���� ��������, ������ � �������������� ��������������
/// </summary>
public class ResourceSource : MonoBehaviour
{
    #region �������� ���������
    /// <summary>
    /// ��� ������������� �������
    /// </summary>
    public ResourceType type;
    /// <summary>
    /// �������� ���������
    /// </summary>
    public string sourceName = "|��� �����|";
    /// <summary>
    /// ������ ������
    /// </summary>
    public Sprite resIcon;
    /// <summary>
    /// ������ ����������������� �����
    /// </summary>
    public Timer timer;

    /// <summary>
    /// ����� �� ���� ���� ������������
    /// </summary>
    public int loopIncome { get; private set; }

    /// <summary>
    /// ����� �� ������ ��������
    /// </summary>
    [SerializeField]
    private int incomeModifier = 1;
    /// <summary>
    /// ����� ���������� ������ ����������������� �����
    /// </summary>
    [SerializeField]
    private float loopTime = 1;
    [SerializeField]
    private bool workWithoutEmployee = false;
    /// <summary>
    /// ���������� �������
    /// </summary>
    private int _emloyeesCount;
    #endregion

    private void Start()
    {
        timer.RedrawUI(resIcon);
        timer.SetTime(loopTime);

        if (workWithoutEmployee)
            timer.Play();
    }

    /// <summary>
    /// ������������� ���������� ������� ������ count
    /// </summary>
    /// <param name="count">���������� �������</param>
    /// <returns>������� ����� ����� � ������ ����������� ����������</returns>
    public int SetEmployees(int count)
    {
        count = Mathf.Clamp(count, 0, int.MaxValue);
        //������� ����� ����� � ������ ���������� ���������� �������
        int delta = count - _emloyeesCount;
        _emloyeesCount = count;
        
        RecalculateIncome();

        return delta;
    }

    /// <summary>
    /// ��������� ��������/�������
    /// </summary>
    /// <param name="count">����� ����������� �������. �� ��������� 1</param>
    /// <returns>������� ����� ����� � ������ ����������� ����������</returns>
    public int AddEmployee(int count = 1)
    {
        return SetEmployees(_emloyeesCount + count);
    }

    /// <summary>
    /// ������� ��������/�������
    /// </summary>
    /// <param name="count">����� ��������� �������. �� ��������� 1</param>
    /// <returns>������� ����� ����� � ������ ����������� ����������</returns>
    public int RemoveEmployee(int count = 1)
    {
        return SetEmployees(_emloyeesCount - count);
    }

    /// <summary>
    /// ���������� ������ �� ������ �����
    /// </summary>
    private void RecalculateIncome()
    {
        if (workWithoutEmployee) {
            loopIncome = incomeModifier;
            return;
        }

        if (_emloyeesCount == 0)
            timer.Pause();
        else if (!timer.isPlaying)
            timer.Play();

        loopIncome = _emloyeesCount * incomeModifier;
    }
}

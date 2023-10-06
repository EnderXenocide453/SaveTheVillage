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
    /// ���������� �������
    /// </summary>
    public int employeesCount { get; private set; }

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
    #endregion

    public delegate void EmployeeHandler();
    public event EmployeeHandler onEmployeesChanged;

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
        int delta = count - employeesCount;
        employeesCount = count;
        
        RecalculateIncome();

        onEmployeesChanged?.Invoke();

        return delta;
    }

    /// <summary>
    /// ��������� ��������/�������
    /// </summary>
    /// <param name="count">����� ����������� �������. �� ��������� 1</param>
    /// <returns>������� ����� ����� � ������ ����������� ����������</returns>
    public int AddEmployee(int count = 1)
    {
        return SetEmployees(employeesCount + count);
    }

    /// <summary>
    /// ������� ��������/�������
    /// </summary>
    /// <param name="count">����� ��������� �������. �� ��������� 1</param>
    /// <returns>������� ����� ����� � ������ ����������� ����������</returns>
    public int RemoveEmployee(int count = 1)
    {
        return SetEmployees(employeesCount - count);
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

        if (employeesCount == 0)
            timer.Pause();
        else if (!timer.isPlaying)
            timer.Play();

        loopIncome = employeesCount * incomeModifier;
    }
}

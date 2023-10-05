using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс источника ресурса. Хранит в себе название, иконку и количественные характеристики
/// </summary>
public class ResourceSource : MonoBehaviour
{
    #region Свойства источника
    /// <summary>
    /// Тип производимого ресурса
    /// </summary>
    public ResourceType type;
    /// <summary>
    /// Название источника
    /// </summary>
    public string sourceName = "|Нет имени|";
    /// <summary>
    /// Спрайт иконки
    /// </summary>
    public Sprite resIcon;
    /// <summary>
    /// Таймер производственного цикла
    /// </summary>
    public Timer timer;

    /// <summary>
    /// Доход за один цикл производства
    /// </summary>
    public int loopIncome { get; private set; }

    /// <summary>
    /// Доход от одного рабочего
    /// </summary>
    [SerializeField]
    private int incomeModifier = 1;
    /// <summary>
    /// Время совершения одного производственного цикла
    /// </summary>
    [SerializeField]
    private float loopTime = 1;
    [SerializeField]
    private bool workWithoutEmployee = false;
    /// <summary>
    /// Количество рабочих
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
    /// Устанавливает количество рабочих равное count
    /// </summary>
    /// <param name="count">Количество ресурса</param>
    /// <returns>Разница между новым и старым количеством работников</returns>
    public int SetEmployees(int count)
    {
        count = Mathf.Clamp(count, 0, int.MaxValue);
        //Разница между новым и старым значениями количества рабочих
        int delta = count - _emloyeesCount;
        _emloyeesCount = count;
        
        RecalculateIncome();

        return delta;
    }

    /// <summary>
    /// Добавляет рабочего/рабочих
    /// </summary>
    /// <param name="count">Число добавляемых рабочих. По умолчанию 1</param>
    /// <returns>Разница между новым и старым количеством работников</returns>
    public int AddEmployee(int count = 1)
    {
        return SetEmployees(_emloyeesCount + count);
    }

    /// <summary>
    /// Убирает рабочего/рабочих
    /// </summary>
    /// <param name="count">Число убираемых рабочих. По умолчанию 1</param>
    /// <returns>Разница между новым и старым количеством работников</returns>
    public int RemoveEmployee(int count = 1)
    {
        return SetEmployees(_emloyeesCount - count);
    }

    /// <summary>
    /// Перерасчет дохода от одного цикла
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

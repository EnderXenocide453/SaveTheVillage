using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VillageController : MonoBehaviour
{
    #region Глобальный ход
    [SerializeField]
    private Timer turnTimer;
    [SerializeField]
    private Text turnField;
    [SerializeField]
    private float turnTime = 60;

    private int turn = 0;
    #endregion

    #region Ресурсы
    [SerializeField]
    private Resource food;
    [SerializeField]
    private Resource wood;
    [SerializeField]
    private Resource metal;
    #endregion

    #region Юниты
    [SerializeField]
    private Timer employeeTimer;
    [SerializeField]
    private float employeeHireTime = 10;
    [SerializeField]
    private Timer soldierTimer;

    [SerializeField]
    private Soldier warrior;
    [SerializeField]
    private Soldier defender;
    [SerializeField]
    private Soldier archer;

    private int warriorCount;
    private int defenderCount;
    private int archerCount;

    private int people;
    #endregion

    private void Start()
    {
        InitResources();
        InitTimers();
    }

    public void StartAddEmployee(ResourceSource src)
    {
        employeeTimer.ClearListeners();
        employeeTimer.onLoopEnds += () => AddEmployee(src);

        employeeTimer.Play();

        SetEmployeesButtonsActive(false);
    }

    public void RemoveEmployee(ResourceSource src)
    {
        people += src.RemoveEmployee();
    }

    /// <summary>
    /// Инициализация словаря дохода ресурсов
    /// </summary>
    private void InitResources()
    {
        food.Init();
        wood.Init();
        metal.Init();
    }

    private void InitTimers()
    {
        turnTimer.onLoopEnds += EndTurn;
        turnTimer.SetTime(turnTime);
        turnTimer.Play();

        employeeTimer.SetTime(employeeHireTime);
    }

    private void EndTurn()
    {
        turn++;
        turnField.text = turn.ToString();

        if (food.RemoveResource(people) > 0)
            Hunger();
    }

    private void Hunger()
    {
        Debug.Log("Начался голод");
    }

    private void AddEmployee(ResourceSource src)
    {
        people += src.AddEmployee();
        employeeTimer.Stop();

        SetEmployeesButtonsActive(true);
    }

    private void SetEmployeesButtonsActive(bool isActive)
    {
        food.plus.interactable = isActive;
        wood.plus.interactable = isActive;
        metal.plus.interactable = isActive;
    }
}

[System.Serializable]
public class Resource
{
    public ResourceSource source;
    public ResourceInfo ui;
    public Text employeeField;
    public Button plus;
    public int initialAmount;

    public int curAmount { get; private set; }

    public delegate void ResourseHandler();
    public event ResourseHandler onAmountChanged;

    public void Init()
    {
        curAmount = initialAmount;
        UpdateUI();

        source.timer.onLoopEnds += () => AddResource(source.loopIncome);
        source.onEmployeesChanged += () => { employeeField.text = source.employeesCount.ToString(); };
    }

    public void AddResource(int amount)
    {
        SetResource(curAmount + amount);
    }

    public int RemoveResource(int amount)
    {
        return SetResource(curAmount - amount);
    }

    private int SetResource(int amount)
    {
        if (amount < 0)
            curAmount = 0;
        else
            curAmount = amount;

        UpdateUI();
        onAmountChanged?.Invoke();

        return curAmount - amount;
    }

    private void UpdateUI()
    {
        ui.SetAmount(curAmount);
    }
}

[System.Serializable]
public class Soldier
{
    public int foodCost;
    public int woodCost;
    public int metalCost;

    public float hireTime = 15;

    public int attack;
    public int defense;

    public float attackPriority = 1;
}
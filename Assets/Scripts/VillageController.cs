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

    private int _people;
    private bool _updateSoldierButtons = true;
    #endregion

    private void Start()
    {
        InitResources();
        InitTimers();
        InitSoldiers();
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
        _people += src.RemoveEmployee();
    }

    /// <summary>
    /// Инициализация словаря дохода ресурсов
    /// </summary>
    private void InitResources()
    {
        food.Init();
        food.onAmountChanged += CalculateSoldierButtons;

        wood.Init();
        wood.onAmountChanged += CalculateSoldierButtons;

        metal.Init();
        metal.onAmountChanged += CalculateSoldierButtons;
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

        if (food.RemoveResource(_people) > 0)
            Hunger();
    }

    private void Hunger()
    {
        Debug.Log("Начался голод");
    }

    private void InitSoldiers()
    {
        CalculateSoldierButtons();

        //Кнопки добавления
        warrior.plusBtn.onClick.AddListener(delegate () {
            StartAddsoldier(warrior);
        });
        defender.plusBtn.onClick.AddListener(delegate () {
            StartAddsoldier(defender);
        });
        archer.plusBtn.onClick.AddListener(delegate () {
            StartAddsoldier(archer);
        });
        //кнопки удаления
        warrior.minusBtn.onClick.AddListener(delegate () {
            RemoveSoldier(warrior);
        });
        defender.minusBtn.onClick.AddListener(delegate () {
            RemoveSoldier(defender);
        });
        archer.minusBtn.onClick.AddListener(delegate () {
            RemoveSoldier(archer);
        });
    }

    private void RemoveSoldier(Soldier soldier)
    {
        _people += soldier.RemoveSoldiers(1);
    }

    private void StartAddsoldier(Soldier soldier)
    {
        DisableSoldierButtons();

        soldierTimer.SetTime(soldier.hireTime);
        soldierTimer.ClearListeners();
        soldierTimer.onLoopEnds += () => AddSoldier(soldier);
        soldierTimer.Play();
    }

    private void DisableSoldierButtons()
    {
        _updateSoldierButtons = false;

        warrior.plusBtn.interactable = false;
        defender.plusBtn.interactable = false;
        archer.plusBtn.interactable = false;
    }

    private void AddSoldier(Soldier soldier)
    {
        soldierTimer.Stop();
        _people += soldier.AddSoldiers(1);

        _updateSoldierButtons = true;
        CalculateSoldierButtons();
    }

    private void CalculateSoldierButtons()
    {
        if (!_updateSoldierButtons) return;

        CalculateSoldierButton(warrior);
        CalculateSoldierButton(defender);
        CalculateSoldierButton(archer);
    }

    private void CalculateSoldierButton(Soldier soldier)
    {
        if (soldier.foodCost > food.curAmount) {
            soldier.plusBtn.interactable = false;
            return;
        } if (soldier.woodCost > wood.curAmount) {
            soldier.plusBtn.interactable = false;
            return;
        } if (soldier.metalCost > metal.curAmount) {
            soldier.plusBtn.interactable = false;
            return;
        }

        soldier.plusBtn.interactable = true;
    }

    private void AddEmployee(ResourceSource src)
    {
        _people += src.AddEmployee();
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
    #region Стоимость найма и время на найм
    public int foodCost;
    public int woodCost;
    public int metalCost;

    public float hireTime = 15;
    #endregion

    #region Боевые параметры
    public int attack;
    public int defense;

    public float attackPriority = 1;
    #endregion

    public int curAmount { get; private set; }

    #region UI
    public Button plusBtn, minusBtn;
    public Text countfield;
    #endregion

    public int AddSoldiers(int amount = 1)
    {
        return SetSoldiers(curAmount + amount);
    }

    public int RemoveSoldiers(int amount = 1)
    {
        return SetSoldiers(curAmount - amount);
    }

    private int SetSoldiers(int amount)
    {
        amount = Mathf.Clamp(amount, 0, int.MaxValue);
        //Разница между новым и старым значениями количества воинов
        int delta = amount - curAmount;
        curAmount = amount;

        UpdateUI();

        return delta;
    }

    private void UpdateUI()
    {
        countfield.text = curAmount.ToString();
    }
}
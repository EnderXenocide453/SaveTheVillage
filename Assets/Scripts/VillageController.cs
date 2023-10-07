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

    private int turn = 1;
    #endregion

    #region Бой
    [SerializeField]
    private int firstAttackTurn = 3;
    [SerializeField]
    private int attackDelay = 1;
    [SerializeField]
    private int startEnemyCount = 5;
    [SerializeField]
    private int maxEnemyCount = 15;
    [SerializeField]
    private float enemyCountMultiplier = 1;

    private float _curEnemyCount;
    private int _enemyDef, _enemyAtk;

    [SerializeField]
    private Timer attackTimer;
    [SerializeField]
    private Text enemyAttackField, enemyDefenseField;
    [SerializeField]
    private Text battleResults;

    private Soldier enemyWarrior;
    private Soldier enemyDefender;
    private Soldier enemyArcher;
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
    private Text attackField;
    [SerializeField]
    private Text defenceField;

    [SerializeField]
    private Soldier warrior;
    [SerializeField]
    private Soldier defender;
    [SerializeField]
    private Soldier archer;

    private int _attack;
    private int _defense;

    private int _people;
    private bool _updateSoldierButtons = true;
    #endregion

    [SerializeField]
    private Transform winPanel, losePanel;
    [SerializeField]
    private int peopleToWin = 40;
    [SerializeField]
    private Text peopleField;

    private bool _paused = false;

    private void Start()
    {
        ChangePause();

        InitResources();
        InitTimers();
        InitSoldiers();
        InitEnemy();
    }

    public void Restart()
    {
        InitResources();
        InitTimers();
        InitSoldiers();
        InitEnemy();

        if (_paused)
            ChangePause();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ChangePause()
    {
        Time.timeScale = _paused ? 1 : 0;
        _paused = !_paused;
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
        SetPeople(_people + src.RemoveEmployee());
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

        if (food.RemoveResource(_people) < 0)
            Hunger();
    }

    private void Hunger()
    {
        Lose();
    }

    private void Lose()
    {
        ChangePause();
        losePanel.gameObject.SetActive(true);
    }

    #region Методы солдат
    private void InitSoldiers()
    {
        CalculateSoldierButtons();

        InitSoldier(warrior);
        InitSoldier(defender);
        InitSoldier(archer);

        UpdateSoldiersInfo();
    }

    private void InitSoldier(Soldier soldier)
    {
        soldier.Init();

        soldier.plusBtn.onClick.AddListener(delegate () {
            StartAddsoldier(soldier);
        });
        soldier.minusBtn.onClick.AddListener(delegate () {
            RemoveSoldier(soldier);
        });
    }

    private void RemoveSoldier(Soldier soldier)
    {
        SetPeople(_people + soldier.RemoveSoldiers(1));

        UpdateSoldiersInfo();
    }

    private void SetPeople(int count)
    {
        _people = count;
        if (_people > peopleToWin)
            Win();

        peopleField.text = _people.ToString();
    }

    private void Win()
    {
        ChangePause();
        winPanel.gameObject.SetActive(true);
    }

    private void UpdateSoldiersInfo()
    {
        _attack = warrior.GetAttack() + defender.GetAttack() + archer.GetAttack();
        _defense = warrior.GetDefense() + defender.GetDefense() + archer.GetDefense();

        attackField.text = _attack.ToString();
        defenceField.text = _defense.ToString();

        warrior.UpdateUI();
        defender.UpdateUI();
        archer.UpdateUI();
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
        SetPeople(_people + soldier.AddSoldiers(1));

        _updateSoldierButtons = true;
        CalculateSoldierButtons();

        UpdateSoldiersInfo();
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
    #endregion

    #region Методы схватки
    private void InitEnemy()
    {
        _curEnemyCount = startEnemyCount;

        GenerateEnemyArmy();

        attackTimer.SetTime(firstAttackTurn * turnTime);
        attackTimer.onLoopEnds += Attack;
        attackTimer.Play();
    }

    private void GenerateEnemyArmy()
    {
        float warriorWeight, defenderWeight, archerWeight;

        (warriorWeight, defenderWeight, archerWeight) = NormilizeWeights(Random.Range(0.1f, 1), Random.Range(0.1f, 1), Random.Range(0.1f, 1), _curEnemyCount);

        enemyWarrior = new Soldier(warrior, Mathf.RoundToInt(warriorWeight));
        enemyDefender = new Soldier(defender, Mathf.RoundToInt(defenderWeight));
        enemyArcher = new Soldier(archer, Mathf.RoundToInt(archerWeight));

        UpdateEnemyStrength();
    }

    private (float, float, float) NormilizeWeights(float x, float y, float z, float multiplier = 1)
    {
        Vector3 vector = new Vector3(x, y, z) / (x + y + z) * multiplier;
        return (vector.x, vector.y, vector.z);
    }

    private void UpdateEnemyStrength()
    {
        _enemyAtk = enemyWarrior.GetAttack() + enemyDefender.GetAttack() + enemyArcher.GetAttack();
        _enemyDef = enemyWarrior.GetDefense() + enemyDefender.GetDefense() + enemyArcher.GetDefense();

        enemyAttackField.text = _enemyAtk.ToString();
        enemyDefenseField.text = _enemyDef.ToString();
    }

    private void Attack()
    {
        Debug.Log("Нападение!");

        CalculateAttackResult();

        ReinitEnemy();
    }

    private void CalculateAttackResult()
    {
        Vector3Int dead = Vector3Int.zero;

        int enemyCount = enemyWarrior.GetCount() + enemyDefender.GetCount() + enemyArcher.GetCount();
        int soldierCount = warrior.GetCount() + defender.GetCount() + archer.GetCount();

        while (true) {
            Vector3Int curDeadSoldiers = Vector3Int.zero;

            var dmgReceived = NormilizeWeights(warrior.GetPriority(), defender.GetPriority(), archer.GetPriority(), _enemyAtk);
            var dmgDone = NormilizeWeights(enemyWarrior.GetPriority(), enemyDefender.GetPriority(), enemyArcher.GetPriority(), _attack);

            Debug.Log($"{soldierCount} : {enemyCount}");

            enemyCount -= enemyWarrior.ReceiveDmg(Mathf.CeilToInt(dmgDone.Item1));
            enemyCount -= enemyDefender.ReceiveDmg(Mathf.CeilToInt(dmgDone.Item2));
            enemyCount -= enemyArcher.ReceiveDmg(Mathf.CeilToInt(dmgDone.Item3));

            curDeadSoldiers.x += warrior.ReceiveDmg(Mathf.CeilToInt(dmgReceived.Item1));
            curDeadSoldiers.y += defender.ReceiveDmg(Mathf.CeilToInt(dmgReceived.Item2));
            curDeadSoldiers.z += archer.ReceiveDmg(Mathf.CeilToInt(dmgReceived.Item3));

            dead += curDeadSoldiers;

            soldierCount -= curDeadSoldiers.x + curDeadSoldiers.y + curDeadSoldiers.z;
            SetPeople(_people - curDeadSoldiers.x + curDeadSoldiers.y + curDeadSoldiers.z);

            if (enemyCount <= 0) {
                WinBattle(dead);
                break;
            } else if (soldierCount <= 0) {
                Lose();
                break;
            }
        }

        UpdateSoldiersInfo();
    }

    private void WinBattle(Vector3Int dead)
    {
        ChangePause();
        battleResults.transform.parent.gameObject.SetActive(true);
        battleResults.text = $"В этой битве мы потеряли\n{dead.x} воинов, {dead.y} защитников и {dead.z} лучников.\nМы будем помнить их вечно.";
    }

    private void ReinitEnemy()
    {
        _curEnemyCount = Mathf.Clamp(_curEnemyCount + enemyCountMultiplier, 0, maxEnemyCount);

        GenerateEnemyArmy();

        attackTimer.SetTime(attackDelay * turnTime);
    }
    #endregion

    #region методы рабочих
    private void AddEmployee(ResourceSource src)
    {
        SetPeople(_people + src.AddEmployee());
        employeeTimer.Stop();

        SetEmployeesButtonsActive(true);
    }

    private void SetEmployeesButtonsActive(bool isActive)
    {
        food.plus.interactable = isActive;
        wood.plus.interactable = isActive;
        metal.plus.interactable = isActive;
    }
    #endregion
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

        source.SetEmployees(0);
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

    [SerializeField]
    private int startCount;

    private int _curCount;

    #region UI
    public Button plusBtn, minusBtn;
    public Text countfield;
    #endregion

    public Soldier(Soldier parent, int count)
    {
        attack = parent.attack;
        defense = parent.defense;
        attackPriority = parent.attackPriority;
        
        _curCount = count;
    }

    public void Init()
    {
        _curCount = startCount;
    }

    public int AddSoldiers(int count = 1)
    {
        return SetSoldiers(_curCount + count);
    }

    public int RemoveSoldiers(int count = 1)
    {
        return SetSoldiers(_curCount - count);
    }

    public int ReceiveDmg(int amount)
    {
        int death = Mathf.CeilToInt(amount / defense);
        RemoveSoldiers(death);

        return death;
    }

    public int GetAttack() => attack * _curCount;

    public int GetDefense() => defense * _curCount;

    public float GetPriority() => attackPriority * _curCount;

    public int GetCount() => _curCount;
    
    public void UpdateUI()
    {
        if (countfield != null)
            countfield.text = _curCount.ToString();
    }

    private int SetSoldiers(int count)
    {
        count = Mathf.Clamp(count, 0, int.MaxValue);
        //Разница между новым и старым значениями количества воинов
        int delta = count - _curCount;
        _curCount = count;

        UpdateUI();

        return delta;
    }
}